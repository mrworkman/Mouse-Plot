﻿// Project Renfrew
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
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace Renfrew.Launcher {
   using Core;
   using NatSpeakInterop;

   public class Launcher {

      private static Logger _logger = LogManager.GetCurrentClassLogger();

      private NatSpeakService _natSpeakService;
      private IntPtr _sitePtr;

      private bool _isTerminated = false;

      public Launcher() {
         _natSpeakService = new NatSpeakService();
      }

      public async void Launch() {

         Application.ApplicationExit += OnApplicationExit;
         Application.ThreadExit      += OnApplicationExit;

         _logger.Info("Renfrew starting...");

         try {
            _logger.Debug("Creating 'site object'...");
            _sitePtr = _natSpeakService.CreateSiteObject();

            _logger.Debug("Calling Connect()...");
            _natSpeakService.Connect(_sitePtr);

            _logger.Debug("Starting Core Application...");

            await CoreApplication.Instance.Start(_natSpeakService);

         } catch (Exception e) {
            _logger.Fatal(e, "");

            CoreApplication.Instance.ShowNotifyError(
               "There was an unexpected error that requires MousePlot to quit. 😞 See the log for more info."
            );

            // Wait long enough for the user to see the error message.
            Thread.Sleep(10000);

         } finally {
            // Kill the application
            Application.ExitThread();
            Environment.Exit(-1);
         }

      }

      private void OnApplicationExit(Object sender, EventArgs eventArgs) {
         Terminate();
      }

      private void Terminate() {
         if (_isTerminated == true)
            return;

         // Stop the application
         CoreApplication.Instance.Stop();

         try {
            // Disconnect from Dragon properly
            _natSpeakService.Disconnect();
            _natSpeakService.ReleaseSiteObject(_sitePtr);
         } catch (Exception e) {
            _logger.Fatal(e, "");
         }

         // Prevent re-entry
         _isTerminated = true;

         _logger.Info("Exiting.");
      }

   }
}
