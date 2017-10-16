// Project Renfrew
// Copyright(C) 2016 Stephen Workman (workman.stephen@gmail.com)
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
#include "IDgnSREngineNotifySink.h"
#include "ISRNotifySink.h"

namespace Renfrew::NatSpeakInterop::Sinks {
   public ref class SrNotifySink :
      public Dragon::ComInterfaces::IDgnGetSinkFlags,
      public Dragon::ComInterfaces::IDgnSrEngineNotifySink,
      public Dragon::ComInterfaces::ISrNotifySink {

      private: Action<UInt64> ^_pausedProcessingCallback;

      public: SrNotifySink(Action<UInt64> ^pausedProcessingCallback);
      public: void virtual SinkFlagsGet(DWORD *pdwFlags);

      // IDgnSREngineNotifySink Methods
      public: void virtual AttribChanged2(DWORD);
      public: void virtual Paused(QWORD cookie);
      public: void virtual MimicDone(DWORD, LPUNKNOWN);
      public: void virtual ErrorHappened(LPUNKNOWN);
      public: void virtual Progress(int, const WCHAR *);

      // ISRNotifySink Methods
      public: void virtual AttribChanged(DWORD);
      public: void virtual Interference(QWORD, QWORD, DWORD);
      public: void virtual Sound(QWORD, QWORD);
      public: void virtual UtteranceBegin(QWORD);
      public: void virtual UtteranceEnd(QWORD, QWORD);
      public: void virtual VUMeter(QWORD, WORD);
   };
}