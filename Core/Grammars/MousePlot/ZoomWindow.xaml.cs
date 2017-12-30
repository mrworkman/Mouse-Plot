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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;

namespace Renfrew.Core.Grammars.MousePlot {
   /// <summary>
   /// Interaction logic for ZoomWindow.xaml
   /// </summary>
   public partial class ZoomWindow : BaseWindow, IWindow {
      public ZoomWindow() {
         InitializeComponent();
      }

      private void Canvas_Loaded(Object sender, RoutedEventArgs e) {

         for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
               if (i == 0 && j == 0)
                  continue;

               var label = new Label {
                  Style = Resources["DigitLabel"] as Style,
                  Content = $"{GetDigitValue(j)}{GetDigitValue(i)}",
                  Margin = new Thickness(5 + i * 33.3333, 5 + j * 33.3333, 0, 0),
               };

               _mainCanvas.Children.Add(label);
            }
         }
      }
      private String GetDigitValue(int i) {
         if (i < 10)
            return ((char)('0' + i)).ToString();
         return ((char)('A' + i - 10)).ToString();
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

      public override void DrawMouseCursor(Bitmap bitmap, int x, int y) {
         if (bitmap == null)
            throw new ArgumentNullException(nameof(bitmap));

         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            using (var s = new MemoryStream()) {
               bitmap.Save(s, ImageFormat.Png);
               s.Seek(0, SeekOrigin.Begin);

               var bitmapImage = new BitmapImage();
               bitmapImage.BeginInit();
               bitmapImage.StreamSource = s;
               bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
               bitmapImage.EndInit();

               _mouseLabel.Background = new ImageBrush(bitmapImage);
               _mouseLabel.Margin = new Thickness(x + 26, y + 26, 0, 0);
            }
         }));

      }
   }
}
