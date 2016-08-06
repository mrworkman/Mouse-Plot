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
   using namespace System::Diagnostics;
   using namespace System::Runtime::InteropServices::ComTypes;

   using namespace Renfrew::Helpers;
   using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;
   using namespace Renfrew::NatSpeakInterop::Sinks;

   public ref class NatSpeakService {
      private:
         ::IServiceProvider *_piServiceProvider;

         ISrCentral ^_isrCentral = nullptr;
         IDgnSpeechServices  ^_idgnSpeechServices = nullptr;
         IDgnSSvcOutputEvent ^_idgnSSvcOutputEvent = nullptr;
         IDgnSSvcInterpreter ^_idgnSSvcInterpreter = nullptr;

         DWORD key;
         
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
      if (site == nullptr)
         throw gcnew ArgumentNullException();

      _piServiceProvider = site;

      ISrCentral ^*ptr = ComHelper::QueryService<IDgnDictate^, ISrCentral^>(_piServiceProvider);
      _isrCentral = (ISrCentral^) Marshal::GetObjectForIUnknown(IntPtr(ptr));

      RegisterEngineSink();
      RegisterSpeechServiceSinks();
   }

   void NatSpeakService::Disconnect() {
      throw gcnew System::NotImplementedException();
   }

   void NatSpeakService::RegisterEngineSink() {
      // Create an engine sink
      ISrNotifySink ^isrNotifySink = gcnew SrNotifySink();

      // https://msdn.microsoft.com/en-us/library/1dz8byfh.aspx
      pin_ptr<DWORD> _key = &key;

      IntPtr i = Marshal::GetIUnknownForObject(isrNotifySink);

      // try {
      //    Apparently does not work on Dragon 12 even though the interface is registered...
      //   _isrCentral->Register(piDgnSREngineNotifySink, __uuidof(IDgnSREngineNotifySink^), _key);
      // } catch () {
         // So far this works on Dragon 12:
         _isrCentral->Register(i, __uuidof(ISrNotifySink^), _key);
      // }

      Marshal::Release(i);
   }

   void NatSpeakService::RegisterSpeechServiceSinks() {
      IDgnSSvcActionNotifySink ^playbackSink = gcnew SSvcActionNotifySink();

      // Speech Services
      IDgnSpeechServices ^*ptr = ComHelper::QueryService<ISpchServices^, IDgnSpeechServices^>(_piServiceProvider);
      _idgnSpeechServices = (IDgnSpeechServices^) Marshal::GetObjectForIUnknown(IntPtr(ptr));
      _idgnSSvcOutputEvent = (IDgnSSvcOutputEvent ^) _idgnSpeechServices;
      _idgnSSvcInterpreter = (IDgnSSvcInterpreter ^) _idgnSSvcOutputEvent;

      IntPtr i = Marshal::GetIUnknownForObject(playbackSink);

      _idgnSSvcOutputEvent->Register(i);
      _idgnSSvcInterpreter->Register(i);
      
      Marshal::Release(i);
   }
}