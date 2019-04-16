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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Renfrew.Grammar.Elements;
using Renfrew.Grammar.Exceptions;
using Renfrew.Grammar.FluentApi;
using Renfrew.NatSpeakInterop;

namespace Renfrew.Grammar {

   public abstract class Grammar : IGrammar, IDisposable {

      private IGrammarService _grammarService;

      private readonly Dictionary<String, IRule> _rules;
      private readonly Dictionary<UInt32, IRule> _rulesById;

      private UInt32 _wordCount = 1;
      private readonly Dictionary<String, UInt32> _wordIds;

      private UInt32 _ruleCount = 1;
      private readonly Dictionary<String, UInt32> _ruleIds;

      private readonly Dictionary<String, UInt32> _activeRules;

      protected Grammar(IGrammarService grammarService)
         : this(new RuleFactory(), grammarService) {

         // This is a list of the rules themselves (by name)
         _rules = new Dictionary<String, IRule>(StringComparer.CurrentCultureIgnoreCase);
         _rulesById = new Dictionary<UInt32, IRule>();

         // These are lookups to find the numeric ids for words/rule names
         _wordIds = new Dictionary<String, UInt32>(StringComparer.CurrentCultureIgnoreCase);
         _ruleIds = new Dictionary<String, UInt32>(StringComparer.CurrentCultureIgnoreCase);

         _activeRules = new Dictionary<String, UInt32>();
      }

      protected Grammar(RuleFactory ruleFactory, IGrammarService grammarService) {
         Debug.Assert(ruleFactory != null);
         Debug.Assert(grammarService != null);

         _grammarService = grammarService;
         RuleFactory = ruleFactory;
      }

      public void ActivateRule(String name) {
         _grammarService.ActivateRule(this, IntPtr.Zero, name);

         if (_activeRules.ContainsKey(name) == false)
            _activeRules.Add(name, _ruleIds[name]);
      }

      public void AddRule(String name, IRule rule) {
         if (String.IsNullOrWhiteSpace(name) == true)
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

         if (rule == null)
            throw new ArgumentNullException(nameof(rule));

         EnforceRuleNaming(name);

         if (_rules.ContainsKey(name) == true)
            throw new ArgumentException($"Grammar already contains a rule called '{name}'.", nameof(name));

         foreach (var word in GetWordsFromRule(rule)) {
            if (_wordIds.ContainsKey(word) == false)
               _wordIds.Add(word, _wordCount++);
         }

         var ruleId = _ruleCount++;

         if (_ruleIds.ContainsKey(name) == false)
            _ruleIds.Add(name, ruleId);

         _rules.Add(name, rule);
         _rulesById.Add(ruleId, rule);
      }

      public void AddRule(String name, Func<IRule, IRule> ruleFunc) =>
         AddRule(name, ruleFunc?.Invoke(RuleFactory.Create()));

      public void DeactivateRule(String name) {
         _grammarService.DeactivateRule(this, name);

         if (_activeRules.ContainsKey(name) == true)
            _activeRules.Remove(name);
      }

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

      protected void Load() {
         _grammarService.LoadGrammar(this);
      }

      protected void MakeGrammarExclusive() {
         _grammarService.SetExclusiveGrammar(this, true);
      }

      protected void MakeGrammarNotExclusive() {
         _grammarService.SetExclusiveGrammar(this, false);
      }

