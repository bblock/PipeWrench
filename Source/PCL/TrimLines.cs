using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes leading and trailing white space from each line of text.
   /// </summary>
   public sealed class TrimLines : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               WriteText(line.Trim());
            }
         }

         finally
         {
            Close();
         }
      }

      public TrimLines(IFilter host) : base(host) {}
   }
}
