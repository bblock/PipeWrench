using System;

namespace Firefly.Pyper
{
   public abstract class SubDivPlugin : FilterPlugin
   {
      public override abstract void Execute();

      protected void Execute(bool isSubFilter)
      {
         string tempStr;

         int arg1CharPos = (int) CmdLine.GetArg(0).Value;
         int arg2CharPos = (int) CmdLine.GetArg(1).Value;
         int resultPos = CmdLine.GetIntSwitch("/I", 0);
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         int noOfDecimals = CmdLine.GetIntSwitch("/D", 2);
         bool sciNotation = CmdLine.GetBooleanSwitch("/S");

         CheckIntRange(arg1CharPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(0).CharPos);
         CheckIntRange(arg2CharPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(1).CharPos);

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
               int num = arg1CharPos - 1;
               tempStr = ScanDecimal(line, ref num);

               try
               {
                  double value1 = double.Parse(tempStr);
                  num = arg2CharPos - 1;
                  tempStr = ScanDecimal(line, ref num);

                  try
                  {
                     double value2 = double.Parse(tempStr);
                     double resultValue;

                     if (isSubFilter)
                        resultValue = value1 - value2;
                     else
                        resultValue = value1 / value2;

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

                  catch (FormatException)
                  {
                     // Numeric value is invalid.

                     ThrowException("Numeric value found on text line " + TextLineNo.ToString() +
                     ", character position " + CmdLine.GetArg(1).Value.ToString() + " is invalid.",
                     CmdLine.GetArg(1).CharPos);
                  }
               }

               catch (FormatException)
               {
                  // Numeric value is invalid.

                  ThrowException("Numeric value found on text line " + TextLineNo.ToString() +
                  ", character position " + CmdLine.GetArg(0).Value.ToString() + " is invalid.",
                  CmdLine.GetArg(0).CharPos);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public SubDivPlugin(IFilter host) : base(host)
      {
         Template = "n n /In /Wn /Dn /S";
      }
   }
}
