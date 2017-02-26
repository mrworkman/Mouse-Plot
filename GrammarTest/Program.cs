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

using Renfrew.Dragon;
using Renfrew.Grammar.FluentApi;

namespace Renfrew.GrammarTest {

   class Program {
      
      static void Main(string[] args) {

         var ruleFactory = new RuleFactory();
         var defFactory = new RuleDefinitionFactory(new RuleDirectiveFactory());

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
