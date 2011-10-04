using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Strips the given number of characters from the end of each line of the input text.
   /// </summary>
   public sealed class StripChars : FilterPlugin
   {
      public override void Execute()
      {
         int noOfChars = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(noOfChars, 1, int.MaxValue, "No. of characters", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int len = line.Length;

               if (noOfChars <= len)
                  line = line.Substring(0, len-noOfChars);
               else
                  line = "";

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public StripChars(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
