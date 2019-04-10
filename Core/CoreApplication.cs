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
using System.Threading;
using System.Threading.Tasks;
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

      private IGrammarService _grammarService;

      // System tray icon and menu
      private NotifyIcon _notifyIcon;
      private ContextMenuStrip _contextMenuStrip;

      private bool _isTerminated = false;

      #region Application Init
      private CoreApplication() {
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
         _notifyIcon = new NotifyIcon();
         _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         _notifyIcon.Text = "Mouse Plot";
         _notifyIcon.Icon = Resources.app2;

         // Create a new context menu for the system tray icon
         _contextMenuStrip = new ContextMenuStrip();

         // Assign the context menu to the system tray icon
         _notifyIcon.ContextMenuStrip = _contextMenuStrip;

         // Add menu items to system tray icon menu
         _contextMenuStrip.Items.Add("&Show Console", null, delegate(Object sender, EventArgs e) {
            ShowConsole();
         });
         _contextMenuStrip.Items.Add("-");
         _contextMenuStrip.Items.Add("E&xit Mouse Plot", null, OnApplicationExit);

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

      private void InitializeGrammarsFromAssembly(Assembly assembly) {

         // Get a list of all of the classes marked with the GrammarExportAttribute.
         var types = assembly.GetTypes()
            .Select(e => new {
               Type = e,
               Attr = e.GetCustomAttributes<GrammarExportAttribute>().FirstOrDefault()
            }).Where(e => e.Attr != null && e.Type.IsClass).ToList();

         _logger.Info($"Found {types.Count} grammars in assembly.");

         // Enumerate the available types in the assembly.
         foreach (var type in types) {
            var a = type.Attr;

            // Make sure the class extends from the Grammar base class.
            if (type.Type.IsSubclassOf(typeof(Grammar)) == false) {
               var fileName = Path.GetFileName(assembly.Location);

               _logger.Warn(
                  $"Class '{type.Type.FullName}' in {fileName} marked as exported grammar, " +
                  $"but it does not extend {typeof(Grammar).FullName}. Ignoring."
               );

               continue;
            }

            _logger.Info($"Initializing '{a.Name}'.");

            // TODO: Find a way to break a grammar's dependency on NatSpeakInterop
            var grammar = (Grammar) Activator.CreateInstance(
               type.Type, _grammarService
            );

            try {
               grammar.Initialize();
            } catch (Exception e) {
               _logger.Error();
               _logger.Error("---=== EXCEPTION CAUGHT ===---");
               _logger.Error(e);
               _logger.Error("---=== END OF EXCEPTION DETAIL ===---");
               continue;
            }

            _logger.Info($"Grammar, '{a.Name}', initialized.");
            _logger.Debug($"Grammar's words: {String.Join(", ", grammar.WordIds.Keys)}");

         }
      }

      private void LoadGrammars() {
         LoadInternalGrammars();
      }

      private void LoadInternalGrammars() {
         _logger.Info("Looking for internal (system) grammars.");

         InitializeGrammarsFromAssembly(Assembly.GetExecutingAssembly());
      }

      private void ShowConsole() {
         if (InfoConsole.Instance.IsVisible == false) {
            InfoConsole.Instance.ShowDialog();
            return;
         }

         InfoConsole.Instance.Focus();
      }

      public void ShowNotifyError(String message, String title = "Mouse Plot for NatSpeak") {
         _notifyIcon.ShowBalloonTip(2000, title, message, ToolTipIcon.Error);
      }

      public void ShowNotifyInfo(String message, String title = "Mouse Plot for NatSpeak") {
         _notifyIcon.ShowBalloonTip(2000, title, message, ToolTipIcon.Info);
      }

      public async Task Start(NatSpeakService natSpeakService) {
         _natSpeakService = natSpeakService ?? throw new ArgumentNullException(nameof(natSpeakService));

         ShowConsole();

         _logger.Info("Starting...");
         _logger.Info(
            $"Mouse Plot version: {Assembly.GetExecutingAssembly().GetName().Version}"
         );

         // Get a reference to the GrammarService instance.
         _grammarService = _natSpeakService.GrammarService;
         _grammarService.GrammarSerializer = new GrammarSerializer();

         _logger.Info("Querying Dragon Naturally Speaking...");

         _logger.Info($"Dragon Version: {_natSpeakService.GetDragonVersion()}");

         String profileName;
         String profilePath;

         for (var notified = false;; notified = true) {

            profileName = _natSpeakService.GetCurrentUserProfileName();

            // If a profile name could not be retrieved, then either the user
            // hasn't selected a profile yet, or NatSpeak hasn't been started.
            if (profileName == null) {

               if (notified == false) {
                  _logger.Info("Could not load profile. Will try again.");
                  ShowNotifyInfo("Could not load profile. Will try again.");
               }

               // Wait for a bit.
               await Task.Delay(2000);

               // Try again...
               continue;
            }

            // Get the file-system location of the user's profile (for informational purposes).
            profilePath = _natSpeakService.GetUserDirectory(profileName);

            // Profile found.
            break;
         }

         _logger.Info($"Dragon Profile Loaded: {profileName}");
         _logger.Info($"Dragon Profile Path: {profilePath}");

         LoadGrammars();

         CloseConsole();
      }
   }
}
