using System;

namespace Firefly.Pyper
{
   /// <summary>
   /// Parses quoted, comma-delimited fields onto separate lines
   /// </summary>
   public sealed class ParseCSV : FilterPlugin
   {
      /// <summary>
      /// This routine returns the next character.
      /// </summary>
      private char GetAChar(ref int i, ref string Source)
      {
         char Result;

         if (i < Source.Length)
         {
            Result = Source[i];
            i++;
         }
         else
         {
            Result = '\0';
         }

         return Result;
      }

      public override void Execute()
      {
         int i;
         string Source;
         string Token;
         char QuoteChar;
         char DelimiterChar;
         bool UsingBackslashQuote;
         int State;
         char Ch;
         bool Done;

         string QuoteStr = CmdLine.GetStrSwitch("/Q", "\"");
         string DelimiterStr = CmdLine.GetStrSwitch("/D", ",");
         UsingBackslashQuote = CmdLine.GetBooleanSwitch("/B");

         if (QuoteStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetSwitchPos("/Q"));
         if (DelimiterStr == string.Empty) ThrowException("String cannot be empty.", CmdLine.GetSwitchPos("/D"));

         QuoteChar = QuoteStr[0];
         DelimiterChar = DelimiterStr[0];

         Open();

         try
         {
            while (!EndOfText)
            {
               Source = ReadLine();
               i = 0;
               State = 1;
               Token = string.Empty;
               Done = false;

               do
               {
                  Ch = GetAChar(ref i, ref Source);

                  if (Ch != '\0')
                  {
                     switch (State)
                     {
                        case 1:
                           if (Ch == QuoteChar)
                           {
                              Token += Ch;
                              State = 2;
                           }
                           else
                           {
                              if (Ch != DelimiterChar)
                              {
                                 if (Char.IsWhiteSpace(Ch))
                                 {
                                    State = 8;
                                 }
                                 else
                                 {
                                    Token += Ch;
                                    State = 5;
                                 }
                              }
                              else
                                 State = 4;
                           }

                           break;

                        case 2:
                           Token += Ch;

                           if (Ch == QuoteChar)
                           {
                              if (UsingBackslashQuote)
                                 State = 7;
                              else
                                 State = 3;
                           }
                           else
                           {
                              if ((Ch == '\b') && UsingBackslashQuote)
                              {
                                 State = 6;
                              }
                           }

                           break;

                        case 3:
                           if (Ch == QuoteChar)
                           {
                              Token += Ch;
                              State = 2;
                           }
                           else
                           {
                              if (Char.IsWhiteSpace(Ch))
                                 State = 8;
                              else
                                 State = 4;
                           }

                           break;

                        case 4:
                           WriteText(Token);
                           Token = string.Empty;

                           if (Ch == QuoteChar)
                           {
                              Token += Ch;
                              State = 2;
                           }
                           else
                           {
                              if (Ch != DelimiterChar)
                              {
                                 if (!Char.IsWhiteSpace(Ch))
                                 {
                                    Token += Ch;
                                    State = 5;
                                 }
                                 else
                                    State = 8;
                              }
                           }

                           break;

                        case 5:
                           if (Ch == DelimiterChar)
                           {
                              State = 4;
                           }
                           else
                           {
                              if (Char.IsWhiteSpace(Ch))
                                 State = 8;
                              else
                                 Token += Ch;
                           }

                           break;

                        case 6:
                           Token += Ch;
                           State = 2;
                           break;

                        case 7:
                           if (Ch == DelimiterChar) State = 4;
                           break;

                        case 8:
                           if (Ch == DelimiterChar)
                           {
                              State = 4;
                           }
                           else
                           {
                              if (Ch == QuoteChar)
                              {
                                 Token += Ch;
                                 State = 2;
                              }
                              else
                              {
                                 if (!Char.IsWhiteSpace(Ch))
                                 {
                                    Token += Ch;
                                    State = 5;
                                 }
                              }
                           }

                           break;
                     }
                  }
                  else
                  {
                     if (Token != string.Empty) WriteText(Token);
                     string TempStr = Source.Trim();

                     if (TempStr[TempStr.Length-1] == DelimiterChar)
                     {
                        WriteText(string.Empty);
                     }

                     Done = true;
                  }
               }
               while (!Done);
            }
         }

         finally
         {
            Close();
         }
      }

      public ParseCSV(IFilter host) : base(host)
      {
         Template = "/Qs /Ds /B";
      }
   }
}
