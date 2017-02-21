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
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms;

using Renfrew.Core.Properties;

using Application = System.Windows.Forms.Application;

namespace Renfrew.Core {
   using NatSpeakInterop;

   public class CoreApplication : ApplicationContext {
      private static CoreApplication _instance;
      
      private DebugConsole _console;

      private readonly NotifyIcon _notifyIcon;
      private ContextMenuStrip _contextMenuStrip;
      private ToolStripMenuItem _exitMenuItem;
      private Thread _thread;

      private bool _isTerminated = false;

      private CoreApplication() {
         _notifyIcon = new NotifyIcon();

         InitializeComponent();
      }

      private void InitializeComponent() {
         // Add application exit event handler(s)
         Application.ApplicationExit += OnApplicationExit;
         Application.ThreadExit      += OnApplicationExit;

         // Set up the system tray icon
         _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         _notifyIcon.Text = "Project Renfrew";
         _notifyIcon.Icon = Resources.SystemTrayIcon;

         // Create a new context menu for the system tray icon
         _contextMenuStrip = new ContextMenuStrip();

         // Assign the context menu to the system tray icon
         _notifyIcon.ContextMenuStrip = _contextMenuStrip;

         // Add menu items to system tray icon menu
         _contextMenuStrip.Items.Add("&Show Debug Console", null, delegate(Object sender, EventArgs e) {
            if (_console.Visibility == Visibility.Hidden)
               _console.ShowDialog();
            else
               _console.Focus();
         });
         _contextMenuStrip.Items.Add("-");
         _contextMenuStrip.Items.Add("E&xit Project Renfrew", null, OnApplicationExit);
         
         _notifyIcon.Visible = true;

      }

      public static CoreApplication Instance {
         get {
            if (_instance == null)
               _instance = new CoreApplication();
            return _instance;
         }
      }

      private void OnApplicationExit(Object sender = null, EventArgs e = null) {
         if (_isTerminated == true)
            return;

         _notifyIcon.Visible = false;

         _console.Close();
         
         Application.Exit();
         Application.ExitThread();

         _isTerminated = true;
      }

      public void Start(NatSpeakService natSpeakService) {
         if (_console != null)
            return;
         
         _thread = new Thread(() => {
            _console = new DebugConsole();
            _console.ShowDialog();

            Dispatcher.Run();
         });

         _thread.SetApartmentState(ApartmentState.STA);
         _thread.IsBackground = true;
         _thread.Start();

         while (_console == null)
            Thread.Sleep(100);

         _console.WriteLine("Querying Dragon Naturally Speaking...");

         try {
            var profileName = natSpeakService.GetCurrentUserProfileName();
            var profilePath = natSpeakService.GetUserDirectory(profileName);

            _console.WriteLine($"Dragon Profile Loaded: {profileName}");
            _console.WriteLine($"Dragon Profile Path: {profilePath}");
         } catch (COMException e) {
            _console.WriteLine($"Fatal error: {e.Message}! Cannot continue!");
            throw;
         }

         //_console.Dispatcher.BeginInvoke(
         //   DispatcherPriority.Background, new Action(() => { }));

      }

      public void Stop() {
         OnApplicationExit();
      }

   }
}
