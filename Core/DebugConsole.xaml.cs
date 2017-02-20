// Project Renfrew
// Copyright(C) 2017  Stephen Workman (workman.stephen@gmail.com)
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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Renfrew.Core {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class DebugConsole : Window {

      private const int GWL_STYLE = -16;
      private const int WS_SYSMENU = 0x80000;
      [DllImport("user32.dll", SetLastError = true)]
      private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
      [DllImport("user32.dll")]
      private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
      
      public DebugConsole() {
          InitializeComponent();
      }

      public new void Close() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Close();
         }));
      }

      private void DebugConsole_OnLoaded(Object sender, RoutedEventArgs e) {
         //var hwnd = new WindowInteropHelper(this).Handle;
         //SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
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

      public void WriteLine(String message = "") {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            RtbConsole.AppendText($"{message}\r\n");
         }));
      }
      
   }
}
