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
   /// Overlays chars at position specified with string.
   /// </summary>
   public sealed class OverlayChars : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               for (int i=0; i < (CmdLine.ArgCount / 2); i++)
               {
                  int j = (i * 2);
                  int charPos = (int) CmdLine.GetArg(j).Value;
                  CheckIntRange(charPos, 1, int.MaxValue, "Character position",
                  CmdLine.GetArg(j).CharPos);
                  string charStr = (string) CmdLine.GetArg(j+1).Value;
                  if (charStr == string.Empty) ThrowException("String cannot be empty.",
                  CmdLine.GetArg(j+1).CharPos);

                  while (line.Length < charPos + charStr.Length - 1)
                  {
                     line += ' ';
                  }

                  line = line.Remove(charPos-1, charStr.Length);
                  line = line.Insert(charPos-1, charStr);
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public OverlayChars(IFilter host) : base(host)
      {
         Template = "n s [n s...]";
      }
   }
}
