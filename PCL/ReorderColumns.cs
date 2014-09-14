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
   /// Re-arranges the column order of each line.
   /// </summary>
   public sealed class ReorderColumns : FilterPlugin
   {
      public override void Execute()
      {
         bool countGiven = CmdLine.GetBooleanSwitch("/C");
         int insertCharPos = CmdLine.GetIntSwitch("/P", 0);

         if (insertCharPos != 0) CheckIntRange(insertCharPos, 1, int.MaxValue,
         "Char. position", CmdLine.GetSwitchPos("/P"));

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               string newLine = string.Empty;

               if (!countGiven)
               {
                  // pos/pos pairs are supplied.

                  for (int i=0; i < (CmdLine.ArgCount / 2); i++)
                  {
                     int j = (i * 2);
                     int begCharPos = (int) CmdLine.GetArg(j).Value;
                     int endCharPos = (int) CmdLine.GetArg(j+1).Value;
                     CheckIntRange(begCharPos, 1, int.MaxValue, "Begin char. position", CmdLine.GetArg(j).CharPos);
                     CheckIntRange(endCharPos, 1, int.MaxValue, "End char. position", CmdLine.GetArg(j+1).CharPos);

                     if (endCharPos >= begCharPos)
                     {
                        while (line.Length < endCharPos)
                        {
                           line += ' ';
                        }

                        int len = endCharPos - begCharPos + 1;
                        string tempStr = line.Substring(begCharPos-1, len);
                        newLine += tempStr;
                     }
                     else
                     {
                        // Begin/End pair is reversed.

                        ThrowException("End position must be >= begin position.", CmdLine.GetArg(j+1).CharPos);
                     }
                  }
               }
               else
               {
                  // pos/count pairs are supplied.

                  for (int i=0; i < (CmdLine.ArgCount / 2); i++)
                  {
                     int j = (i * 2);
                     int begCharPos = (int) CmdLine.GetArg(j).Value;
                     int numOfChars = (int) CmdLine.GetArg(j+1).Value;
                     int endCharPos = begCharPos + numOfChars - 1;
                     CheckIntRange(begCharPos, 1, int.MaxValue, "Begin char. position", CmdLine.GetArg(j).CharPos);
                     CheckIntRange(numOfChars, 1, int.MaxValue, "No. of characters", CmdLine.GetArg(j+1).CharPos);

                     while (line.Length < endCharPos)
                     {
                        line += ' ';
                     }

                     string tempStr = line.Substring(begCharPos-1, numOfChars);
                     newLine += tempStr;
                  }
               }

               if (insertCharPos > 0)
               {
                  // Insert the resulting text back into the line:

                  while (line.Length < insertCharPos)
                  {
                     line += ' ';
                  }

                  newLine = line.Insert(insertCharPos-1, newLine);
               }

               WriteText(newLine);
            }
         }

         finally
         {
            Close();
         }
      }

      public ReorderColumns(IFilter host) : base(host)
      {
         Template = "n n [n n...] /C /Pn";
      }
   }
}
