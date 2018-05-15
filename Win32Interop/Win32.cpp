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

#include "WindowHandle.h"
#include "Win32.h"
#include "Win32InteropException.h"

using namespace System;
using namespace Renfrew::Win32::Interop;

void Win32::BringWindowToTop(WindowHandle ^handle) {
   if (handle == nullptr)
      throw gcnew ArgumentNullException();

   if (::BringWindowToTop(handle->Hwnd) == TRUE)
      return;

   throw gcnew Win32InteropException(GetLastError());
}

void Win32::SetActiveWindow(WindowHandle ^handle) {
   if (handle == nullptr)
      throw gcnew ArgumentNullException();

   if (::SetActiveWindow(handle->Hwnd) != nullptr)
      return;

   throw gcnew Win32InteropException(GetLastError());
}

void Win32::SetForegroundWindow(WindowHandle ^handle) {
   if (handle == nullptr)
      throw gcnew ArgumentNullException();

   if (::SetForegroundWindow(handle->Hwnd) == TRUE)
      return;

   throw gcnew Win32InteropException(GetLastError());
}

void Win32::SendKeystrokes(String ^keystrokes) {
   
   if (keystrokes == nullptr)
      throw gcnew ArgumentNullException();

   INPUT input;

   memset(&input, 0, sizeof(INPUT));

   for (int i = 0; i < keystrokes->Length; i++) {
      Char c = keystrokes[i];

      if (Char::IsLetterOrDigit(c) == true) {
         c = Char::ToUpper(c);

         input.type = INPUT_KEYBOARD;
         input.ki.wVk = c;

         SendInput(1, &input, sizeof(INPUT));

         input.ki.dwFlags = KEYEVENTF_KEYUP;
         SendInput(1, &input, sizeof(INPUT));

      }
   }


}

WindowHandle ^Win32::WindowFromPoint(int x, int y) {
   POINT p = { x, y };

   HWND hWnd = ::WindowFromPoint(p);

   if (hWnd == nullptr)
      return nullptr;

   return gcnew WindowHandle(hWnd);
}
