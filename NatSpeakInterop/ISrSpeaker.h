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

#define ISrSpeakerGUID "090CD9AE-DA1A-11CD-B3CA-00AA0047BA4F"

namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces {

   [ComImport, Guid(ISrSpeakerGUID)]
   [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
   public interface class
      DECLSPEC_UUID(ISrSpeakerGUID) ISrSpeaker {

      void Delete(PCWSTR);
      void Enum(PWSTR *, DWORD *);
      void Merge(PCWSTR, PVOID, DWORD);
      void New(PCWSTR);
      void Query(PWSTR, DWORD, DWORD *);
      void Read(PCWSTR, PVOID *, DWORD *);
      void Revert(PCWSTR);
      void Select(PCWSTR, BOOL);
      void Write(PCWSTR, PVOID, DWORD);
   };
}
