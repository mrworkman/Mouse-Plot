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

// NOTE: 
// Original dragon interfaces (c) Copyright Dragon Systems, Inc. 1998
//   Portions (c) Copyright Dragon Systems, Inc. 1998
//   Portions (c) Copyright 1997-1999 by Joel Gould.

#pragma once

#define IDgnSSvcInterpreterGUID "dd109203-6205-11cf-ae61-0000e8a28647"

namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces {
   using namespace System;
   using namespace System::Runtime::InteropServices;

   [ComImport, Guid(IDgnSSvcInterpreterGUID)]
   [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
   public interface class
      DECLSPEC_UUID(IDgnSSvcInterpreterGUID) IDgnSSvcInterpreter {
   
      void Register(/* IDgnSSvcActionNotifySink **/ IntPtr); // TODO: validate/test pointer !
      void CheckScript(const PWCHAR, DWORD*, DWORD*);
      void ExecuteScript(const PWCHAR, DWORD*, DWORD*, const PWCHAR, DWORD);
      void ExecuteScriptWithListResults(const PWCHAR, DWORD, const PWCHAR, DWORD*, DWORD*, const PWCHAR, DWORD);
   };

}