using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Disables pipe debugging.
   /// </summary>
   public sealed class SetDebugOff : SetDebugPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public SetDebugOff(IFilter host) : base(host) {}
   }
}
