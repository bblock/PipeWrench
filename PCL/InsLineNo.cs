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
   /// Inserts a line number at the given character position of each line.
   /// </summary>
   public sealed class InsLineNo : FilterPlugin
   {
      public override void Execute()
      {
         char padChar;
         string tempStr;

         int initLineNo = CmdLine.GetIntSwitch("/L", 1);
         int lineNoIncr = CmdLine.GetIntSwitch("/I", 1);
         int lineNoPos = CmdLine.GetIntSwitch("/P", 1);
         int lineNoSetSize = CmdLine.GetIntSwitch("/S", 0);
         bool paddingWithZeros = CmdLine.GetBooleanSwitch("/Z");
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);

         CheckIntRange(lineNoPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/P"));
         if (lineNoSetSize != 0) CheckIntRange(lineNoSetSize, 1, int.MaxValue, "Set size",
         CmdLine.GetSwitchPos("/S"));
         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));

         int currLineNo = initLineNo;
         int count = 1;

         Open();

         try
         {
            while (!EndOfText)
            {
               string source = ReadLine();

               if (paddingWithZeros)
                  padChar = '0';
               else
                  padChar = ' ';

               if (currLineNo < 0)
               {
                  // The line # is negative.

                  if (paddingWithZeros)
                  {
                     tempStr = (-currLineNo).ToString().PadLeft(numericWidth, padChar);
                     if (tempStr[0] == '0') tempStr = '-' + tempStr.Substring(1, tempStr.Length-1);
                  }
                  else
                  {
                     tempStr = currLineNo.ToString().PadLeft(numericWidth, padChar);
                  }
               }
               else
               {
                  // The line # is positive.

                  tempStr = currLineNo.ToString().PadLeft(numericWidth, padChar);
               }

               while (source.Length < lineNoPos-1)
               {
                  source += ' ';
               }

               source = source.Insert(lineNoPos-1, tempStr);
               WriteText(source);

               if (lineNoSetSize > 0)
               {
                  // Repeating line number sequence over and over.

                  if (count == lineNoSetSize)
                  {
                     // Restart the sequence:

                     currLineNo = initLineNo;
                     count = 1;
                  }
                  else
                  {
                     // Increment the current line number and the set count:

                     currLineNo += lineNoIncr;
                     count++;
                  }
               }
               else
               {
                  // Not repeating.  Just increment the current line number:

                  currLineNo += lineNoIncr;
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public InsLineNo(IFilter host) : base(host)
      {
         Template = "/Ln /In /Pn /Sn /Wn /Z";
      }
   }
}
