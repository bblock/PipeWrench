using System;
using System.Collections.Generic;

namespace Firefly.Pyper
{
   /// <summary>
   /// Totals columns of numeric values.
   /// </summary>
   public sealed class TotalColumns : FilterPlugin
   {
      public override void Execute()
      {
         string tempStr;
         double theValue;
         string line;
         List<double> totals = new List<double>();
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         int noOfDecimals = CmdLine.GetIntSwitch("/D", 2);
         bool appendToEnd = CmdLine.GetBooleanSwitch("/A");
         bool sciNotation = CmdLine.GetBooleanSwitch("/S");

         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));
         CheckIntRange(noOfDecimals, 0, int.MaxValue, "No. of decimals", CmdLine.GetSwitchPos("/D"));

         for (int i=1; i <= CmdLine.ArgCount; i++)
         {
            // Initialize the totals:

            totals.Add(0.0);
         }

         Open();

         try
         {
            while (!EndOfText)
            {
               line = ReadLine();
               if (appendToEnd) WriteText(line);

               // Total this line:

               for (int i=0; i < CmdLine.ArgCount; i++)
               {
                  int charPos = (int) CmdLine.GetArg(i).Value;
                  CheckIntRange(charPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(i).CharPos);
                  int n = charPos - 1;
                  tempStr = ScanDecimal(line, ref n);

                  try
                  {
                     theValue = double.Parse(tempStr);
                     totals[i] += theValue;
                  }
                  catch
                  {
                     // Numeric value is invalid.

                     ThrowException("Numeric value on text line " + TextLineNo.ToString() + " is invalid.");
                  }
               }
            }

            // Output the totals:

            line = string.Empty;

            for (int i=0; i < totals.Count; i++)
            {
               theValue = totals[i];
               int charPos = (int) CmdLine.GetArg(i).Value;

               // Build the result string:

               if (!sciNotation)
               {
                  tempStr = theValue.ToString("0." +
                  new string('0', noOfDecimals)).PadLeft(numericWidth) + ' ';
               }
               else
               {
                  tempStr = theValue.ToString("#." + new string('#', noOfDecimals) +
                  "e+00").PadLeft(numericWidth) + ' ';
               }

               // Pad the source line to the result column:

               while (line.Length < charPos - 1)
               {
                  line += ' ';
               }

               // Insert the result string:

               line = line.Insert(charPos-1, tempStr);
            }

            WriteText(line);
         }

         finally
         {
            Close();
         }
      }

      public TotalColumns(IFilter host) : base(host)
      {
         Template = "n [n...] /Wn /Dn /A /S";
      }
   }
}
