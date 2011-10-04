using System;

namespace Firefly.Pyper
{
   /// <summary>
   ///    Parses the text into individual words.
   /// </summary>
   public sealed class ParseWords : FilterPlugin
   {
      public override void Execute()
      {
         string delims = CmdLine.GetStrSwitch("/D", " ");
         char[] delimsArr = delims.ToCharArray();

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               string[] words = line.Split(delimsArr, StringSplitOptions.RemoveEmptyEntries);

               foreach (string word in words)
               {
                  WriteText(word);
               }
            }
         }

         finally
         {
            Close();
         }
      }

      public ParseWords(IFilter host) : base(host)
      {
         Template = "/Ds";
      }
   }
}
