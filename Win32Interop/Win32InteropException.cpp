// Project Renfrew
// Copyright(C) 2018 Stephen Workman (workman.stephen@gmail.com)
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

#include "stdafx.h"
#include "Win32InteropException.h"

using namespace System;
using namespace Renfrew::Win32::Interop;

String ^GetErrorMessage(int errorCode);

Win32InteropException::Win32InteropException(int errorCode)
   : Exception(GetErrorMessage(_errorCode)) {

   _errorCode = 0;
}

int Win32InteropException::ErrorCode::get() {
   return _errorCode;
}

String ^GetErrorMessage(int errorCode) {
   LPTSTR message = nullptr;

   auto result = FormatMessage(
      FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
      nullptr,
      errorCode,
      MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
      (LPTSTR) &message,
      0,
      nullptr
   );

   if (result == 0 || message == nullptr)
      return "An unknown error occurred.";

   auto str = gcnew String(message);

   LocalFree(message);

   return str;
}
