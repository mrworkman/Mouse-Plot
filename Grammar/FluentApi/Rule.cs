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
using System.Linq.Expressions;

namespace Renfrew.Grammar.FluentApi {
   using Elements;
   using Exceptions;

   internal class Rule : IRule {
      private IElementContainer _container;
      private Stack<IElementContainer> _containerStack;

      private ISequence _currentSequence;
      private UInt32 _countInChain;
      private Stack<dynamic> _chainStack;

      internal Rule() {
         _container = new Sequence();
         _containerStack = new Stack<IElementContainer>();

         _currentSequence = _container as ISequence;
         _countInChain = 0;
         _chainStack = new Stack<dynamic>();
      }

      public IElementContainer Elements {
         get { return _container; }
      }

      public IActionableRule OneOf(params Expression<Action<IRule>>[] actions) {
         OneOf(null, actions);
         return (ActionableRule) this;
      }

      private void OneOf(IElementContainer subContainer, params Expression<Action<IRule>>[] actions) {
         IAlternatives alternatives = new Alternatives();

         SaveCurrentContainer();
         SetContainer(subContainer ?? alternatives);

         if (actions.Length < 2) {
            throw new InvalidNumberOfSubruleElementsException(
               $"{nameof(OneOf)} requires more than one sub-rule element!"
            );
         }

         // We need to "alternate" between actions
         if (subContainer != null) {
            subContainer.AddElement(alternatives);
            SetContainer(alternatives);
         }

         SaveCurrentSequence();

         foreach (var action in actions) {
            ResetSequence();
            action.Compile()(this);
         }

         RestoreSequence();

         RestoreContainer();
         AddElementToContainer(subContainer ?? alternatives);
      }

      public IActionableRule Optionally(Expression<Action<IRule>> action) =>
         OptionallyOneOf(action);

      public IActionableRule OptionallyOneOf(params Expression<Action<IRule>>[] actions) {
         OneOf(new Optionals(), actions);
         return (ActionableRule) this;
      }

      public IActionableRule OptionallySay(String word) =>
         Optionally(r => r.Say(word));

      // Repeats: A+
      public IActionableRule Repeat(Expression<Action<IRule>> action) =>
         RepeatOneOf(action);
      
      // Repeats + Alternatives: ( A | B | C )+
      public IActionableRule RepeatOneOf(params Expression<Action<IRule>>[] actions) {
         OneOf(new Repeats(), actions);
         return (ActionableRule) this;
      }

      private void ResetSequence() {
         _countInChain = 0;
         _currentSequence = null;
      }

      private void RestoreContainer() {
         _container = _containerStack.Pop();
      }

      private void RestoreSequence() {
         var cs = _chainStack.Pop();
         _countInChain = cs.Count;
         _currentSequence = cs.Sequence;
      }

      private void SaveCurrentContainer() {
         _containerStack.Push(_container);
      }

      private void SaveCurrentSequence() {
         _chainStack.Push(new {
            Count = _countInChain,
            Sequence = _currentSequence
         });
      }

      public IActionableRule Say(String word) {
         AddElementToContainer( new WordElement(word) );
         return (ActionableRule) this;
      }

      public IActionableRule SayOneOf(params String[] words) =>
         SayOneOf(words as IEnumerable<String>);

      public IActionableRule SayOneOf(IEnumerable<String> words) {
         var alternatives = new Alternatives();

         alternatives.AddElements(words.Select(w => new WordElement(w)));

         AddElementToContainer(alternatives);

         return (ActionableRule) this;
      }

      private void SetContainer(IElementContainer container) =>
         _container = container;

      internal void AddElementToContainer(IElement element) {

         // Because we can't tell ahead of time if there will be
         // more than one element added to an element container,
         // we need to check at run-time.
         //
         // If the provided element is the first in the chain, we
         // assume it's the only one, and simply add it to the
         // container (whatever type that may be).
         // If a second element comes along, and it needs to go
         // in that container, then that means there is a
         // "sequence" of elements. Remove the first element
         // that was added earlier, add it and the new element
         // to a new sequence object, and put the sequence
         // into the container.
         //
         // Example of a "chain" (probably not the best term):
         // 
         // |-- 0 --|------------ 1 ------------|-- 2 --| // <-- Chain #
         // Say("X").Optionally(r => r.Say("Y")).Say("Z")
         
         if (_countInChain == 1) {
            ISequence sequence = new Sequence();

            sequence.AddElement(_container.Pop());
            sequence.AddElement(element);

            _container.AddElement(sequence);
            _currentSequence = sequence;
         } else if (_countInChain > 1) {
            _currentSequence.AddElement(element);
         } else {
            _container.AddElement(element);
         }
            
         _countInChain++;
      }

   }
}
