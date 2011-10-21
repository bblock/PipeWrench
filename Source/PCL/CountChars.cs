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
   /// Returns the number of characters in the input text.
   /// </summary>
   public sealed class CountChars : FilterPlugin
   {
      public override void Execute()
      {
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         bool paddingWithZeros = CmdLine.GetBooleanSwitch("/Z");
         bool perLineCount = CmdLine.GetBooleanSwitch("/L");
         int insertPos = CmdLine.GetIntSwitch("/I", 0);
         char padChar;
         long count = 0;

         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));
         if (insertPos != 0) CheckIntRange(insertPos, 1, int.MaxValue, "Char. position",
         CmdLine.GetSwitchPos("/I"));

         if (paddingWithZeros)
            padChar = '0';
         else
            padChar = ' ';

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int lineLen = line.Length;

               if (perLineCount)
               {
                  if (insertPos > 0)
                  {
                     // Inserting the count back into the line.
                     // Pad the source line to the insertion point:

                     while (line.Length < insertPos - 1)
                     {
                        line += ' ';
                     }

                     // Insert the count:

                     WriteText(line.Insert(insertPos-1, lineLen.ToString().PadLeft(numericWidth, padChar)));
                  }
                  else
                  {
                     // Outputting only the count.

                     WriteText(lineLen.ToString().PadLeft(numericWidth, padChar));
                  }
               }
               else
                  count += lineLen;
            }

            if (!perLineCount) WriteText(count.ToString().PadLeft(numericWidth, padChar));
         }

         finally
         {
            Close();
         }
      }

      public CountChars(IFilter host) : base(host)
      {
         Template = "/In /L /Wn /Z";
      }
   }
}
