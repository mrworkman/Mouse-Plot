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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Renfrew.Core.Grammars.MousePlot {
   /// <summary>
   /// Interaction logic for PlotWindow.xaml
   /// </summary>
   public partial class PlotWindow : Window {
      public PlotWindow() {
         InitializeComponent();
      }

      #region Builtins
      public new void Close() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Hide();
         }));
      }

      public new void Focus() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Focus();
            Activate();
         }));
      }

      public new void Show() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Show();
         }));
      }

      public new void ShowDialog() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.ShowDialog();
         }));
      }
      #endregion

      private void Canvas_Loaded(Object sender, RoutedEventArgs e) {

         for (int i = 0; i < 36; i++) {
            for (int j = 0; j < 36; j++) {
               if (i == 0 && j == 0)
                  continue;

               var label = new Label {
                  Content = $"{GetDigitValue(j)}{GetDigitValue(i)}",
                  Height = 99,
                  Width = 99,
                  FontSize = 48,
                  FontFamily = new FontFamily("Consolas"),
                  Background = (i == 0 && j == 0) ? Brushes.Wheat : null,
                  Foreground = Brushes.Yellow,

                  HorizontalContentAlignment = HorizontalAlignment.Center,
                  VerticalContentAlignment = VerticalAlignment.Center,

                  Opacity = 0.7,

                  Margin = new Thickness(8 + i * 100, 8 + j * 100, 0, 0),
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

   }
}
