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

#pragma once

// Native types are private by default with /clr
#pragma make_public(::IServiceProvider)

#include "Stdafx.h"

namespace Renfrew::NatSpeakInterop {
   using namespace System;

   public ref class NatSpeakService {
      private:
         ::IServiceProvider *_piServiceProvider;

         DWORD key; // <-- Investigate the purpose of this

      private:
         void RegisterEngineSink();
         void RegisterSpeechServiceSinks();

      public:
         NatSpeakService();
         ~NatSpeakService();

         void Connect(void *site);
         void Connect(IntPtr site);
         void Connect(::IServiceProvider *site);

         void Disconnect();

   };

   NatSpeakService::NatSpeakService() {
      key = 0;
   }

   NatSpeakService::~NatSpeakService() {

   }

   void NatSpeakService::Connect(void *site) {
      Connect(reinterpret_cast<::IServiceProvider*>(site));
   }

   void NatSpeakService::Connect(IntPtr site) {
      Connect(site.ToPointer());
   }

   void NatSpeakService::Connect(::IServiceProvider *site) {
      _piServiceProvider = site;

      if (_piServiceProvider == nullptr)
         throw gcnew ArgumentNullException();

      try {
         RegisterEngineSink();
         RegisterSpeechServiceSinks();
      } catch (...) {
         
      }

   }

   void NatSpeakService::Disconnect() {
      throw gcnew System::NotImplementedException();
   }

   void NatSpeakService::RegisterEngineSink() {
      throw gcnew System::NotImplementedException();
   }

   void NatSpeakService::RegisterSpeechServiceSinks() {
      throw gcnew System::NotImplementedException();
   }
}