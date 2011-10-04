using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Returns the number of lines in the input text.
   /// </summary>
   public sealed class CountLines : FilterPlugin
   {
      public override void Execute()
      {
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         bool paddingWithZeros = CmdLine.GetBooleanSwitch("/Z");
         char padChar;

         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));

         if (paddingWithZeros)
            padChar = '0';
         else
            padChar = ' ';

         Open();

         try
         {
            int lineCount = Host.TextLineCount;
            WriteText(lineCount.ToString().PadLeft(numericWidth, padChar));
         }

         finally
         {
            Close();
         }
      }

      public CountLines(IFilter host) : base(host)
      {
         Template = "/Wn /Z";
      }
   }
}
