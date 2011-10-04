using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes leading and trailing white space from each line of text.
   /// </summary>
   public sealed class TrimLinesLeft : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               WriteText(line.TrimStart());
            }
         }

         finally
         {
            Close();
         }
      }

      public TrimLinesLeft(IFilter host) : base(host) {}
   }
}
