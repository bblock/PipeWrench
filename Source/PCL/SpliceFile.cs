using System;
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   /// Splices the text from a text file to the end of each line of the input text.
   /// Any lines in the text file beyond the # of lines in the input text are ignored.
   /// </summary>
   public sealed class SpliceFile : FilterPlugin
   {
      public override void Execute()
      {
         string spliceFileName = (string) CmdLine.GetArg(0).Value;
         string delimiterStr = CmdLine.GetStrSwitch("/D", string.Empty);
         bool mergingText = CmdLine.GetBooleanSwitch("/M");

         try
         {
            using (TextReader tr = new StreamReader(spliceFileName))
            {
               Open();

               try
               {
                  string tempStr;
                  string source;

                  if (!mergingText)
                  {
                     // Appending each line from splice file to end of input line.

                     while (!EndOfText)
                     {
                        source = ReadLine();

                        if ((tempStr = tr.ReadLine()) != null)
                        {
                           WriteText(source + delimiterStr + tempStr);
                        }
                        else
                        {
                           WriteText(source);
                        }
                     }

                     while ((tempStr = tr.ReadLine()) != null)
                     {
                        WriteText(tempStr);
                     }
                  }
                  else
                  {
                     // Adding all lines from splice file to end of input text (merging mode).

                     while (!EndOfText)
                     {
                        source = ReadLine();
                        WriteText(source);
                     }

                     while ((tempStr = tr.ReadLine()) != null)
                     {
                        WriteText(tempStr);
                     }
                  }
               }

               finally
               {
                  Close();
               }
            }
         }

         catch (IOException)
         {
            // Error reading the splice file.

            ThrowException("Error reading the splice file.");
         }
      }

      public SpliceFile(IFilter host) : base(host)
      {
         Template = "s /Ds /M";
      }
   }
}
