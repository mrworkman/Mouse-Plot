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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

using Renfrew.Grammar;
using Renfrew.NatSpeakInterop;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Renfrew.Core.Grammars.MousePlot {

   [GrammarExport("Mouse Plot", "A replacement for \"Mouse Grid\".")]
   public class MousePlotGrammar : Grammar.Grammar {

      #region Move Elsewhere
      [System.Runtime.InteropServices.DllImport("user32.dll")]
      public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

      public const Int32 MOUSEEVENTF_LEFTDOWN   = 0x02;
      public const Int32 MOUSEEVENTF_LEFTUP     = 0x04;
      public const Int32 MOUSEEVENTF_RIGHTDOWN  = 0x08;
      public const Int32 MOUSEEVENTF_RIGHTUP    = 0x10;
      public const Int32 MOUSEEVENTF_MIDDLEDOWN = 0x20;
      public const Int32 MOUSEEVENTF_MIDDLEUP   = 0x40;
      #endregion

      private IScreen _currentScreen;
      private Size _cellSize = new Size(100, 100);

      private IWindow _plotWindow;
      private IWindow _cellWindow;
      private IZoomWindow _zoomWindow;

      private Point _currentCell = Point.Empty;

      private Bitmap _bitmap;
      private Bitmap _cursorBitmap;

      private bool _isZoomed = false;

      #region Word Lists
      private Dictionary<String, String> _alphaList = new Dictionary<String, String> {
         { "Alpha"   , "A" }, { "A", "A" },
         { "Bravo"   , "B" }, { "B", "B" },
         { "Charlie" , "C" }, { "C", "C" },
         { "Delta"   , "D" }, { "D", "D" },
         { "Echo"    , "E" }, { "E", "E" },
         { "Foxtrot" , "F" }, { "F", "F" },
         { "Golf"    , "G" }, { "G", "G" },
         { "Hotel"   , "H" }, { "H", "H" },
         { "India"   , "I" }, { "I", "I" },
         { "Juliet"  , "J" }, { "J", "J" },
         { "Kilo"    , "K" }, { "K", "K" },
         { "Lima"    , "L" }, { "L", "L" },
         { "Mike"    , "M" }, { "M", "M" },
         { "November", "N" }, { "N", "N" },
         { "Oscar"   , "O" }, { "O", "O" },
         { "Papa"    , "P" }, { "P", "P" },
         { "Quebec"  , "Q" }, { "Q", "Q" },
         { "Romeo"   , "R" }, { "R", "R" },
         { "Sierra"  , "S" }, { "S", "S" },
         { "Tango"   , "T" }, { "T", "T" },
         { "Uniform" , "U" }, { "U", "U" },
         { "Victor"  , "V" }, { "V", "V" },
         { "Whiskey" , "W" }, { "W", "W" },
         { "X-ray"   , "X" }, { "X", "X" },
         { "Yankee"  , "Y" }, { "Y", "Y" },
         { "Zulu"    , "Z" }, { "Z", "Z" },

         { "Zero",  "0" },
         { "One",   "1" },
         { "Two",   "2" },
         { "Three", "3" },
         { "Four",  "4" },
         { "Five",  "5" },
         { "Six",   "6" },
         { "Seven", "7" },
         { "Eight", "8" },
         { "Nine",  "9" },
      };
      private List<String> _colourList = new List<String> {
         "Black",
         "Blue",
         "Green",
         "Grey",
         "Red",
         "White",
         "Yellow",
      };
      private List<String> _clickList = new List<String> {
         "Click",        "Right Click",
         "Double Click", "Right Double",
         "Triple Click", "Right Triple",
         "Middle Click",
      };
      private Dictionary<String, Int32> _screenList = new Dictionary<String, Int32> {
         { "One",    1 },
         { "Two",    2 },
         { "Three",  3 },
         { "Four",   4 },
         { "Five",   5 },
         { "Six",    6 },
         { "Seven",  7 },
         { "Eight",  8 },
         { "Nine",   9 },
         { "Ten",    10 },
         { "Eleven", 11 },
         { "Twelve", 12 },
      };
      #endregion

      // For Testing
      public MousePlotGrammar(IGrammarService grammarService, IScreen screen,
                              IWindow plotWindow, IZoomWindow zoomWindow, IWindow cellWindow)
         : base(grammarService) {

         _currentScreen = screen;
         _plotWindow = plotWindow;
         _zoomWindow = zoomWindow;
         _cellWindow = cellWindow;
      }

      public MousePlotGrammar(IGrammarService grammarService)
         : this(grammarService, new TestableScreen().PrimaryScreen,
              new PlotWindow(), new ZoomWindow(), new CellWindow()) {

      }

      public override void Dispose() {
         throw new NotImplementedException();
      }

      public override void Initialize() {
         var alphaWords = _alphaList.Select(e => e.Key).ToArray();

         AddRule("mouse_plot", e => e
            .Say("Plot")
               .Do(() => {
                  ActivateRule("post_plot");
                  MakeGrammarExclusive();

                  _zoomWindow.Close();
                  _cellWindow.Close();
                  
                  _plotWindow.Show();

                  _isZoomed = false;
               })
            .OptionallyWithRule("post_plot")
         );

         AddRule("post_plot", e => e
            .OneOf(
               p => p.WithRule("mouse_click"),

               p => p
                  .SayOneOf("Monitor", "Screen")
                  .SayOneOf(_screenList.Keys)
                  .Do(spokenWords => SwitchScreen( _screenList[spokenWords.Last()] )),

               p => p.SayOneOf(_colourList)
                  .Do(spokenWords => SetColour(spokenWords.First())),

               p => p.SayOneOf("Mark", "Drag"),

               p => p
                  .SayOneOf(alphaWords)
                  .SayOneOf(alphaWords)
                     .Do(spokenWords => MoveCursor(spokenWords.Last(), spokenWords.First()))
                  .OptionallyOneOf(
                     o => o.WithRule("mouse_click"),
                     o => o
                        .SayOneOf(alphaWords)
                        .SayOneOf(alphaWords)
                           .Do(spokenWords => MoveCursor(spokenWords.Last(), spokenWords.First()))
                        .Optionally(q => q.WithRule("mouse_click"))
                  )
            )
            .Do(spokenWords => {

               foreach (var w in spokenWords) {
                  Debug.WriteLine($"Spoken: {w}");
               }

            })
         );

         AddRule("mouse_click", e => e
            .OptionallySay("Mouse")
            .SayOneOf(_clickList)
               .Do(spokenWords => {
                  MakeGrammarNotExclusive();
                  DeactivateRule("post_plot");

                  _zoomWindow.Close();
                  _cellWindow.Close();
                  _plotWindow.Close();

                  // Wait a short period to make sure the windows have closed
                  System.Threading.Thread.Sleep(100);

                  var s = spokenWords.Last();
                  var b = MouseButtons.Left;
                  var c = 1;

                  // Which button ?
                  if (s.Contains("Right") == true)
                     b = MouseButtons.Right;
                  if (s.Contains("Middle") == true)
                     b = MouseButtons.Middle;

                  // How many clicks ?
                  if (s.Contains("Double") == true)
                     c = 2;
                  if (s.Contains("Triple") == true)
                     c = 3;

                  ClickMouse(b, c);

                  _isZoomed = false;
            })
         );

         Load();

         ActivateRule("mouse_plot");

      }

      private void ClickMouse(MouseButtons button, Int32 times) {
         Int32 downButton, upButton;

         switch (button) {
            case MouseButtons.Right:
               downButton = MOUSEEVENTF_RIGHTDOWN;
               upButton   = MOUSEEVENTF_RIGHTUP;
               break;
            case MouseButtons.Middle:
               downButton = MOUSEEVENTF_MIDDLEDOWN;
               upButton   = MOUSEEVENTF_MIDDLEUP;
               break;
            default:
               downButton = MOUSEEVENTF_LEFTDOWN;
               upButton   = MOUSEEVENTF_LEFTUP;
               break;
         }

         Int32 x = Cursor.Position.X;
         Int32 y = Cursor.Position.Y;

         for (int i = 0; i < times; i++) {
            mouse_event(downButton, x, y, 0, 0);
            mouse_event(upButton,   x, y, 0, 0);
         }
      }

      public Int32 GetCellXCoord(Int32 x) {
         var i = GetXScreenOffset(x);

         // Integer truncation will have the desired effect
         if (i > _currentScreen.Bounds.Right)
            i = _currentScreen.Bounds.Left + (_currentScreen.Bounds.Width / _cellSize.Width) * _cellSize.Width;

         return i;
      }

      public Int32 GetCellYCoord(Int32 y) {
         var i = GetYScreenOffset(y);

         // Integer truncation will have the desired effect
         if (i > _currentScreen.Bounds.Bottom)
            i = _currentScreen.Bounds.Top + (_currentScreen.Bounds.Height / _cellSize.Height) * _cellSize.Height;

         return i;
      }

      public Int32 GetCoordinateOrdinal(String c) {
         if (_alphaList.ContainsKey(c) == false)
            throw new ArgumentOutOfRangeException(nameof(c), c);

         c = _alphaList[c];

         if (Int32.TryParse(c, out int i) == true)
            return i;

         return c.First() - 'A' + 10;
      }

      public Int32 GetMouseXCoord(Int32 x) {
         x = GetXScreenOffset(x) + (_cellSize.Width / 2);

         if (x > _currentScreen.Bounds.Right)
            x = _currentScreen.Bounds.Right - 1;

         return x;
      }

      public Int32 GetMouseYCoord(Int32 y) {
         y = GetYScreenOffset(y) + (_cellSize.Height / 2);

         if (y > _currentScreen.Bounds.Bottom)
            y = _currentScreen.Bounds.Bottom - 1;

         return y;
      }

      public Int32 GetXScreenOffset(Int32 x) =>
         _currentScreen.Bounds.Left + (_cellSize.Width * x);

      public Int32 GetYScreenOffset(Int32 y) =>
         _currentScreen.Bounds.Top + (_cellSize.Height * y);

      public Int32 GetZoomedCellXCoord(Int32 x) {
         if (x > 8) x = 8;

         var subCellWidth = _cellSize.Width * 3 / 9;
         return (subCellWidth * x) + (subCellWidth / 2);
      }

      public Int32 GetZoomedCellYCoord(Int32 y) {
         if (y > 8) y = 8;

         var subCellHeight = _cellSize.Height * 3 / 9;
         return (subCellHeight * y) + (subCellHeight / 2);
      }

      public Int32 GetZoomedMouseXCoord(Int32 x) {
         if (x > 8) x = 8;

         var subCellWidth = _cellSize.Width / 9;
         return _currentCell.X + (subCellWidth * x) + (subCellWidth / 2);
      }

      public Int32 GetZoomedMouseYCoord(Int32 y) {
         if (y > 8) y = 8;

         var subCellHeight = _cellSize.Height / 9;
         return _currentCell.Y + (subCellHeight * y) + (subCellHeight / 2);
      }

      public void MoveCursor(String x, String y) {
         Int32 mouseX, mouseY, cellX, cellY;

         if (_isZoomed == true) {
            mouseX = GetZoomedMouseXCoord(GetCoordinateOrdinal(x));
            mouseY = GetZoomedMouseYCoord(GetCoordinateOrdinal(y));

            Cursor.Position = new Point(mouseX, mouseY);

            return;
         }

         mouseX = GetMouseXCoord(GetCoordinateOrdinal(x));
         mouseY = GetMouseYCoord(GetCoordinateOrdinal(y));

         _currentCell = new Point(
            GetCellXCoord(GetCoordinateOrdinal(x)),
            GetCellYCoord(GetCoordinateOrdinal(y))
         );

         Zoom(x, y);

         Cursor.Position = new Point(mouseX, mouseY);
      }

      private void SetColour(String colourName) {
         if (Enum.TryParse(colourName, out GridColour colour) == false)
            colour = GridColour.Yellow;

         _plotWindow.SetColour(colour);
         _zoomWindow.SetColour(colour);
         _cellWindow.SetColour(colour);
      }

      private void SwitchScreen(Int32 screenNumber) {
         screenNumber--;

         if (screenNumber >= Screen.AllScreens.Length)
            return;

         _currentScreen = _currentScreen.AllScreens[screenNumber];

         _plotWindow.Move(_currentScreen.Bounds.Left, _currentScreen.Bounds.Top);
         _plotWindow.Show();
      }

      private void TakeScreenshot(Int32 x, Int32 y, Int32 width, Int32 height) {
         if (_bitmap != null) {
            _bitmap.Dispose();
         }

         _bitmap = new Bitmap(width, height);

         // Take the screenshot
         using (var g = Graphics.FromImage(_bitmap)) {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;

            g.CopyFromScreen(x, y, 0, 0, _bitmap.Size);
         }
      }

      public void Zoom(String x, String y) {
         var mouseX = GetMouseXCoord(GetCoordinateOrdinal(x));
         var mouseY = GetMouseYCoord(GetCoordinateOrdinal(y));
         var cellX  = GetCellXCoord(GetCoordinateOrdinal(x));
         var cellY  = GetCellYCoord(GetCoordinateOrdinal(y));

         _plotWindow.Close();

         // Position the zoom window so it appears
         // on the current screen in its entirety.
         Int32 offsetX = (_cellSize.Width / 4) * 3;
         Int32 offsetY = (_cellSize.Height / 4) * 3;

         if (mouseX + offsetX + _zoomWindow.Width >= _currentScreen.Bounds.Right)
            offsetX = -offsetX - (Int32) _zoomWindow.Width;

         if (mouseY + offsetY + _zoomWindow.Height >= _currentScreen.Bounds.Bottom)
            offsetY = -offsetY - (Int32) _zoomWindow.Height;

         _cellWindow.Move(cellX-4, cellY-4);
         _cellWindow.Show();

         _zoomWindow.SetSource(cellX, cellY, _cellSize.Width, _cellSize.Height);

         _zoomWindow.Move(mouseX + offsetX, mouseY + offsetY);
         _zoomWindow.Show();

         _isZoomed = true;
      }
   }
}
