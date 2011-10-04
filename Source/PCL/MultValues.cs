using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Multiplies two or more numbers in the input text.
   /// </summary>
   public sealed class MultValues : AddMultPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public MultValues(IFilter host) : base(host) {}
   }
}
