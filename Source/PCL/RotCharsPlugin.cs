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

namespace Firefly.Pyper
{
   public abstract class RotCharsPlugin : FilterPlugin
   {
      /// <summary>
      /// Rotates source string by # of characters.
      /// </summary>
      private void RotateStr(ref string source, bool rotatingLeft, int noOfChars)
      {
         for (int i=1; i <= noOfChars; i++)
         {
            if (rotatingLeft)
            {
               source = source.Substring(1, source.Length-1) + source[0];
            }
            else
            {
               source = source[source.Length-1] + source.Substring(0, source.Length-1);
            }
         }
      }

      public override abstract void Execute();

      protected void Execute(bool rotatingLeft)
      {
         int noOfChars = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(noOfChars, 1, int.MaxValue, "No. of characters", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line.Length > 0)
               {
                  RotateStr(ref line, rotatingLeft, noOfChars);
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public RotCharsPlugin(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
