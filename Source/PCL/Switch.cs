using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Implements a command line switch.
   /// </summary>
   public class Switch
   {
      public string ID { get; set; }
         // The switch's ID ("/B", "/R", etc.).
      public object Value { get; set; }
         // The switch's value.
      public int CharPos { get; set; }
         // The switch's command line position.

      public Switch(string iD, object value, int CharPos)
      {
         this.ID = iD.ToUpper();
         this.Value = value;
         this.CharPos = CharPos;
      }
   }
}
