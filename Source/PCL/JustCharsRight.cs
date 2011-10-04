using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Justifies characters right.
   /// </summary>
   public sealed class JustCharsRight : JustCharsPlugin
   {
      public override void Execute()
      {
         base.Execute(false);
      }

      public JustCharsRight(IFilter host) : base(host)
      {
         Template = "[n n]";
      }
   }
}
