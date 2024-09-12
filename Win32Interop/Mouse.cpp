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
#include "Mouse.h"

using namespace System;
using namespace System::Threading;

using namespace Renfrew::Win32::Interop;

static int interval = 10;

void Mouse::Animate(int startX, int startY, int endX, int endY) {
   Animate(startX, startY, endX, endY, 10);
}

void Mouse::Animate(int startX, int startY, int endX, int endY, int stepSize) {
   double x = startX;
   double y = startY;

   double rise = Math::Abs(endY - startY);
   double run  = Math::Abs(endX - startX);

   double xi = 0.0;
   double yi = 0.0;

   if (rise == 0.0) {
      xi = stepSize;
   } else if (run == 0.0) {
      yi = stepSize;
   } else {
      double theta = Math::Atan(rise / run);

      xi = Math::Cos(theta) * stepSize;
      yi = Math::Sin(theta) * stepSize;
   }

   // y screen coordinates are inverted
   yi = -yi;

   while ((int)x != endX || (int)y != endY) {

      if (endX >= startX) {
         x += xi;

         if (x > endX)
            x = endX;
      } else {
         x -= xi;

         if (x < endX)
            x = endX;
      }

      if (endY <= startY) {
         y += yi;

         if (y < endY)
            y = endY;
      } else {
         y -= yi;

         if (y > endY)
            y = endY;
      }

      SetCursorPos((int) x, (int) y);

      Thread::Sleep(interval);
   }
}

void Mouse::Click(MouseButtons buttons) {
   Down(buttons);

   Thread::Sleep(1);

   Up(buttons);
}

void Mouse::Down(MouseButtons buttons) {
   DWORD flags = 0;

   if ((buttons & MouseButtons::Left) == MouseButtons::Left)
      flags |= MOUSEEVENTF_LEFTDOWN;
   if ((buttons & MouseButtons::Right) == MouseButtons::Right)
      flags |= MOUSEEVENTF_RIGHTDOWN;
   if ((buttons & MouseButtons::Middle) == MouseButtons::Middle)
      flags |= MOUSEEVENTF_MIDDLEDOWN;

   mouse_event(flags, 0, 0, 0, 0);
}

void Mouse::SetPosition(int x, int y) {
   SetCursorPos(x, y);
}

void Mouse::Up(MouseButtons buttons) {
   DWORD flags = 0;

   if ((buttons & MouseButtons::Left) == MouseButtons::Left)
      flags |= MOUSEEVENTF_LEFTUP;
   if ((buttons & MouseButtons::Right) == MouseButtons::Right)
      flags |= MOUSEEVENTF_RIGHTUP;
   if ((buttons & MouseButtons::Middle) == MouseButtons::Middle)
      flags |= MOUSEEVENTF_MIDDLEUP;

   mouse_event(flags, 0, 0, 0, 0);
}

void Mouse::Scroll(MouseScrollDirection scrollDirection, DWORD scrollDelta) {
   if (scrollDirection == MouseScrollDirection::Down) {
      mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -scrollDelta, 0);
   }
   if (scrollDirection == MouseScrollDirection::Up) {
      mouse_event(MOUSEEVENTF_WHEEL, 0, 0, scrollDelta, 0);
   }
}
