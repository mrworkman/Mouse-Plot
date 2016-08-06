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

      //private NatLink4NetEngine engine;
      private NatSpeakService _service;
      private IntPtr _sitePtr;

      public Launcher() {
         _service = new NatSpeakService();
      }

      #region Launch Control
      public void Launch() {

         CreateSiteObject();

         Trace.WriteLine("Calling Connect()...");

         _service.Connect(_sitePtr);

         Trace.WriteLine("Success!");
         Trace.WriteLine("");

      }

      public void Terminate() {
         _service.Disconnect();
         ReleaseSiteObject();
      }
      #endregion

      #region Interop

      // TODO: Move to NatspeakInterop.

      private void CreateSiteObject() {
         Guid iServiceProviderGuid = new Guid("6d5140c1-7436-11ce-8034-00aa006009fa");
         Guid dgnSiteGuid = new Guid("dd100006-6205-11cf-ae61-0000e8a28647");
         
         Type dgnSiteType = Type.GetTypeFromCLSID(dgnSiteGuid);
         var dgnSite = Activator.CreateInstance(dgnSiteType);
         
         IntPtr pUnknown = Marshal.GetIUnknownForObject(dgnSite);

         try {
            // http://stackoverflow.com/a/22160325/1254575
            Marshal.QueryInterface(pUnknown, ref iServiceProviderGuid, out _sitePtr);
         } finally {
            Marshal.Release(pUnknown);
         }
      }

      private void ReleaseSiteObject() {
         Marshal.Release(_sitePtr);
      }
      #endregion

   }
}
