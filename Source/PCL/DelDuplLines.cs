using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Acts on sorted lists removing all duplicate lines.
   /// </summary>
   public sealed class DelDuplLines : FilterPlugin
   {
      public override void Execute()
      {
         bool deletingAll = CmdLine.GetBooleanSwitch("/A");
         string delimiterStr = CmdLine.GetStrSwitch("/D", string.Empty);
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool rangeIsGiven = CmdLine.ArgCount > 0;
         bool delimSpecified = delimiterStr != string.Empty;

         char delimiter = '\0';
         if (delimSpecified) delimiter = delimiterStr[0];

         int begPos = 0;
         int endPos = 0;

         if (rangeIsGiven)
         {
            begPos = (int) CmdLine.GetArg(0).Value;
            endPos = (int) CmdLine.GetArg(1).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(0).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(1).CharPos);

            if (begPos > endPos)
            {
               // Oops.

               ThrowException("End position must be >= begin position.", CmdLine.GetArg(1).CharPos);
            }
         }

         string savedLine = null; // Must default to null.
         int count = 0;

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (!((delimSpecified && StringsCompare(line, savedLine, ignoringCase, delimiter)) ||
               (!delimSpecified && StringsCompare(line, savedLine, ignoringCase, begPos, endPos))))
               {
                  if ((deletingAll && (count == 1)) || (!deletingAll && (count > 0)))
                  {
                     WriteText(savedLine);
                  }

                  savedLine = line;
                  count = 1;
               }
               else
                  count++;
            }

            if ((deletingAll && (count == 1)) || (!deletingAll && (count > 0)))
            {
               WriteText(savedLine);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelDuplLines(IFilter host) : base(host)
      {
         Template = "[n n] /A /Ds /I";
      }
   }
}
