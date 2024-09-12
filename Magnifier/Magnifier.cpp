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
#include "MagnifierException.h"

using namespace Renfrew::Utility;

/// <summary>Creates a new Magnifier surface.</summary>
Magnifier::Magnifier() {
   _parentHwnd = nullptr;
   _magnifierHwnd = nullptr;
}

/// <summary>Binds the Magnifier to the given WPF surface/window.</summary>
/// <param name="handleRef">The window handle of the parent window.</param>
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
      throw gcnew MagnifierException("Failed to create magnifier host window.", GetLastError());

   return HandleRef(this, IntPtr(_parentHwnd));
}

/// <summary>Unbinds the Magnifier from the given WPF surface/window.</summary>
/// <param name="handleRef">The window handle of the parent window.</param>
void Magnifier::DestroyWindowCore(HandleRef handleRef) {
   if (DestroyWindow(_parentHwnd) == TRUE)
      return;

   throw gcnew MagnifierException("Failed to destroy magnifier host window.", GetLastError());
}

/// <summary>
/// Initializes the magnifier.
/// </summary>
void Magnifier::Initialize(double scaleMultiplier) {
   if (MagInitialize() == FALSE)
      throw gcnew Exception("Could not initialize magnification subsystem.");

   _magnifierHwnd = CreateWindow(
      WC_MAGNIFIER, TEXT("MagnifierWindow"),
      WS_CHILD | MS_SHOWMAGNIFIEDCURSOR | WS_VISIBLE, // | MS_INVERTCOLORS,
      0, 0,
      static_cast<int>(300 * scaleMultiplier),
      static_cast<int>(300 * scaleMultiplier),
      _parentHwnd, NULL,
      _hInstance, NULL
   );

   if (_magnifierHwnd == nullptr)
      throw gcnew MagnifierException("Failed to create magnifier window.", GetLastError());

   SetMagnification(3);
   Update(0, 0, 100, 100);
}

/// <summary>
/// Sets the magnification multiplier.
/// </summary>
/// <param name="multiplier">The multiplier for the zoom-level.</param>
void Magnifier::SetMagnification(Int32 multiplier) {
   if (multiplier < 0)
      throw gcnew ArgumentOutOfRangeException("multiplier must be a positive number!");

   MAGTRANSFORM matrix;
   memset(&matrix, 0, sizeof(matrix));
   matrix.v[0][0] = (float) multiplier;
   matrix.v[1][1] = (float) multiplier;
   matrix.v[2][2] = 1.0f;

   if (MagSetWindowTransform(_magnifierHwnd, &matrix) == TRUE)
      return;

   throw gcnew MagnifierException("Failed to set magnification multiplier.", GetLastError());
}

/// <summary>
/// Updates the magnifier surface with a new source rectangle.
/// </summary>
/// <param name="x">
/// The offset, in pixels, from the left edge of the primary screen where the
/// source rectangle starts (can be negative).
/// </param>
/// <param name="y">
/// The offset, in pixels, from the top edge of the primary screen where the
/// source rectangle starts (can be negative).
/// </param>
/// <param name="width">The width of the source rectangle, in pixels.</param>
/// <param name="height">The height of the source rectangle, in pixels.</param>
void Magnifier::Update(Int32 x, Int32 y, Int32 width, Int32 height) {
   RECT r = { x, y, width, height };

   if (MagSetWindowSource(_magnifierHwnd, r) == TRUE)
      return;

   throw gcnew MagnifierException("Failed to update magnifier.", GetLastError());
}

