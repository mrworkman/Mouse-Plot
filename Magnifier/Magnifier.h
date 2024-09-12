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

#pragma once

using namespace System;

namespace Renfrew::Utility {

   public ref class Magnifier : public HwndHost {
      private: HWND _parentHwnd;
      private: HWND _magnifierHwnd;
      private: HINSTANCE _hInstance;

      public: Magnifier();

      // From HwndHost
      protected: virtual HandleRef BuildWindowCore(HandleRef handleRef) override;
      protected: virtual void DestroyWindowCore(HandleRef handleRef) override;

      public: void Initialize(double scaleMultiplier);
      public: void SetMagnification(Int32 multiplier);
      public: void Update(Int32 x, Int32 y, Int32 width, Int32 height);
   };

}
