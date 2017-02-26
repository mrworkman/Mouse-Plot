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

namespace Renfrew.Grammar.Dragon {
   using Elements;

   public class RuleDefinitionFactory {
      private RuleDirectiveFactory _ruleDirectiveFactory;

      private UInt32 _nextWordIndex = 1;
      private Dictionary<String, UInt32> _wordLookup = new Dictionary<String, UInt32>();

      public RuleDefinitionFactory(RuleDirectiveFactory ruleDirectiveFactory) {
         _ruleDirectiveFactory = ruleDirectiveFactory;
      }

      public IEnumerable<RuleDirective> CreateDefinitionTable(IElementContainer elementContainer) {
         var tmp = GenerateRuleDirectives(elementContainer);

         // Reset for each rule parsed
         _nextWordIndex = 1;
         _wordLookup = new Dictionary<String, UInt32>();

         // If the "top-level" grouping (aka the start of the rule) has more
         // than ONE sub-element, then it must be a SEQUENCE, otherwise it
         // it must be any one of the other IElements.
         if (elementContainer.Elements.Count(e => e != null) > 1) {
            var directives = new List<RuleDirective>();

            directives.Add(_ruleDirectiveFactory.CreateStartDirective(elementContainer as ISequence));
            directives.AddRange(tmp);
            directives.Add(_ruleDirectiveFactory.CreateEndDirective(elementContainer as ISequence));

            return directives;
         }

         return tmp;
      }

      private IEnumerable<RuleDirective> GenerateRuleDirectives(IElementContainer elementContainer) {
         var directives = new List<RuleDirective>();

         foreach (var element in elementContainer.Elements) {

            if (element is IElementContainer) {
               var subGroup = element as IElementContainer;

               if (subGroup.HasElements)
                  directives.Add(_ruleDirectiveFactory.CreateStartDirective(subGroup));

               directives.AddRange(GenerateRuleDirectives(subGroup));

               if (subGroup.HasElements)
                  directives.Add(_ruleDirectiveFactory.CreateEndDirective(subGroup));

            } else {
               UInt32 id = _nextWordIndex;
               String word = element.ToString();

               // This should probably be done in another class, no?
               if (_wordLookup.ContainsKey(word) == true) {
                  id = _wordLookup[word];
               } else {
                  _wordLookup[word] = _nextWordIndex;
                  _nextWordIndex++;
               }

               directives.Add(_ruleDirectiveFactory.CreateElementDirective(element, id));
            }

         }

         return directives;
      }
   }
}
