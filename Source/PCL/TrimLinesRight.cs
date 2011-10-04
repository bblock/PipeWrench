using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes leading and trailing white space from each line of text.
   /// </summary>
   public sealed class TrimLinesRight : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               WriteText(line.TrimEnd());
            }
         }

         finally
         {
            Close();
         }
      }

      public TrimLinesRight(IFilter host) : base(host) {}
   }
}
