using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Outputs the given # of lines from the top of the text.
   /// </summary>
   public sealed class TopLines : FilterPlugin
   {
      public override void Execute()
      {
         int noOfLines = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(noOfLines, 1, int.MaxValue, "No. of lines", CmdLine.GetArg(0).CharPos);

         int lineCount = 0;

         Open();

         try
         {
            while (!EndOfText && (lineCount < noOfLines))
            {
               lineCount++;
               WriteText(ReadLine());
            }
         }

         finally
         {
            Close();
         }
      }

      public TopLines(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
