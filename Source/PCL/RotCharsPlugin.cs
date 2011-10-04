using System;

namespace Firefly.Pyper
{
   public abstract class RotCharsPlugin : FilterPlugin
   {
      /// <summary>
      /// Rotates source string by # of characters.
      /// </summary>
      private void RotateStr(ref string source, bool rotatingLeft, int noOfChars)
      {
         for (int i=1; i <= noOfChars; i++)
         {
            if (rotatingLeft)
            {
               source = source.Substring(1, source.Length-1) + source[0];
            }
            else
            {
               source = source[source.Length-1] + source.Substring(0, source.Length-1);
            }
         }
      }

      public override abstract void Execute();

      protected void Execute(bool rotatingLeft)
      {
         int noOfChars = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(noOfChars, 1, int.MaxValue, "No. of characters", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line.Length > 0)
               {
                  RotateStr(ref line, rotatingLeft, noOfChars);
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public RotCharsPlugin(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
