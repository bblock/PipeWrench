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
   /// Surrounds lines with quotes.
   /// </summary>
   public sealed class QuoteLines : FilterPlugin
   {
      private char quoteChar;
      private bool unquoting;
      private bool usingBackslashQuote;
      private char delimiterChar;

      private bool IsNumeric(string theStr)
      {
         bool isDigit = false;

         for (int i=0; i < theStr.Length; i++)
         {
            isDigit = Char.IsDigit(theStr[i]);
            if (!isDigit) break;
         }

         return isDigit;
      }

      private void QuoteLine(string line, int option)
      {
         string tempStr;

         if (unquoting)
         {
            if ((line.Length > 1) && (line[0] == quoteChar) && (line[line.Length-1] == quoteChar))
            {
               // Unquote the quoted line:

               tempStr = line.Substring(1, line.Length-2);

               if (usingBackslashQuote)
                  tempStr = tempStr.Replace("\\" + quoteChar, quoteChar.ToString());
               else
                  tempStr = tempStr.Replace(quoteChar.ToString() + quoteChar.ToString(), quoteChar.ToString());

               WriteText(tempStr);
            }
            else
               WriteText(line);
         }
         else
         {
            if (usingBackslashQuote)
               tempStr = line.Replace(quoteChar.ToString(), "\\" + quoteChar);
            else
               tempStr = line.Replace(quoteChar.ToString(), quoteChar.ToString() + quoteChar.ToString());

            switch (option)
            {
               case 0:

                  // Quoting all lines.

                  WriteText(quoteChar + tempStr + quoteChar);
                  break;

               case 1:

                  // Only enclosing fields that have embedded quotes or delimiters.

                  if ((tempStr.Contains(quoteChar.ToString())) || (tempStr.Contains(delimiterChar.ToString())))
                     WriteText(quoteChar + tempStr + quoteChar);
                  else
                     WriteText(tempStr);

                  break;

               case 2:

                  // Quoting non-numeric lines only.

                  if (!IsNumeric(tempStr))
                     WriteText(quoteChar + tempStr + quoteChar);
                  else
                     WriteText(tempStr);

                  break;
            }
         }
      }

      public override void Execute()
      {
         string quoteStr = CmdLine.GetStrSwitch("/Q", "\"");
         string delimiterStr = CmdLine.GetStrSwitch("/D", ",");
         unquoting = CmdLine.GetBooleanSwitch("/U");
         usingBackslashQuote = CmdLine.GetBooleanSwitch("/B");
         int noOfSets = CmdLine.GetIntSwitch("/S", 0);
         int option = CmdLine.GetIntSwitch("/O", 0);

         if (quoteStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetSwitchPos("/Q"));
         if (delimiterStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetSwitchPos("/D"));
         if (noOfSets != 0) CheckIntRange(noOfSets, 1, int.MaxValue, "Set size", CmdLine.GetSwitchPos("/S"));
         CheckIntRange(option, 0, 2, "Quote option", CmdLine.GetSwitchPos("/O"));

         quoteChar = quoteStr[0];
         delimiterChar = delimiterStr[0];

         int setNo = 0;
         bool inRange = false;
         int rangeIndex = 0;
         int prevLine = 0;
         string line =  string.Empty;

         Open();

         try
         {
            while (!EndOfText)
            {
               int j = rangeIndex * 2;
               int begLine = (int) CmdLine.GetArg(j).Value;
               int endLine = (int) CmdLine.GetArg(j+1).Value;

               CheckIntRange(begLine, 1, int.MaxValue, "Line no.", CmdLine.GetArg(j).CharPos);
               CheckIntRange(endLine, 1, int.MaxValue, "Line no.", CmdLine.GetArg(j+1).CharPos);

               if (begLine > prevLine)
               {
                  prevLine = begLine;

                  if (endLine >= begLine)
                  {
                     // Scan thru lines prior to range:

                     while ((!EndOfText) && (!inRange))
                     {
                        line = ReadLine();
                        inRange = (setNo+1 >= begLine) && (setNo+1 <= endLine);

                        if (!inRange)
                        {
                           WriteText(line);
                           setNo++;

                           if (noOfSets > 0)
                           {
                              setNo = setNo % noOfSets;
                           }
                        }
                     }

                     // Scan thru lines in range:

                     do
                     {
                        QuoteLine(line, option);
                        setNo++;

                        if (noOfSets > 0)
                        {
                           setNo = setNo % noOfSets;
                        }

                        inRange = (setNo + 1 >= begLine) && (setNo + 1 <= endLine);

                        if ((!EndOfText) && inRange)
                        {
                           line = ReadLine();

                           if (EndOfText)
                           {
                              QuoteLine(line, option);
                           }
                        }
                     }
                     while ((!EndOfText) && inRange);

                     if (rangeIndex == (CmdLine.ArgCount / 2) - 1)
                     {
                        // Last range - Scan to first set of next group:

                        while ((!EndOfText) && (setNo != 0))
                        {
                           line = ReadLine();
                           WriteText(line);
                           setNo += 1;

                           if (noOfSets > 0)
                           {
                              setNo = setNo % noOfSets;
                           }
                        }

                        prevLine = 0;
                     }

                     rangeIndex = (rangeIndex + 1) % (CmdLine.ArgCount / 2);
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
                  // Begin/End pairs are not ascending.

                  ThrowException("Begin/End pairs must be in ascending order.",
                  CmdLine.GetArg(j).CharPos);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public QuoteLines(IFilter host) : base(host)
      {
         Template = "n n [n n...] /B /Ds /On /Qs /Sn /U";
      }
   }
}
