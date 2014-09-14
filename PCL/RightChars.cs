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
   /// Returns the right-most characters from each line of the input text.
   /// </summary>
   public sealed class RightChars : FilterPlugin
   {
      public override void Execute()
      {
         int noOfChars = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(noOfChars, 1, int.MaxValue, "No. of characters", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string source = ReadLine();
               int len = source.Length;

               if (len >= noOfChars)
               {
                  string tempStr = source.Substring(len-noOfChars, noOfChars);
                  WriteText(tempStr);
               }
               else
                  WriteText(source);
            }
         }

         finally
         {
            Close();
         }
      }

      public RightChars(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
