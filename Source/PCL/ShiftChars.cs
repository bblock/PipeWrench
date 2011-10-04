using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Shifts characters until given string is located at the given character position.
   /// </summary>
   public sealed class ShiftChars : FilterPlugin
   {
      //private Match match;

      public override void Execute()
      {
         string theMatch;
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int begPos = 1;

               for (int i = 0; i < CmdLine.ArgCount / 2; i++)
               {
                  int j = (i * 2);
                  int charPos = (int) CmdLine.GetArg(j).Value;
                  string matchStr = (string) CmdLine.GetArg(j+1).Value;
                  string subSource = line.Substring(begPos - 1, line.Length - begPos + 1);

                  CheckIntRange(charPos, 1, int.MaxValue, "Character position",
                  CmdLine.GetArg(j).CharPos);
                  if (matchStr == string.Empty) ThrowException("String cannot be empty.",
                  CmdLine.GetArg(j+1).CharPos);
                  int p = StringPos(matchStr, subSource, ignoringCase, isRegEx, out theMatch) + 1;

                  if (p > 0)
                  {
                     // Match string was found in the line.

                     int len;

                     if (isRegEx)
                        len = theMatch.Length;
                     else
                        len = matchStr.Length;

                     int noOfChars;

                     if ((p + begPos - 1) < charPos)
                     {
                        // The match string is located prior to the column position.

                        noOfChars = charPos - (p + begPos - 1);
                        string stringOfBlanks = new string(' ', noOfChars);
                        line = line.Insert(p + begPos - 2, stringOfBlanks);
                        begPos = p + begPos - 1 + len + noOfChars;
                     }
                     else
                     {
                        // The match string is located after the column position.

                        noOfChars = (p + begPos - 1) - charPos;
                        line = line.Remove(charPos - 1, noOfChars);
                        begPos = p + begPos - 1 + len - noOfChars;
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

      public ShiftChars(IFilter host) : base(host)
      {
         Template = "n s [n s...] /I /R";
      }
   }
}
