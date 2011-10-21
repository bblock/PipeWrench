// 
// Pyper - automate the transformation of text using "stackable" text filters
// Copyright (C) 2011  Barry Block 
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
using System.Collections;
using System.Collections.Generic;

namespace Firefly.Pyper
{
   /// <summary>
   /// Implements a list of pipe compile errors.
   /// </summary>
   public class CompileErrors : IEnumerable
   {
      private List<PyperCompileException> Items { get; set; }

      public int Count
      {
         get { return Items.Count; }
      }

      public PyperCompileException this[int i]
      {
         get { return Items[i]; }
         set { Items[i] = value; }
      }

      public IEnumerator GetEnumerator()
      {
         return Items.GetEnumerator();
      }

      public CompileErrors()
      {
         Items = new List<PyperCompileException>();
      }

      public void Add(PyperCompileException ex)
      {
         Items.Add(ex);
      }

      public override string ToString()
      {
         string result = string.Empty;
         string oldSource = string.Empty;

         foreach (PyperCompileException err in Items)
         {
            string source = (string) err.Data["Source"];

            if (source != oldSource)
            {
               oldSource = source;
               result += source + System.Environment.NewLine;
            }

            int lineNo = (int) err.Data["LineNo"];
            string lineNoStr = lineNo.ToString();
            string cmdLine = (string) err.Data["CmdLine"];
            int charPos = (int) err.Data["CharPos"];

            if (lineNo > 0)
            {
               // This error originated from inside a pipe.

               result += "   line " + lineNoStr + ": " + cmdLine;
               result += System.Environment.NewLine + "^".PadLeft(charPos+10+lineNoStr.Length, ' ');
               result += System.Environment.NewLine + "      " + err.Message + System.Environment.NewLine +
               System.Environment.NewLine;
            }
            else
            {
               // This error originated from the main pipe's command line.

               result += "Error in pipe's arguments." + System.Environment.NewLine + "Pipe Arguments: " +
               cmdLine + System.Environment.NewLine + "^".PadLeft(charPos+17, ' ');
               result += System.Environment.NewLine + err.Message + System.Environment.NewLine +
               System.Environment.NewLine;
            }
         }

         return result;
      }
   }
}
