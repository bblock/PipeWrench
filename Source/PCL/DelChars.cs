using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes chars from each line at char position.
   /// </summary>
   public sealed class DelChars : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               int offset = 0;
               int prevPos = 0;
               string text = ReadLine();

               // Delete characters at each character position:

               for (int j = 0; j < CmdLine.ArgCount / 2; j++)
               {
                  int charPos = (int) CmdLine.GetArg(j*2).Value;
                  CheckIntRange(charPos, 1, int.MaxValue, "Char. position",
                  CmdLine.GetArg(j*2).CharPos);

                  if (charPos > prevPos)
                  {
                     int noOfChars = (int) CmdLine.GetArg((j*2)+1).Value;
                     CheckIntRange(noOfChars, 1, int.MaxValue, "No. of characters",
                     CmdLine.GetArg((j*2)+1).CharPos);

                     while (text.Length < charPos-offset-1 + noOfChars) text += ' ';
                     text = text.Remove(charPos-offset-1, noOfChars);
                     offset += noOfChars;
                     prevPos = charPos + noOfChars - 1;
                  }
                  else
                  {
                     // Character position arguments are not ascending.

                     ThrowException("Character position arguments must be non-overlapping " +
                     "and in ascending order.", CmdLine.GetArg(j*2).CharPos);
                  }
               }

               WriteText(text);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelChars(IFilter host) : base(host)
      {
         Template = "n n [n n...]";
      }
   }
}
