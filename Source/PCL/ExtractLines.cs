using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Extracts groups of lines encountered in the input text.
   /// </summary>
   public sealed class ExtractLines : FilterPlugin
   {
      public override void Execute()
      {
         bool extractingAll;
         string begCharStr;
         string endCharStr;
         bool done = false;
         bool ignoringCase;
         bool isRegEx;

         begCharStr = (string) CmdLine.GetArg(0).Value;
         endCharStr = (string) CmdLine.GetArg(1).Value;
         extractingAll = CmdLine.GetBooleanSwitch("/A");
         ignoringCase = CmdLine.GetBooleanSwitch("/I");
         isRegEx = CmdLine.GetBooleanSwitch("/R");

         Open();

         try
         {
            while (!done && !EndOfText)
            {
               string line = ReadLine();

               if (StringMatched(begCharStr, line, ignoringCase, isRegEx))
               {
                  // Found the first string.  Output the line:

                  WriteText(line);

                  // Output lines until the second string is found:

                  while (!EndOfText && !StringMatched(endCharStr, line, ignoringCase, isRegEx))
                  {
                     line = ReadLine();
                     WriteText(line);
                  }

                  done = !extractingAll;
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public ExtractLines(IFilter host) : base(host)
      {
         Template = "s s /A /I /R";
      }
   }
}
