using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Subtracts two numbers in the input text.
   /// </summary>
   public sealed class SubValues : SubDivPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public SubValues(IFilter host) : base(host) {}
   }
}
