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

#include "Magnifier.h"

using namespace Renfrew;

Magnifier::Magnifier() {
   _parentHwnd = nullptr;
   _magnifierHwnd = nullptr;
}

HandleRef Magnifier::BuildWindowCore(HandleRef hwndParent) {
   _hInstance = GetModuleHandle(nullptr);

   _parentHwnd = CreateWindow(
      TEXT("STATIC"), nullptr,
      WS_CHILD | WS_VISIBLE,
      0, 0, 100, 100, // x, y, w, h
      (HWND)hwndParent.Handle.ToPointer(),
      nullptr,
      _hInstance,
      0
   );

   return HandleRef(this, IntPtr(_parentHwnd));
}

void Magnifier::DestroyWindowCore(HandleRef hwnd) {

}

void Magnifier::Initialize() {

   if (MagInitialize() == FALSE) {
      throw gcnew ApplicationException("Could not initialize magnification subsystem.");
   }

   _magnifierHwnd = CreateWindow(
      WC_MAGNIFIER, TEXT("MagnifierWindow"),
      WS_CHILD | MS_SHOWMAGNIFIEDCURSOR | WS_VISIBLE, // | MS_INVERTCOLORS,
      0, 0,
      400,
      400,
      _parentHwnd, NULL,
      _hInstance, NULL
   );

   MAGTRANSFORM matrix;
   memset(&matrix, 0, sizeof(matrix));
   matrix.v[0][0] = 4;
   matrix.v[1][1] = 4;
   matrix.v[2][2] = 1.0f;

   MagSetWindowTransform(_magnifierHwnd, &matrix);

   //HRESULT result = GetLastError();

   //MessageBox::Show("ERROR: " + result.ToString());

   RECT r = { 0, 0, 100, 100 };
   MagSetWindowSource(_magnifierHwnd, r);
}

void Magnifier::Update(Int32 x, Int32 y) {
   RECT r = { x, y, 100, 100 };
   MagSetWindowSource(_magnifierHwnd, r);
}
