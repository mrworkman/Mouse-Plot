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
using System.Linq.Expressions;

namespace Renfrew.Grammar.FluentApi {
   using Elements;

   internal class ActionableRule : IActionableRule {

      Rule _rule;

      private ActionableRule(Rule baseRule) {
         _rule = baseRule;
      }

      public IRule Do(Action action) {
         //baseRule.AddExpressionComponent(new RuleAction(action));
         return _rule;
      }

      public IRule Do(Action<IEnumerable<String>> action, bool nearestOnly) {
         //baseRule.AddExpressionComponent(new RuleAction(action));
         return _rule;
      }

      #region Defer to Base Rule
      public IElementContainer Elements {
         get { return _rule.Elements; }
      }

      public IActionableRule OneOf(params Expression<Action<IRule>>[] actions) =>
         _rule.OneOf(actions);

      public IActionableRule Optionally(Expression<Action<IRule>> action) =>
         _rule.Optionally(action);

      public IActionableRule OptionallyOneOf(params Expression<Action<IRule>>[] actions) =>
         _rule.OptionallyOneOf(actions);

      public IActionableRule OptionallySay(String word) =>
         _rule.OptionallySay(word);

      public IActionableRule Repeat(Expression<Action<IRule>> action) =>
         _rule.Repeat(action);

      public IActionableRule RepeatOneOf(params Expression<Action<IRule>>[] actions) =>
         _rule.RepeatOneOf(actions);

      public IActionableRule Say(String word) =>
         _rule.Say(word);

      public IActionableRule SayOneOf(params String[] words) =>
         _rule.SayOneOf(words);

      public IActionableRule SayOneOf(IEnumerable<String> words) =>
         _rule.SayOneOf(words);
      #endregion

      public static explicit operator ActionableRule(Rule rule) =>
         new ActionableRule(rule);
   }
}
