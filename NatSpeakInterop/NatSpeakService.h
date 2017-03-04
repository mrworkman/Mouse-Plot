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

// NOTE:
//   Portions (c) Copyright 1997-1999 by Joel Gould.

#pragma once

// Native types are private by default with /clr
#pragma make_public(::IServiceProvider)

#include "Stdafx.h"
#include <vcclr.h>

#define IServiceProviderGUID "6d5140c1-7436-11ce-8034-00aa006009fa"
#define IDgnSiteGUID "dd100006-6205-11cf-ae61-0000e8a28647"

#define  E_BUFFERTOOSMALL     0x8004020D
#define  SRERR_NOUSERSELECTED 0x8004041A

#define  SRERR_GRAMMARERROR   0x80040416

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
         IDgnSpeechServices  ^_idgnSpeechServices  = nullptr;
         IDgnSrEngineControl ^_idgnSrEngineControl = nullptr;
         IDgnSSvcOutputEvent ^_idgnSSvcOutputEvent = nullptr;
         IDgnSSvcInterpreter ^_idgnSSvcInterpreter = nullptr;

         DWORD _key;
         
      private:
         void InitializeIsrCentral(::IServiceProvider *);
         void InitializeSpeechServicesInterfaces();
         void InitializeSrEngineControlInterface();
         void RegisterEngineSink();
         void RegisterPlaybackSink();
         
      public:
         NatSpeakService();
         ~NatSpeakService();

         void Connect(IntPtr site);
         void Connect(::IServiceProvider *site);
         void Disconnect();

         IntPtr CreateSiteObject();
         void ReleaseSiteObject(IntPtr sitePtr);

         String ^GetCurrentUserProfileName();
         String ^GetUserDirectory(String ^userProfile);

         void LoadGrammar(array<byte> ^serializedGrammarBytes, ISrGramNotifySink ^iSrGramNotifySink);
   };

   NatSpeakService::NatSpeakService() {
      _key = 0;
   }

   NatSpeakService::~NatSpeakService() { }

   void NatSpeakService::Connect(IntPtr serviceProviderPtr) {
      Connect(reinterpret_cast<::IServiceProvider*>(serviceProviderPtr.ToPointer()));
   }

   void NatSpeakService::Connect(::IServiceProvider *pServiceProvider) {
      if (pServiceProvider == nullptr)
         throw gcnew ArgumentNullException();

      InitializeIsrCentral(pServiceProvider);
      RegisterEngineSink();
      InitializeSrEngineControlInterface();
      InitializeSpeechServicesInterfaces();
      RegisterPlaybackSink();

   }

   IntPtr NatSpeakService::CreateSiteObject() {
      IntPtr sitePtr;
      
      Guid iServiceProviderGuid(IServiceProviderGUID);
      Type ^type = Type::GetTypeFromCLSID(Guid(IDgnSiteGUID));

      Object ^idgnSite = Activator::CreateInstance(type);      
      IntPtr i = Marshal::GetIUnknownForObject(idgnSite);

      try {
         // http://stackoverflow.com/a/22160325/1254575
         Marshal::QueryInterface(i, iServiceProviderGuid, sitePtr);
      } finally {
         Marshal::Release(i);
      }

      return sitePtr;
   }

   void NatSpeakService::Disconnect() {
	   Trace::WriteLine(__FUNCTION__);
     
      if (_key != 0) {
         _isrCentral->UnRegister(_key);
         _key = 0;
      }

      Marshal::ReleaseComObject(_isrCentral);
      Marshal::ReleaseComObject(_idgnSpeechServices);
   }

   /// <summary>
   /// Gets the the profile name of the current Dragon user.
   /// </summary>
   /// <returns>The dragon profile name, if available. null otherwise.</returns>
   String ^NatSpeakService::GetCurrentUserProfileName() {
      ISrSpeaker ^isrSpeaker = (ISrSpeaker ^) _isrCentral;
      
      DWORD dwSize, dwNeeded = 0;
      PWSTR profileName = nullptr;

      // Find out how big our buffer should be
      try {
         isrSpeaker->Query(profileName, 0, &dwNeeded);
      } catch (COMException ^e) {
         if (!(e->ErrorCode == EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT ||
               e->ErrorCode == E_BUFFERTOOSMALL || e->ErrorCode == SRERR_NOUSERSELECTED)) {
            throw;
         }
      }

      if (dwNeeded == 0)
         return nullptr;

      // Allocate a buffer to hold the string
      dwSize = dwNeeded;
      profileName = new WCHAR[dwSize];

      // Get the string
      isrSpeaker->Query(profileName, dwSize, &dwNeeded);

      try {
         return gcnew String(profileName);
      } finally {
         delete profileName;
      }
   }

   /// <summary>
   /// Gets the the file system path to the specified Dragon user's profile directory.
   /// </summary>
   /// <param name="userProfile">The name of the user profile to look up.</param>
   /// <returns>The dragon profile path, if available. null otherwise.</returns>
   String ^NatSpeakService::GetUserDirectory(String ^userProfile) {
      IDgnSrSpeaker ^idgnSrSpeaker = (IDgnSrSpeaker ^) _isrCentral;

      DWORD dwSize, dwNeeded = 0;
      PWSTR path = nullptr;

      pin_ptr<const WCHAR> user = PtrToStringChars(userProfile);

      // Find out how big our buffer should be
      try {
         idgnSrSpeaker->GetSpeakerDirectory(user, path, 0, &dwNeeded);
      } catch (COMException ^e) {
         if (!(e->ErrorCode == EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT ||
               e->ErrorCode == E_BUFFERTOOSMALL || e->ErrorCode == SRERR_NOUSERSELECTED)) {
            throw;
         }
      }

      if (dwNeeded == 0)
         return nullptr;

      // Allocate a buffer to hold the string
      dwSize = dwNeeded;
      path = new WCHAR[dwSize];

      // Get the string
      idgnSrSpeaker->GetSpeakerDirectory(user, path, dwSize, &dwNeeded);

      try {
         return gcnew String(path) + "\\current";
      } finally {
         delete path;
      }
   }

   void NatSpeakService::InitializeIsrCentral(::IServiceProvider *pServiceProvider) {
      _piServiceProvider = pServiceProvider;

      ISrCentral ^*ptr = ComHelper::QueryService<IDgnDictate^, ISrCentral^>(_piServiceProvider);
      _isrCentral = (ISrCentral^)Marshal::GetObjectForIUnknown(IntPtr(ptr));

      Marshal::Release(IntPtr(ptr));
   }

   void NatSpeakService::InitializeSpeechServicesInterfaces() {
      // Speech Services
      IDgnSpeechServices ^*ptr = ComHelper::QueryService<ISpchServices^, IDgnSpeechServices^>(_piServiceProvider);
      _idgnSpeechServices = (IDgnSpeechServices^)Marshal::GetObjectForIUnknown(IntPtr(ptr));
      _idgnSSvcOutputEvent = (IDgnSSvcOutputEvent ^)_idgnSpeechServices;
      _idgnSSvcInterpreter = (IDgnSSvcInterpreter ^)_idgnSSvcOutputEvent;

      Marshal::Release(IntPtr(ptr));
   }

   void NatSpeakService::InitializeSrEngineControlInterface() {
      _idgnSrEngineControl = (IDgnSrEngineControl ^) _isrCentral;
   }

   void NatSpeakService::LoadGrammar(array<byte> ^serializedGrammarBytes, ISrGramNotifySink ^iSrGramNotifySink) {
      IntPtr iSrGramNotifySinkPtr;
      LPUNKNOWN pUnknown = nullptr;

      // Pinning any sub-element of a managed array pins the entire array
      pin_ptr<byte> bytes = &serializedGrammarBytes[0];
            
      SDATA data;
      data.dwSize = serializedGrammarBytes->Length;
      data.pData  = bytes;

      unsigned char *c = bytes;
      int j = 1;
      for(int i = 0; i < data.dwSize; i++, j++) {
         Console::Write("{0:X2}", c[i]);
         if (j == 4) {
            Console::WriteLine();
            j = 0;
         }  
      }

      iSrGramNotifySinkPtr = Marshal::GetIUnknownForObject(iSrGramNotifySink);

      try {
         _isrCentral->GrammarLoad(SRGRMFMT_CFG, data, iSrGramNotifySinkPtr, __uuidof(ISrGramNotifySink^), &pUnknown);
      } catch (COMException ^e) {
         if (e->HResult == SRERR_GRAMMARERROR) {
            Debug::WriteLine("SRERR_GRAMMARERROR");
         } else {
            throw;
         }
      }

      ISrGramCommon ^o = (ISrGramCommon^) Marshal::GetTypedObjectForIUnknown(IntPtr(pUnknown), ISrGramCommon::typeid);

      o->Activate(nullptr, false, L"hello_rule");

      pUnknown->Release();
      Marshal::Release(iSrGramNotifySinkPtr);

      // Marshal::ReleaseComObject(o);
   }

   void NatSpeakService::RegisterEngineSink() {
      IntPtr /*isrNotifySinkPtr,*/ idgnSrEngineNotifySinkPtr;
      pin_ptr<DWORD> key = &_key; // https://msdn.microsoft.com/en-us/library/1dz8byfh.aspx
      
      // Create an engine sink
      auto sink = gcnew SrNotifySink();

      // ISrNotifySink ^isrNotifySink = sink;
      IDgnSrEngineNotifySink ^idgnSrEngineNotifySink = sink;

      // isrNotifySinkPtr          = Marshal::GetIUnknownForObject(isrNotifySink);
      idgnSrEngineNotifySinkPtr = Marshal::GetIUnknownForObject(idgnSrEngineNotifySink);

      // Register our notification sinks
      // _isrCentral->Register(isrNotifySinkPtr, __uuidof(ISrNotifySink^), key);
      _isrCentral->Register(idgnSrEngineNotifySinkPtr, __uuidof(IDgnSrEngineNotifySink^), key);

      // Marshal::Release(isrNotifySinkPtr);
      Marshal::Release(idgnSrEngineNotifySinkPtr);
   }

   void NatSpeakService::RegisterPlaybackSink() {
      // Playback sink
      IDgnSSvcActionNotifySink ^playbackSink = gcnew SSvcActionNotifySink();
      IntPtr i = Marshal::GetIUnknownForObject(playbackSink);

      _idgnSSvcOutputEvent->Register(i);
      _idgnSSvcInterpreter->Register(i);
      
      Marshal::Release(i);
   }

   void NatSpeakService::ReleaseSiteObject(IntPtr sitePtr) {
      Marshal::Release(sitePtr);
   }
}