using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes any extraneous blank lines from the input text.
   /// </summary>
   public sealed class DelExtraBlankLines : FilterPlugin
   {
      public override void Execute()
      {
         bool prevWasBlank = false;

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line == string.Empty)
               {
                  if (!prevWasBlank)
                  {
                     WriteText(line);
                  }

                  prevWasBlank = true;
               }
               else
               {
                  WriteText(line);
                  prevWasBlank = false;
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public DelExtraBlankLines(IFilter host) : base(host) {}
   }
}
