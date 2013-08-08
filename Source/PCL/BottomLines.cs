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
   /// Outputs the given number of lines from the end of the input text.
   /// </summary>
   public sealed class BottomLines : FilterPlugin
   {
      public override void Execute()
      {
         int NoOfLines = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(NoOfLines, 1, int.MaxValue, "No. of lines", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (TextLineNo >= TextLineCount - NoOfLines + 1)
               {
                  WriteText(line);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public BottomLines(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
