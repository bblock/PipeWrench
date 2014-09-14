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
   /// Reverses each line of the input text.
   /// </summary>
   public sealed class ReverseChars : FilterPlugin
   {
      private string ReverseStr(string source)
      {
         string result = string.Empty;

         for (int i=source.Length-1; i >= 0; i--)
         {
            result += source[i];
         }

         return result;
      }

      public override void Execute()
      {
         int begPos = 0;
         int endPos = 0;

         bool rangeGiven = (CmdLine.ArgCount == 2);

         if (rangeGiven)
         {
            begPos = (int) CmdLine.GetArg(0).Value;
            endPos = (int) CmdLine.GetArg(1).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Begin char. position", CmdLine.GetArg(0).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "End char. position", CmdLine.GetArg(1).CharPos);
         }

         if (begPos > endPos)
         {
            // Oops.

            ThrowException("End char. position must be >= begin char. position.", CmdLine.GetArg(1).CharPos);
         }

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               string tempStr = string.Empty;

               if (rangeGiven)
               {
                  // Reversing substring of line only. Reverse the range of characters:

                  string subStr = string.Empty;

                  if (line.Length < endPos)
                  {
                     // Range is beyond end of line.

                     if (line.Length >= begPos)
                        subStr = line.Substring(begPos-1, line.Length - begPos + 1);
                  }
                  else
                     subStr = line.Substring(begPos-1, endPos - begPos + 1);

                  string revSubStr = ReverseStr(subStr);
                  string beforeStr;

                  if (line.Length >= begPos)
                     beforeStr = line.Substring(0, begPos-1);
                  else
                     beforeStr = line;

                  string afterStr = string.Empty;

                  if (line.Length >= endPos) afterStr = line.Substring(endPos);

                  tempStr = beforeStr + revSubStr + afterStr;
               }
               else
               {
                  // Reversing entire line.

                  tempStr = ReverseStr(line);
               }

               WriteText(tempStr);
            }
         }

         finally
         {
            Close();
         }
      }

      public ReverseChars(IFilter host) : base(host)
      {
         Template = "[n n]";
      }
   }
}
