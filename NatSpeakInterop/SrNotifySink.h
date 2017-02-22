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

      public:
         SrNotifySink() {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual SinkFlagsGet(DWORD *pdwFlags) {
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
         void virtual AttribChanged2(DWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual Paused(QWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual MimicDone(DWORD, LPUNKNOWN) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual ErrorHappened(LPUNKNOWN) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual Progress(int, const WCHAR *) {
            Debug::WriteLine(__FUNCTION__);
         }

         // ISRNotifySink Methods
         void virtual AttribChanged(DWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual Interference(QWORD, QWORD, DWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual Sound(QWORD, QWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual UtteranceBegin(QWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual UtteranceEnd(QWORD, QWORD) {
            Debug::WriteLine(__FUNCTION__);
         }

         void virtual VUMeter(QWORD, WORD) {
            Debug::WriteLine(__FUNCTION__);
         }
   };
}