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

using namespace System;
using namespace System::Diagnostics;

namespace Renfrew::NatSpeakInterop::Sinks {
   public ref class SrNotifySink :
      public Dragon::ComInterfaces::IDgnGetSinkFlags,
      public Dragon::ComInterfaces::IDgnSrEngineNotifySink,
      public Dragon::ComInterfaces::ISrNotifySink {

      public:
         SrNotifySink() {
            Debug::WriteLine(L" constructor \n");
         }

         void virtual SinkFlagsGet(DWORD *pdwFlags) {
            Debug::WriteLine(L"sink flags get\n");
         }

         // IDgnSREngineNotifySink Methods
         void virtual AttribChanged2(DWORD) {
            Debug::WriteLine(L"attribute changed 2\n");
         }
         void virtual Paused(QWORD) {
            Debug::WriteLine(L"paused\n");
         }

         void virtual MimicDone(DWORD, LPUNKNOWN) {
            Debug::WriteLine(L"mimic done\n");
         }

         void virtual ErrorHappened(LPUNKNOWN) {
            Debug::WriteLine(L"error happened\n");
         }

         void virtual Progress(int, const char *) {
            Debug::WriteLine(L"progress\n");
         }

         // ISRNotifySink Methods
         void virtual AttribChanged(DWORD) {
            Debug::WriteLine(L"attribute changed\n");
         }

         void virtual Interference(QWORD, QWORD, DWORD) {
            Debug::WriteLine(L" interference \n");
         }

         void virtual Sound(QWORD, QWORD) {
            Debug::WriteLine(L" sound \n");
         }

         void virtual UtteranceBegin(QWORD) {
            Debug::WriteLine(L" utterance begin \n");
         }

         void virtual UtteranceEnd(QWORD, QWORD) {
            Debug::WriteLine(L" utterance end \n");
         }

         void virtual VUMeter(QWORD, WORD) {
            Debug::WriteLine(L" vu meter \n");
         }
   };
}