using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Deletes characters in a line until the given string is found at its beginning.
   /// If the string exists in the line multiple times, you can delete to a given
   /// occurence of the string.  If the string is not found the specified # of times
   /// then the line is left in its original state.
   /// </summary>
   public sealed class DelCharsToStr : FilterPlugin
   {

      public override void Execute()
      {
         string theStr = (string) CmdLine.GetArg(0).Value;
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         int noOfRotates = CmdLine.GetIntSwitch("/N", 1);
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         CheckIntRange(noOfRotates, 1, int.MaxValue, "No. of rotates", CmdLine.GetSwitchPos("/N"));
         if (theStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line.Length > 0)
               {
                  string matchStr = string.Empty;
                  string savedLine = line;

                  for (int j=1; j <= noOfRotates; j++)
                  {
                     if (j > 1)
                     {
                        // Delete the previous match from beginning of line:

                        line = line.Substring(matchStr.Length);
                     }

                     // Find the next match:

                     int charPos = StringPos(theStr, line, ignoringCase, isRegEx, out matchStr);

                     if (charPos >= 0)
                     {
                        // Found it.

                        if (charPos > 0)
                        {
                           // Rotate to it:

                           line = line.Substring(charPos, line.Length - charPos);
                        }
                     }
                     else
                     {
                        // No match found.

                        if (j > 1)
                        {
                           // Restore the line:

                           line = savedLine;
                        }

                        // Quit:

                        break;
                     }
                  }
               }

               WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelCharsToStr(IFilter host) : base(host)
      {
         Template = "s /I /Nn /R";
      }
   }
}
