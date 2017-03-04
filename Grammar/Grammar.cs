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

using System;
using System.Collections.Generic;
using System.Linq;

using Renfrew.Grammar.Elements;
using Renfrew.Grammar.FluentApi;

namespace Renfrew.Grammar {

   public interface IGrammar {
      void AddRule(String name, IActionableRule rule);
   }

   public class Grammar : IGrammar {
      private readonly Dictionary<String, IActionableRule> _rules;
      private readonly HashSet<String> _words;

      public Grammar() {
         _rules = new Dictionary<String, IActionableRule>();
         _words = new HashSet<String>();
      }

      public void AddRule(String name, IActionableRule rule) {

         if (String.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

         if (rule == null)
            throw new ArgumentNullException(nameof(rule));

         // TODO: Enforce rule naming convention

         name = name.ToLower();

         if (_rules.ContainsKey(name) == true)
            throw new ArgumentException($"Grammar already contains a rule called '{name}'.", nameof(name));
         
         _rules.Add(name, rule);
      }

      public void RemoveRule(String name) {
         if (String.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

         name = name.ToLower();

         if (_rules.ContainsKey(name) == true)
            _rules.Remove(name);
      }


      private IList<String> GetWords() {
         _words.Clear();

         foreach (var ruleName in RuleNames) {
            var rule = _rules[ruleName];
            UpdateWordsFrom(rule.Elements.Elements);
         }

         return _words.OrderBy(e => e).ToList();
      }

      private void UpdateWordsFrom(IEnumerable<IElement> elements) {
         foreach (var element in elements) {
            if (element is IElementContainer == false) {
               var word = element.ToString().ToLower();

               if (_words.Contains(word) == false)
                  _words.Add(word);

            } else {
               UpdateWordsFrom((element as IElementContainer).Elements);
            }
         }
      }

      internal IList<String> RuleNames =>
         Rules.Keys.OrderBy(e => e).ToList();

      // Expose internally for serialization
      internal IDictionary<String, IActionableRule> Rules => _rules;

      public IList<String> Words => GetWords();
   }
}
