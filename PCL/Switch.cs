// 
// PipeWrench - automate the transformation of text using "stackable" text filters
// Copyright (c) 2014  Barry Block 
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version. 
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE.  See the GNU General Public License for more details. 
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
using System;

namespace Firefly.PipeWrench
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
