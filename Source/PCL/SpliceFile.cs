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
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   /// Splices the text from a text file to the end of each line of the input text.
   /// Any lines in the text file beyond the # of lines in the input text are ignored.
   /// </summary>
   public sealed class SpliceFile : FilterPlugin
   {
      public override void Execute()
      {
         string spliceFileName = (string) CmdLine.GetArg(0).Value;
         string delimiterStr = CmdLine.GetStrSwitch("/D", string.Empty);
         bool mergingText = CmdLine.GetBooleanSwitch("/M");

         try
         {
            using (TextReader tr = new StreamReader(spliceFileName))
            {
               Open();

               try
               {
                  string tempStr;
                  string source;

                  if (!mergingText)
                  {
                     // Appending each line from splice file to end of input line.

                     while (!EndOfText)
                     {
                        source = ReadLine();

                        if ((tempStr = tr.ReadLine()) != null)
                        {
                           WriteText(source + delimiterStr + tempStr);
                        }
                        else
                        {
                           WriteText(source);
                        }
                     }

                     while ((tempStr = tr.ReadLine()) != null)
                     {
                        WriteText(tempStr);
                     }
                  }
                  else
                  {
                     // Adding all lines from splice file to end of input text (merging mode).

                     while (!EndOfText)
                     {
                        source = ReadLine();
                        WriteText(source);
                     }

                     while ((tempStr = tr.ReadLine()) != null)
                     {
                        WriteText(tempStr);
                     }
                  }
               }

               finally
               {
                  Close();
               }
            }
         }

         catch (IOException)
         {
            // Error reading the splice file.

            ThrowException("Error reading the splice file.");
         }
      }

      public SpliceFile(IFilter host) : base(host)
      {
         Template = "s /Ds /M";
      }
   }
}
