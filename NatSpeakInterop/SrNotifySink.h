// Project Renfrew
// Copyright(C) 2016  Stephen Workman (workman.stephen@gmail.com)
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

#include "SinkFlags.h"

using namespace System;
using namespace System::Diagnostics;

namespace Renfrew::NatSpeakInterop::Sinks {
   using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;

   public ref class SrNotifySink :
      public Dragon::ComInterfaces::IDgnGetSinkFlags,
      public Dragon::ComInterfaces::IDgnSrEngineNotifySink,
      public Dragon::ComInterfaces::ISrNotifySink {

      private: Action<UInt64> ^_pausedProcessingCallback;

      public: SrNotifySink(Action<UInt64> ^pausedProcessingCallback) {
         if (pausedProcessingCallback == nullptr)
            throw gcnew ArgumentNullException("pausedProcessingCallback");

         _pausedProcessingCallback = pausedProcessingCallback;
      }

      public: void virtual SinkFlagsGet(DWORD *pdwFlags) {
         Debug::WriteLine(__FUNCTION__);

         if (pdwFlags == nullptr)
            return;

         // These are the notifications handled by this sink
         *pdwFlags = DGNSRSINKFLAG_SENDJITPAUSED |
                     DGNSRSINKFLAG_SENDATTRIB    |
                     #ifdef _DEBUG
                     DGNSRSINKFLAG_SENDBEGINUTT  |
                     DGNSRSINKFLAG_SENDENDUTT    |
                     #endif
                     DGNSRSINKFLAG_SENDMIMICDONE;
      }

      // IDgnSREngineNotifySink Methods
      public: void virtual AttribChanged2(DWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Paused(QWORD cookie) {
         Debug::WriteLine(__FUNCTION__);

         if (_pausedProcessingCallback != nullptr)
            _pausedProcessingCallback(cookie);
      }

      public: void virtual MimicDone(DWORD, LPUNKNOWN) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual ErrorHappened(LPUNKNOWN) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Progress(int, const WCHAR *) {
         Debug::WriteLine(__FUNCTION__);
      }

         // ISRNotifySink Methods
      public: void virtual AttribChanged(DWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Interference(QWORD, QWORD, DWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual Sound(QWORD, QWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual UtteranceBegin(QWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual UtteranceEnd(QWORD, QWORD) {
         Debug::WriteLine(__FUNCTION__);
      }

      public: void virtual VUMeter(QWORD, WORD) {
         Debug::WriteLine(__FUNCTION__);
      }
   };
}