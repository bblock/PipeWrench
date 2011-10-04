using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Splits each line at one or more character positions.
   /// </summary>
   public sealed class SplitLines : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int offset = 0;
               int priorPos = 0;

               for (int i=0; i < CmdLine.ArgCount; i++)
               {
                  int charPos = (int) CmdLine.GetArg(i).Value;
                  CheckIntRange(charPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(i).CharPos);

                  if (charPos > priorPos)
                  {
                     priorPos = charPos;
                     int offsetPos = charPos - offset;

                     if (offsetPos <= line.Length)
                     {
                        WriteText(line.Substring(0, offsetPos-1));
                        line = line.Remove(0, offsetPos-1);
                        offset += offsetPos - 1;
                     }
                  }
                  else
                  {
                     // Character positions are not ascending.

                     ThrowException("Character positions must be in ascending order.",
                     CmdLine.GetArg(i).CharPos);
                  }
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public SplitLines(IFilter host) : base(host)
      {
         Template = "n [n...]";
      }
   }
}
