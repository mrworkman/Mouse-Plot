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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using NLog;
using NLog.Fluent;

using Renfrew.Core.Properties;

using Application = System.Windows.Forms.Application;

namespace Renfrew.Core {
   using Grammar;
   using NatSpeakInterop;

   public class CoreApplication : ApplicationContext {
      private static Logger _logger = LogManager.GetCurrentClassLogger();

      private static CoreApplication _instance;

      // Main interface to dragon
      private NatSpeakService _natSpeakService;

      // System tray icon and menu
      private readonly NotifyIcon _notifyIcon;
      private ContextMenuStrip    _contextMenuStrip;

      private bool _isTerminated = false;

      #region Application Init
      private CoreApplication() {
         _notifyIcon = new NotifyIcon();
         InitializeComponent();
      }

      private void CloseConsole() {
         InfoConsole.Instance.Close();
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
         _contextMenuStrip.Items.Add("&Show Console", null, delegate(Object sender, EventArgs e) {
            ShowConsole();
         });
         _contextMenuStrip.Items.Add("-");
         _contextMenuStrip.Items.Add("E&xit Project Renfrew", null, OnApplicationExit);

         _notifyIcon.Visible = true;
      }

      public static CoreApplication Instance => _instance ?? (_instance = new CoreApplication());

      #endregion

      #region Application Termination
      private void OnApplicationExit(Object sender = null, EventArgs e = null) {
         if (_isTerminated == true)
            return;

         _notifyIcon.Visible = false;

         CloseConsole();

         Application.Exit();
         Application.ExitThread();

         _isTerminated = true;
      }

      public void Stop() {
         OnApplicationExit();
      }
      #endregion

      private void LoadGrammars() {
         IGrammarService grammarService = _natSpeakService.GrammarService;

         grammarService.GrammarSerializer = new GrammarSerializer();

         var currentDirectory = Directory.GetCurrentDirectory();

         foreach (var f in Directory.EnumerateFiles(currentDirectory, "*.dll")) {
            _logger.Info(f);
            _logger.Info(Directory.GetCurrentDirectory());

            var path = Path.Combine(Directory.GetCurrentDirectory(), f);

            try {
               var a = Assembly.LoadFrom(path);

            } catch (FileLoadException e) {
               _logger.Error(e, "NAWP");
            }
         }

         var currentAssembly = Assembly.GetExecutingAssembly();

         var types = currentAssembly.GetTypes()
            .Select(e => new {
               Type = e,
               Attr = e.GetCustomAttributes<GrammarExportAttribute>().FirstOrDefault()
            }).Where(e => e.Attr != null).ToList();

         _logger.Info();
         _logger.Info($"Found {types.Count} system grammars:");

         foreach (var type in types) {
            var a = type.Attr;

            _logger.Info($"{type.Type}: (Name: {a.Name}, Description: {a.Description})");

            var grammar = (Grammar) Activator.CreateInstance(
               type.Type, grammarService
            );

            try {
               grammar.Initialize();
            } catch (Exception e) {
               _logger.Error();
               _logger.Error("---=== EXCEPTION CAUGHT ===---");
               _logger.Error($"{e.GetType().Name}: {e.Message}");
               _logger.Error(e.StackTrace);
               _logger.Error("---=== END OF EXCEPTION DETAIL ===---");
               continue;
            }

            _logger.Info($"Grammar's words: {String.Join(", ", grammar.WordIds.Keys)}");

         }
      }

      private void ShowConsole() {
         if (InfoConsole.Instance.IsVisible == false) {
            InfoConsole.Instance.ShowDialog();
            return;
         }

         InfoConsole.Instance.Focus();
      }

      public void Start(NatSpeakService natSpeakService) {
         _natSpeakService = natSpeakService ?? throw new ArgumentNullException(nameof(natSpeakService));

         ShowConsole();

         _logger.Info("Querying Dragon Naturally Speaking...");

         try {
            var profileName = _natSpeakService.GetCurrentUserProfileName();
            var profilePath = _natSpeakService.GetUserDirectory(profileName);

            _logger.Info($"Dragon Profile Loaded: {profileName}");
            _logger.Info($"Dragon Profile Path: {profilePath}");

            LoadGrammars();

         } catch (COMException e) {
            _logger.Fatal(e, "Cannot continue.");
            throw;
         }
      }
   }
}
