using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Justifies characters left.
   /// </summary>
   public sealed class JustCharsLeft : JustCharsPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public JustCharsLeft(IFilter host) : base(host)
      {
         Template = "[n n]";
      }
   }
}
