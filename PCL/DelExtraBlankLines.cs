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
   /// Removes any extraneous blank lines from the input text.
   /// </summary>
   public sealed class DelExtraBlankLines : FilterPlugin
   {
      public override void Execute()
      {
         bool prevWasBlank = false;

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line == string.Empty)
               {
                  if (!prevWasBlank)
                  {
                     WriteText(line);
                  }

                  prevWasBlank = true;
               }
               else
               {
                  WriteText(line);
                  prevWasBlank = false;
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public DelExtraBlankLines(IFilter host) : base(host) {}
   }
}
