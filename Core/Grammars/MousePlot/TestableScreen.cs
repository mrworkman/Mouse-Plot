// Project Renfrew
// Copyright(C) 2017 Stephen Workman (workman.stephen@gmail.com)
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

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Renfrew.Core.Grammars.MousePlot {
   public class TestableScreen : IScreen {

      private Screen _screen;

      public TestableScreen() {
         _screen = Screen.PrimaryScreen;
      }

      private TestableScreen(Screen screen) {
         _screen = screen;
      }

      public IScreen[] AllScreens =>
         Screen.AllScreens.Select(e => (IScreen) new TestableScreen(e)).ToArray();

      public Rectangle Bounds =>
         _screen.Bounds;

      public IScreen PrimaryScreen =>
         new TestableScreen(Screen.PrimaryScreen);
   }
}
