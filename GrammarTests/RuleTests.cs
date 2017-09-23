using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Renfrew.Grammar.Elements;
using Renfrew.Grammar.FluentApi;

namespace GrammarTests {
   [TestClass]
   public class RuleTests {
      private RuleFactory _factory;

      [TestInitialize]
      public void Initialize() {
         _factory = new RuleFactory();
      }

      [TestMethod]
      [ExpectedException(typeof(Exception))]
      public void OneOfShouldThrowExceptionIfOnlyOneElement() {
         var rule = _factory.Create(r => r.OneOf(r2 => r2.Say("test")));
      }

      [TestMethod]
      public void OneWordGrammarNestedInSequenceGrouping() {
         var rule = _factory.Create();

         rule.Say("test");

         Assert.IsTrue(rule.Elements is ISequence);
         Assert.IsTrue(rule.Elements.Elements.First() is IWordElement);
      }

   }
}
