// 
// Pyper - automate the transformation of text using "stackable" text filters
// Copyright (C) 2011  Barry Block 
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version. 
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE.  See the GNU General Public License for more details. 
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
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
