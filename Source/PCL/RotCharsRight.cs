using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Rotates each line right the given # of characters.
   /// </summary>
   public sealed class RotCharsRight : RotCharsPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public RotCharsRight(IFilter host) : base(host) {}
   }
}
