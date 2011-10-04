using System;
using System.Collections.Generic;

namespace Firefly.Pyper
{
   /// <summary>
   /// A pre-filter to the JoinLines filter which allows
   /// columns to be ordered down instead of across.
   /// </summary>
   public sealed class ColumnOrder : FilterPlugin
   {
      public override void Execute()

      {
         int rows = (int) CmdLine.GetArg(0).Value;
         int cols = (int) CmdLine.GetArg(1).Value;
         CheckIntRange(rows, 1, int.MaxValue, "Rows", CmdLine.GetArg(0).CharPos);
         CheckIntRange(cols, 1, int.MaxValue, "Columns", CmdLine.GetArg(1).CharPos);

         // Create a temporary list:

         List<string> tempList = new List<string>();

         // Load the source lines into the temporary list:

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               tempList.Add(line);
            }

            // Translate them into the desired table:

            int noOfElements = rows * cols;

            if (TextLineCount < noOfElements)
            {
               // Pad the list, (with blank strings) to rows x cols elements:

               int noOfLines = TextLineCount;

               while (noOfLines < noOfElements)
               {
                  tempList.Add(string.Empty);
                  noOfLines++;
               }
            }
            else
            {
               if (TextLineCount > noOfElements)
               {
                  // Table dimensions are too small.

                  ThrowException("Table dimensions are too small.");
               }
            }

            int i = 0;

            for (int r=1; r <= rows; r++)
            {
               for (int c=1; c <= cols; c++)
               {
                  // Output the string:

                  WriteText(tempList[i]);

                  // Position to the next element of the row:

                  i = (i + rows) % noOfElements;
               }

               // Position to the first element of the next row:

               i++;
            }
         }

         finally
         {
            Close();
         }
      }

      public ColumnOrder(IFilter host) : base(host)
      {
         Template = "n n";
      }
   }
}
