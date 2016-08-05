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
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Renfrew.Launcher {
   class LauncherApplicationContext : ApplicationContext {
      private Launcher _launcher;
      private ResourceManager _resourceManager;

      private NotifyIcon _notifyIcon;
      private ContextMenuStrip _contextMenuStrip;
      private ToolStripMenuItem _exitMenuItem;

      // Constructor
      public LauncherApplicationContext(Launcher launcher) {
         _launcher = launcher;

         if (launcher == null)
            throw new ArgumentNullException();

         // Create a new system tray icon
         _notifyIcon = new NotifyIcon();

         // Get a handle to the managed resources file
         _resourceManager = new ResourceManager(
            "NatLink4Net.Resources",
            Assembly.GetExecutingAssembly()
         );

         InitializeComponent();
      }

      private void InitializeComponent() {
         // Add add exit event handler
         Application.ApplicationExit += new EventHandler(OnApplicationExit);
         Application.ThreadExit += new EventHandler(OnApplicationExit);

         // Add menu items to system tray icon menu
         _exitMenuItem = new ToolStripMenuItem();
         _exitMenuItem.Text = "E&xit";
         _exitMenuItem.Click += new EventHandler(ExitMenu_OnClick);

         // Create a new context menu for the system tray icon
         _contextMenuStrip = new ContextMenuStrip();
         _contextMenuStrip.Items.Add(_exitMenuItem);

         _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         _notifyIcon.Text = "Project Renfrew";
         _notifyIcon.Icon = Resources.SystemTrayIcon;

         // Assigned the context menu to the system tray icon
         _notifyIcon.ContextMenuStrip = _contextMenuStrip;

         _notifyIcon.Visible = true;

         // Let's fire up the Quattro!
         _launcher.Launch();
      }

      private void OnApplicationExit(Object sender, EventArgs e) {
         _notifyIcon.Visible = false;
         _launcher.Terminate();
      }

      private void ExitMenu_OnClick(Object sender, EventArgs e) {
         Application.Exit();
         Application.ExitThread();
      }
   }
}
