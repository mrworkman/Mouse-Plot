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

#include "GrammarService.h"

// Native types are private by default with /clr
#pragma make_public(::IServiceProvider)

#define IServiceProviderGUID "6d5140c1-7436-11ce-8034-00aa006009fa"
#define IDgnSiteGUID "dd100006-6205-11cf-ae61-0000e8a28647"

#define  E_BUFFERTOOSMALL     0x8004020D
#define  SRERR_NOUSERSELECTED 0x8004041A

#define  SRERR_GRAMMARERROR   0x80040416

namespace Renfrew::NatSpeakInterop {
   public ref class NatSpeakService {

      private: ::IServiceProvider *_piServiceProvider;

      private: ISrCentral ^_isrCentral = nullptr;
      private: IDgnSpeechServices  ^_idgnSpeechServices  = nullptr;
      private: IDgnSrEngineControl ^_idgnSrEngineControl = nullptr;
      private: IDgnSSvcOutputEvent ^_idgnSSvcOutputEvent = nullptr;
      private: IDgnSSvcInterpreter ^_idgnSSvcInterpreter = nullptr;
      private: IDgnSSvcTracking    ^_idgnSSvcTracking    = nullptr;

      private: GrammarService ^_grammarService = nullptr;

      private: DWORD _key;
      private: DWORD _playbackCode;

      public: NatSpeakService();
      public: ~NatSpeakService();

      private: void CreateGrammarService();
      private: void ReleaseGrammarService();

      private: void InitializeIsrCentral(::IServiceProvider *pServiceProvider);
      private: void InitializeSpeechServicesInterfaces();
      private: void InitializeSrEngineControlInterface();
      private: void RegisterEngineSink();
      private: void RegisterPlaybackSink();

      public: void Connect(IntPtr serviceProviderPtr);
      public: void Connect(::IServiceProvider *pServiceProvider);
      public: void Disconnect();

      public: IntPtr CreateSiteObject();
      public: void ReleaseSiteObject(IntPtr sitePtr);

      /// <summary>
      /// Gets the the profile name of the current Dragon user.
      /// </summary>
      /// <returns>The dragon profile name, if available. null otherwise.</returns>
      public: String ^GetCurrentUserProfileName();

      /// <summary>
      /// Gets the version of Dragon.
      /// </summary>
      /// <returns>The dragon version.</returns>
      public: DragonVersion ^GetDragonVersion();

      /// <summary>
      /// Attempts to check if Dragon is "alive" by trying to access one of its interfaces.
      /// </summary>
      /// <returns><b>true</b>: Dragon is running. <b>false</b>: Dragon is not running.</returns>
      public: bool IsDragonAlive();

      /// <summary>
      /// Gets the the file system path to the specified Dragon user's profile directory.
      /// </summary>
      /// <param name="userProfile">The name of the user profile to look up.</param>
      /// <returns>The dragon profile path, if available. null otherwise.</returns>
      public: String ^GetUserDirectory(String ^userProfile);

      public: property IGrammarService ^GrammarService {
         IGrammarService ^get();
      }

      public: property ISrCentral ^SrCentral {
         ISrCentral ^get();
      };

      public: void PlayString(String ^str);
   };
}