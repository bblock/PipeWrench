using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Outputs lines that are duplicated in the text.
   /// </summary>
   public sealed class OutDuplLines : FilterPlugin
   {
      public override void Execute()
      {
         string delimiterStr = CmdLine.GetStrSwitch("/D", string.Empty);
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool rangeIsGiven = (CmdLine.ArgCount > 0);
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

         string oldLine = null; // Must default to null.
         bool begOfSeq = false;

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if ((delimSpecified && StringsCompare(line, oldLine, ignoringCase, delimiter)) ||
               (!delimSpecified && StringsCompare(line, oldLine, ignoringCase, begPos, endPos)))
               {
                  // Found a duplicate line.

                  if (begOfSeq)
                  {
                     // Output the previous, (first duplicate) line:

                     WriteText(oldLine);
                     begOfSeq = false;
                  }

                  // Output the current duplicate line:

                  WriteText(line);
               }
               else
               {
                  // The line differs from the previous one.

                  oldLine = line;
                  begOfSeq = true;
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public OutDuplLines(IFilter host) : base(host)
      {
         Template = "[n n] /Ds /I";
      }
   }
}
