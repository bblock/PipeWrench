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
   public abstract class PadLinesPlugin : FilterPlugin
   {
      public static void PadString(ref string source,
      int width, string padStr, bool left)
      {
         int j = -1;
         int len = padStr.Length;

         if (len > 0)
         {
            while (source.Length < width)
            {
               j++;
               if (j == len) j = 0;

               if (left)
                  source = padStr[j] + source;
               else
                  source += padStr[j];
            }
         }
      }

      public override abstract void Execute();

      protected void Execute(bool left)
      {
         int Len;
         string Source;
         int SetIndex;

         string PadStr = (string) CmdLine.GetArg(0).Value;
         int Width = CmdLine.GetIntSwitch("/W", 0);
         int SetSize = CmdLine.GetIntSwitch("/S", 1);

         if (PadStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);
         CheckIntRange(Width, 0, int.MaxValue, "Numeric width", CmdLine.GetSwitchPos("/W"));
         CheckIntRange(SetSize, 1, int.MaxValue, "Set size", CmdLine.GetSwitchPos("/S"));

         Open();

         try
         {
            if (Width > 0)
            {
               // Padding to a specified width.

               while (!EndOfText)
               {
                  Source = ReadLine();
                  PadString(ref Source, Width, PadStr, left);
                  WriteText(Source);
               }
            }
            else
            {
               // The width is unspecified.  Padding all lines
               // to the width of the longest line IN ITS SET.
               // Clear the line lengths:

               int[] LongestLineLengths = new int[SetSize];

               for (int i=0; i < SetSize; i++)
               {
                  LongestLineLengths[i] = 0;
               }

               // Determine the longest line in each set:

               while (!EndOfText)
               {
                  Source = ReadLine();
                  Len = Source.Length;
                  SetIndex = (TextLineNo-1) % SetSize;

                  if (Len > LongestLineLengths[SetIndex])
                  {
                     LongestLineLengths[SetIndex] = Len;
                  }
               }

               // Re-position to top of text:

               Reset();

               // Pad each line to the longest line in its set:

               while (!EndOfText)
               {
                  Source = ReadLine();
                  SetIndex = (TextLineNo-1) % SetSize;
                  Width = LongestLineLengths[SetIndex];
                  PadString(ref Source, Width, PadStr, left);
                  WriteText(Source);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public PadLinesPlugin(IFilter host) : base(host)
      {
         Template = "s /Wn /Sn";
      }
   }
}
