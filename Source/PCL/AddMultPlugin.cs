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
   public abstract class AddMultPlugin : FilterPlugin
   {
      public override abstract void Execute();

      protected void Execute(bool isAddFilter)
      {
         double resultValue;
         string tempStr;

         int resultPos = CmdLine.GetIntSwitch("/I", 0);
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         int noOfDecimals = CmdLine.GetIntSwitch("/D", 2);
         bool sciNotation = CmdLine.GetBooleanSwitch("/S");

         if (resultPos != 0)
            CheckIntRange(resultPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/I"));

         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));
         CheckIntRange(noOfDecimals, 0, int.MaxValue, "No. of decimals", CmdLine.GetSwitchPos("/D"));

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               // Initialize the result:

               if (isAddFilter)
                  resultValue = 0.0;
               else
                  resultValue = 1.0;

               if (CmdLine.ArgCount > 0)
               {
                  // Adding (or multiplying) the numbers located as specific character positions.

                  for (int i=0; i < CmdLine.ArgCount; i++)
                  {
                     int charPos = (int) CmdLine.GetArg(i).Value;
                     CheckIntRange(charPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(i).CharPos);

                     int num = charPos - 1;
                     tempStr = ScanDecimal(line, ref num);

                     try
                     {
                        double theValue = double.Parse(tempStr);

                        if (isAddFilter)
                           resultValue += theValue;
                        else
                           resultValue *= theValue;
                     }

                     catch (FormatException)
                     {
                        // Numeric value is invalid.

                        ThrowException("Numeric value found on text line " + TextLineNo.ToString() +
                        ", character position " + CmdLine.GetArg(i).Value.ToString() + " is invalid.",
                        CmdLine.GetArg(i).CharPos);
                     }
                  }

                  // Build the result string:

                  if (!sciNotation)
                  {
                     tempStr = resultValue.ToString("0." +
                     new string('0', noOfDecimals)).PadLeft(numericWidth);
                  }
                  else
                  {
                     tempStr = resultValue.ToString("#." + new string('#', noOfDecimals) +
                     "e+00").PadLeft(numericWidth) + ' ';
                  }
               }
               else
               {
                  // Adding (or multiplying) all numbers found on each line separated by whitespace.

                  int countOfScannedValues = 0;
                  int charIndex = 0;

                  while (charIndex < line.Length)
                  {
                     countOfScannedValues++;
                     tempStr = ScanDecimal(line, ref charIndex);

                     try
                     {
                        double theValue = double.Parse(tempStr);

                        if (isAddFilter)
                           resultValue += theValue;
                        else
                           resultValue *= theValue;
                     }

                     catch (FormatException)
                     {
                        // Numeric value is invalid.

                        ThrowException("Numeric value " + countOfScannedValues.ToString() +
                        "found on text line " + TextLineNo.ToString() + " is invalid.");
                     }
                  }

                  if (countOfScannedValues > 0)
                  {
                     // Build the result string:

                     if (!sciNotation)
                     {
                        tempStr = resultValue.ToString("0." +
                        new string('0', noOfDecimals)).PadLeft(numericWidth);
                     }
                     else
                     {
                        tempStr = resultValue.ToString("#.########e+00");
                     }
                  }
                  else
                  {
                     // Build the result string:

                     tempStr = "n/a";
                  }
               }

               if (resultPos > 0)
               {
                  // Inserting the result back into source.

                  tempStr += ' ';

                  // Pad the source line to the result column:

                  while (line.Length < resultPos - 1)
                  {
                     line += ' ';
                  }

                  // Insert the result string:

                  line = line.Insert(resultPos-1, tempStr);
                  WriteText(line);
               }
               else
               {
                  // Returning only the result string.

                  WriteText(tempStr);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public AddMultPlugin(IFilter host) : base(host)
      {
         Template = "[n...] /In /Wn /Dn /S";
      }
   }
}
