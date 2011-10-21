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
   /// Wraps the text to a given width.
   /// </summary>
   public sealed class WrapText : FilterPlugin
   {
      /// <summary>
      /// Wraps the given line of text so that it fits within the given margin.  This is done
      /// mostly by replacing blank characters that are nearest to the margin with a newline.
      /// A string of break characters may be specified to allow line breaks in addition to
      /// those that occur at spaces; however, note that break characters are not replaced by
      /// newlines (as is the case with blanks) but rather, a newline is inserted AFTER the
      /// break character.  This is useful, for example, to force line breaks to occur follow-
      /// ing a hyphen (as is illustrated here).  Another use for break characters would be
      /// to force line breaks after forward slashes (/) in anticipation of URLs found in the
      /// text being wrapped.  The "allowedJaggedness" parameter allows you to specify the
      /// percentage of the line that can be jagged.  If you allow 100% jaggedness, then the
      /// algothithm will favor breaking the line even at a blank (or break char.) that it
      /// finds located near the text's left margin, thus possibly resulting in "loose" lines
      /// that contain just a word or two.  If set to 0%, no blank or break character found
      /// will be used as a break point and instead, the line will simply be broken at the
      /// margin (hence 0% jaggedness).  A good value to choose as a default is 30%.
      /// </summary>
      private string Wrap(string text, int margin, string breakChars, int allowedJaggedness)
      {
         int jagCharLimit = (int) (margin * ((float) allowedJaggedness / 100));
         string result = string.Empty;

         while (text.Length > margin)
         {
            // Get index of blank character that is nearest the margin:

            int i = margin;
            while (i > 0 && text[i-1] != ' ') i--;
            int locOfBlank = i;

            // Get index of break character that is nearest the margin:

            i = margin;
            while ((i > 0) && (!breakChars.Contains(text[i-1].ToString()))) i--;
            int locOfBreak = i;

            // Wrap the text:

            if (locOfBlank + locOfBreak > 0)
            {
               if (locOfBlank > locOfBreak)
               {
                  // Found a blank character.

                  if (margin-locOfBlank < jagCharLimit)
                  {
                     // The blank was found within the % jaggedness
                     // allowed.  Replace the blank with a newline:

                     result += text.Substring(0, locOfBlank-1) + System.Environment.NewLine;
                     text = text.Remove(0, locOfBlank);
                  }
                  else
                  {
                     // The blank was found beyond the % jaggedness allowed limit.
                     // Gotta be tough here...  Split the line at the margin:

                     result += text.Substring(0, margin) + System.Environment.NewLine;
                     text = text.Remove(0, margin);
                  }
               }
               else
               {
                  // Found a break char.

                  if (margin-locOfBreak < jagCharLimit)
                  {
                     // The break char. was found within the % jaggedness
                     // allowed.  Insert a newline after the break char:

                     result += text.Substring(0, locOfBreak) + System.Environment.NewLine;
                     text = text.Remove(0, locOfBreak);
                  }
                  else
                  {
                     // The break char. was found beyond the % jaggedness allowed limit.
                     // Gotta be tough here...  Split the line at the margin:

                     result += text.Substring(0, margin) + System.Environment.NewLine;
                     text = text.Remove(0, margin);
                  }
               }
            }
            else
            {
               // No blank or break character was found.  Split line at margin:

               result += text.Substring(0, margin) + System.Environment.NewLine;
               text = text.Remove(0, margin);
            }
         }

         if (text.Length > 0) result += text;

         return result;
      }

      public override void Execute()
      {
         string breakChars = CmdLine.GetStrSwitch("/B", string.Empty);
         int jaggednessAllowed = CmdLine.GetIntSwitch("/J", 25);
         int charPos = (int) CmdLine.GetArg(0).Value;

         CheckIntRange(charPos, 1, int.MaxValue, "Char. position", CmdLine.GetArg(0).CharPos);
         CheckIntRange(jaggednessAllowed, 0, 100, "% jaggedness", CmdLine.GetSwitchPos("/J"));

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               WriteText(Wrap(line, charPos, breakChars, jaggednessAllowed));
            }
         }

         finally
         {
            Close();
         }
      }

      public WrapText(IFilter host) : base(host)
      {
         Template = "n /Bs /Jn";
      }
   }
}
