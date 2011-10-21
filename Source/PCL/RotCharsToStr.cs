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

namespace Firefly.Pyper
{
   /// <summary>
   /// Rotates each line until the given string is found at its beginning.
   /// If the string exists in the line multiple times, you can rotate to
   /// a given occurence of the string.  Rotation continues with the first
   /// occurence if the string is not found the specified # of times.
   /// </summary>
   public sealed class RotCharsToStr : FilterPlugin
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

                  for (int j=1; j <= noOfRotates; j++)
                  {
                     if (j > 1)
                     {
                        // Move previous match found to end of line:

                        line = line.Substring(matchStr.Length) + matchStr;
                     }

                     // Find the next match:

                     int charPos = StringPos(theStr, line, ignoringCase, isRegEx, out matchStr);

                     if (charPos >= 0)
                     {
                        // Found it.

                        if (charPos > 0)
                        {
                           // Rotate to it:

                           string front = line.Substring(0, charPos);
                           string back = line.Substring(charPos, line.Length - charPos);
                           line = back + front;
                        }
                     }
                     else
                     {
                        // No match found.  Quit:

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

      public RotCharsToStr(IFilter host) : base(host)
      {
         Template = "s /I /Nn /R";
      }
   }
}
