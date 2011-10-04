using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes any blank lines from the input text.
   /// </summary>
   public sealed class DelBlankLines : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();

               if (line.Length > 0)
               {
                  WriteText(line);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public DelBlankLines(IFilter host) : base(host) {}
   }
}
