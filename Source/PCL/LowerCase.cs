using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Lowercases the input text.
   /// </summary>
   public sealed class LowerCase : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               WriteText(ReadLine().ToLower());
            }
         }

         finally
         {
            Close();
         }
      }

      public LowerCase(IFilter host) : base(host) {}
   }
}
