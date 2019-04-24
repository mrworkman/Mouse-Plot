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
using System.IO;
using System.Linq;
using System.Text;

using NLog;

using Renfrew.Grammar.Dragon;
using Renfrew.Grammar.FluentApi;
using Renfrew.NatSpeakInterop;

namespace Renfrew.Grammar {
   public class GrammarSerializer : IGrammarSerializer {

      private static Logger _logger = LogManager.GetCurrentClassLogger();

      #region Speech Recognition Constants
      private const UInt32 SRHDRTYPE_CFG     = 0;
      private const UInt32 SRHDRFLAG_UNICODE = 1;

      private const UInt32 SRCK_LANGUAGE       = 1;
      private const UInt32 SRCKCFG_WORDS       = 2;
      private const UInt32 SRCKCFG_RULES       = 3;
      private const UInt32 SRCKCFG_EXPORTRULES = 4;
      private const UInt32 SRCKCFG_IMPORTRULES = 5;
      #endregion

      public GrammarSerializer() {

      }

      private byte[] BuildRulesChunk(Grammar grammar) {
         var memoryStream = new MemoryStream();
         var stream = new BinaryWriter(memoryStream);

         var definitionFactory = new RuleDefinitionFactory(new RuleDirectiveFactory());

         var tables = definitionFactory.CreateDefinitionTables(grammar);

         Int32 ruleNumber = 1;
         foreach (var table in tables) {

            // The SRCFGRULE struct is 8 bytes long
            var length = table.Count() * (sizeof(Int32) * 2);

            stream.Write(length + sizeof(Int32) * 2);
            stream.Write(ruleNumber);

            foreach (var row in table) {
               _logger.Trace(row);

               stream.Write((UInt16) row.DirectiveType);
               stream.Write((UInt16) 0); // Assume probability of Zero

               if (row.ElementGrouping == ElementGroupings.NOT_APPLICABLE) {
                  stream.Write((UInt32) row.Id);
               } else {
                  stream.Write((UInt32) row.ElementGrouping);
               }
            }

            ruleNumber++;
         }

         try {
            return memoryStream.ToArray();
         } finally {
            stream.Dispose();
            memoryStream.Dispose();
         }
      }

      private byte[] BuildWordsChunk(IReadOnlyDictionary<String, UInt32> words) {
         var memoryStream = new MemoryStream();
         var stream = new BinaryWriter(memoryStream);

         foreach (var w in words) {
            var length = GetPaddedStringLength(w.Key);
            var nameBytes = Encoding.Unicode.GetBytes(w.Key);

            stream.Write(length + sizeof(Int32) * 2);
            stream.Write(w.Value);
            stream.Write(nameBytes);

            // Make sure that the word/rule name is padded to a 4-byte boundary
            stream.Write(new byte[length - nameBytes.Length]);
         }

         stream.Flush();

         try {
            return memoryStream.ToArray();
         } finally {
            stream.Dispose();
            memoryStream.Dispose();
         }
      }

      private Int32 GetPaddedStringLength(String s) {
         var numBytes = Encoding.Unicode.GetByteCount(s) + 2;
         var diff = numBytes % sizeof(Int32);

         // Pad to 4-byte boundary
         if (diff != 0)
            numBytes += (sizeof(Int32) - diff);

         return numBytes;
      }

      public byte[] Serialize(IGrammar grammar) {
         var memoryStream = new MemoryStream();
         var stream = new BinaryWriter(memoryStream);

         byte[] bytes;

         // Start off with the necessary header and flags
         stream.Write(SRHDRTYPE_CFG);
         stream.Write(SRHDRFLAG_UNICODE);

         // Export Rules Chunk
         stream.Write(SRCKCFG_EXPORTRULES);

         // Rule/Word chunks have the same format
         bytes = BuildWordsChunk(grammar.RuleIds);
         stream.Write(bytes.Length); // Chunk Size
         stream.Write(bytes);        // Chunk

         // Words Chunk
         stream.Write(SRCKCFG_WORDS);

         // Rule/Word chunks have the same format
         bytes = BuildWordsChunk(grammar.WordIds);
         stream.Write(bytes.Length); // Chunk Size
         stream.Write(bytes);        // Chunk

         // Rule Definition (Symbol) Chunk
         stream.Write(SRCKCFG_RULES);

         // This chunk has its own special format
         bytes = BuildRulesChunk(grammar as Grammar);
         stream.Write(bytes.Length); // Chunk Size
         stream.Write(bytes);        // Chunk

         stream.Flush();

         try {
            return memoryStream.ToArray();
         } finally {
            stream.Dispose();
            memoryStream.Dispose();
         }
      }
   }
}
