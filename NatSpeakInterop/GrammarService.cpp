// Project Renfrew
// Copyright(C) 2017 Stephen Workman (workman.stephen@gmail.com)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.
//

#include "stdafx.h"

#include "SrGramNotifySink.h"

#include "GrammarAlreadyLoadedException.h"
#include "GrammarException.h"
#include "GrammarNotLoadedException.h"
#include "InvalidStateException.h"
#include "SrErrorCodes.h"

using namespace Renfrew::NatSpeakInterop;
using namespace Renfrew::NatSpeakInterop::Dragon;
using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;
using namespace Renfrew::NatSpeakInterop::Exceptions;
using namespace Renfrew::NatSpeakInterop::Sinks;

#include "GrammarService.h"

GrammarService::GrammarService(ISrCentral ^isrCentral,
   IDgnSrEngineControl ^idgnSrEngineControl) {

   if (isrCentral == nullptr)
      throw gcnew ArgumentNullException("isrCentral");
   if (idgnSrEngineControl == nullptr)
      throw gcnew ArgumentNullException("iDgnSrEngineControl");

   _isrCentral = isrCentral;
   _idgnSrEngineControl = idgnSrEngineControl;

   _grammars = gcnew Dictionary<IGrammar^, GrammarExecutive^>();
}

GrammarService::~GrammarService() {

}

void GrammarService::ActivateRule(IGrammar ^grammar, HWND hWnd, String ^ruleName) {
   pin_ptr<const WCHAR> wstrRuleName = PtrToStringChars(ruleName);

   if (_grammars->ContainsKey(grammar) == false)
      throw gcnew GrammarNotLoadedException("FILL ME IN");

   // Check if the handle points to an exsiting window
   if (hWnd != nullptr && IsWindow(hWnd) == false)
      return; // TODO: Throw exception?

   auto ge = _grammars[grammar];

   // TODO: Check that the grammar actually has the matching rule name!

   try {
      ge->GramCommonInterface->Activate(
         hWnd, // TODO: Set to hWnd (where applicable) 
         false, wstrRuleName
      );
   }
   catch (COMException ^e) {
      if (e->HResult == SrErrorCodes::SRERR_INVALIDRULE)
         throw gcnew GrammarException(String::Format("Invalid Rule: {}!", ruleName), e);
      if (e->HResult == SrErrorCodes::SRERR_GRAMMARTOOCOMPLEX)
         throw gcnew GrammarException("Grammar too complex!", e);
      if (e->HResult == SrErrorCodes::SRERR_RULEALREADYACTIVE)
         throw gcnew GrammarException(String::Format("Rule Already Active: {}!", ruleName), e);
      throw gcnew GrammarException("Unexpected Grammar Error!", e);
   }
}

void GrammarService::ActivateRule(IGrammar ^grammar, IntPtr ^hWnd, String ^ruleName) {
   ActivateRule(grammar, (HWND)hWnd->ToPointer(), ruleName);
}

void GrammarService::ActivateRules(IGrammar ^grammar) {
   throw gcnew NotImplementedException();
}

GrammarExecutive ^GrammarService::AddGrammarToList(IGrammar ^grammar) {
   GrammarExecutive ^ge;

   // Make sure the grammar's not already loaded
   if (_grammars->ContainsKey(grammar) == true)
      throw gcnew GrammarAlreadyLoadedException("FILL ME IN");

   ge = gcnew GrammarExecutive(grammar);

   _grammars->Add(grammar, ge);

   return ge;
}

void GrammarService::DeactivateRule(IGrammar ^grammar, String ^ruleName) {
   pin_ptr<const WCHAR> wstrRuleName = PtrToStringChars(ruleName);

   if (_grammars->ContainsKey(grammar) == false)
      throw gcnew GrammarNotLoadedException("FILL ME IN");

   auto ge = _grammars[grammar];

   // TODO: Check that the grammar actually has the matching rule name!

   try {
      ge->GramCommonInterface->Deactivate(
         wstrRuleName
      );
   }
   catch (COMException ^e) {
      if (e->HResult == SrErrorCodes::SRERR_RULENOTACTIVE)
         throw gcnew GrammarException(String::Format("Rule Is Not Active: {}!", ruleName), e);
      throw gcnew GrammarException("Unexpected Grammar Error!", e);
   }
}

void GrammarService::GrammarSerializer::set(IGrammarSerializer ^grammarSerializer) {
   if (grammarSerializer == nullptr)
      throw gcnew ArgumentNullException("grammarSerializer");

   _grammarSerializer = grammarSerializer;
}

