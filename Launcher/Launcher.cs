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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Renfrew.Launcher {
   using Core;
   using NatSpeakInterop;
   using System.Diagnostics;

   public class Launcher {
      
      private NatSpeakService _natSpeakService;
      private IntPtr _sitePtr;

      private bool _isTerminated = false;

      public Launcher() {
         _natSpeakService = new NatSpeakService();
      }

      public void Launch() {

         Application.ApplicationExit += OnApplicationExit;
         Application.ThreadExit      += OnApplicationExit;

         try {
            _sitePtr = _natSpeakService.CreateSiteObject();
         } catch (COMException e) {
            Trace.WriteLine($"COM Exception: {e.Message}");

            MessageBox.Show(
               "Could not connect to Dragon Naturally Speaking. Please " +
              $"make sure it's installed correctly!\r\r {e.Message}", 
               "COM Error", 
               MessageBoxButtons.OK, 
               MessageBoxIcon.Error
            );

            Environment.Exit(-1);
         }

         Trace.WriteLine("Calling Connect()...");

         _natSpeakService.Connect(_sitePtr);

         CoreApplication.Instance.Start(_natSpeakService);

         Trace.WriteLine("Success!");
         Trace.WriteLine("");
      }

      private void OnApplicationExit(Object sender, EventArgs eventArgs) {
         Terminate();
      }

      private void Terminate() {
         if (_isTerminated == true)
            return;

         CoreApplication.Instance.Stop();

         _natSpeakService.Disconnect();
         _natSpeakService.ReleaseSiteObject(_sitePtr);

         _isTerminated = true;
      }

   }
}
