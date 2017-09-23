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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Renfrew.Grammar.Elements;
using Renfrew.Grammar.Exceptions;
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
      public void OneWordGrammarNestedInSequenceGrouping() {
         var rule = _factory.Create();

         rule.Say("test");

         Assert.IsTrue(rule.Elements is ISequence);
         Assert.IsTrue(rule.Elements.Elements.First() is IWordElement);
      }

   }
}
