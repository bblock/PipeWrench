using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Rotates each line left the given # of characters.
   /// </summary>
   public sealed class RotCharsLeft : RotCharsPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public RotCharsLeft(IFilter host) : base(host) {}
   }
}
