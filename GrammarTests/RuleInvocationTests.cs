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
using System.Diagnostics;

using Moq;

using NUnit.Framework;

using Renfrew.Grammar;
using Renfrew.Grammar.Exceptions;
using Renfrew.NatSpeakInterop;

namespace GrammarTests {

   [TestFixture]
   public class RuleInvocationTests {

      private static int rule1Result = 0;
      private static int rule2Result = 0;
      private static int rule3Result = 0;

      #region TestGrammar
      private class TestGrammar : Grammar {

         public TestGrammar(IGrammarService grammarService)
            : base(grammarService) {

         }


         public override void Dispose() { }

         public override void Initialize() {
            AddRule("test_rule_01", r =>
               r.Say("Hello").Say("Jello").Do(words => {
                  rule1Result = 1;

                  foreach (var word in words)
                     Debug.WriteLine($"Rule1: RECEIVED WORD: {word}");

               })
            );

            AddRule("test_rule_02", r =>
               r.Say("Hello").Say("Jello").OptionallySay("Cheese").Say("Please").Do(words => {
                  rule2Result = 1;

                  foreach (var word in words)
                     Debug.WriteLine($"Rule2: RECEIVED WORD: {word}");

               })
            );

            AddRule("test_rule_03", r =>
               r.Say("Hello")
                  .Optionally(o => o.Say("Skee"))
                  .RepeatOneOf(
                     rp => rp.SayOneOf("a", "b", "c"),
                     rp => rp.Say("Hi"),
                     rp => rp.Say("Sty")
                  ).Do(words => {
                     rule3Result = 1;
                     foreach (var word in words)
                        Debug.WriteLine($"Rule3: RECEIVED WORD: {word}");
                  })
            );

         }
      }
      #endregion

      private Grammar _g;

      [SetUp]
      public void Initialize() {
         var grammarServiceMock = new Mock<IGrammarService>(MockBehavior.Loose);

         _g = new TestGrammar(grammarServiceMock.Object);
         _g.Initialize();

         rule1Result = 0;
         rule2Result = 0;
         rule3Result = 0;
      }

      [Test]
      public void ComplexRuleActionShouldBeInvoked_Variant1() {
         _g.InvokeRule(3, new[] { "Hello", "Skee", "Sty", "Sty", "Hi", "Hi", "Sty" });

         Assert.That(rule3Result, Is.EqualTo(1));
      }

      [Test]
      public void ComplexRuleActionShouldBeInvoked_Variant2() {
         _g.InvokeRule(3, new[] { "Hello", "Hi", "Hi" });

         Assert.That(rule3Result, Is.EqualTo(1));
      }

      [Test]
      public void ComplexRuleActionShouldBeInvoked_Variant3() {
         _g.InvokeRule(3, new[] { "Hello", "Hi", "a", "a", "Sty", "Hi", "c", "b", "c", "c" });

         Assert.That(rule3Result, Is.EqualTo(1));
      }


      [Test]
      public void InvalidRuleIdShouldCauseException() {
         Assert.That(() => {
            _g.InvokeRule(0, new[] { "Hello", "Jello" });
         }, Throws.InstanceOf<ArgumentOutOfRangeException>());

      }

      [Test]
      public void SimpleRuleActionShouldBeInvoked() {
         _g.InvokeRule(1, new[] {"Hello", "Jello"});

         Assert.That(rule1Result, Is.EqualTo(1));
      }

      [Test]
      public void SimpleRuleActionWithExtraWordShouldThrowException() {
         Assert.That(() => {
            _g.InvokeRule(1, new[] { "Hello", "Jello", "Smello" });
         }, Throws.InstanceOf<TooManyWordsInCallbackException>());
      }

      [Test]
      public void SimpleRuleActionWithInvalidWordShouldThrowException() {
         Assert.That(() => {
            _g.InvokeRule(1, new[] { "Hello", "Mellow" });
         }, Throws.InstanceOf<InvalidSequenceInCallbackException>());
      }

      [Test]
      public void SimpleRuleActionWithNotEnoughWordsShouldThrowException() {
         Assert.That(() => {
            _g.InvokeRule(1, new[] { "Hello" });
         }, Throws.InstanceOf<InvalidSequenceInCallbackException>());
      }

      [Test]
      public void SimpleRuleActionWithMissiongOptionalWordShouldBeInvoked() {
         _g.InvokeRule(2, new[] { "Hello", "Jello", "Please" });

         Assert.That(rule2Result, Is.EqualTo(1));
      }

      [Test]
      public void SimpleRuleActionWithOptionalWordShouldBeInvoked() {
         _g.InvokeRule(2, new[] { "Hello", "Jello", "Cheese", "Please" });

         Assert.That(rule2Result, Is.EqualTo(1));
      }

      [Test]
      public void SimpleRuleActionWithInvalidWordInPlaceOfOptionalWordShouldThrowException() {
         Assert.That(() => {
            _g.InvokeRule(2, new[] { "Hello", "Jello", "Sneeze", "Please" });
         }, Throws.InstanceOf<InvalidSequenceInCallbackException>());
      }


   }

}
