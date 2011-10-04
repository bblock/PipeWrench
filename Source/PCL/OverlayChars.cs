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
