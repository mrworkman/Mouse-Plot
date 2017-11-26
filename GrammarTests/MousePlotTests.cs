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
using System.Drawing;

using Moq;

using NUnit.Framework;

using Renfrew.Core.Grammars.MousePlot;
using Renfrew.NatSpeakInterop;

namespace GrammarTests {
   [TestFixture]
   public class MousePlotTests {
      private Mock<IScreen> _screenMock;
      private MousePlotGrammar _grammar;

      private Mock<IWindow> _plotWindowMock;
      private Mock<IWindow> _zoomWindowMock;

      [SetUp]
      public void SetUp() {
         _screenMock = new Mock<IScreen>(MockBehavior.Strict);
         _plotWindowMock = new Mock<IWindow>(MockBehavior.Strict);
         _zoomWindowMock = new Mock<IWindow>(MockBehavior.Strict);

         _grammar = new MousePlotGrammar(
            grammarService: new Mock<IGrammarService>().Object,
            screen:         _screenMock.Object,
            plotWindow:     _plotWindowMock.Object,
            zoomWindow:     _zoomWindowMock.Object
         );
      }

      [Test]
      [TestCase("Zero",  0x0 )]
      [TestCase("Two",   0x2 )]
      [TestCase("Three", 0x3 )]
      [TestCase("Alpha", 0xa )]
      [TestCase("Bravo", 0xb )]
      [TestCase("Zulu",  0x23)]
      [TestCase("A",     0xa)]
      [TestCase("B",     0xb)]
      [TestCase("Z",     0x23)]
      public void ShouldReturnCorrectOrdinalsForCoordinate(String c, Int32 expected) {
         Assert.That(_grammar.GetCoordinateOrdinal(c), Is.EqualTo(expected));
      }

      [Test]
      [TestCase("10", 0)]
      [TestCase("99", 0)]
      [TestCase("a", 0)]
      [TestCase("alpha", 0)]
      [TestCase("Bob", 0)]
      public void InvalidCoordinateShouldThrowException(String c, Int32 expected) {
         Assert.That(
            () => _grammar.GetCoordinateOrdinal(c),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
         );
      }

      [Test]
      [TestCase(0, 50)]
      [TestCase(1, 150)]
      [TestCase(2, 250)]
      [TestCase(7, 750)]
      [TestCase(12, 1250)]
      [TestCase(15, 1550)]
      [TestCase(19, 1920)] // <-- Beyond the edge of the screen
      [TestCase(20, 1920)] // <-- Beyond the edge of the screen
      [TestCase(36, 1920)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectXCoordFor100X100CellsOn1920X1080Screen(Int32 x, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetMouseXCoord(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0,  50  )]
      [TestCase(1,  150 )]
      [TestCase(2,  250 )]
      [TestCase(7,  750 )]
      [TestCase(10, 1050)]
      [TestCase(11, 1080)] // <-- Beyond the edge of the screen
      [TestCase(36, 1080)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectYCoord100X100CellsOn1920X1080Screen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetMouseYCoord(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 50)]
      [TestCase(1, 150)]
      [TestCase(2, 250)]
      [TestCase(7, 750)]
      [TestCase(9, 950)]
      [TestCase(10, 1024)] // <-- Beyond the edge of the screen
      [TestCase(15, 1024)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectXCoordFor100X100CellsOn1024X768Screen(Int32 x, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1024, 768)
         );

         Assert.That(_grammar.GetMouseXCoord(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0,  50)]
      [TestCase(1,  150)]
      [TestCase(2,  250)]
      [TestCase(7,  750)]
      [TestCase(8,  768)] // <-- Beyond the edge of the screen
      [TestCase(9,  768)] // <-- Beyond the edge of the screen
      [TestCase(36, 768)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectYCoord100X100CellsOn1024X768Screen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1024, 768)
         );

         Assert.That(_grammar.GetMouseYCoord(y), Is.EqualTo(expected));
      }
   }
}
