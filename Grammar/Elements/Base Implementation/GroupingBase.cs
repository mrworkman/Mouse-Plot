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

using System.Collections.Generic;
using System.Linq;

namespace Renfrew.Grammar.Elements {
   public abstract class GroupingBase : IGrouping {
      private List<IElement> _subElements;

      public GroupingBase() {
         _subElements = new List<IElement>();
      }

      public virtual void AddElement(IElement element) =>
         AddElements(element);

      public virtual void AddElements(params IElement[] elements) {
         AddElements(elements as IEnumerable<IElement>);
      }

      public virtual void AddElements(IEnumerable<IElement> elements) {
         _subElements.AddRange(elements);
      }

      public virtual IEnumerable<IElement> Elements {
         get {
            return _subElements.Where(e => e != null);
         }
      }

      public virtual bool HasElements {
         get {
            return Elements.Any();
         }
      }

      public IElement Pop() {
         var lastElement = _subElements.Last(e => e != null);

         // An exception would be thrown on the previous statement if
         // there were nothing to remove. No need to check here
         // if there are items in the list before calling RemoveAt()
         _subElements.RemoveAt(_subElements.Count(e => e != null) - 1);

         return lastElement;
      }
   }
}
