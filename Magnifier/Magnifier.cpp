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

#using "PresentationFramework.dll"
#using "PresentationCore.dll"
#using "WindowsBase.dll"
#using "System.Xaml.dll"

using namespace System::Windows;
using namespace System::Windows::Interop;
using namespace System::Runtime::InteropServices;

using namespace System::Diagnostics;

#include "Magnifier.h"

using namespace Renfrew;

Magnifier::Magnifier() {
   _parentHwnd = nullptr;
   _magnifierHwnd = nullptr;
}

HandleRef Magnifier::BuildWindowCore(HandleRef handleRef) {
   _hInstance = GetModuleHandle(nullptr);
   
   _parentHwnd = CreateWindow(
      TEXT("STATIC"), nullptr,
      WS_CHILD | WS_VISIBLE,
      0, 0, 100, 100, // x, y, w, h
      (HWND) handleRef.Handle.ToPointer(),
      nullptr,
      _hInstance,
      0
   );

   if (_parentHwnd == nullptr)
      throw gcnew Exception("Failed to create host window. Error Code: " + GetLastError());

   return HandleRef(this, IntPtr(_parentHwnd));
}

void Magnifier::DestroyWindowCore(HandleRef handleRef) {

}

void Magnifier::Initialize() {
   if (MagInitialize() == FALSE)
      throw gcnew Exception("Could not initialize magnification subsystem.");

   _magnifierHwnd = CreateWindow(
      WC_MAGNIFIER, TEXT("MagnifierWindow"),
      WS_CHILD | MS_SHOWMAGNIFIEDCURSOR | WS_VISIBLE, // | MS_INVERTCOLORS,
      0, 0,
      300,
      300,
      _parentHwnd, NULL,
      _hInstance, NULL
   );

   if (_magnifierHwnd == nullptr)
      throw gcnew Exception("Failed to create magnifier window. Error Code: " + GetLastError());
   
   SetMagnification(3);
   Update(0, 0, 100, 100);
}

void Magnifier::SetMagnification(Int32 factor) {
   if (factor < 0)
      throw gcnew ArgumentOutOfRangeException("factor must be a positive number!");

   MAGTRANSFORM matrix;
   memset(&matrix, 0, sizeof(matrix));
   matrix.v[0][0] = factor;
   matrix.v[1][1] = factor;
   matrix.v[2][2] = 1.0f;

   MagSetWindowTransform(_magnifierHwnd, &matrix);
}

void Magnifier::Update(Int32 x, Int32 y, Int32 width, Int32 height) {
   RECT r = { x, y, width, height };
   MagSetWindowSource(_magnifierHwnd, r);
}

