// Project Renfrew
// Copyright(C) 2016  Stephen Workman (workman.stephen@gmail.com)
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
using System.Windows.Forms;

using NLog;

namespace Renfrew.Launcher {
   public class Program {
      private static Logger _logger = LogManager.GetCurrentClassLogger();

      [STAThread]
      public static void Main(params String[] args) {
         try {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            Application.Run(new LauncherApplicationContext(
               new Launcher()
            ));
         } catch (Exception e) {
            _logger.Fatal(e, "An unexpected exception occurred.");

            MessageBox.Show(
               "An unexpected exception occurred. Please see the log for more information.",
               "Mouse Plot for NatSpeak",
               MessageBoxButtons.OK, MessageBoxIcon.Error
            );

         }
      }
   }
}
