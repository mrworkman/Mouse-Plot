// Project Renfrew
// Copyright(C) 2019 Stephen Workman (workman.stephen@gmail.com)
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

#define IDgnSSvcAppTrackingNotifySinkGUID "dd109207-6205-11cf-ae61-0000e8a28647"

namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces {

   [ComImport, Guid(IDgnSSvcAppTrackingNotifySinkGUID)]
   [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
   public interface class
      DECLSPEC_UUID(IDgnSSvcAppTrackingNotifySinkGUID) IDgnSSvcAppTrackingNotifySink {

      void AppLoaded(DWORD, const wchar_t*, const wchar_t*);
      void AppTerminated(DWORD, const wchar_t*, const wchar_t*);
   };
}
