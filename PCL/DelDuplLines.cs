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
   /// Acts on sorted lists removing all duplicate lines.
   /// </summary>
   public sealed class DelDuplLines : FilterPlugin
   {
      public override void Execute()
      {
         bool deletingAll = CmdLine.GetBooleanSwitch("/A");
         string delimiterStr = CmdLine.GetStrSwitch("/D", string.Empty);
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool rangeIsGiven = CmdLine.ArgCount > 0;
         bool delimSpecified = delimiterStr != string.Empty;

         char delimiter = '\0';
         if (delimSpecified) delimiter = delimiterStr[0];

         int begPos = 0;
         int endPos = 0;

         if (rangeIsGiven)
         {
            begPos = (int) CmdLine.GetArg(0).Value;
            endPos = (int) CmdLine.GetArg(1).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(0).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(1).CharPos);

            if (begPos > endPos)
            {
               // Oops.

               ThrowException("End position must be >= begin position.", CmdLine.GetArg(1).CharPos);
            }
         }

         string savedLine = null; // Must default to null.
         int count = 0;

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (!((delimSpecified && StringsCompare(line, savedLine, ignoringCase, delimiter)) ||
               (!delimSpecified && StringsCompare(line, savedLine, ignoringCase, begPos, endPos))))
               {
                  if ((deletingAll && (count == 1)) || (!deletingAll && (count > 0)))
                  {
                     WriteText(savedLine);
                  }

                  savedLine = line;
                  count = 1;
               }
               else
                  count++;
            }

            if ((deletingAll && (count == 1)) || (!deletingAll && (count > 0)))
            {
               WriteText(savedLine);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelDuplLines(IFilter host) : base(host)
      {
         Template = "[n n] /A /Ds /I";
      }
   }
}
