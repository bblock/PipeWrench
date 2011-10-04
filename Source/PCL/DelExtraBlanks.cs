using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Removes extraneous blanks from the input text.
   /// </summary>
   public sealed class DelExtraBlanks : FilterPlugin
   {
      public override void Execute()
      {
         Open();

         try
         {
            while (!EndOfText)
            {
               string Source = ReadLine();
               int i = 0;

               while (i < Source.Length)
               {
                  while ((i < Source.Length) && (Source[i] != ' '))
                  {
                     i++;
                  }

                  if (i < Source.Length)
                  {
                     int BegPos = i;

                     while ((i < Source.Length) && (Source[i] == ' '))
                     {
                        i++;
                     }

                     int NoOfChars = i - BegPos - 1;
                     if (NoOfChars > 0)
                     {
                        Source = Source.Remove(BegPos, NoOfChars);
                        i = BegPos + 1;
                     }
                  }
               }

               WriteText(Source);
            }
         }

         finally
         {
            Close();
         }
      }

      public DelExtraBlanks(IFilter host) : base(host) {}
   }
}
