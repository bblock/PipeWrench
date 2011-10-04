using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Excludes all lines in text that contain a string.
   /// </summary>
   public sealed class ExclLines : InclExclPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public ExclLines(IFilter host) : base(host)
      {
         Template = "s [n n] /I /R";
      }
   }
}
