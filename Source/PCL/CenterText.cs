using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Centers each line of the input text.
   /// </summary>
   public sealed class CenterText : FilterPlugin
   {
      public override void Execute()
      {
         int width = (int) CmdLine.GetArg(0).Value;
         CheckIntRange(width, 1, int.MaxValue, "Width", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               int i = (width - line.Length) / 2;

               if (i > 0)
                  WriteText(string.Empty.PadRight(i) + line);
               else
                  WriteText(line);
            }
         }

         finally
         {
            Close();
         }
      }

      public CenterText(IFilter host) : base(host)
      {
         Template = "n";
      }
   }
}
