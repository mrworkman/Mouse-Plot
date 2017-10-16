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

namespace Renfrew::NatSpeakInterop {
   private ref class GrammarExecutive {
      private: IGrammar ^_grammar;
      private: ISrGramCommon ^_isrGramCommon;

      public: GrammarExecutive(IGrammar ^grammar) {
         if (grammar == nullptr)
            throw gcnew ArgumentNullException("grammar");

         _grammar = grammar;
      }

      public: property IGrammar ^Grammar {
         IGrammar ^get() {
            return _grammar;
         };
      };

      public: property ISrGramCommon ^GramCommonInterface {
         ISrGramCommon ^get() {
            return _isrGramCommon;
         }

         void set(ISrGramCommon ^isrGramCommon) {
            _isrGramCommon = isrGramCommon;
         }
      };

      public: int GetHashCode() override {
         return _grammar->GetHashCode();
      }
   };
}