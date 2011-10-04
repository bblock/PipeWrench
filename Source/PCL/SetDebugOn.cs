using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Enables pipe debugging.
   /// </summary>
   public sealed class SetDebugOn : SetDebugPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public SetDebugOn(IFilter host) : base(host) {}
   }
}
