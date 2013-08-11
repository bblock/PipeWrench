// 
// Pyper - automate the transformation of text using "stackable" text filters
// Copyright (C) 2013  Barry Block 
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
   /// Groups lines together that contain a string.
   /// </summary>
   public sealed class GroupLines : FilterPlugin
   {
      public override void Execute()
      {
         string theStr = (string) CmdLine.GetArg(0).Value;
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         if (theStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         bool rangeGiven = (CmdLine.ArgCount > 1);

         int begPos = 0;
         int endPos = 0;

         if (rangeGiven)
         {
            begPos = (int) CmdLine.GetArg(1).Value;
            endPos = (int) CmdLine.GetArg(2).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(1).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(2).CharPos);
         }

         if (begPos > endPos)
         {
            // Oops.

            ThrowException("Ending position must be >= begin position.", CmdLine.GetArg(2).CharPos);
         }

         // Create a temporary text list for storing "grouped" lines:

         List<string> groupedLines = new List<string>();

         Open();

         try
         {
            // Process the text:

            while (!EndOfText)
            {
               string line = ReadLine();
               string newLine;

               if (rangeGiven)
                  newLine = line.Substring(begPos-1, endPos-begPos+1);
               else
                  newLine = line;

               if (StringMatched(theStr, newLine, ignoringCase, isRegEx))
               {
                  // string found.  Add it to the list of "grouped" lines:

                  groupedLines.Add(line);
               }
               else
               {
                  // string not found in line.  Output the line now:

                  WriteText(line);
               }
            }

            // Now, output all of the grouped lines:

            foreach (string tempStr in groupedLines)
            {
               WriteText(tempStr);
            }
         }

         finally
         {
            Close();
         }
      }

      public GroupLines(IFilter host) : base(host)
      {
         Template = "s [n n] /I /R";
      }
   }
}
