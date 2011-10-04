using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Sets pipe debugging on/off.
   /// </summary>
   public abstract class SetDebugPlugin : FilterPlugin
   {
      public override abstract void Execute();

      protected void Execute(bool enabled)
      {
         // Set debugging OFF for this filter:

         ((Filter) Host).Debugging = false;

         // Merely copy the input text to output unchanged (passthrough):

         WriteAllText();

         // Set debugging on/off:

         ((Filter) Host).Debugging = enabled;
      }

      public SetDebugPlugin(IFilter host) : base(host)
      {
         Template = string.Empty;
      }
   }
}
