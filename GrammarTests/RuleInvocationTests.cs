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
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Renfrew.Grammar;
using Renfrew.Grammar.FluentApi;

namespace GrammarTests {

   [TestClass]
   public class RuleInvocationTests {

      private static int rule1Result = 0;
      private static int rule2Result = 0;

      #region TestGrammar
      private class TestGrammar : Grammar {
         public override void Dispose() { }

         public override void Initialize() {
            AddRule("test_rule_01", r =>
               r.Say("Hello").Say("Jello").Do(words => {
                  rule1Result = 1;

                  foreach (var word in words)
                     Debug.WriteLine($"RECEIVED WORD: {word}");

               })
            );

            AddRule("test_rule_02", r =>
               r.Say("Hello").Say("Jello").OptionallySay("Cheese").Say("Please").Do(words => {
                  rule2Result = 1;

                  foreach (var word in words)
                     Debug.WriteLine($"RECEIVED WORD: {word}");

               })
            );

         }
      }
      #endregion

      private Grammar _g;

      [TestInitialize]
      public void Initialize() {
         _g = new TestGrammar();
         _g.Initialize();

         rule1Result = 0;
         rule2Result = 0;
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void InvalidRuleIdShouldCauseException() {
         _g.InvokeRule(0, new[] { "Hello", "Jello" });
      }

      [TestMethod]
      public void SimpleRuleActionShouldBeInvoked() {
         _g.InvokeRule(1, new[] {"Hello", "Jello"});

         Assert.AreEqual(1, rule1Result);
      }

      [TestMethod]
      [ExpectedException(typeof(Exception))]
      public void SimpleRuleActionWithExtraWordShouldThrowException() {
         _g.InvokeRule(1, new[] { "Hello", "Jello", "Smello" });
      }

      [TestMethod]
      [ExpectedException(typeof(Exception))]
      public void SimpleRuleActionWithInvalidWordShouldThrowException() {
         _g.InvokeRule(1, new[] { "Hello", "Mellow" });
      }

      [TestMethod]
      [ExpectedException(typeof(Exception))]
      public void SimpleRuleActionWithNotEnoughWordsShouldThrowException() {
         _g.InvokeRule(1, new[] { "Hello" });
      }

      [TestMethod]
      public void SimpleRuleActionWithMissiongOptionalWordShouldBeInvoked() {
         _g.InvokeRule(2, new[] { "Hello", "Jello", "Please" });

         Assert.AreEqual(1, rule2Result);
      }

      [TestMethod]
      public void SimpleRuleActionWithOptionalWordShouldBeInvoked() {
         _g.InvokeRule(2, new[] { "Hello", "Jello", "Cheese", "Please" });

         Assert.AreEqual(1, rule2Result);
      }

      [TestMethod]
      [ExpectedException(typeof(Exception))]
      public void SimpleRuleActionWithInvalidWordInPlaceOfOptionalWordShouldThrowException() {
         _g.InvokeRule(2, new[] { "Hello", "Jello", "Sneeze", "Please" });
      }


   }

}
