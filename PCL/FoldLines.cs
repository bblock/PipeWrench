// 
// PipeWrench - automate the transformation of text using "stackable" text filters
// Copyright (c) 2014  Barry Block 
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

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Acts on sorted lists folding duplicate lines.
   /// </summary>
   public sealed class FoldLines : FilterPlugin
   {
      private int joinLinesOpt;
      private bool joiningLines;
      private int numericWidth;
      private char padChar;
      private string savedLine;
      private string accumLine;
      private int count;
      private bool explicitCounts;
      private int begPos;
      private int endPos;
      private bool rangeIsGiven;

      private void OutputLine()
      {
         if ((count > 0))
         {
            string tempStr = count.ToString().PadLeft(numericWidth, padChar);

            // Output line with count appended:

            if (joiningLines)
            {
               if (joinLinesOpt == 1)
               {
                  // Count is not appended to line.

                  WriteText(accumLine);
               }
               else
               {
                  // Count is appended to line.

                  WriteText(accumLine + ' ' + tempStr);
               }
            }
            else
            {
               if (rangeIsGiven)
               {
                  // Extend line if necessary:

                  while (savedLine.Length < endPos)
                  {
                     savedLine += ' ';
                  }

                  WriteText(savedLine.Substring(begPos-1, endPos-begPos+1) + ' ' + tempStr);
               }
               else
               {
                  WriteText(savedLine + ' ' + tempStr);
               }
            }
         }
      }

      private int GetPosOfAppendedCount(string st)
      {
         int i = st.Length-1;
         while ((i > -1) && (st[i] != ' ')) i--;
         return i+1;
      }

      /// <summary>
      /// Returns the given line's fold count or a "1" if lines implicitly contain a 1 count.
      /// </summary>
      private int GetCount(string line)
      {
         // Default to implicit count value:

         int result = 1;

         if (explicitCounts)
         {
            int charPos = GetPosOfAppendedCount(line);
            string tempStr = ScanInteger(line, ref charPos);

            if (tempStr != string.Empty)
            {
               try
               {
                  result = Int32.Parse(tempStr);
               }
               catch (Exception)
               {
                  // Numeric value is invalid.

                  result = -1;
               }
            }
            else
            {
               // Numeric value not found.

               result = -2;
            }
         }

         return result;
      }

      /// <summary>
      /// Removes the count (and the separating blank) from the given line.
      /// </summary>
      private void StripCount(ref string st)
      {
         if (explicitCounts)
         {
            int i;
            int j;

            // Remove the count from the end of the line:

            i = st.Length-1;
            j = 0;

            while ((i > -1) && (st[i] != ' '))
            {
               i--;
               j++;
            }

            if (i > -1) st = st.Remove(i, j+1);
         }
      }

      public override void Execute()
      {
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         numericWidth = CmdLine.GetIntSwitch("/W", 6);
         bool paddingWithZeros = CmdLine.GetBooleanSwitch("/Z");
         string delimiterStr = CmdLine.GetStrSwitch("/D", string.Empty);
         explicitCounts = CmdLine.GetBooleanSwitch("/E");
         joinLinesOpt = CmdLine.GetIntSwitch("/J", 0);
         joiningLines = (joinLinesOpt == 1) || (joinLinesOpt == 2);
         rangeIsGiven = CmdLine.ArgCount > 0;
         bool delimSpecified = delimiterStr != string.Empty;

         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));
         CheckIntRange(joinLinesOpt, 0, 2, "Join option", CmdLine.GetSwitchPos("/J"));

         char delimiter = '\0';
         if (delimSpecified) delimiter = delimiterStr[0];

         begPos = 0;
         endPos = 0;

         if (rangeIsGiven)
         {
            begPos = (int) CmdLine.GetArg(0).Value;
            endPos = (int) CmdLine.GetArg(1).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(0).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(1).CharPos);
         }

         if (begPos > endPos)
         {
            // Oops.

            ThrowException("Ending position must be >= begin position.", CmdLine.GetArg(1).CharPos);
         }

         if (paddingWithZeros)
            padChar = '0';
         else
            padChar = ' ';

         savedLine = null;
         accumLine = string.Empty;
         count = 0;

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int countVal = GetCount(line);
               StripCount(ref line);

               if ((delimSpecified && StringsCompare(line, savedLine, ignoringCase, delimiter)) ||
               (!delimSpecified && StringsCompare(line, savedLine, ignoringCase, begPos, endPos)))
               {
                  // The line compares with the last one.

                  switch (countVal)
                  {
                     case -1:

                        // Numeric value is invalid.

                        ThrowException("Numeric value on text line " + TextLineNo.ToString() + " is invalid.");
                        break;

                     case -2:

                        // Numeric value not found.

                        ThrowException("Numeric value on text line " + TextLineNo.ToString() + " not found.");
                        break;

                     default:

                        // Accumulate the count value:

                        count += countVal;

                        if (joiningLines)
                        {
                           // Accumulate the line:

                           accumLine += line;
                        }

                        break;
                  }
               }
               else
               {
                  // The line does not compare to the last one.

                  OutputLine();
                  savedLine = line;
                  if (joiningLines) accumLine = savedLine;
                  count = countVal;
               }
            }

            OutputLine();
         }

         finally
         {
            Close();
         }
      }

      public FoldLines(IFilter host) : base(host)
      {
         Template = "[n n] /Ds /E /I /Jn /Wn /Z";
      }
   }
}
