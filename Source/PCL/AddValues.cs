using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Adds two or more numbers found in each input line
   /// </summary>
   public sealed class AddValues : AddMultPlugin
   {
      public override void Execute()
      {
         base.Execute(true);
      }

      public AddValues(IFilter host) : base(host) {}
   }
}
