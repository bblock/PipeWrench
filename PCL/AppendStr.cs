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
   ///    Appends a string to each line of the input text.
   /// </summary>
   public sealed class AppendStr : FilterPlugin
   {
      public override void Execute()
      {
         LoggingEnabled = true;
         bool prepending = CmdLine.GetBooleanSwitch("/P");
         string appStr = (string) CmdLine.GetArg(0).Value;

         if (appStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               string tempStr;

               if (prepending)
                  tempStr = appStr + line;
               else
                  tempStr = line + appStr;

               WriteText(tempStr);
            }
         }

         finally
         {
            Close();
         }
      }

      public AppendStr(IFilter host) : base(host)
      {
         Template = "s /P";
      }
   }
}
