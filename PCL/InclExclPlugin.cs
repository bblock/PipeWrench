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
   public abstract class InclExclPlugin : FilterPlugin
   {
      public override abstract void Execute();

      protected void Execute(bool excludingLines)
      {
         string newSource;
         bool found;

         string matchStr = (string) CmdLine.GetArg(0).Value;
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         if (matchStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         int begPos = 0;
         int endPos = 0;

         bool rangeGiven = (CmdLine.ArgCount > 1);

         if (rangeGiven)
         {
            begPos = (int) CmdLine.GetArg(1).Value;
            endPos = (int) CmdLine.GetArg(2).Value;

            CheckIntRange(begPos, 1, int.MaxValue, "Begin char. position", CmdLine.GetArg(1).CharPos);
            CheckIntRange(endPos, 1, int.MaxValue, "End char. position", CmdLine.GetArg(2).CharPos);
         }

         if (begPos > endPos)
         {
            // Oops.

            ThrowException("End char. position must be >= begin char. position.", CmdLine.GetArg(2).CharPos);
         }

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (rangeGiven)
               {
                  if (line.Length >= endPos)
                  {
                     newSource = line.Substring(begPos-1, endPos-begPos+1);
                     found = StringMatched(matchStr, newSource, ignoringCase, isRegEx);
                  }
                  else
                  {
                     // Range extends past end of line.

                     found = false;
                  }
               }
               else
                  found = StringMatched(matchStr, line, ignoringCase, isRegEx);

               if ((!found && excludingLines) || (found && !excludingLines))
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

      public InclExclPlugin(IFilter host) : base(host) {}
   }
}
