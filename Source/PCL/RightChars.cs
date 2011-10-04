using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Returns the right-most characters from each line of the input text.
   /// </summary>
   public sealed class RightChars : FilterPlugin
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
               string source = ReadLine();
               int len = source.Length;

               if (len >= noOfChars)
               {
                  string tempStr = source.Substring(len-noOfChars, noOfChars);
                  WriteText(tempStr);
               }
               else
                  WriteText(source);
            }
         }

         finally
         {
            Close();
         }
      }

      public RightChars(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
