using System;
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   /// Used in conjuction with IsolateLines to constrain
   /// pipe commands to an isolated block of text.
   /// </summary>
   public sealed class EndIsolate : FilterPlugin
   {
      public override void Execute()
      {
         if (((Filter) Host).DivTextStack.Count > 0)
         {
            // There's diverted text on the stack.

            Open();

            try
            {
               // Pop the diverted text:

               string divText = ((Filter) Host).DivTextStack.Pop();

               // Open the text file for reading:

               StreamReader divTextReader = new StreamReader(divText);

               try
               {
                  // Output the prior saved "top" lines:

                  string line;

                  while (!divTextReader.EndOfStream)
                  {
                     line = divTextReader.ReadLine();

                     if (line != "<rekram yradnuob>")
                        WriteText(line);
                     else
                        break;
                  }

                  // Output the lines processed by the prior constrained filters:

                  while (!EndOfText)
                  {
                     line = ReadLine();
                     WriteText(line);
                  }

                  // Output the prior saved "bottom" lines:

                  while (!divTextReader.EndOfStream)
                  {
                     line = divTextReader.ReadLine();
                     WriteText(line);
                  }
               }

               finally
               {
                  // Delete the no longer needed diverted text file:

                  divTextReader.Close();
                  File.Delete(divText);
               }
            }

            finally
            {
               Close();
            }
         }
         else
         {
            // There's no diverted text object.

            ThrowException("Unmatched IsolateLines/EndIsolate commands.");
         }
      }

      public EndIsolate(IFilter host) : base(host) {}
   }
}
