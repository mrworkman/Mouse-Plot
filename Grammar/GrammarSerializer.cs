using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renfrew.Grammar {
   using Elements;

   internal class GrammarSerializer {

      #region Speech Recognition Constants
      private const UInt32 SRHDRTYPE_CFG     = 0;
      private const UInt32 SRHDRFLAG_UNICODE = 1;

      private const UInt32 SRCK_LANGUAGE       = 1;
      private const UInt32 SRCKCFG_WORDS       = 2;
      private const UInt32 SRCKCFG_RULES       = 3;
      private const UInt32 SRCKCFG_EXPORTRULES = 4;
      private const UInt32 SRCKCFG_IMPORTRULES = 5;
      #endregion

      internal GrammarSerializer() {

      }

      private byte[] BuildRulesChunk(dynamic x) {
         throw new NotImplementedException();
      }

      private byte[] BuildWordsChunk(IList<String> words) {
         var memoryStream = new MemoryStream();
         var stream = new BinaryWriter(memoryStream);

         Int32 wordNumber = 0;
         foreach (var e in words) {
            var length = GetPaddedStringLength(e);
            var nameBytes = Encoding.Unicode.GetBytes(e);

            stream.Write(length + sizeof(Int32) * 2);
            stream.Write(wordNumber);
            stream.Write(nameBytes);

            var diff = length - nameBytes.Length;

            // Make sure that the word/rule name is padded to a 4-byte boundary
            for (Int32 i = 0; i < diff; i++)
               stream.Write((byte)0);

            wordNumber++;
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
         var numBytes = Encoding.Unicode.GetByteCount(s) + 1;
         var diff = numBytes % sizeof(Int32);

         // Pad to 4-byte boundary
         if (diff != 0)
            numBytes += (sizeof(Int32) - diff);

         return numBytes;
      }

      internal byte[] Serialize(Grammar grammar) {
         var memoryStream = new MemoryStream();
         var stream = new BinaryWriter(memoryStream);

         byte[] bytes;

         // Start off with the necessary header and flags
         stream.Write(SRHDRTYPE_CFG);
         stream.Write(SRHDRFLAG_UNICODE);

         // Export Rules Chunk
         stream.Write(SRCKCFG_EXPORTRULES);

         // Rule/Word chunks have the same format
         bytes = BuildWordsChunk(grammar.RuleNames);
         stream.Write(bytes.Length); // Chunk Size
         stream.Write(bytes);        // Chunk
         
         // Words Chunk
         stream.Write(SRCKCFG_WORDS);

         // Rule/Word chunks have the same format
         bytes = BuildWordsChunk(grammar.Words);
         stream.Write(bytes.Length); // Chunk Size
         stream.Write(bytes);        // Chunk

         // Rule Definition (Symbol) Chunk
         stream.Write(SRCKCFG_RULES);

         // This chunk has its own special format
         bytes = BuildRulesChunk(grammar.Rules);
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
