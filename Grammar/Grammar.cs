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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Renfrew.Grammar.Elements;
using Renfrew.Grammar.FluentApi;
using Renfrew.NatSpeakInterop;

namespace Renfrew.Grammar {

   public abstract class Grammar : IGrammar, IDisposable {
      
      private readonly Dictionary<String, IActionableRule> _rules;

      private UInt32 _wordCount = 0;
      private readonly Dictionary<String, UInt32> _wordIds;

      private UInt32 _ruleCount = 0;
      private readonly Dictionary<String, UInt32> _ruleIds;

      protected Grammar() 
         : this(new RuleFactory()) {

         // This is a list of the rules themselves (by name)
         _rules = new Dictionary<String, IActionableRule>(StringComparer.CurrentCultureIgnoreCase);

         // These are lookups to find the numeric ids for words/rule names
         _wordIds = new Dictionary<String, UInt32>(StringComparer.CurrentCultureIgnoreCase);
         _ruleIds = new Dictionary<String, UInt32>(StringComparer.CurrentCultureIgnoreCase);
      }

      protected Grammar(RuleFactory ruleFactory) {
         RuleFactory = ruleFactory;
      }

      protected void AddRule(String name, IActionableRule rule) {
         if (String.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

         if (rule == null)
            throw new ArgumentNullException(nameof(rule));
         
         try {
            EnforceRuleNaming(name);
         } catch { throw; }
         
         if (_rules.ContainsKey(name) == true)
            throw new ArgumentException($"Grammar already contains a rule called '{name}'.", nameof(name));

         foreach (var word in GetWordsFromRule(rule)) {
            Debug.WriteLine($"WORD: {word}");

            if (_wordIds.ContainsKey(word) == false)
               _wordIds.Add(word, _wordCount++);

         }

         if (_ruleIds.ContainsKey(name) == false)
            _ruleIds.Add(name, _ruleCount++);

         _rules.Add(name, rule);
      }

      protected void AddRule(String name, Expression<Action<IRule>> expression) =>
         AddRule(name, RuleFactory.CreateActionableRule(expression));

      public abstract void Dispose();

      private void EnforceRuleNaming(String ruleName) {
         var validChars = @"[a-zA-Z0-9_]";

         if (Regex.IsMatch(ruleName, $@"^{validChars}+$") == false) {
            throw new ArgumentOutOfRangeException(nameof(ruleName), 
               $@"Rule name '{ruleName}' contains invalid character(s): '{
                  Regex.Replace(ruleName, validChars, String.Empty)
               }'"
            );
         }  
      }

      public abstract void Initialize();
      
      protected void RemoveRule(String name) {
         if (String.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

         _rules.Remove(name);
         _ruleIds.Remove(name);
      }

      private IEnumerable<String> GetWordsFromRule(IRule rule) {
         return GetWordsFromRuleElements(rule.Elements.Elements);
      }

      private IEnumerable<String> GetWordsFromRuleElements(IEnumerable<IElement> elements) {
         foreach (var element in elements) {
            if (element is IElementContainer == false) {
               yield return element.ToString();
            } else {
               foreach (var word in GetWordsFromRuleElements((element as IElementContainer).Elements))
                  yield return word;
            }
         }
      }

      protected RuleFactory RuleFactory { get; private set; }

      public IReadOnlyCollection<String> RuleNames =>
         _rules.Keys.OrderBy(e => e).ToList();

      // Expose internally for serialization
      internal IReadOnlyCollection<IActionableRule> Rules => 
         _rules.OrderBy(e => e.Key).Select(e => e.Value).ToList();

      public IReadOnlyCollection<String> Words =>
         _wordIds.Keys.OrderBy(e => e).ToList();
      
   }
   
}
