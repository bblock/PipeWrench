using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Returns the left-most characters from each line of the input text.
   /// </summary>
   public sealed class LeftChars : FilterPlugin
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

               if (line.Length > noOfChars)
               {
                  line = line.Substring(0, noOfChars);
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public LeftChars(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
