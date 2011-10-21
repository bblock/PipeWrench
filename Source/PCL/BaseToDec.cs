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
   /// Converts a positive, non-base 10 (i.e. 2-62) whole
   /// number found in the input text, to base-10 (decimal).
   /// </summary>
   public sealed class BaseToDec : FilterPlugin
   {
      /// <summary>
      /// This function returns true if the given
      /// digit ASCII value is valid for the base.
      /// </summary>
      private bool ValidDigit(int theValue, int radix)
      {
         bool C1 =
            (
               (
                  (radix >= 2) && (radix <= 10)
               )
               &&
               (
                  theValue >= 48
               )
               &&
               (
                  theValue <= radix + 47
               )
            );

         bool C2 =
            (
               (
                  (radix >= 11) && (radix <= 36)
               )
               &&
               (
                  (
                     (theValue >= 48) && (theValue <= 57)
                  )
                  ||
                  (
                     (theValue >= 65) && (theValue <= radix + 54)
                  )
               )
            );

         bool C3 =
            (
               (
                  (radix >= 37) && (radix <= 62)
               )
               &&
               (
                  (
                     (theValue >= 48) && (theValue <= 57)
                  )
                  ||
                  (
                     (theValue >= 65) && (theValue <= 90)
                  )
                  ||
                  (
                     (theValue >= 97) && (theValue <= radix + 60)
                  )
               )
            );

            return C1 || C2 || C3;
      }

      /// <summary>
      /// This routine converts a positive, non-base 10 whole number to
      /// base-10 (decimal).  The result is returned as an integer.
      /// </summary>
      private ulong Base2Decimal(string baseValue, int radix)
      {
         int digit;
         ulong decValue = 0;
         ulong weight = 1;

         int i = baseValue.Length - 1;

         while (i >= 0)
         {
            char ch = baseValue[i];
            int asciiVal = (int) ch;
            LogWrite("asciiVal = " + asciiVal.ToString());

            if (ValidDigit(asciiVal, radix))
            {
               // The digit is in range.

               if ((ch >= '0') && (ch <= '9'))
               {
                  digit = asciiVal - 48;
               }
               else
               {
                  if ((ch >= 'A') && (ch <= 'Z'))
                  {
                     digit = asciiVal - 55;
                  }
                  else
                  {
                     digit = asciiVal - 61;
                  }
               }

               decValue += ((ulong) digit * weight);
               weight *= (ulong) radix;
               i--;
            }
            else
            {
               // The digit is out of range.

               ThrowException("The digit is out of range.");
            }
         }

         return decValue;
      }

      public override void Execute()
      {
         //LoggingEnabled = true;
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
               string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
               string tempStr = ScanToken(line, ref charPos, validChars);

               if (tempStr != string.Empty)
               {
                  ulong num = Base2Decimal(tempStr, radix);
                  tempStr = num.ToString();
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
               else
               {
                  // No value found to convert.

                  ThrowException("No value was found on text line " + TextLineNo.ToString() + " to convert.");
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public BaseToDec(IFilter host) : base(host)
      {
         Template = "n /In /Sn /Wn /Z";
      }
   }
}
