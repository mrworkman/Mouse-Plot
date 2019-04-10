// Project Renfrew
// Copyright(C) 2019 Stephen Workman (workman.stephen@gmail.com)
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
   public ref struct DragonVersion {

      private: UInt16 _major;
      private: UInt16 _minor;
      private: UInt16 _patch;

      public: DragonVersion(UInt16 major, UInt16 minor, UInt16 patch) {
         _major = major;
         _minor = minor;
         _patch = patch;
      }

      public: UInt64 GetVersionAsUInt64() {
         return (UInt64(_major) << 48) | (UInt64(_minor) << 32) | (UInt64(_patch) << 16);
      }

      public: static bool operator ==(DragonVersion ^lhs, DragonVersion ^rhs) {
         return lhs->GetVersionAsUInt64() == rhs->GetVersionAsUInt64();
      }

      public: static bool operator <(DragonVersion ^lhs, DragonVersion ^rhs) {
         return lhs->GetVersionAsUInt64() < rhs->GetVersionAsUInt64();
      }

      public: static bool operator <=(DragonVersion ^lhs, DragonVersion ^rhs) {
         return lhs->GetVersionAsUInt64() <= rhs->GetVersionAsUInt64();
      }

      public: static bool operator >(DragonVersion ^lhs, DragonVersion ^rhs) {
         return lhs->GetVersionAsUInt64() >= rhs->GetVersionAsUInt64();;
      }

      public: static bool operator >=(DragonVersion ^lhs, DragonVersion ^rhs) {
         return lhs->GetVersionAsUInt64() >= rhs->GetVersionAsUInt64();
      }

      public: String ^ToString() override {
         return String::Format("{0}.{1}.{2}", _major, _minor, _patch);
      }
   };
}