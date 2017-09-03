// Project Renfrew
// Copyright(C) 2017  Stephen Workman (workman.stephen@gmail.com)
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

#pragma once

#include "Stdafx.h"
#include <vcclr.h>

namespace Renfrew::NatSpeakInterop {
   using namespace System::Collections::Generic;

   using namespace Renfrew::NatSpeakInterop::Dragon;
   using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;
   using namespace Renfrew::NatSpeakInterop::Exceptions;
   using namespace Renfrew::NatSpeakInterop::Sinks;

   public ref class GrammarService {
      private: ISrCentral ^_isrCentral;
      private: IGrammarSerializer ^_grammarSerializer;

      private: Dictionary<IGrammar ^, GrammarExecutive^> ^_grammars;

      public: GrammarService(ISrCentral ^isrCentral,
                             IGrammarSerializer ^grammarSerializer) {

         if (isrCentral == nullptr)
            throw gcnew ArgumentNullException("isrCentral");
         if (grammarSerializer == nullptr)
            throw gcnew ArgumentNullException("grammarSerializer");

         _isrCentral = isrCentral;
         _grammarSerializer = grammarSerializer;
         _grammars = gcnew Dictionary<IGrammar ^, GrammarExecutive^>();
      }

      public: ~GrammarService() {

      }

      public: void ActivateRule(IGrammar ^grammar, HWND hWnd, String ^ruleName) {
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
         } catch (COMException ^e) {
            if (e->HResult == SrErrorCodes::SRERR_INVALIDRULE)
               throw gcnew GrammarException(String::Format("Invalid Rule: {}!", ruleName), e);
            if (e->HResult == SrErrorCodes::SRERR_GRAMMARTOOCOMPLEX)
               throw gcnew GrammarException("Grammar too complex!", e);
            if (e->HResult == SrErrorCodes::SRERR_RULEALREADYACTIVE)
               throw gcnew GrammarException(String::Format("Rule Already Active: {}!", ruleName), e);
            throw gcnew GrammarException("Unexpected Grammar Error!", e);
         }
      }

      public: void ActivateRule(IGrammar ^grammar, IntPtr ^hWnd, String ^ruleName) {
         ActivateRule(grammar, (HWND) hWnd->ToPointer(), ruleName);
      }

      public: void ActivateRules(IGrammar ^grammar) {
         throw gcnew NotImplementedException();
      }

      private: GrammarExecutive ^AddGrammarToList(IGrammar ^grammar) {
         GrammarExecutive ^ge;

         // Make sure the grammar's not already loaded
         if (_grammars->ContainsKey(grammar) == true)
            throw gcnew GrammarAlreadyLoadedException("FILL ME IN");

         ge = gcnew GrammarExecutive(grammar);

         _grammars->Add(grammar, ge);

         return ge;
      }

      public: void DeactivateRule(IGrammar ^grammar, String ^ruleName) {
         pin_ptr<const WCHAR> wstrRuleName = PtrToStringChars(ruleName);

         if (_grammars->ContainsKey(grammar) == false)
            throw gcnew GrammarNotLoadedException("FILL ME IN");

         auto ge = _grammars[grammar];

         // TODO: Check that the grammar actually has the matching rule name!

         try {
            ge->GramCommonInterface->Deactivate(
               wstrRuleName
            );
         } catch (COMException ^e) {
            if (e->HResult == SrErrorCodes::SRERR_RULENOTACTIVE)
               throw gcnew GrammarException(String::Format("Rule Is Not Active: {}!", ruleName), e);
            throw gcnew GrammarException("Unexpected Grammar Error!", e);
         }
      }

      public: void LoadGrammar(IGrammar ^grammar) {

         ISrGramNotifySink ^isrGramNotifySink;
         IntPtr iSrGramNotifySinkPtr;
         
         LPUNKNOWN pUnknown;
         array<byte> ^grammarBytes;

         if (grammar == nullptr)
            throw gcnew ArgumentNullException("grammar");

         auto ge = AddGrammarToList(grammar);

         grammarBytes = _grammarSerializer->Serialize(grammar);
         
         // Pinning any sub-element of a managed array pins the entire array
         pin_ptr<byte> bytes = &grammarBytes[0];

         SDATA data;
         data.dwSize = grammarBytes->Length;
         data.pData = bytes;

         isrGramNotifySink = gcnew SrGramNotifySink();
         iSrGramNotifySinkPtr = Marshal::GetIUnknownForObject(isrGramNotifySink);
         
         try {
            _isrCentral->GrammarLoad(
               SRGRMFMT_CFG, data, iSrGramNotifySinkPtr, __uuidof(ISrGramNotifySink^), &pUnknown
            );
         } catch (COMException ^e) {
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

      private: GrammarExecutive ^RemoveGrammarFromList(IGrammar ^grammar) {
         GrammarExecutive ^ge;

         // Make sure the grammar's loaded
         if (_grammars->ContainsKey(grammar) == false)
            throw gcnew GrammarNotLoadedException("FILL ME IN");

         ge = _grammars[grammar];

         _grammars->Remove(grammar);

         return ge;
      }

      public: void UnloadGrammar(IGrammar ^grammar) {
         if (grammar == nullptr)
            throw gcnew ArgumentNullException("grammar");

         auto ge = RemoveGrammarFromList(grammar);

         if (ge->GramCommonInterface == nullptr)
            throw gcnew InvalidStateException("isrGramCommon interface is not set!");

         Marshal::ReleaseComObject(ge->GramCommonInterface);
         ge->GramCommonInterface = nullptr;
      }

   };

}