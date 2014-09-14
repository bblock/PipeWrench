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
