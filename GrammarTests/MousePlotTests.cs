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
      private Mock<IWindow> _cellWindowMock;
      private Mock<IZoomWindow> _zoomWindowMock;

      [SetUp]
      public void SetUp() {
         _screenMock = new Mock<IScreen>(MockBehavior.Strict);
         _plotWindowMock = new Mock<IWindow>();
         _zoomWindowMock = new Mock<IZoomWindow>();
         _cellWindowMock = new Mock<IWindow>();

         _grammar = new MousePlotGrammar(
            grammarService: new Mock<IGrammarService>().Object,
            screen:         _screenMock.Object,
            plotWindow:     _plotWindowMock.Object,
            zoomWindow:     _zoomWindowMock.Object,
            cellWindow:     _cellWindowMock.Object
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
      [TestCase("10",    0)]
      [TestCase("99",    0)]
      [TestCase("a",     0)]
      [TestCase("alpha", 0)]
      [TestCase("Bob",   0)]
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
      [TestCase(19, 1919)] // <-- Beyond the edge of the screen
      [TestCase(20, 1919)] // <-- Beyond the edge of the screen
      [TestCase(36, 1919)] // <-- Beyond the edge of the screen
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
      [TestCase(11, 1079)] // <-- Beyond the edge of the screen
      [TestCase(36, 1079)] // <-- Beyond the edge of the screen
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
      [TestCase(10, 1023)] // <-- Beyond the edge of the screen
      [TestCase(15, 1023)] // <-- Beyond the edge of the screen
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
      [TestCase(8,  767)] // <-- Beyond the edge of the screen
      [TestCase(9,  767)] // <-- Beyond the edge of the screen
      [TestCase(36, 767)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectYCoord100X100CellsOn1024X768Screen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1024, 768)
         );

         Assert.That(_grammar.GetMouseYCoord(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0,  50 - 1920)]
      [TestCase(1,  150 - 1920)]
      [TestCase(2,  250 - 1920)]
      [TestCase(7,  750 - 1920)]
      [TestCase(12, 1250 - 1920)]
      [TestCase(15, 1550 - 1920)]
      [TestCase(19, 1919 - 1920)] // <-- Beyond the edge of the screen
      [TestCase(20, 1919 - 1920)] // <-- Beyond the edge of the screen
      [TestCase(36, 1919 - 1920)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectXCoordFor100X100CellsOn1920X1080SecondaryScreen(Int32 x, Int32 expected) {

         // Simulate a secondary screen that's to the left of the primary screen
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(-1920, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetMouseXCoord(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0,  50 - 1024)]
      [TestCase(1,  150 - 1024)]
      [TestCase(2,  250 - 1024)]
      [TestCase(7,  750 - 1024)]
      [TestCase(9,  950 - 1024)]
      [TestCase(10, 1023 - 1024)] // <-- Beyond the edge of the screen
      [TestCase(11, 1023 - 1024)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectYCoord100X100CellsOn1920X1080SecondaryScreen(Int32 y, Int32 expected) {
         // Simulate a secondary screen that's above the primary screen
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, -1024, 1280, 1024)
         );

         Assert.That(_grammar.GetMouseYCoord(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 50 + 1080)]
      [TestCase(1, 150 + 1080)]
      [TestCase(2, 250 + 1080)]
      [TestCase(7, 750 + 1080)]
      [TestCase(9, 950 + 1080)]
      [TestCase(10, 1023 + 1080)] // <-- Beyond the edge of the screen
      [TestCase(11, 1023 + 1080)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectYCoord100X100CellsOn1920X1080SecondaryScreenBelow(Int32 y, Int32 expected) {
         // Simulate a secondary screen that's below a larger primary screen
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 1080, 1280, 1024)
         );

         Assert.That(_grammar.GetMouseYCoord(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0)]
      [TestCase(1, 100)]
      [TestCase(2, 200)]
      [TestCase(7, 700)]
      [TestCase(12, 1200)]
      [TestCase(15, 1500)]
      [TestCase(19, 1900)]
      [TestCase(20, 1900)] // <-- Beyond the edge of the screen
      [TestCase(36, 1900)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectCellXCoordFor100X100CellsOn1920X1080Screen(Int32 x, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetCellXCoord(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0)]
      [TestCase(1, 100)]
      [TestCase(2, 200)]
      [TestCase(7, 700)]
      [TestCase(10, 1000)]
      [TestCase(11, 1000)] // <-- Beyond the edge of the screen
      [TestCase(36, 1000)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectCellYCoord100X100CellsOn1920X1080Screen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetCellYCoord(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0 - 1920)]
      [TestCase(1, 100 - 1920)]
      [TestCase(2, 200 - 1920)]
      [TestCase(7, 700 - 1920)]
      [TestCase(12, 1200 - 1920)] // <-- Beyond the edge of the screen
      [TestCase(15, 1200 - 1920)] // <-- Beyond the edge of the screen
      [TestCase(19, 1200 - 1920)] // <-- Beyond the edge of the screen
      [TestCase(20, 1200 - 1920)] // <-- Beyond the edge of the screen
      [TestCase(36, 1200 - 1920)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectCellXCoordFor100X100CellsOn1280X720SecondaryScreen(Int32 x, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(-1920, 0, 1280, 720)
         );

         Assert.That(_grammar.GetCellXCoord(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0 - 1080)]
      [TestCase(1, 100 - 1080)]
      [TestCase(2, 200 - 1080)]
      [TestCase(7, 700 - 1080)]
      [TestCase(10, 700 - 1080)]
      [TestCase(11, 700 - 1080)] // <-- Beyond the edge of the screen
      [TestCase(36, 700 - 1080)] // <-- Beyond the edge of the screen
      public void ShouldGetCorrectCellYCoord100X100CellsOn1280X720SecondaryScreen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, -1080, 1280, 720)
         );

         Assert.That(_grammar.GetCellYCoord(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0)]
      [TestCase(1, 100)]
      [TestCase(2, 200)]
      [TestCase(7, 700)]
      [TestCase(12, 1200)]
      [TestCase(15, 1500)]
      [TestCase(19, 1900)]
      [TestCase(20, 2000)]
      [TestCase(36, 3600)]
      public void ShouldGetCorrectXOffsetFor100X100CellsOn1920X1080Screen(Int32 x, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetXScreenOffset(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0)]
      [TestCase(1, 100)]
      [TestCase(2, 200)]
      [TestCase(7, 700)]
      [TestCase(10, 1000)]
      [TestCase(11, 1100)]
      [TestCase(36, 3600)]
      public void ShouldGetCorrectYOffsetFor100X100CellsOn1920X1080Screen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Assert.That(_grammar.GetYScreenOffset(y), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0 - 1920)]
      [TestCase(1, 100 - 1920)]
      [TestCase(2, 200 - 1920)]
      [TestCase(7, 700 - 1920)]
      [TestCase(12, 1200 - 1920)]
      [TestCase(15, 1500 - 1920)]
      [TestCase(19, 1900 - 1920)]
      [TestCase(20, 2000 - 1920)]
      [TestCase(36, 3600 - 1920)]
      public void ShouldGetCorrectXOffsetFor100X100CellsOn1920X1080SecondaryScreen(Int32 x, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(-1920, -10, 1920, 1080)
         );

         Assert.That(_grammar.GetXScreenOffset(x), Is.EqualTo(expected));
      }

      [Test]
      [TestCase(0, 0 - 1080)]
      [TestCase(1, 100 - 1080)]
      [TestCase(2, 200 - 1080)]
      [TestCase(7, 700 - 1080)]
      [TestCase(10, 1000 - 1080)]
      [TestCase(11, 1100 - 1080)]
      [TestCase(36, 3600 - 1080)]
      public void ShouldGetCorrectYOffsetFor100X100CellsOn1920X1080SecondaryScreen(Int32 y, Int32 expected) {
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(-1300, -1080, 1920, 1080)
         );

         Assert.That(_grammar.GetYScreenOffset(y), Is.EqualTo(expected));
      }


      [Test]
      public void ShouldOpenZoomWindowAndCloseMainPlotWindow() {
         // Arrange
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         _zoomWindowMock.Setup(e => e.Width).Returns(300);
         _zoomWindowMock.Setup(e => e.Height).Returns(300);

         // Act
         _grammar.Zoom("One", "One");

         // Assert
         _plotWindowMock.Verify(e => e.Close(), Times.Once);

         //_zoomWindowMock.Verify(e => e.SetImage(It.IsAny<Bitmap>()), Times.Once);
         _zoomWindowMock.Verify(e => e.Move(200.0, 200.0), Times.Once);
         _zoomWindowMock.Verify(e => e.Show(), Times.Once);

         _cellWindowMock.Verify(e => e.Move(100.0, 100.0), Times.Once);
         _cellWindowMock.Verify(e => e.Show(), Times.Once);
      }


      [Test]
      [TestCase("Zero", "Zero",  0,    0)]
      [TestCase("Nine", "Zero",  0,    900)]
      [TestCase("Zero", "India", 1800, 0)]
      [TestCase("Nine", "India", 1800, 900)]
      public void ShouldRepositionZoomWindowSoItStaysOnScreen(String y, String x, Int32 ox, Int32 oy) {
         // Arrange
         _screenMock.Setup(e => e.Bounds).Returns(
            new Rectangle(0, 0, 1920, 1080)
         );

         Int32 cellSize = 100;
         Int32 zoomWindowSize = 350;
         Int32 zoomx = ox + cellSize;
         Int32 zoomy = oy + cellSize;

         if (ox + zoomWindowSize > 1920)
            zoomx -= zoomWindowSize + cellSize;
         if (oy + zoomWindowSize > 1080)
            zoomy -= zoomWindowSize + cellSize;

         _zoomWindowMock.Setup(e => e.Width).Returns(zoomWindowSize);
         _zoomWindowMock.Setup(e => e.Height).Returns(zoomWindowSize);

         // Act
         _grammar.Zoom(x, y);

         // Assert
         _cellWindowMock.Verify(e => e.Move(ox, oy), Times.Once);
         _zoomWindowMock.Verify(e => e.Move(zoomx, zoomy), Times.Once);
      }

   }
}
