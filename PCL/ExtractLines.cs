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
