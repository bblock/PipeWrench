using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Right pads each line in the text.
   /// </summary>
   public sealed class PadLinesRight : PadLinesPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public PadLinesRight(IFilter host) : base(host) {}
   }
}
