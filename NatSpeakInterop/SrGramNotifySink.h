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

      public: SrGramNotifySink() {
         
      }

      // IDgnGetSinkFlags Methods
      public: void virtual SinkFlagsGet(DWORD *pdwFlags) {
         Debug::WriteLine(__FUNCTION__);

         if (pdwFlags == nullptr)
            return;

         // These are the notifications handled by this sink
         *pdwFlags = DGNSRGRAMSINKFLAG_SENDPHRASEFINISH;
      }

         // ISrGramNotifySink Methods
      public: void virtual BookMark(DWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Paused() {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual PhraseFinish(DWORD, QWORD, QWORD, PSRPHRASEW, LPUNKNOWN) {
         Debug::WriteLine(__FUNCTION__);
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