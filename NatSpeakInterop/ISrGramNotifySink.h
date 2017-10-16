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

#define ISrGramNotifySinkGUID "f106bfa0-c743-11cd-80e5-00aa003e4b50"

namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces {

   [ComImport, Guid(ISrGramNotifySinkGUID)]
   [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
   public interface class
      DECLSPEC_UUID(ISrGramNotifySinkGUID) ISrGramNotifySink {

      void BookMark(DWORD);
      void Paused();
      void PhraseFinish(DWORD, QWORD, QWORD, PSRPHRASEW, LPUNKNOWN);
      void PhraseHypothesis(DWORD, QWORD, QWORD, PSRPHRASEW, LPUNKNOWN);
      void PhraseStart(QWORD);
      void ReEvaluate(LPUNKNOWN);
      void Training(DWORD);
      void UnArchive(LPUNKNOWN);
   };
}
