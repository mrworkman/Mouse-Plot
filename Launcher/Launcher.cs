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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Renfrew.Launcher {
   using NatSpeakInterop;
   using System.Diagnostics;

   public class Launcher {
      
      private NatSpeakService _service;
      private IntPtr _sitePtr;

      public Launcher() {
         _service = new NatSpeakService();
      }

      public void Launch() {
         try {
            _sitePtr = _service.CreateSiteObject();
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

         _service.Connect(_sitePtr);

         String profileName;
         Debug.WriteLine($"User Profile: {profileName = _service.GetCurrentUserProfileName()}");
         Debug.WriteLine($"Profile Path: {_service.GetUserDirectory(profileName)}");
         
         Trace.WriteLine("Success!");
         Trace.WriteLine("");
      }

      public void Terminate() {
         _service.ReleaseSiteObject(_sitePtr);
         _service.Disconnect();
      }

   }
}
