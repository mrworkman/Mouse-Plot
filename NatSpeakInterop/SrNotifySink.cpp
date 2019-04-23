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

#include "Stdafx.h"
#include "SrNotifySink.h"
#include "SinkFlags.h"

using namespace Renfrew::NatSpeakInterop::Sinks;

SrNotifySink::SrNotifySink(Action<UInt64> ^pausedProcessingCallback) {
   if (pausedProcessingCallback == nullptr)
      throw gcnew ArgumentNullException("pausedProcessingCallback");

   _pausedProcessingCallback = pausedProcessingCallback;
}

void SrNotifySink::AttribChanged(DWORD) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::AttribChanged2(DWORD dword) {
   Debug::WriteLine("{0}: {1}", __FUNCTION__, dword);
}

void SrNotifySink::ErrorHappened(LPUNKNOWN) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::Interference(QWORD, QWORD, DWORD) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::MimicDone(DWORD, LPUNKNOWN) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::Paused(QWORD cookie) {
   Debug::WriteLine(__FUNCTION__);

   if (_pausedProcessingCallback != nullptr)
      _pausedProcessingCallback(cookie);
}

void SrNotifySink::Progress(int, const WCHAR *) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::SinkFlagsGet(DWORD *pdwFlags) {
   Debug::WriteLine(__FUNCTION__);

   if (pdwFlags == nullptr)
      return;

   // These are the notifications handled by this sink
   *pdwFlags = DGNSRSINKFLAG_SENDJITPAUSED |
      DGNSRSINKFLAG_SENDATTRIB |
#ifdef _DEBUG
      DGNSRSINKFLAG_SENDBEGINUTT |
      DGNSRSINKFLAG_SENDENDUTT |
#endif
      DGNSRSINKFLAG_SENDMIMICDONE |
      DGNSRSINKFLAG_SENDERROR;
}

void SrNotifySink::Sound(QWORD, QWORD) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::UtteranceBegin(QWORD) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::UtteranceEnd(QWORD, QWORD) {
   Debug::WriteLine(__FUNCTION__);
}

void SrNotifySink::VUMeter(QWORD, WORD) {
   Debug::WriteLine(__FUNCTION__);
}