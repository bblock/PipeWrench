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
   /// Removes leading and trailing white space from each line of text.
   /// </summary>
   public sealed class TrimLinesRight : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               WriteText(line.TrimEnd());
            }
         }

         finally
         {
            Close();
         }
      }

      public TrimLinesRight(IFilter host) : base(host) {}
   }
}
