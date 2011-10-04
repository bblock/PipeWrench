using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Outputs lines according to their position.
   /// </summary>
   public sealed class LinesByPos : FilterPlugin
   {
      private bool InRange(int setNo)
      {
         bool result = false;
         int prevLine = 0;

         for (int i=0; i < (CmdLine.ArgCount / 2); i++)
         {
            int j = (i * 2);
            int begLine = (int) CmdLine.GetArg(j).Value;
            int endLine = (int) CmdLine.GetArg(j+1).Value;

            CheckIntRange(begLine, 1, int.MaxValue, "Line no.", CmdLine.GetArg(j).CharPos);
            CheckIntRange(endLine, 1, int.MaxValue, "Line no.", CmdLine.GetArg(j+1).CharPos);

            if (begLine > prevLine)
            {
               prevLine = begLine;

               if (endLine >= begLine)
               {
                  if ((setNo >= begLine) && (setNo <= endLine))
                  {
                     result = true;
                     break;
                  }
               }
               else
               {
                  // Begin/End pairs are reversed.

                  ThrowException("End line no. must be >= begin line no.",
                  CmdLine.GetArg(j+1).CharPos);
               }
            }
            else
            {
               // Begin/End pairs must be in ascending order.

               ThrowException("Begin/End pairs must be in ascending order.",
               CmdLine.GetArg(j).CharPos);
            }
         }

         return result;
      }

      public override void Execute()
      {
         int noOfSets = CmdLine.GetIntSwitch("/S", 0);

         if (noOfSets != 0) CheckIntRange(noOfSets, 1, int.MaxValue,
         "Set size", CmdLine.GetSwitchPos("/S"));

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int setNo;

               if (noOfSets <= 0)
                  setNo = TextLineNo;
               else
                  setNo = (TextLineNo-1) %  noOfSets + 1;

               if (InRange(setNo)) WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public LinesByPos(IFilter host) : base(host)
      {
         Template = "n n [n n...] /Sn";
      }
   }
}
