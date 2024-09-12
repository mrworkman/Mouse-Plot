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

#include "MouseButtons.h"
#include "MouseScrollDirection.h"

namespace Renfrew::Win32::Interop {
   public ref class Mouse abstract {
      public: static void Animate(int startX, int startY, int endX, int endY);
      public: static void Animate(int startX, int startY, int endX, int endY, int stepSize);

      public: static void Click(MouseButtons buttons);

      public: static void Down(MouseButtons buttons);
      public: static void Up(MouseButtons buttons);

      public: static void Scroll(MouseScrollDirection scrollDirection, DWORD scrollDelta);

      public: static void SetPosition(int x, int y);
   };
}