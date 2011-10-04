using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Outputs the given number of lines from the end of the input text.
   /// </summary>
   public sealed class BottomLines : FilterPlugin
   {
      public override void Execute()
      {
         int NoOfLines = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(NoOfLines, 1, int.MaxValue, "No. of lines", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (TextLineNo >= TextLineCount - NoOfLines + 1)
               {
                  WriteText(line);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public BottomLines(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
