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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;

namespace Renfrew.Core.Grammars.MousePlot {
   /// <summary>
   /// Interaction logic for PlotWindow.xaml
   /// </summary>
   public partial class PlotWindow : BaseWindow, IWindow {
      public PlotWindow() {
         InitializeComponent();
      }

      private void Canvas_Loaded(Object sender, RoutedEventArgs e) {

         for (int i = 0; i < 36; i++) {
            for (int j = 0; j < 36; j++) {
               if (i == 0 && j == 0)
                  continue;

               var label = new Label {
                  Style = Resources["DigitLabel"] as Style,
                  Content = $"{GetDigitValue(j)}{GetDigitValue(i)}",
                  Margin = new Thickness(2 + i * 100, 2 + j * 100, 0, 0),
               };

               //label.MouseEnter += Label_MouseEnter;
               //label.MouseLeave += Label_MouseLeave;

               mainCanvas.Children.Add(label);
            }
         }
      }

      private String GetDigitValue(int i) {
         if (i < 10)
            return ((char)('0' + i)).ToString();
         return ((char)('A' + i - 10)).ToString();
      }

      public override void Move(Double x, Double y) {
         Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => {
            WindowState = WindowState.Normal;
            Left = x;
            Top = y;
            WindowState = WindowState.Maximized;
         })).Wait();
      }
   }
}
