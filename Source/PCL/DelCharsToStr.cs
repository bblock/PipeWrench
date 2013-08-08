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

namespace Firefly.Pyper
{
   /// <summary>
   /// Deletes characters in a line until the given string is found at its beginning.
   /// If the string exists in the line multiple times, you can delete to a given
   /// occurence of the string.  If the string is not found the specified # of times
   /// then the line is left in its original state.
   /// </summary>
   public sealed class DelCharsToStr : FilterPlugin
   {

      public override void Execute()
      {
         string theStr = (string) CmdLine.GetArg(0).Value;
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         int noOfRotates = CmdLine.GetIntSwitch("/N", 1);
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         CheckIntRange(noOfRotates, 1, int.MaxValue, "No. of rotates", CmdLine.GetSwitchPos("/N"));
         if (theStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line.Length > 0)
               {
                  string matchStr = string.Empty;
                  string savedLine = line;

                  for (int j=1; j <= noOfRotates; j++)
                  {
                     if (j > 1)
                     {
                        // Delete the previous match from beginning of line:

                        line = line.Substring(matchStr.Length);
                     }

                     // Find the next match:

                     int charPos = StringPos(theStr, line, ignoringCase, isRegEx, out matchStr);

                     if (charPos >= 0)
                     {
                        // Found it.

                        if (charPos > 0)
                        {
                           // Rotate to it:

                           line = line.Substring(charPos, line.Length - charPos);
                        }
                     }
                     else
                     {
                        // No match found.

                        if (j > 1)
                        {
                           // Restore the line:

                           line = savedLine;
                        }

                        // Quit:

                        break;
                     }
                  }
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelCharsToStr(IFilter host) : base(host)
      {
         Template = "s /I /Nn /R";
      }
   }
}
