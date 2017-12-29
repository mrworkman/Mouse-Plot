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
using Drawing = System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Renfrew.Grammar;
using Renfrew.NatSpeakInterop;

namespace Renfrew.Core.Grammars.MousePlot {

   [GrammarExport("Mouse Plot", "A replacement for \"Mouse Grid\".")]
   public class MousePlotGrammar : Grammar.Grammar {

      private IScreen _currentScreen;
      private Drawing.Size _cellSize = new Drawing.Size(100, 100);

      private IWindow _plotWindow;
      private IWindow _zoomWindow;

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
         "White",
         "Yellow",
         "Green",
         "Red",
         "Gray",
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
      public MousePlotGrammar(IGrammarService grammarService, IScreen screen, IWindow plotWindow, IWindow zoomWindow)
         : base(grammarService) {

         _currentScreen = screen;
         _plotWindow = plotWindow;
         _zoomWindow = zoomWindow;
      }

      public MousePlotGrammar(IGrammarService grammarService)
         : this(grammarService, new TestableScreen().PrimaryScreen, new PlotWindow(), new ZoomWindow()) {

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
                  _plotWindow.Show();

               })
            .OptionallyWithRule("post_plot")
         );

         AddRule("post_plot", e => e
            .OneOf(
               p => p.WithRule("mouse_click"),

               p => p.SayOneOf("Monitor", "Screen")
                  .SayOneOf(_screenList.Keys)
                  .Do(spokenWords => SwitchScreen( _screenList[spokenWords.Last()] )),

               p => p.SayOneOf(_colourList),
               p => p.SayOneOf("Mark", "Drag"),

               p => p
                  .SayOneOf(alphaWords)
                  .SayOneOf(alphaWords)
                     .Do(spokenWords => Zoom(spokenWords.Last(), spokenWords.First()))
                  .OptionallyOneOf(
                     o => o.WithRule("mouse_click"),
                     o => o
                        .SayOneOf(alphaWords)
                        .SayOneOf(alphaWords)
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
               .Do(() => {
                  MakeGrammarNotExclusive();
                  DeactivateRule("post_plot");

                  _zoomWindow.Close();
                  _plotWindow.Close();
            })
         );

         Load();

         ActivateRule("mouse_plot");

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
         var i = _currentScreen.Bounds.Left + (_cellSize.Width * x) + (_cellSize.Width / 2);

         return (i > _currentScreen.Bounds.Right) ? _currentScreen.Bounds.Right - 1 : i;
      }

      public Int32 GetMouseYCoord(Int32 y) {
         var i = _currentScreen.Bounds.Top + (_cellSize.Height * y) + (_cellSize.Height / 2);

         return (i > _currentScreen.Bounds.Bottom) ? _currentScreen.Bounds.Bottom - 1: i;
      }

      private void SwitchScreen(Int32 screenNumber) {
         screenNumber--;

         if (screenNumber >= Screen.AllScreens.Length)
            return;

         _currentScreen = _currentScreen.AllScreens[screenNumber];

         _plotWindow.Move(_currentScreen.Bounds.Left, _currentScreen.Bounds.Top);
         _plotWindow.Show();
      }

      private void Zoom(String x, String y) {
         var mouseX = GetMouseXCoord(GetCoordinateOrdinal(x));
         var mouseY = GetMouseYCoord(GetCoordinateOrdinal(y));

         Cursor.Position = new Drawing.Point(mouseX, mouseY);

         _plotWindow.Close();

         // Position the zoom window so it appears
         // on the current screen in its entirety.
         Int32 offsetX = _cellSize.Width / 2;
         Int32 offsetY = _cellSize.Height / 2;

         if (mouseX + offsetX + _zoomWindow.Width >= _currentScreen.Bounds.Right)
            offsetX = -offsetX - (Int32) _zoomWindow.Width;

         if (mouseY + offsetY + _zoomWindow.Height >= _currentScreen.Bounds.Bottom)
            offsetY = -offsetY - (Int32) _zoomWindow.Height;

         _zoomWindow.Move(mouseX + offsetX, mouseY + offsetY);

         _zoomWindow.Show();
      }
   }
}
