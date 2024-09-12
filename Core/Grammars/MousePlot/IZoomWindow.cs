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

using System;
using System.Drawing;

namespace Renfrew.Core.Grammars.MousePlot {
   public interface IZoomWindow : IWindow {
      void SetScaleMultiplier(double multiplier);
      void SetSource(Int32 x, Int32 y, Int32 width, Int32 height);
      void SetSource(Rectangle sourceRectangle);
   }
}
