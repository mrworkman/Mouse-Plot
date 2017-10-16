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

namespace Renfrew::Helpers {
   public ref class ComHelper {
      public:
         template <typename _ServiceType, typename _ReturnType, typename _ServicePtr>
         static _ReturnType *QueryService(_ServicePtr instance) {
            HRESULT r;
            _ReturnType *ptr;

            r = instance->QueryService(__uuidof(_ServiceType), __uuidof(_ReturnType), (void**)&ptr);

            if (FAILED(r))
               throw gcnew Exception(r.ToString());

            return static_cast<_ReturnType*>(ptr);
         }
   };
}