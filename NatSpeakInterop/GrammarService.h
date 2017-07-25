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

// TODO: Move this:
#define  SRERR_GRAMMARERROR   0x80040416

namespace Renfrew::NatSpeakInterop {
   using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;
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

      public: void ActivateRule(IGrammar ^grammar, /*HWND hWnd,*/ String ^ruleName) {
         pin_ptr<const WCHAR> wstrRuleName = PtrToStringChars(ruleName);

         try {
            grammar->GramCommonInterface->Activate(
               nullptr, // TODO: Set to hWnd (where applicable) 
               false, wstrRuleName
            );
         } catch (COMException ^e) {
            // SRERR_INVALIDRULE
            // SRERR_GRAMMARTOOCOMPLEX
            // SRERR_RULEALREADYACTIVE
            // default on other errors

            throw;
         }

      }

      public: void ActivateRules(IGrammar ^grammar) {

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
            if (e->HResult == SRERR_GRAMMARERROR) {
               Debug::WriteLine("SRERR_GRAMMARERROR");
            } else {
               throw;
            }
            // SRERR_INVALIDCHAR
            // SRERR_GRAMMARERROR
            // default on other errors
         }

         ISrGramCommon ^isrGramCommon = (ISrGramCommon^)
            Marshal::GetTypedObjectForIUnknown(IntPtr(pUnknown), ISrGramCommon::typeid);

         pUnknown->Release();
         Marshal::Release(iSrGramNotifySinkPtr);
         
         return isrGramCommon;
      }

   };

}