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

// NOTE:
//   Portions (c) Copyright 1997-1999 by Joel Gould.
//   - Based on original NatLink Code

#pragma once

#include "Stdafx.h"
#include <vcclr.h>

namespace Renfrew::NatSpeakInterop {
   using namespace Renfrew::NatSpeakInterop::Dragon;
   using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;
   using namespace Renfrew::NatSpeakInterop::Exceptions;
   using namespace Renfrew::NatSpeakInterop::Sinks;

   public ref class GrammarService {
      private: ISrCentral ^_isrCentral;
      private: IGrammarSerializer ^_grammarSerializer;

      public: GrammarService(ISrCentral ^isrCentral,
                             IGrammarSerializer ^grammarSerializer) {

         if (isrCentral == nullptr)
            throw gcnew ArgumentNullException("isrCentral");
         if (grammarSerializer == nullptr)
            throw gcnew ArgumentNullException("grammarSerializer");

         _isrCentral = isrCentral;
         _grammarSerializer = grammarSerializer;
      }

      public: ~GrammarService() {

      }

      public: void ActivateRule(IGrammar ^grammar, HWND hWnd, String ^ruleName) {
         pin_ptr<const WCHAR> wstrRuleName = PtrToStringChars(ruleName);

         if (hWnd != nullptr && IsWindow(hWnd) == false)
            return;

         try {
            grammar->GramCommonInterface->Activate(
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

      }

      public: void DeactivateRule(IGrammar ^grammar, String ^ruleName) {
         pin_ptr<const WCHAR> wstrRuleName = PtrToStringChars(ruleName);

         try {
            grammar->GramCommonInterface->Deactivate(
               wstrRuleName
            );
         } catch (COMException ^e) {
            if (e->HResult == SrErrorCodes::SRERR_RULENOTACTIVE)
               throw gcnew GrammarException(String::Format("Rule Is Not Active: {}!", ruleName), e);
            throw gcnew GrammarException("Unexpected Grammar Error!", e);
         }
      }

      public: ISrGramCommon ^LoadGrammar(IGrammar ^grammar) {

         ISrGramNotifySink ^isrGramNotifySink;
         IntPtr iSrGramNotifySinkPtr;
         
         LPUNKNOWN pUnknown;
         array<byte> ^grammarBytes;

         if (grammar == nullptr)
            throw gcnew ArgumentNullException("grammar");

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
         
         return isrGramCommon;
      }

   };

}