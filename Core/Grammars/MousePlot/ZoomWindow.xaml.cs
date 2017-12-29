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
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Renfrew.Core.Grammars.MousePlot {
   /// <summary>
   /// Interaction logic for ZoomWindow.xaml
   /// </summary>
   public partial class ZoomWindow : BaseWindow, IWindow {
      public ZoomWindow() {
         InitializeComponent();
      }

      public override void SetImage(Bitmap bitmap) {
         if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            using (var s = new MemoryStream()) {
               bitmap.Save(s, ImageFormat.Bmp);
               s.Seek(0, SeekOrigin.Begin);

               var bitmapImage = new BitmapImage();
               bitmapImage.BeginInit();
               bitmapImage.StreamSource = s;
               bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
               bitmapImage.EndInit();


               _screenshot.Fill = new ImageBrush(bitmapImage);
            }
         }));

      }
   }
}
