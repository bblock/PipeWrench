using System;

namespace Firefly.Pyper
{
   /// <summary>
   ///    Uppercases the input text.
   /// </summary>
   public sealed class UpperCase : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               WriteText(ReadLine().ToUpper());
            }
         }

         finally
         {
            Close();
         }
      }

      public UpperCase(IFilter host) : base(host) {}
   }
}