      protected void RemoveRule(String name) {
         if (String.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

         _rules.Remove(name);

         _rulesById.Remove(_ruleIds[name]);
         _ruleIds.Remove(name);

      }

      private IEnumerable<String> GetWordsFromRule(IRule rule) {
         return GetWordsFromRuleElements(rule.Elements.Elements);
      }

      private IEnumerable<String> GetWordsFromRuleElements(IEnumerable<IElement> elements) {
         foreach (var element in elements) {

            // Ignore action elements
            if (element is IGrammarAction)
               continue;

            // Get the word from the element/sub-elements
            if (element is IElementContainer == false) {
               yield return element.ToString();
            } else {
               foreach (var word in GetWordsFromRuleElements((element as IElementContainer).Elements))
                  yield return word;
            }
         }
      }

      public void InvokeRule(IEnumerable<String> spokenWords) {

         if (spokenWords == null)
            throw new ArgumentNullException(nameof(spokenWords));

         // Make sure there is at least one rule activated
         if (_activeRules.Any() == false)
            throw new NoActiveRulesException();

         bool result = false;

         // Enumerate each rule in the grammar, trying to invoke each one.
         // The one that "works" will be assumed to be the correct rule.
         foreach (var ruleNumber in _activeRules.Values.OrderBy(e => e)) {
            var rule = _rulesById[ruleNumber];

            var spokenWordsStack = new Stack<String>(spokenWords.Reverse());
            var callbacks = new List<KeyValuePair<IGrammarAction, IEnumerable<String>>>();

            // Check the word sequence to see if it's a match.
            result = ProcessSpokenWords(
               rule.Elements,
               spokenWordsStack,
               callbacks
            );

            // Make sure there are no words left in the stack
            if (spokenWordsStack.Any() == true) {
               Debug.WriteLine(
                  $"There are extra words in the callback: {String.Join(", ", spokenWords)}"
               );

               // The result of ProcessSpokenWords above could be "true", but
               // that doesn't mean that this rule is the correct one. If there
               // are spoken words that aren't accounted for, then it's the wrong
               // rule.
               result = false;
            }

            // Invoke callback(s)
            if (result == true) {
               foreach (var callback in callbacks)
                  callback.Key.InvokeAction(callback.Value);
               break;
            }
         }

         // Did the spoken words match the rule's structure?
         if (result == false)
            throw new InvalidSequenceInCallbackException();

      }

      private bool ProcessSpokenWords(
         IElementContainer elementContainer, Stack<String> spokenWordsStack,
         List<KeyValuePair<IGrammarAction, IEnumerable<String>>> callbacks,
         List<String> aw = null
         ) {

         if (callbacks == null)
            throw new ArgumentNullException(nameof(callbacks));

         var sc = StringComparison.CurrentCultureIgnoreCase;
         var actionWords = new List<String>();

         foreach (var element in elementContainer.Elements) {

            if (element is IWordElement) {
               var wordElement = element as IWordElement;

               var spokenWord = spokenWordsStack.FirstOrDefault();

               // If the words don't match, then this sub-rule doesn't match.
               if (spokenWord == null || String.Equals(spokenWord, element.ToString(), sc) == false)
                  return false;

               // Add word to callback stack
               spokenWordsStack.Pop();
               actionWords.Add(spokenWord);

               continue;
            }

            // This rule refers to another rule, so we need to
            // look it up and traverse it as well...a
            if (element is IRuleElement) {
               var ruleElement = element as IRuleElement;
               var nestedRule = _rules[ruleElement.ToString()];

               var nestedResult = ProcessSpokenWords(
                  nestedRule.Elements,
                  spokenWordsStack,
                  callbacks,
                  actionWords
               );

               if (nestedResult == false)
                  return false;

               continue;
            }

            // Check if we need to descend into a sub-rule (Optional, Repeats, Alternatives...)
            if (element is IElementContainer) {
               var subRule = (element as IElementContainer);
               var subRuleResult = false;

               if (subRule is IOptionals) {
                  ProcessSpokenWords(subRule, spokenWordsStack, callbacks, actionWords);
                  subRuleResult = true;
               } else if (subRule is IAlternatives) {
                  var alternatives = (subRule as IAlternatives)?.Elements;

                  foreach (var alternative in alternatives) {

                     // Encapsulate in a sequence
                     var s = new Sequence();
                     s.AddElement(alternative);

                     subRuleResult = ProcessSpokenWords(s, spokenWordsStack, callbacks, actionWords);

                     if (subRuleResult == true)
                        break;
                  }

               } else if (subRule is IRepeats) {
                  var repeatable = (subRule as IRepeats)?.Elements;

                  while (ProcessSpokenWords(subRule, spokenWordsStack, callbacks, actionWords))
                     subRuleResult = true;

               } else { // Must be an ISequence
                  subRuleResult = ProcessSpokenWords(subRule, spokenWordsStack, callbacks, actionWords);
               }

               if (subRuleResult == false)
                  return false;

               continue;
            }

            if (element is IGrammarAction) {

               callbacks.Add(
                  new KeyValuePair<IGrammarAction, IEnumerable<string>>(
                     element as IGrammarAction, actionWords
                  )
               );

               actionWords = new List<String>();
            }
         }

         aw?.AddRange(actionWords);

         // If we get here, the rule matches the spoken words
         return true;
      }

      /// <summary>
      /// Due to a problem with Dragon 15, rules we want to remain active
      /// need to be explicitly re-activated when another is de-activated.
      /// </summary>
      /// <param name="name">The name of the rule</param>
      public void ReactivateRule(String name) {
         DeactivateRule(name);
         ActivateRule(name);
      }

      protected RuleFactory RuleFactory { get; private set; }

      public IReadOnlyDictionary<String, UInt32> RuleIds => _ruleIds;

      // Expose internally for serialization
      internal IReadOnlyList<IRule> Rules =>
         _rulesById.OrderBy(e => e.Key).Select(e => e.Value).ToList();

      public IReadOnlyDictionary<String, UInt32> WordIds => _wordIds;

   }

}
