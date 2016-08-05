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

using Renfrew.Grammar.Elements;
using Renfrew.Grammar.FluentApi;

namespace Renfrew.GrammarTest {

   public enum DirectiveTypes {
      SRCFG_STARTOPERATION = 1,
      SRCFG_ENDOPERATION   = 2,
      SRCFG_WORD           = 3,
      SRCFG_RULE           = 4,
      SRCFG_WILDCARD       = 5,
      SRCFG_LIST           = 6
   }

   public enum ElementGroupings {
      SRCFGO_SEQUENCE    = 1,
      SRCFGO_ALTERNATIVE = 2,
      SRCFGO_REPEAT      = 3,
      SRCFGO_OPTIONAL    = 4,

      NOT_APPLICABLE     = 0
   }

   public class RuleDirective {
      public DirectiveTypes DirectiveType { get; set; }
      public ElementGroupings? ElementGrouping { get; set; }
      public UInt32 Id { get; set; }

      public override String ToString() {
         if (ElementGrouping == ElementGroupings.NOT_APPLICABLE)
            return $"{DirectiveType} {Id}";
         return $"{DirectiveType} {ElementGrouping}";
      }
   }

   public class RuleDirectiveFactory {

      private Dictionary<Type, ElementGroupings> _groupingTypes;
      private Dictionary<Type, DirectiveTypes> _elementTypes;

      #region Default Constructor
      public RuleDirectiveFactory() : this(
         new Dictionary<Type, ElementGroupings> {
            { typeof(IAlternatives), ElementGroupings.SRCFGO_ALTERNATIVE },
            { typeof(IOptionals),    ElementGroupings.SRCFGO_OPTIONAL    },
            { typeof(IRepeats),      ElementGroupings.SRCFGO_REPEAT      },
            { typeof(ISequence),     ElementGroupings.SRCFGO_SEQUENCE    },
         },
         new Dictionary<Type, DirectiveTypes> {
            { typeof(IListElement),     DirectiveTypes.SRCFG_LIST     },
            { typeof(IRuleElement),     DirectiveTypes.SRCFG_RULE     },
         /* { typeof(IWildcardElement), DirectiveTypes.SRCFG_WILDCARD }, */ // <-- Not Implemented!
            { typeof(IWordElement),     DirectiveTypes.SRCFG_WORD     },
         }) {

         /* poor-man's ioc */
      }
      #endregion

      public RuleDirectiveFactory(
         Dictionary<Type, ElementGroupings> groupingTypes, 
         Dictionary<Type, DirectiveTypes> elementTypes
      ) {
         _groupingTypes = groupingTypes;
         _elementTypes = elementTypes;
      }
      
      public RuleDirective CreateEndDirective(IElementContainer grouping) =>
         CreateGroupingDirective(grouping as dynamic, DirectiveTypes.SRCFG_ENDOPERATION);
      
      public RuleDirective CreateStartDirective(IElementContainer grouping) =>
         CreateGroupingDirective(grouping as dynamic, DirectiveTypes.SRCFG_STARTOPERATION);
      
      public RuleDirective CreateElementDirective(IElement element, UInt32 id = 0) {
         return new RuleDirective {
            DirectiveType = GetDirectiveType(element),
            ElementGrouping = ElementGroupings.NOT_APPLICABLE,
            Id = id,
         };
      }

      private RuleDirective CreateGroupingDirective(IElementContainer grouping, DirectiveTypes directiveType) {
         return new RuleDirective {
            DirectiveType = directiveType,
            ElementGrouping = GetGroupingType(grouping),
         };
      }

      private ElementGroupings GetGroupingType(IElementContainer grouping) {
         return _groupingTypes.First(t => t.Key.IsAssignableFrom(grouping.GetType())).Value;
      }

      private DirectiveTypes GetDirectiveType(IElement element) {
         return _elementTypes.First(t => t.Key.IsAssignableFrom(element.GetType())).Value;
      }

   }

   public class RuleDefinitionFactory {
      private RuleDirectiveFactory _ruleDirectiveFactory =
         new RuleDirectiveFactory();

      private UInt32 _nextWordIndex = 1;
      private Dictionary<String, UInt32> _wordLookup = new Dictionary<String, UInt32>();

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

   class Program {
      
      static void Main(string[] args) {

         var ruleFactory = new RuleFactory();
         var defFactory = new RuleDefinitionFactory();

         IRule rule = ruleFactory.Create();

         /*rule.Say("hello").Do(() => {
            Debug.WriteLine("Well, hello to you!!");
         });*/

         rule.OneOf(r => r.Say("Hi"));

         var x =  defFactory.CreateDefinitionTable(rule.Elements);

         foreach (var q in x)
            Console.WriteLine(q);

         rule = ruleFactory.CreateActionableRule(_ => _
            .Say("hello")
            .OptionallyOneOf( // Note: Need to implement a "OneOf" method where the elements are NOT optional
               r => r.Say("hello"),
               r => r.Say("jello"),
               r => 
                  r.Say("bacon")
                     .Optionally(
                        r1 => r1.RepeatOneOf(
                           r2 => r2.Say("tacos"),
                           r2 => r1.Say("nachos"),
                           r2 => r1.SayOneOf("A", "B", "C"),
                           r2 => r1.Say("X").Say("Y").Say("Z")
                        )
                     )
                     .Optionally(
                        r1 => r1.Say("Q").Say("R").Say("S")  // Equivalent to "Q R S"
                     )
            )
            .Say("cheese").SayOneOf("G", "H", "I")
            .Repeat(r => r.Say("abcdefg"))
         ).Do(() => { });

         x = defFactory.CreateDefinitionTable(rule.Elements);

         Console.WriteLine();
         Console.WriteLine();

         foreach (var q in x)
            Console.WriteLine(q);
         
         Console.ReadKey();
      }
   }
}
