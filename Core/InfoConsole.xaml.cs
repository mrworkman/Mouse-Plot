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
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using NLog;
using NLog.Targets;

namespace Renfrew.Core {

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class InfoConsole : Window {
      private static Logger _logger = LogManager.GetCurrentClassLogger();

      /// <summary>
      /// The singleton instance of <code>InfoConsole</code>.
      /// </summary>
      private static InfoConsole _instance;

      /// <summary>
      /// The <code>InfoConsole</code> runs on its own thread.
      /// </summary>
      private static Thread _thread;

      /// <summary>
      /// Prevents multiple threads from initializing the singleton.
      /// </summary>
      private static readonly object Lock = new object();

      /// <summary>
      /// Registers the InfoConsoleTarget NLog target.
      /// </summary>
      static InfoConsole() {
         Target.Register<InfoConsoleTarget>("InfoConsole");
      }

      /// <summary>
      /// Initializes the window.
      /// </summary>
      private InfoConsole() {
         InitializeComponent();
         RtbConsole.FontFamily = new FontFamily("Consolas, Courier New");
         RtbConsole.FontSize = 14;
      }

      /// <summary>
      /// The instance of the InfoConsole.
      /// </summary>
      public static InfoConsole Instance {
         get {
            lock (Lock) {
               if (_instance == null) {
                  _logger.Debug("Starting application console thread.");

                  // Create the console on a new thread.
                  _thread = new Thread(() => {
                     _instance = new InfoConsole();

                     _logger.Debug("Thread started.");

                     Dispatcher.Run();
                  });

                  // Start the thread in the background. STA is needed to support WPF
                  _thread.SetApartmentState(ApartmentState.STA);
                  _thread.IsBackground = true;
                  _thread.Start();

                  // Wait for the thread to start
                  while (_instance == null) {
                     _logger.Debug("Waiting...");
                     Thread.Sleep(100);
                  }

                  // Set NLog target instance
                  InfoConsoleTarget.InfoConsole = _instance;
               }
            }

            return _instance;
         }
      }

      /// <inheritdoc cref="Window.Close" />
      public new void Close() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Close();
         }));
      }

      /// <inheritdoc cref="Window.Focus" />
      public new void Focus() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Focus();
            Activate();
         }));
      }

      /// <inheritdoc />
      protected override void OnClosing(CancelEventArgs e) {
         e.Cancel = true;
         Visibility = Visibility.Hidden;
      }

      /// <inheritdoc cref="Window.Show" />
      public new void Show() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.Show();
         }));
      }

      /// <inheritdoc cref="Window.ShowDialog" />
      public new void ShowDialog() {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            base.ShowDialog();
         }));
      }

      /// <summary>
      /// Print a message to the console.
      /// </summary>
      /// <param name="message">The message to be printed.</param>
      private void WriteText(String message, Brush brush, FontWeight fontWeight) {
         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {

            var end = RtbConsole.Document.ContentEnd;
            var r = new TextRange(end, end) {
               Text = message
            };
            
            r.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            r.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
            
            RtbConsole.ScrollToEnd();
         }));
      }

      /// <summary>
      /// A custom NLog target. Sends log messages into the <see cref="InfoConsole" />
      /// </summary>
      [Target("InfoConsole")]
      private class InfoConsoleTarget : TargetWithLayout {
         public static InfoConsole InfoConsole { get; set; }

         /// <summary>
         /// Intercept the log message.
         /// </summary>
         /// <param name="logEvent">The log message object.</param>
         protected override void Write(LogEventInfo logEvent) {
            FontWeight weight = FontWeights.Regular;
            Brush colour = Brushes.Black;

            if (logEvent.Level >= LogLevel.Error) {
               weight = FontWeights.Bold;
               colour = Brushes.Red;
            }

            if (logEvent.Level == LogLevel.Warn) {
               weight = FontWeights.Bold;
               colour = Brushes.DarkOrange;
            }

            InfoConsole?.WriteText(
               Layout.Render(logEvent), 
               colour, 
               weight
            );
         }
      }

   }


}
