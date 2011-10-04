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
