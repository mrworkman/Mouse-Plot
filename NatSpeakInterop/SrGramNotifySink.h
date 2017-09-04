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

#pragma once

#include "IDgnGetSinkFlags.h"
#include "ISrGramNotifySink.h"

#include "SinkFlags.h"

using namespace System;
using namespace System::Diagnostics;

namespace Renfrew::NatSpeakInterop::Sinks {
   using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;

   public ref class SrGramNotifySink :
      public Dragon::ComInterfaces::ISrGramNotifySink,
      public Dragon::ComInterfaces::IDgnGetSinkFlags {

      private: Action<UInt32, ISrResBasic^> ^_phraseFinishCallback;

      public: SrGramNotifySink() {
         _phraseFinishCallback = nullptr;
      }

      public: SrGramNotifySink(Action<UInt32, ISrResBasic^> ^phraseFinishCallback) {
         _phraseFinishCallback = phraseFinishCallback;
      }

      // IDgnGetSinkFlags Methods
      public: void virtual SinkFlagsGet(DWORD *pdwFlags) {
         Debug::WriteLine(__FUNCTION__);

         if (pdwFlags == nullptr)
            return;

         // These are the notifications handled by this sink
         *pdwFlags = DGNSRGRAMSINKFLAG_SENDPHRASEFINISH;

         /* TODO: Decide if I'll support this...
         if ( all results wanted )
            *pdwFlags |= DGNSRGRAMSINKFLAG_SENDFOREIGNFINISH; */

         /* TODO: Decide if I'll support this...
         if ( hypothesis wanted )
            *pdwFlags |= DGNSRGRAMSINKFLAG_SENDPHRASEHYPO; */

      }

         // ISrGramNotifySink Methods
      public: void virtual BookMark(DWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Paused() {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual PhraseFinish(DWORD flags, QWORD, QWORD, PSRPHRASEW pSrPhrase, LPUNKNOWN pIUnknown) {
         Debug::WriteLine(__FUNCTION__);

         // Check if a results object was provided, and silently return if not
         if (pIUnknown == nullptr)
            return;

         // Debugging (for now)
         if (pSrPhrase != nullptr) {
           
            UInt32 offset = 0;

            while (offset < (pSrPhrase->dwSize - sizeof(DWORD))) {
               PSRWORDW word = (PSRWORDW)(pSrPhrase->abWords + offset);

               Debug::WriteLine(
                  "Word (# {0}, Length: {1}): {2}",
                  word->dwWordNum,
                  word->dwSize,
                  gcnew String((PWCHAR)word->szWord)
               );
               
               offset += word->dwSize;
            }
         }
         
         // Use our callback (if set)
         if (_phraseFinishCallback != nullptr) {
            auto isrResBasic = (ISrResBasic^) Marshal::GetObjectForIUnknown(IntPtr(pIUnknown));
            
            _phraseFinishCallback(flags, isrResBasic);

            // TODO: Move to a more appropriate place (if this _isn't_ appropriate).
            Marshal::ReleaseComObject(isrResBasic);
         }
      }

      public: void virtual PhraseHypothesis(DWORD, QWORD, QWORD, PSRPHRASEW, LPUNKNOWN) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual PhraseStart(QWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual ReEvaluate(LPUNKNOWN) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Training(DWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual UnArchive(LPUNKNOWN) {
         Debug::WriteLine(__FUNCTION__);
      }
   };
}