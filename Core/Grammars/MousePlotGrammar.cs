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

using Renfrew.NatSpeakInterop;

namespace Renfrew.Core.Grammars {
   using Grammar;

   [GrammarExport("Mouse Plot", "A replacement for \"Mouse Grid\".")]
   public class MousePlotGrammar : Grammar {

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
      #endregion

      public MousePlotGrammar(IGrammarService grammarService)
         : base(grammarService) {

      }

      public override void Dispose() {
         throw new NotImplementedException();
      }

      public override void Initialize() {

         var alphaWords = _alphaList.Select(e => e.Key).ToArray();

         AddRule("mouse_plot", e => e
            .Say("Plot")
               .Do(MakeGrammarExclusive)
            .OptionallyWithRule("post_plot")
         );

         AddRule("post_plot", e => e
            .OneOf(

               p => p.SayOneOf(_clickList),

               p => p.Say("Monitor")
                  .SayOneOf("One", "Two", "Three", "Four"),

               p => p.SayOneOf(_colourList),
               p => p.SayOneOf("Mark", "Drag"),

               p => p.SayOneOf(alphaWords)
                  .SayOneOf(alphaWords)
                  .OptionallyOneOf(
                     o => o.SayOneOf(_clickList),
                     o => o.SayOneOf(alphaWords)
                        .SayOneOf(alphaWords)
                        .Optionally(q => q.SayOneOf(_clickList))
                  )
            )
            .Do(spokenWords => {

               foreach (var w in spokenWords) {
                  Debug.WriteLine($"Spoken: {w}");
               }

               if (spokenWords.Any() == true)
                  MakeGrammarNotExclusive();
            })
         );

         Load();

         ActivateRule("mouse_plot");

      }
   }
}
