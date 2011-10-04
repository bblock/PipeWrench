using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Left pads each line in the text.
   /// </summary>
   public sealed class PadLinesLeft : PadLinesPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public PadLinesLeft(IFilter host) : base(host) {}
   }
}
