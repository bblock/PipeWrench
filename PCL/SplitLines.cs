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

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Splits each line at one or more character positions.
   /// </summary>
   public sealed class SplitLines : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int offset = 0;
               int priorPos = 0;

               for (int i=0; i < CmdLine.ArgCount; i++)
               {
                  int charPos = (int) CmdLine.GetArg(i).Value;
                  CheckIntRange(charPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(i).CharPos);

                  if (charPos > priorPos)
                  {
                     priorPos = charPos;
                     int offsetPos = charPos - offset;

                     if (offsetPos <= line.Length)
                     {
                        WriteText(line.Substring(0, offsetPos-1));
                        line = line.Remove(0, offsetPos-1);
                        offset += offsetPos - 1;
                     }
                  }
                  else
                  {
                     // Character positions are not ascending.

                     ThrowException("Character positions must be in ascending order.",
                     CmdLine.GetArg(i).CharPos);
                  }
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public SplitLines(IFilter host) : base(host)
      {
         Template = "n [n...]";
      }
   }
}
