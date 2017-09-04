// Project Renfrew
// Copyright(C) 2017 Stephen Workman (workman.stephen@gmail.com)
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

namespace Renfrew::NatSpeakInterop {
   using namespace System;
   using namespace System::Collections::Generic;

   public interface class IGrammarService {
      public: void ActivateRule(IGrammar ^grammar, HWND hWnd, String ^ruleName);
      public: void ActivateRule(IGrammar ^grammar, IntPtr ^hWnd, String ^ruleName);
      public: void ActivateRules(IGrammar ^grammar);
      public: void DeactivateRule(IGrammar ^grammar, String ^ruleName);

      public: property IGrammarSerializer ^GrammarSerializer {
         void set(IGrammarSerializer ^grammarSerializer);
      };

      public: void LoadGrammar(IGrammar ^grammar);
      public: void UnloadGrammar(IGrammar ^grammar);
   };
}