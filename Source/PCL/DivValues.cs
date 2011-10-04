using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Divides two numbers in the input text.
   /// </summary>
   public sealed class DivValues : SubDivPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public DivValues(IFilter host) : base(host) {}
   }
}
