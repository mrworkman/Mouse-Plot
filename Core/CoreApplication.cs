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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms;

using Renfrew.Core.Grammars;
using Renfrew.Core.Properties;
using Renfrew.Grammar.Dragon;
using Renfrew.Grammar.FluentApi;
using Renfrew.NatSpeakInterop.Sinks;

using Application = System.Windows.Forms.Application;

namespace Renfrew.Core {
   using Grammar;
   using NatSpeakInterop;

   public class CoreApplication : ApplicationContext {
      private static CoreApplication _instance;

      // Main interface to dragon
      private NatSpeakService _natSpeakService;

      // Debug console
      private DebugConsole _console;
      private Thread       _consoleThread;

      // System tray icon and menu
      private readonly NotifyIcon _notifyIcon;
      private ContextMenuStrip    _contextMenuStrip;
      
      private bool _isTerminated = false;

      #region Application Init
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
      #endregion

      #region Application Termination
      private void OnApplicationExit(Object sender = null, EventArgs e = null) {
         if (_isTerminated == true)
            return;

         _notifyIcon.Visible = false;

         _console.Close();

         Application.Exit();
         Application.ExitThread();

         _isTerminated = true;
      }

      public void Stop() {
         OnApplicationExit();
      }
      #endregion

      private DebugConsole DebugConsole => _console;

      private void InitializeDebugConsole() {
         if (_console != null)
            return;

         // The debug console runs on a thread of its own
         _consoleThread = new Thread(() => {
            _console = new DebugConsole();
            _console.ShowDialog();

            Dispatcher.Run();
         });

         // Start the thread in the background. STA is needed to support WPF
         _consoleThread.SetApartmentState(ApartmentState.STA);
         _consoleThread.IsBackground = true;
         _consoleThread.Start();

         // Wait a spell
         while (_console == null)
            Thread.Sleep(100);
      }

      private void LoadGrammars() {

         IGrammarService grammarService = _natSpeakService.GrammarService;

         grammarService.GrammarSerializer = new GrammarSerializer();

         var currentAssembly = Assembly.GetExecutingAssembly();

         var types = currentAssembly.GetTypes()
            .Select(e => new {
               Type = e,
               Attr = e.GetCustomAttributes<GrammarExportAttribute>().FirstOrDefault()
            }).Where(e => e.Attr != null).ToList();

         DebugConsole.WriteLine();
         DebugConsole.WriteLine($"Found {types.Count} system grammars:");

         foreach (var type in types) {
            var a = type.Attr;

            DebugConsole.WriteLine($"{type.Type}: (Name: {a.Name}, Description: {a.Description})");

            var grammar = (Grammar) Activator.CreateInstance(type.Type);

            try {
               grammar.Initialize();
            } catch (Exception e) {
               DebugConsole.WriteLine($"{e.Message} {e.StackTrace}");
            }

            DebugConsole.WriteLine($"Grammar's words: {String.Join(", ", grammar.Words)}");

            grammarService.LoadGrammar(grammar);
            grammarService.ActivateRule(grammar, (IntPtr) null, "test_rule");

         }
      }

      public void Start(NatSpeakService natSpeakService) {

         if (natSpeakService == null)
            throw new ArgumentNullException(nameof(natSpeakService));

         _natSpeakService = natSpeakService;

         InitializeDebugConsole();

         DebugConsole.WriteLine("Querying Dragon Naturally Speaking...");
         
         try {
            var profileName = _natSpeakService.GetCurrentUserProfileName();
            var profilePath = _natSpeakService.GetUserDirectory(profileName);

            DebugConsole.WriteLine($"Dragon Profile Loaded: {profileName}");
            DebugConsole.WriteLine($"Dragon Profile Path: {profilePath}");

            LoadGrammars();

         } catch (COMException e) {
            DebugConsole.WriteLine($"Fatal error: {e.Message}! Cannot continue!");
            throw;
         }
      }
   }
}
