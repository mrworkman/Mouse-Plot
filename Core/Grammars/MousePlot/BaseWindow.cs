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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Renfrew.Core.Grammars.MousePlot {
   public abstract class BaseWindow : Window, IWindow {
      private readonly String DefaultColourName = "Yellow";

      public BaseWindow() {

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

      public new double Height {
         get {
            return (double)Dispatcher.Invoke(DispatcherPriority.Background, new Func<double>(() => base.Height));
         }
         set => base.Height = value;
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

      public new double Width {
         get {
            return (double)Dispatcher.Invoke(DispatcherPriority.Background, new Func<double>(() => base.Width));
         }
         set => base.Width = value;
      }
      #endregion

      public virtual void Move(Double x, Double y) {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            Left = x;
            Top = y;
         }));
      }

      public virtual void DrawMouseCursor(Bitmap bitmap, int x, int y) { }

      public virtual void SetColour(GridColour colour) {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            try {
               SetColour(colour.ToString());
            } catch (IOException e) {
               SetColour(DefaultColourName);
            }
         }));
      }

      private void SetColour(String c) {
         var merged = this.Resources.MergedDictionaries;

         merged.Clear();

         merged.Add(new ResourceDictionary() {
            Source = new Uri(
               $"Core;component/Grammars/MousePlot/Themes/{c}.xaml",
               UriKind.RelativeOrAbsolute
            )
         });
      }

      public virtual void SetImage(Bitmap bitmap) { }
   }
}
