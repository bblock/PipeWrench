using System;

namespace Firefly.Pyper
{
   /// <summary>
   ///    Appends a string to each line of the input text.
   /// </summary>
   public sealed class AppendStr : FilterPlugin
   {
      public override void Execute()
      {
         LoggingEnabled = true;
         bool prepending = CmdLine.GetBooleanSwitch("/P");
         string appStr = (string) CmdLine.GetArg(0).Value;

         if (appStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetArg(0).CharPos);

         Open();

         try
         {
            while (!EndOfText)
            {
               string line = ReadLine();
               string tempStr;

               if (prepending)
                  tempStr = appStr + line;
               else
                  tempStr = line + appStr;

               WriteText(tempStr);
            }
         }

         finally
         {
            Close();
         }
      }

      public AppendStr(IFilter host) : base(host)
      {
         Template = "s /P";
      }
   }
}
