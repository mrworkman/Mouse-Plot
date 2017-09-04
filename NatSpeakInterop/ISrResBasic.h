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

#define ISrResBasicGUID "090cd9a5-da1a-11cd-b3ca-00aa0047ba4f"

namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces {
   using namespace System::Runtime::InteropServices;

   [ComImport, Guid(ISrResBasicGUID)]
   [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
   public interface class
      DECLSPEC_UUID(ISrResBasicGUID) ISrResBasic {

      void PhraseGet(DWORD, PSRPHRASEW, DWORD, DWORD *);
      void Identify(GUID *);
      void TimeGet(PQWORD, PQWORD);
      void FlagsGet(DWORD, DWORD *);
   };
}
