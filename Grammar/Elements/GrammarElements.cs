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

   public interface IElement { }

   public interface IElementContainer : IElement {
      void AddElement(IElement element);
      IEnumerable<IElement> Elements { get; }
      bool HasElements { get; } // <--- Not *really* needed
      IElement Pop();
   }

   public interface IGrouping : IElementContainer {
      void AddElements(params IElement[] elements);
      void AddElements(IEnumerable<IElement> elements);
   }

   #region Container/Grouping Interfaces
   // Element Containers
   public interface IAlternatives : IGrouping { }
   public interface ISequence : IGrouping { }

   // Element Groupings
   public interface IOptionals : IElementContainer { }
   public interface IRepeats : IElementContainer { }

   #endregion

   #region Element Type Interfaces
   public interface IListElement : IElement { }
   public interface IRuleElement : IElement { }
   public interface IWordElement : IElement { }

   public interface IGrammarAction : IElement { }
   #endregion

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

   public class Alternatives : GroupingBase, IAlternatives { }
   public class Sequence : GroupingBase, ISequence { }

   public class Optionals : ElementContainerBase, IOptionals { }
   public class Repeats : ElementContainerBase, IRepeats { }

   // TODO: Implement...
   //public class GrammarAction : IGrammarAction {

   //}

   public class WordElement : IWordElement {
      private String _word;

      public WordElement(String word) {
         _word = word;
      }

      public override String ToString() =>
         _word;

   }

}
