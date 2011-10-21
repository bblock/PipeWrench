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
   /// <summary>
   /// Returns the number of lines in the input text.
   /// </summary>
   public sealed class CountLines : FilterPlugin
   {
      public override void Execute()
      {
         int numericWidth = CmdLine.GetIntSwitch("/W", 6);
         bool paddingWithZeros = CmdLine.GetBooleanSwitch("/Z");
         char padChar;

         CheckIntRange(numericWidth, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));

         if (paddingWithZeros)
            padChar = '0';
         else
            padChar = ' ';

         Open();

         try
         {
            int lineCount = Host.TextLineCount;
            WriteText(lineCount.ToString().PadLeft(numericWidth, padChar));
         }

         finally
         {
            Close();
         }
      }

      public CountLines(IFilter host) : base(host)
      {
         Template = "/Wn /Z";
      }
   }
}
