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
   /// Converts decimal numbers found in the input text to another base.
   /// </summary>
   public sealed class DecToBase : FilterPlugin
   {
      /// <summary>
      /// returns the decimal (whole number) value converted to a string of digits
      /// in the given base using the ASCII sequence, '0'-'9', 'A'-'Z' ...
      /// </summary>
      private string DecimalToBase(ulong decVal, int radix)
      {
         string newVal = string.Empty;
         char digit;

         do
         {
            ulong quot = decVal / (ulong) radix;
            ulong rem = decVal % (ulong) radix;

            if ((rem >= 0) && (rem <= 9))
            {
               digit = Convert.ToChar(rem + 48);
            }
            else
            {
               if ((rem >= 10) && (rem <= 35))
               {
                  digit = Convert.ToChar(rem + 55);
               }
               else
               {
                  digit = Convert.ToChar(rem + 61);
               }
            }

            newVal = digit + newVal;
            decVal = quot;
         }
         while (decVal != 0);

         return newVal;
      }

      public override void Execute()
      {
         int radix = (int) CmdLine.GetArg(0).Value;
         int entryPos = CmdLine.GetIntSwitch("/I", 0);
         int scanPos = CmdLine.GetIntSwitch("/S", 1);
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         bool paddingWithZeros = CmdLine.GetBooleanSwitch("/Z");

         CheckIntRange(radix, 2, 62, "Radix", CmdLine.GetArg(0).CharPos);

         if (entryPos != 0)
            CheckIntRange(entryPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/I"));

         CheckIntRange(scanPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/S"));
         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int charPos = scanPos - 1;
               string tempStr = ScanInteger(line, ref charPos);

               if (tempStr != string.Empty)
               {
                  try
                  {
                     ulong num = ulong.Parse(tempStr);

                     // The decimal value converted ok.

                     tempStr = DecimalToBase(num, radix);
                     char padChar;

                     if (paddingWithZeros)
                        padChar = '0';
                     else
                        padChar = ' ';

                     tempStr = tempStr.PadLeft(numericWidth, padChar);

                     if (entryPos > 0)
                     {
                        // Insert the result back into source:

                        tempStr += ' ';

                        while (line.Length < entryPos - 1)
                        {
                           line += ' ';
                        }

                        line = line.Insert(entryPos-1, tempStr);
                        WriteText(line);
                     }
                     else
                     {
                        // Only return the result:

                        WriteText(tempStr);
                     }
                  }

                  catch (FormatException)
                  {
                     // Numeric value is invalid.

                     ThrowException("Numeric value on text line " +
                     TextLineNo.ToString() + " is invalid.");
                  }
               }
               else
               {
                  // Numeric value not found.

                  ThrowException("No numeric value was found on text line " +
                  TextLineNo.ToString() + " to convert.");
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public DecToBase(IFilter host) : base(host)
      {
         Template = "n /In /Sn /Wn /Z";
      }
   }
}
