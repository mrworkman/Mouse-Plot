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

#include "stdafx.h"

#include "DragonVersion.h"
#include "SrNotifySink.h"
#include "SSvcActionNotifySink.h"
#include "SSvcAppTrackingNotifySink.h"
#include "InvalidStateException.h"

using namespace Renfrew::Helpers;
using namespace Renfrew::NatSpeakInterop;
using namespace Renfrew::NatSpeakInterop::Dragon::ComInterfaces;
using namespace Renfrew::NatSpeakInterop::Exceptions;
using namespace Renfrew::NatSpeakInterop::Sinks;

#include "NatSpeakService.h"

NatSpeakService::NatSpeakService() {
   _key = 0;
   _playbackCode = 0;
}

NatSpeakService::~NatSpeakService() {

}

void NatSpeakService::Connect(IntPtr serviceProviderPtr) {
   Connect(reinterpret_cast<::IServiceProvider*>(serviceProviderPtr.ToPointer()));
}

void NatSpeakService::Connect(::IServiceProvider *pServiceProvider) {
   if (pServiceProvider == nullptr)
      throw gcnew ArgumentNullException();

   InitializeIsrCentral(pServiceProvider);
   InitializeSrEngineControlInterface();

   CreateGrammarService();

   RegisterEngineSink();
   InitializeSpeechServicesInterfaces();
   RegisterPlaybackSink();
}

void NatSpeakService::CreateGrammarService() {
   _grammarService = gcnew Renfrew::NatSpeakInterop::
      GrammarService(_isrCentral, _idgnSrEngineControl);
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
      Marshal::ReleaseComObject(idgnSite);
   }

   return sitePtr;
}

void NatSpeakService::Disconnect() {
   Trace::WriteLine(__FUNCTION__);

   if (_key != 0) {
      _isrCentral->UnRegister(_key);
      _key = 0;
   }

   ReleaseGrammarService();

   Marshal::ReleaseComObject(_isrCentral);
   Marshal::ReleaseComObject(_idgnSpeechServices);
}

String ^NatSpeakService::GetCurrentUserProfileName() {
   ISrSpeaker ^isrSpeaker = (ISrSpeaker ^)_isrCentral;

   DWORD dwSize, dwNeeded = 0;
   PWSTR profileName = nullptr;

   // Find out how big our buffer should be
   try {
      isrSpeaker->Query(profileName, 0, &dwNeeded);
   } catch (COMException ^e) {

      // Has a user profile been selected?
      if (e->ErrorCode == SRERR_NOUSERSELECTED)
         return nullptr;

      if (!(e->ErrorCode == EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT ||
         e->ErrorCode == E_BUFFERTOOSMALL)) {
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

DragonVersion ^NatSpeakService::GetDragonVersion() {
   WORD major, minor, patch;

   _idgnSrEngineControl->GetVersion(&major, &minor, &patch);

   return gcnew DragonVersion(major, minor, patch);
}

String ^NatSpeakService::GetUserDirectory(String ^userProfile) {
   IDgnSrSpeaker ^idgnSrSpeaker = (IDgnSrSpeaker ^)_isrCentral;

   DWORD dwSize, dwNeeded = 0;
   PWSTR path = nullptr;

   pin_ptr<const WCHAR> user = PtrToStringChars(userProfile);

   // Find out how big our buffer should be
   try {
      idgnSrSpeaker->GetSpeakerDirectory(user, path, 0, &dwNeeded);
   } catch (COMException ^e) {

      // Has a user profile been selected?
      if (e->ErrorCode == SRERR_NOUSERSELECTED)
         return nullptr;

      if (!(e->ErrorCode == EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT ||
         e->ErrorCode == E_BUFFERTOOSMALL)) {
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

IGrammarService ^NatSpeakService::GrammarService::get() {
   return _grammarService;
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
   _idgnSpeechServices  = (IDgnSpeechServices ^) Marshal::GetObjectForIUnknown(IntPtr(ptr));
   _idgnSSvcOutputEvent = (IDgnSSvcOutputEvent ^) _idgnSpeechServices;
   _idgnSSvcInterpreter = (IDgnSSvcInterpreter ^) _idgnSSvcOutputEvent;
   _idgnSSvcTracking    = (IDgnSSvcTracking ^) _idgnSpeechServices;

   Marshal::Release(IntPtr(ptr));
}

void NatSpeakService::InitializeSrEngineControlInterface() {
   _idgnSrEngineControl = (IDgnSrEngineControl ^)_isrCentral;
}

// "Pings" Dragon to check if it is "alive". We need to be able to determine if Dragon is
// running in order to decide whether to Disconnect or not.
bool NatSpeakService::IsDragonAlive() {
   IDgnSSvcActionNotifySink ^playbackSink = gcnew SSvcActionNotifySink();
   IntPtr i = Marshal::GetIUnknownForObject(playbackSink);

   IDgnSSvcAppTrackingNotifySink ^appTrackingSink = gcnew SSvcAppTrackingNotifySink();
   IntPtr appTrackingSinkPtr = Marshal::GetIUnknownForObject(appTrackingSink);

   try {

      // Exploit the fact that Dragon *should* already have one of these loaded. This may prove unreliable.
      _idgnSSvcTracking->Register(i, IntPtr::Zero, appTrackingSinkPtr);

      // Hitting this would be quite unexpected indeed.
      throw gcnew InvalidStateException("Unexpected result when calling SSvcAppTrackingNotifySink::Register");

   } catch (COMException ^e) {

      // This appears to work since Dragon only allows one tracker to be registered,
      // and it seems like it loads its own when it starts up. If we cannot register
      // a new tracker and we get this HRESULT, then Dragon must be running.
      if (e->HResult == DGNERR_ONLYONETRACKER)
         return true;

      // If we get an SERVERFAULT error, then Dragon must not be running.
      //
      // If we get any other HRESULT, re-throw the exception.
      if (e->HResult != RPC_E_SERVERFAULT)
         throw;

   } finally {
      Marshal::Release(i);
      Marshal::Release(appTrackingSinkPtr);
   }

   return false;
}

void NatSpeakService::PlayString(String ^str) {

   if (str == nullptr)
      throw gcnew ArgumentNullException();

   HRESULT result;
   DWORD dwNumUndo;
   BSTR bstr;

   auto ptr = Marshal::StringToBSTR(str);

   bstr = static_cast<BSTR>(ptr.ToPointer());
   pin_ptr<BSTR> b = &bstr;

   result = _idgnSSvcOutputEvent->PlayString(
      bstr,
      HOOK_F_DEFERTERMINATION,
      -1,            // <-- delay?
      _playbackCode, // <-- required?
      &dwNumUndo     // <-- unused
   );

   // FIXME: Throw more appropriate exception
   if (FAILED(result))
      throw gcnew Exception("PlayString failed!");

   Marshal::FreeBSTR(ptr);
}

void NatSpeakService::RegisterEngineSink() {
   IntPtr /*isrNotifySinkPtr,*/ idgnSrEngineNotifySinkPtr;
   pin_ptr<DWORD> key = &_key; // https://msdn.microsoft.com/en-us/library/1dz8byfh.aspx

                               // Create an engine sink
   auto sink = gcnew SrNotifySink(
      gcnew Action<UInt64>(_grammarService, &NatSpeakInterop::GrammarService::PausedProcessor)
   );

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

void NatSpeakService::ReleaseGrammarService() {
   delete _grammarService;
}

void NatSpeakService::ReleaseSiteObject(IntPtr sitePtr) {
   Marshal::Release(sitePtr);
}

ISrCentral ^NatSpeakService::SrCentral::get() {
   return _isrCentral;
};
