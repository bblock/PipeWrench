using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Implements a command line argument.
   /// </summary>
   public class Argument : IArgument
   {
      public object Value { get; set; }
         // The argument's value.
      public int CharPos { get; set; }
         // The argument's command line position.

      public Argument(object value, int CharPos)
      {
         this.Value = value;
         this.CharPos = CharPos;
      }
   }
}
