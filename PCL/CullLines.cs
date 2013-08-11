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
   /// Removes groups of lines encountered in the input text.
   /// </summary>
   public sealed class CullLines : FilterPlugin
   {
      public override void Execute()
      {
         bool cullingAll;
         string begCharStr;
         string endCharStr;
         bool ignoringCase;
         bool isRegEx;

         begCharStr = (string) CmdLine.GetArg(0).Value;
         endCharStr = (string) CmdLine.GetArg(1).Value;
         cullingAll = CmdLine.GetBooleanSwitch("/A");
         ignoringCase = CmdLine.GetBooleanSwitch("/I");
         isRegEx = CmdLine.GetBooleanSwitch("/R");

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (StringMatched(begCharStr, line, ignoringCase, isRegEx))
               {
                  // Found the first string.  Omit lines until after the second string is found:

                  while (!EndOfText && !StringMatched(endCharStr, line, ignoringCase, isRegEx))
                  {
                     // Omit the line && get the next one:

                     line = ReadLine();
                  }

                  if (!cullingAll)
                  {
                     // Output the remaining lines:

                     while (!EndOfText)
                     {
                        line = ReadLine();
                        WriteText(line);
                     }
                  }
               }
               else
               {
                  // This line is outside of an omitted segment.  Output it:

                  WriteText(line);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public CullLines(IFilter host) : base(host)
      {
         Template = "s s /A /I /R";
      }
   }
}
