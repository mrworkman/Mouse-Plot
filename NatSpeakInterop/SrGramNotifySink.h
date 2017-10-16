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
#include "ISrResBasic.h"

namespace Renfrew::NatSpeakInterop::Sinks {
   public ref class SrGramNotifySink :
      public Dragon::ComInterfaces::ISrGramNotifySink,
      public Dragon::ComInterfaces::IDgnGetSinkFlags {

      private: Object ^_callbackParam;
      private: Action<UInt32, Object^, Dragon::ComInterfaces::ISrResBasic^> ^_phraseFinishCallback;
      public: SrGramNotifySink(Action<UInt32, Object^,
                               Dragon::ComInterfaces::ISrResBasic^> ^phraseFinishCallback,
                               Object ^callbackParam);

      // IDgnGetSinkFlags Methods
      public: void virtual SinkFlagsGet(DWORD *pdwFlags);

      // ISrGramNotifySink Methods
      public: void virtual BookMark(DWORD);
      public: void virtual Paused();
      public: void virtual PhraseFinish(DWORD flags, QWORD, QWORD, PSRPHRASEW pSrPhrase, LPUNKNOWN pIUnknown);
      public: void virtual PhraseHypothesis(DWORD, QWORD, QWORD, PSRPHRASEW, LPUNKNOWN);
      public: void virtual PhraseStart(QWORD);
      public: void virtual ReEvaluate(LPUNKNOWN);
      public: void virtual Training(DWORD);
      public: void virtual UnArchive(LPUNKNOWN);
   };
}