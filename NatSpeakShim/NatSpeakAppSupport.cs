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

namespace Renfrew.NatSpeakShim {
   using NatSpeakInterop;
   using NatSpeakInterop.Dragon.ComInterfaces;

   [ComVisible(true), Guid("bb5d23dd-e6ff-4571-84b1-6c6f70199bb8")]
   [ClassInterface(ClassInterfaceType.None)]
   public class NatSpeakAppSupport : IDgnAppSupport {
      private NatSpeakService _natSpeakService;

      public NatSpeakAppSupport() {
         _natSpeakService = new NatSpeakService();
      }

      #region Unneeded Dragon Methods
      public unsafe void AddProcess(UInt32 A_0, Char* A_1, Char* A_2, UInt32 A_3) {

      }

      public void EndProcess(UInt32 A_0) {

      }
      #endregion

      public unsafe void Register(IServiceProvider* site) {
         _natSpeakService.Connect(site);
      }

      public void UnRegister() {
         _natSpeakService.Disconnect();
      }
      
   }
}
