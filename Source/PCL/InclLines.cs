using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Includes all lines in text that contain a string.
   /// </summary>
   public sealed class InclLines : InclExclPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public InclLines(IFilter host) : base(host)
      {
         Template = "s [n n] /I /R";
      }
   }
}
