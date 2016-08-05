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
using System.Collections.Generic;
using System.Linq;

namespace Renfrew.Grammar.Elements {
   public abstract class ElementContainerBase : IElementContainer {
      protected IElement _subElement;

      public virtual void AddElement(IElement element) {
         if (_subElement != null)
            throw new NotImplementedException(); // FIXME: <-- Throw a proper exception 
         _subElement = element;
      }

      public virtual IEnumerable<IElement> Elements {
         get {
            return new List<IElement> { _subElement };
         }
      }

      public virtual bool HasElements {
         get {
            return Elements.Any(e => e != null);
         }
      }

      public IElement Pop() {
         var element = _subElement;
         _subElement = null;
         return element;
      }
   }
}