void GrammarService::LoadGrammar(IGrammar ^grammar) {

   ISrGramNotifySink ^isrGramNotifySink;
   IntPtr iSrGramNotifySinkPtr;

   LPUNKNOWN pUnknown;
   array<byte> ^grammarBytes;

   if (grammar == nullptr)
      throw gcnew ArgumentNullException("grammar");

   if (_grammarSerializer == nullptr)
      throw gcnew InvalidStateException("GrammarSerializer hasn't been set!");

   auto ge = AddGrammarToList(grammar);

   grammarBytes = _grammarSerializer->Serialize(grammar);

   // Pinning any sub-element of a managed array pins the entire array
   pin_ptr<byte> bytes = &grammarBytes[0];

   SDATA data;
   data.dwSize = grammarBytes->Length;
   data.pData = bytes;

   isrGramNotifySink = gcnew SrGramNotifySink(
      gcnew Action<UInt32, Object^, ISrResBasic^>(this, &GrammarService::PhraseFinishedCallback), ge
   );

   iSrGramNotifySinkPtr = Marshal::GetIUnknownForObject(isrGramNotifySink);

   try {
      _isrCentral->GrammarLoad(
         SRGRMFMT_CFG, data, iSrGramNotifySinkPtr, __uuidof(ISrGramNotifySink^), &pUnknown
      );
   }
   catch (COMException ^e) {
      if (e->HResult == SrErrorCodes::SRERR_INVALIDCHAR)
         throw gcnew GrammarException("Invalid Word/Character in Grammar", e);
      if (e->HResult == SrErrorCodes::SRERR_GRAMMARERROR)
         throw gcnew GrammarException("Grammar Error", e);
      throw gcnew GrammarException("Unexpected Grammar Error!", e);
   }

   ISrGramCommon ^isrGramCommon = (ISrGramCommon^)
      Marshal::GetTypedObjectForIUnknown(IntPtr(pUnknown), ISrGramCommon::typeid);

   pUnknown->Release();
   Marshal::Release(iSrGramNotifySinkPtr);

   // Store isrGramCommon with our grammar
   ge->GramCommonInterface = isrGramCommon;

}

void GrammarService::PausedProcessor(UInt64 cookie) {
   Debug::WriteLine(__FUNCTION__ + "(cookie: " + cookie + ")");

   // TODO: Call grammar activation method(s)

   Debug::WriteLine(__FUNCTION__ + ", Resuming.");
   _idgnSrEngineControl->Resume(cookie);
}

void GrammarService::PhraseFinishedCallback(UInt32 flags, Object ^grammarObj, ISrResBasic^ isrResBasic) {
   Debug::WriteLine(__FUNCTION__);

   auto ge = (GrammarExecutive^)grammarObj;

   if (ge == nullptr)
      throw gcnew InvalidStateException("grammarObj is unexpectedly NULL!");

   //
   // Test Processing The Results
   //

   /* MOVE BELOW BasePathWord
   *if ((flags & ISRNOTEFIN_RECOGNIZED) == 0) {
   Debug::WriteLine("Rejected");
   return;
   }

   if ((flags & ISRNOTEFIN_THISGRAMMAR) == 0) {
   Debug::WriteLine("Other");
   return;
   }*/

   auto isrResGraph = (ISrResGraph^)isrResBasic;

   DWORD pathSize = 0;
   PDWORD path = nullptr;

   try {
      // Find out how big the path is
      isrResGraph->BestPathWord(0, &pathSize, 0, &pathSize);
   }
   catch (COMException ^e) {
      // Allocate space for the path
      if (e->HResult == EVENT_E_ALL_SUBSCRIBERS_FAILED) {
         path = new DWORD[pathSize];
         isrResGraph->BestPathWord(0, path, pathSize * sizeof(DWORD), &pathSize);
      }
   }

   UInt32 ruleNumber = 0;
   auto spokenWords = gcnew List<String^>();

   DWORD numWords = pathSize / sizeof(DWORD);

   for (DWORD i = 0; i < numWords; i++) {
      SRRESWORDNODE node;

      DWORD srWordSize = 0;
      PSRWORDW srWord = nullptr;

      // Get the word size to allocate space for it
      isrResGraph->GetWordNode(path[i], &node, (PSRWORDW)1, 0, &srWordSize);

      if (srWordSize == 0)
         throw gcnew InvalidStateException("Word with no size!");

      srWord = (PSRWORDW) new BYTE[srWordSize];

      isrResGraph->GetWordNode(path[i], &node, srWord, srWordSize, &srWordSize);

      ruleNumber = node.dwCFGParse;

      Debug::WriteLine(
         "Word Number: {0}, Word: {1}, Rule: {2}",
         srWord->dwWordNum,
         gcnew String(srWord->szWord),
         node.dwCFGParse
      );

      spokenWords->Add(gcnew String(srWord->szWord));

      delete srWord;
   }

   delete[] path;

   ge->Grammar->InvokeRule(ruleNumber, spokenWords);

}

GrammarExecutive ^GrammarService::RemoveGrammarFromList(IGrammar ^grammar) {
   GrammarExecutive ^ge;

   // Make sure the grammar's loaded
   if (_grammars->ContainsKey(grammar) == false)
      throw gcnew GrammarNotLoadedException("FILL ME IN");

   ge = _grammars[grammar];

   _grammars->Remove(grammar);

   return ge;
}

void GrammarService::UnloadGrammar(IGrammar ^grammar) {
   if (grammar == nullptr)
      throw gcnew ArgumentNullException("grammar");

   auto ge = RemoveGrammarFromList(grammar);

   if (ge->GramCommonInterface == nullptr)
      throw gcnew InvalidStateException("isrGramCommon interface is not set!");

   Marshal::ReleaseComObject(ge->GramCommonInterface);
   ge->GramCommonInterface = nullptr;
}

