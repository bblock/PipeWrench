using System;
using System.Text.RegularExpressions;

namespace Firefly.Pyper
{
   /// <summary>
   /// Replaces character strings found in the text.
   /// </summary>
   public sealed class ReplStr : FilterPlugin
   {
      private string GetReplString(string line, int begPos, int endPos, char delimiter) 
      {
         string result = string.Empty;

         if (endPos == 0)
         {
            // Return replace string based on begPos / delimiter:

            while ((begPos <= line.Length) && (line[begPos-1] != delimiter))
            {
               result += line[begPos-1];
               begPos++;
            }
         }
         else
         {
            // Return replace string based on begPos / endPos:

            int count = endPos - begPos + 1;
            result = line.Substring(begPos-1, count);
         }

         return result;
      }

      public override void Execute()
      {
         string delimiterStr = CmdLine.GetStrSwitch("/D", " ");
         string placeHolderStr = CmdLine.GetStrSwitch("/P", "%");
         bool ignoringCase = CmdLine.GetBooleanSwitch("/I");
         int begPos = CmdLine.GetIntSwitch("/B", 0);
         int endPos = CmdLine.GetIntSwitch("/E", 0); 
         bool isRegEx = CmdLine.GetBooleanSwitch("/R");
         RegexOptions defaultOptions = RegexOptions.Compiled | RegexOptions.Multiline;

         if (delimiterStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetSwitchPos("/D"));
         if (placeHolderStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetSwitchPos("/P"));
         if (begPos != 0) CheckIntRange(begPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/B"));
         if (endPos != 0) CheckIntRange(endPos, 1, int.MaxValue, "Char. position", CmdLine.GetSwitchPos("/E"));

         char delimiter = delimiterStr[0];

         Open();

         try
         {
            while (!EndOfText)
            {
               // Read a line of the source text:

               string text = ReadLine();

               // Perform text replacements to line:

               for (int i = 0; i < CmdLine.ArgCount / 2; i++)
               {
                  string oldStr = (string) CmdLine.GetArg(i*2).Value;
                  if (oldStr == string.Empty) ThrowException("String cannot be empty.",
                  CmdLine.GetArg(i*2).CharPos);
                  string newStr;

                  if (begPos == 0)
                  {
                     // Use the specified replace string literally.

                     newStr = (string) CmdLine.GetArg((i*2)+1).Value;
                  }
                  else
                  {
                     // Use the specified replace string as a template only.
                     // The real replace string originates from the input text.

                     string replaceStr = GetReplString(text, begPos, endPos, delimiter);
                     string templateStr = (string) CmdLine.GetArg((i * 2) + 1).Value;
                     newStr = templateStr.Replace(placeHolderStr, replaceStr);
                  }

                  if (isRegEx)
                  {
                     if (ignoringCase)
                     {
                        text = Regex.Replace(text, oldStr, newStr, defaultOptions |
                        RegexOptions.IgnoreCase);
                     }
                     else
                     {
                        text = Regex.Replace(text, oldStr, newStr, defaultOptions);
                     }
                  }
                  else
                  {
                     text = ReplaceString(text, oldStr, newStr, ignoringCase);
                  }
               }

               // Write the edited line to the output file:

               WriteText(text);
            }
         }

         finally
         {
            Close();
         }
      }

      public ReplStr(IFilter host) : base(host)
      {
         Template = "s s [s s...] /Bn /Ds /En /I /Ps /R";
      }
   }
}
