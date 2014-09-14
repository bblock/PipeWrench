// 
// PipeWrench - automate the transformation of text using "stackable" text filters
// Copyright (c) 2014  Barry Block 
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

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Used in conjuction with IsolateLines to constrain
   /// pipe commands to an isolated block of text.
   /// </summary>
   public sealed class EndIsolate : FilterPlugin
   {
      public override void Execute()
      {
         if (((Filter) Host).DivTextStack.Count > 0)
         {
            // There's diverted text on the stack.

            Open();

            try
            {
               // Pop the diverted text:

               string divText = ((Filter) Host).DivTextStack.Pop();

               // Open the text file for reading:

               StreamReader divTextReader = new StreamReader(divText);

               try
               {
                  // Output the prior saved "top" lines:

                  string line;

                  while (!divTextReader.EndOfStream)
                  {
                     line = divTextReader.ReadLine();

                     if (line != "<rekram yradnuob>")
                        WriteText(line);
                     else
                        break;
                  }

                  // Output the lines processed by the prior constrained filters:

                  while (!EndOfText)
                  {
                     line = ReadLine();
                     WriteText(line);
                  }

                  // Output the prior saved "bottom" lines:

                  while (!divTextReader.EndOfStream)
                  {
                     line = divTextReader.ReadLine();
                     WriteText(line);
                  }
               }

               finally
               {
                  // Delete the no longer needed diverted text file:

                  divTextReader.Close();
                  File.Delete(divText);
               }
            }

            finally
            {
               Close();
            }
         }
         else
         {
            // There's no diverted text object.

            ThrowException("Unmatched IsolateLines/EndIsolate commands.");
         }
      }

      public EndIsolate(IFilter host) : base(host) {}
   }
}
