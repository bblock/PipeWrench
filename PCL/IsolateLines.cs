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
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   /// Used in conjuction with EndIsolate to constrain pipe commands so that they
   /// operate on a limited block of text.  This "block" is defined in one of two ways:
   /// either as a) the first group of consecutive lines encountered in the text that
   /// each contain a given marker string, or b) the first group of consecutive lines
   /// encountered in the text in which the first line contains a beginning marker
   /// string and the last line contains an ending marker string.
   /// </summary>
   public sealed class IsolateLines : FilterPlugin
   {
      public override void Execute()
      {
         //LoggingEnabled = true;

         int state = 1;
         string line;
         string newLine;

         string begStr = (string) CmdLine.GetArg(0).Value;
         string endStr = CmdLine.GetStrSwitch("/E", string.Empty);
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         bool rangeGiven = (CmdLine.ArgCount > 1);

         int begPos = 0;
         int endPos = 0;

         if (rangeGiven)
         {
            begPos = (int) CmdLine.GetArg(1).Value;
            endPos = (int) CmdLine.GetArg(2).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(1).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(2).CharPos);
         }

         if (begPos > endPos)
         {
            // Oops.

            ThrowException("End position must be >= begin position.", CmdLine.GetArg(2).CharPos);
         }

         // Create/open a text file to contain the diverted text:

         string divText = Path.GetTempFileName();
         StreamWriter divTextWriter = new StreamWriter(divText);

         try
         {
            // Open the input/output files:

            Open();

            try
            {
               // Process the text:

               while (!EndOfText)
               {
                  line = ReadLine();

                  if (rangeGiven)
                     newLine = line.Substring(begPos-1, endPos-begPos+1);
                  else
                     newLine = line;

                  if (endStr != string.Empty)
                  {
                     // The block of text is defined as the *first* encountered
                     // group of lines bounded by the two given boundary strings.

                     switch (state)
                     {
                        case 1:
                           if (!StringMatched(begStr, newLine, ignoringCase, isRegEx))
                           {
                              // Save line to "top" of diverted text file:

                              divTextWriter.WriteLine(line);
                           }
                           else
                           {
                              // Output the line:

                              WriteText(line);

                              // Write a boundary marker between the "top"
                              // and "bottom" of the diverted text:

                              divTextWriter.WriteLine("<rekram yradnuob>"); 
                                 // TODO: Need to remove the need for this boundary marker (likely by using an additional list).

                              if (StringMatched(endStr, newLine, ignoringCase, isRegEx))
                                 state = 3;
                              else
                                 state = 2;
                           }
                           break;

                        case 2:

                           // Looking for ending string.  Output the line:

                           WriteText(line);

                           if (StringMatched(endStr, newLine, ignoringCase, isRegEx)) state = 3;
                           break;

                        case 3:

                           // Save line to "bottom" of the diverted text file:

                           divTextWriter.WriteLine(line);
                           break;
                     }
                  }
                  else
                  {
                     // The isolated block of text is defined as the *first* encountered
                     // group of lines that contain the given string (optionally within the
                     // given range of character positions).

                     switch (state)
                     {
                        case 1:

                           // Collecting "top" lines.

                           if (StringMatched(begStr, newLine, ignoringCase, isRegEx))
                           {
                              // Output the line:

                              WriteText(line);

                              // Write a boundary marker between the "top"
                              // and "bottom" of the diverted text:

                              divTextWriter.WriteLine("<rekram yradnuob>");
                              state = 2;
                           }
                           else
                           {
                              // Save line to "top" of diverted text file:

                              divTextWriter.WriteLine(line);
                           }
                           break;

                        case 2:

                           // Collecting the "isolated" lines.

                           if (StringMatched(begStr, newLine, ignoringCase, isRegEx))
                           {
                              // Output the line:

                              WriteText(line);
                           }
                           else
                           {
                              // Save line to "bottom" of the diverted text file:

                              divTextWriter.WriteLine(line);
                              state = 3;
                           }
                           break;

                        case 3:

                           // Save line to "bottom" of the diverted text file:

                           divTextWriter.WriteLine(line);
                           break;
                     }
                  }
               }

               // Push the diverted text filename onto the stack:

               ((Filter) Host).DivTextStack.Push(divText);
            }

            finally
            {
               Close();
            }
         }

         finally
         {
            divTextWriter.Close();
         }
      }

      public IsolateLines(IFilter host) : base(host)
      {
         Template = "s [n n] /Es /I /R";
      }
   }
}
