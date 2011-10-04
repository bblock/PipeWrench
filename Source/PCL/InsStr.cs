using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Inserts a string at the given character position of each line.
   /// </summary>
   public sealed class InsStr : FilterPlugin
   {
      public override void Execute()
      {
         int charPos;
         string charStr;

         Open();

         try
         {
            while (!EndOfText)
            {
               int offset = 0;
               int prevPos = 0;

               // Read a line of the source text:

               string text = ReadLine();

               // Insert each string argument into it:

               for (int j = 0; j < CmdLine.ArgCount / 2; j++)
               {
                  charPos = (int) CmdLine.GetArg((j*2)).Value + offset;
                  CheckIntRange(charPos, 1, int.MaxValue, "Character position",
                  CmdLine.GetArg((j*2)).CharPos);

                  if (charPos > prevPos)
                  {
                     prevPos = charPos;
                     charStr = (string) CmdLine.GetArg((j*2)+1).Value;
                     if (charStr == string.Empty) ThrowException("String cannot be empty.",
                     CmdLine.GetArg((j*2)+1).CharPos);

                     while (text.Length < charPos - 1)
                     {
                        text += ' ';
                     }

                     text = text.Insert(charPos-1, charStr);
                     offset += charStr.Length;
                  }
                  else
                  {
                     // Oops!

                     ThrowException("Character position arguments must be in ascending order.",
                     CmdLine.GetArg((j*2)).CharPos);
                  }
               }

               // Write the edited line to the output file:

               WriteText(text);
            }
         }

         finally
         {
            Close();
         }
      }

      public InsStr(IFilter host) : base(host)
      {
         Template = "n s [n s...]";
      }
   }
}
