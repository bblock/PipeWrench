// 
// PipeWrench - automate the transformation of text using "stackable" text filters
// Copyright (c) 2014  Barry Block 
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version. 
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE.  See the GNU General Public License for more details. 
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Firefly.PipeWrench
{
   public interface ILogger
   {
      void Write(string Message);
         // Writes the given message to the current log file.
      void WriteText(string text, string heading, string prefix);
         // Writes the given text to the current log file.
   }
   
   /// <summary>
   /// Implements a filter host interface.
   /// </summary>
   public interface IFilter
   {
      string Name { get; }
      ICommandLine CmdLine { get; }
         // The filter's command line.
      ILogger Log { get; }
         // Performs logging functions.
      object Eng { get; }
         // Pipe engine reference.
      int TextLineCount { get; }
         // The number of lines of text input.
      bool EndOfText { get; }
         // Is true if there is no more text for the filter to process.
      int TextLineNo { get; }
         // The line number of the text line being processed.
      int PipeLineNo { get; }
         // The filter's line number in the pipe.
      string Source { get; }
         // The filter's parent pipe.
      string ReadLine();
         // Reads a line of text to be processed.
      void WriteText(string text);
         // Writes the processed lines of text with trailing newline.
      void Write(string text);
         // Writes the processed lines of text without trailing newline.
      void WriteAllText();
         // Transfers all input text to output.
      void Reset();
         // Reset's the input text read pointer.
      void Open();
         // Opens both input and output text for processing.
      void Close();
         // Closes both input and output text after processing.
   }
   
   /// <summary>
   /// Implements a command line interface.
   /// </summary>
   public interface ICommandLine
   {
      string Text { get; set; }
         // The command line text.
      int ArgCount { get; }
         // Count of command line arguments passed to filter.
      bool IgnoringExtraneousText { get; set; }
         // Determines whether extraneous text is allowed 
         // beyond that expected by the template.
      IArgument GetArg(int argIndex);
         // Returns the given argument passed to the filter.
         // Note that argIndex is 0-based.
      int GetIntSwitch(string iD, int defaultValue);
         // Returns the value of the integer switch specified by iD.
      int GetSwitchPos(string iD); 
         // Returns the command line position of the specified switch.
      string GetStrSwitch(string iD, string defaultValue);
         // Returns the value of the string switch specified by iD.
      bool GetBooleanSwitch(string iD);
         // Returns the value of the boolean switch specified by iD.
      void Parse(int initPos, bool xlatStrings); 
         // Parses the filter's command line.
   }
   
   /// <summary>
   /// Implements an argument interface.
   /// </summary>
   public interface IArgument
   {
      object Value { get; }
      int CharPos { get; }
   }
   
   /// <summary>
   /// Implements a base PipeWrench exception.
   /// </summary>
   [Serializable]
   public class PipeWrenchException : ApplicationException
   {
      public PipeWrenchException()
         : base("A PipeWrench error occurred.")
      {
      }

      public PipeWrenchException(string message)
         : base(message)
      {
      }

      public PipeWrenchException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench engine exception.  These originate
   /// from inside of the PipeWrench engine but are not caused by 
   /// any fault of the user.
   /// </summary>
   [Serializable]
   public class PipeWrenchEngineException : PipeWrenchException
   {
      public PipeWrenchEngineException()
         : base("A PipeWrench engine error occurred.")
      {
      }

      public PipeWrenchEngineException(string message)
         : base(message)
      {
      }

      public PipeWrenchEngineException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchEngineException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench user exception.  This is the base
   /// class for all exceptions that result from end-user error
   /// and it should not be referenced directly.
   /// </summary>
   [Serializable]
   public class PipeWrenchUserException : PipeWrenchException
   {
      public PipeWrenchUserException()
         : base("A PipeWrench user error occurred.")
      {
      }

      public PipeWrenchUserException(string message)
         : base(message)
      {
      }

      public PipeWrenchUserException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchUserException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench compile-time exception.  These exceptions originate
   /// from inside of the PipeWrench engine but are the result of parsing errors.
   /// </summary>
   [Serializable]
   public class PipeWrenchCompileException : PipeWrenchUserException
   {
      public PipeWrenchCompileException()
         : base("A PipeWrench compile error occurred.")
      {
      }

      public PipeWrenchCompileException(string message)
         : base(message)
      {
      }

      public PipeWrenchCompileException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchCompileException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench conversion exception.  These exceptions originate
   /// from inside of the PipeWrench engine but are the result of an improperly
   /// formed pipe (with regards to importing or exporting).
   /// </summary>
   [Serializable]
   public class PipeWrenchConvException : PipeWrenchUserException
   {
      public PipeWrenchConvException()
         : base("A PipeWrench conversion error occurred.")
      {
      }

      public PipeWrenchConvException(string message)
         : base(message)
      {
      }

      public PipeWrenchConvException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchConvException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench template exception. These exceptions are caused
   /// by improperly formed templates.
   /// </summary>
   [Serializable]
   public class PipeWrenchTemplateException : PipeWrenchUserException 
   {
      public PipeWrenchTemplateException()
         : base("A PipeWrench template error occurred.")
      {
      }

      public PipeWrenchTemplateException(string message)
         : base(message)
      {
      }

      public PipeWrenchTemplateException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchTemplateException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench run-time exception.  These exceptions originate
   /// from inside of the PipeWrench engine but are the result of bad data or 
   /// arguments.
   /// </summary>
   [Serializable]
   public class PipeWrenchExecException : PipeWrenchUserException
   {
      public PipeWrenchExecException()
         : base("A PipeWrench execution error occurred.")
      {
      }

      public PipeWrenchExecException(string message)
         : base(message)
      {
      }

      public PipeWrenchExecException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchExecException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements a PipeWrench server exception.  These originate
   /// from inside of the PipeWrench server.
   /// </summary>
   [Serializable]
   public class PipeWrenchServerException : PipeWrenchException
   {
      public PipeWrenchServerException()
         : base("A PipeWrench server error occurred.")
      {
      }

      public PipeWrenchServerException(string message)
         : base(message)
      {
      }

      public PipeWrenchServerException(string message, Exception innerText)
         : base(message, innerText)
      {
      }
      
      protected PipeWrenchServerException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
      }
   }
   
   /// <summary>
   /// Implements the base filter plug-in class.
   /// </summary>
   public abstract class FilterPlugin
   {
      public string Template { get; set; }
      protected IFilter Host { get; set; }
      protected bool LoggingEnabled { get; set; }

      protected string Name
      {
         get { return Host.Name; }
      }
      
      protected ICommandLine CmdLine
      {
         get { return Host.CmdLine; }
      }
      
      protected object Eng
      {
         get { return Host.Eng; }
      }
      
      protected int TextLineCount  
      {
         get { return Host.TextLineCount; }
      }
      
      protected int TextLineNo  
      {
         get { return Host.TextLineNo; }
      }
      
      protected int PipeLineNo 
      {
         get { return Host.PipeLineNo; }
      }
      
      protected string Source 
      {
         get { return Host.Source; }
      }
      
      protected bool EndOfText  
      {
         get { return Host.EndOfText; }
      }
      
      protected string ReadLine()  
      {
         return Host.ReadLine();
      }
      
      protected void Reset()  
      {
         Host.Reset();
      }
      
      protected void Open()
      {
         Host.Open();
      }
      
      protected void Close()
      {
         Host.Close();
      }

      protected void WriteText(string text)  
      {
         Host.WriteText(text);
      }
      
      protected void Write(string text)  
      {
         Host.Write(text);
      }
      
      protected void WriteAllText()
      {
         Host.WriteAllText();
      }
      
      public FilterPlugin(IFilter host)
      {
         this.Template = string.Empty;
         this.Host = host;
         this.LoggingEnabled = false;
      }
      
      /// <summary>
      /// Writes the given one-line message to the log.
      /// </summary>
      protected void LogWrite(string message)
      {
         if (LoggingEnabled)
         {
            Host.Log.Write(message);
         }
      }
      
      /// <summary>
      /// Writes the given multi-line text to the log.
      /// </summary>
      protected void LogWriteText(string text, string heading, string prefix)
      {
         if (LoggingEnabled)
         {
            Host.Log.WriteText(text, heading, prefix);
         }
      }
      
      /// <summary>
      /// Executes the filter plug-in.
      /// </summary>
      public abstract void Execute();
      
      /// <summary>
      /// Parses the filter's command line.
      /// </summary>
      public virtual void Compile(string cmdLineText, int initialCharPos) 
      {
         Host.CmdLine.Text = cmdLineText;
         Host.CmdLine.IgnoringExtraneousText = false;
         
         // Parse the filter's arguments (note: a filter's string 
         // arguments should always be interpreted, hence the 
         // xlatStrings parameter is passed as true):
         
         Host.CmdLine.Parse(initialCharPos, true); 
      }
      
      /// <summary>
      /// Prepares and throws a plug-in execution exception.
      /// </summary>
      protected void ThrowException(string message) 
      {
         PipeWrenchExecException ex = new PipeWrenchExecException(message); 
         ex.Data.Add("CmdLine", CmdLine.Text);
         ex.Data.Add("Source", Source);
         ex.Data.Add("LineNo", PipeLineNo);
         throw ex;
      }
      
      /// <summary>
      /// Prepares and throws a plug-in execution exception that includes a character position.
      /// </summary>
      protected void ThrowException(string message, int charPos) 
      {
         PipeWrenchExecException ex = new PipeWrenchExecException(message); 
         ex.Data.Add("CmdLine", CmdLine.Text);
         ex.Data.Add("Source", Source);
         ex.Data.Add("LineNo", PipeLineNo);
         ex.Data.Add("CharPos", charPos);
         throw ex;
      }
      
      /// <summary>
      /// Prepares and throws an integer out-of-range exception.
      /// </summary>
      protected void CheckIntRange(int number, int begVal, int endVal, string tag, int charPos) 
      {
         if ((number < begVal) || (number > endVal))
         {
            string message;
            
            if (tag == string.Empty)
               message = "Integer value ";
            else
               message = tag + " ";
            
            message +=  "must be >= " + begVal.ToString();
            
            if (endVal < int.MaxValue) message += " and <= " + endVal.ToString();
            
            if (charPos > 0)
               ThrowException(message, charPos);
            else
               ThrowException(message);
         }
      }
      
      /// <summary>
      /// Replaces each occurrence of a string and allows for case insensitivity.
      /// Source: http://www.west-wind.com/weblog/posts/60355.aspx
      /// </summary>
      protected static string ReplaceString(string source, string findStr, string replStr, 
      bool caseInsensitive)
      {
         if (findStr != string.Empty)
         {
            int CharPos = 0;
            
            do
            {
               if (caseInsensitive)
               {
                  CharPos = source.IndexOf(findStr, CharPos, source.Length-CharPos, 
                  StringComparison.OrdinalIgnoreCase);
               }
               else
               {
                  CharPos = source.IndexOf(findStr, CharPos);
               }
         
               if (CharPos != -1)
               {
                  source = source.Substring(0, CharPos) + replStr + 
                  source.Substring(CharPos + findStr.Length);
                  CharPos += replStr.Length;
               }
            } 
            while (CharPos != -1);
         }
            
         return source;
      }

      /// <summary>
      /// Returns 0-based index of theStr found in source. Also outputs the
      /// actual string that is matched (for use if matching regex pattern).
      /// </summary>
      protected static int StringPos(string theStr, string source, bool ignoringCase, bool isRegEx, 
      out string matchStr)
      {
         int result;
         
         if (isRegEx)
         {
            RegexOptions options = RegexOptions.Compiled | RegexOptions.Multiline;
            if (ignoringCase) options = options | RegexOptions.IgnoreCase;
            Match match = Regex.Match(source, theStr, options);
            
            if (match.Success)
            {
               result = match.Index;
               matchStr = match.Value;
            }
            else
            {
               // The following is needed because Regex.Match doesn't 
               // set match.Index to -1 when match fails.
               
               result = -1;
               matchStr = string.Empty;
            }
         }
         else
         {
            if (ignoringCase)
               result = source.IndexOf(theStr, StringComparison.InvariantCultureIgnoreCase);
            else
               result = source.IndexOf(theStr);

            matchStr = theStr;
         }
         
         return result;
      }

      /// <summary>
      /// Returns true if theStr is found/matched in source. 
      /// </summary>
      protected static bool StringMatched(string theStr, string source, bool ignoringCase, bool isRegEx)
      {
         string matchStr;
         return StringPos(theStr, source, ignoringCase, isRegEx, out matchStr) > -1;
      }

      /// <summary>
      /// Returns true if theStr is found in source at thePos. 
      /// </summary>
      protected static bool StringFoundAtPos(string theStr, string source,
      int thePos, bool ignoringCase, bool isRegEx, bool enabled)
      {
         bool result;
         
         if (enabled)
         {
            if (isRegEx)
            {
               RegexOptions options = RegexOptions.Compiled | RegexOptions.Multiline;
               if (ignoringCase) options = options | RegexOptions.IgnoreCase;
               result = Regex.Match(source, theStr, options).Index == 0;
            }
            else
            {
               result = string.Compare(theStr, 0, source, thePos-1, 
               theStr.Length, ignoringCase) == 0;
            }
         }
         else
            result = false;
         
         return result;
      }

      /// <summary>
      /// This routine returns true if the two specified strings compare. If a range
      /// of character positions is supplied, then the comparison is based on that 
      /// range only (note: these character positions are 1-based). Set ignoringCase 
      /// true to ignore case during the comparison. If either string is null, the 
      /// result is false.
      /// </summary>
      protected static bool StringsCompare(string str1, string str2, bool ignoringCase, int begPos, int endPos)
      {
         int noOfChars;
         bool rangeIsGiven = (begPos + endPos) >= 2;
         bool result;

         if ((str1 != null) && (str2 != null))
         {
            if (rangeIsGiven)
            {
               // Compare is based on range of character positions. 
   
               noOfChars = endPos - begPos + 1;
               result = string.Compare(str1, begPos-1, str2, begPos-1, noOfChars, ignoringCase) == 0;
            }
            else
            {
               // Compare is based on each entire string. 
   
               result = string.Compare(str1, str2, ignoringCase) == 0;
            }
         }
         else
         {
            // One of the strings is null.
            
            result = false;
         }
         
         return result;
      }
   
      /// <summary>
      /// Returns the text in source that preceeds the given delimiter 
      /// or the source itself if the delimiter is not found.
      /// </summary>
      protected static string GetDelimitedText(string source, char delim)
      {
         int delimIndex = source.IndexOf(delim);
         
         if (delimIndex > -1)
            return source.Substring(0, delimIndex);
         else
            return source;
      }
      
      /// <summary>
      /// This routine returns true if the two strings compare up to the position of 
      /// the specified delimiter. Set ignoringCase true to ignore case during the 
      /// comparison. If either string is null, the result is false.
      /// </summary>
      protected static bool StringsCompare(string str1, string str2, bool ignoringCase, char delim)
      {
         bool result;

         if ((str1 != null) && (str2 != null))
         {
            string temp1Str = GetDelimitedText(str1, delim);
            string temp2Str = GetDelimitedText(str2, delim);
            result = string.Compare(temp1Str, temp2Str, ignoringCase) == 0;
         }
         else
         {
            // One of the strings is null.
            
            result = false;
         }
         
         return result;
      }
   
      /// <summary>
      /// Returns the next token in source starting at charPos that consists of 
      /// characters found in validChars. Note that charPos is zero-based.
      /// </summary>
      protected static string ScanToken(string source, ref int charPos, string validChars)
      {
         string token = string.Empty;
         int len = source.Length;
   
         while ((charPos < len) && !validChars.Contains(source[charPos].ToString()))  
         {
            charPos++;
         }
   
         while ((charPos < len) && validChars.Contains(source[charPos].ToString()))
         {
            token += source[charPos];
            charPos++;
         }
         
         return token;
      }

      /// <summary>
      /// Returns the next token in source starting at charPos thats delimited 
      /// by characters found in delims. Note that charPos is zero-based.
      /// </summary>
      protected static string ScanDelimitedToken(string source, ref int charPos, string delims) 
      {
         string token = string.Empty;
         int len = source.Length;
   
         while ((charPos < len) && delims.Contains(source[charPos].ToString()))  
         {
            charPos++;
         }
   
         while ((charPos < len) && !delims.Contains(source[charPos].ToString()))
         {
            token += source[charPos];
            charPos++;
         }
         
         return token;
      }

      /// <summary>
      /// Returns the string representation of the integer value found in source 
      /// starting at charPos or an empty string if no value was found. Note that 
      /// charPos is zero-based.
      /// </summary>
      protected static string ScanInteger(string source, ref int charPos)
      {
         string result = string.Empty;
         int len = source.Length;
   
         while ((charPos < len) && Char.IsWhiteSpace(source[charPos]))  
         {
            charPos++;
         }
   
         while ((charPos < len) && Char.IsDigit(source[charPos]))
         {
            result += source[charPos];
            charPos++;
         }
         
         return result;
      }

      /// <summary>
      /// Returns the next character in theStr or \0 if no more characters. 
      /// </summary>
      private static char GetNextChar(string theStr, ref int charPos)
      {
         char ch;

         if (charPos < theStr.Length)
         {
            ch = theStr[charPos];
            charPos++;
         }
         else
         {
            ch = '\0';
         }
         
         return ch;
      }

      /// <summary>
      /// Returns the string representation of the decimal value found in source 
      /// starting at charPos or an empty string if no value was found. Note that 
      /// charPos is zero-based.
      /// </summary>
      protected static string ScanDecimal(string source, ref int charPos)
      {
         // TODO: Doesn't .NET already do what this function does?  If not, add note to summary above.  
         
         string result = string.Empty;
         bool done = false;
         char ch;
         int state = 1;
         
         while (!done)
         {
            switch (state)
            {
               case 1:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 

                     if (!Char.IsWhiteSpace(ch))
                     {   
                        if ((ch == '-') || (ch == '+'))
                        {
                           result += ch;
                           state = 2;
                        }
                        else
                        {
                           if (Char.IsDigit(ch))
                           {
                              result += ch;
                              state = 3;
                           }
                           else
                           {
                              // Termination of an invalid token. 
                              
                              done = true;
                           }
                        }
                     }
                  }
                  else
                  {
                     // Termination of an invalid token.  

                     done = true;
                  }
                  
                  break;
               }
                  
               case 2:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 

                     if (Char.IsDigit(ch))
                     {
                        result += ch;
                        state = 3;
                     }
                     else
                     {
                        // Termination of an invalid token.
                        
                        done = true;
                     }
                  }
                  else
                  {
                     // Termination of an invalid token. 

                     done = true;
                  }
                  
                  break;
               }
                  
               case 3:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 
                     
                     if (Char.IsDigit(ch))
                     {
                        result += ch;
                     }
                     else
                     {
                        if (ch == '.')
                        {
                           result += ch;
                           state = 4;
                        }  
                        else
                        {
                           if (Char.ToUpper(ch) == 'E')
                           {
                              result += ch;
                              state = 6;
                           }
                           else
                           {
                              // Termination of a valid token.
                              
                              done = true;
                           }
                        }
                     }
                  }
                  else
                  {
                     // Termination of a valid token.

                     done = true;
                  }
                  
                  break;
               }
                  
               case 4:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 
                     
                     if (Char.IsDigit(ch))
                     {
                        result += ch;
                        state = 5;
                     }
                     else
                     {
                        // Termination of an invalid token.
   
                        done = true;
                     }
                  }   
                  else
                  {
                     // Termination of an invalid token.

                     done = true;
                  }
                  
                  break;
               }
                  
               case 5:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 
                     
                     if (Char.IsDigit(ch))
                     {
                        result += ch;
                     }
                     else
                     {
                        if (Char.ToUpper(ch) == 'E')
                        {
                           result += ch;
                           state = 6;
                        }
                        else
                        {
                           // Termination of an valid token.
      
                           done = true;
                        }
                     }
                  }   
                  else
                  {
                     // Termination of an valid token.

                     done = true;
                  }
                  
                  break;
               }
                  
               case 6:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 

                     if (!Char.IsWhiteSpace(ch))
                     {   
                        if ((ch == '-') || (ch == '+'))
                        {
                           result += ch;
                           state = 7;
                        }
                        else
                        {
                           if (Char.IsDigit(ch))
                           {
                              result += ch;
                              state = 8;
                           }
                           else
                           {
                              // Termination of an invalid token. 
                              
                              done = true;
                           }
                        }
                     }
                  }
                  else
                  {
                     // Termination of an invalid token.  

                     done = true;
                  }
                  
                  break;
               }
                  
               case 7:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 

                     if (Char.IsDigit(ch))
                     {
                        result += ch;
                        state = 8;
                     }
                     else
                     {
                        // Termination of an invalid token.
                        
                        done = true;
                     }
                  }
                  else
                  {
                     // Termination of an invalid token. 

                     done = true;
                  }
                  
                  break;
               }
                  
               case 8:
               {
                  ch = GetNextChar(source, ref charPos);
                  
                  if (ch != '\0')
                  {
                     // Got a character. 

                     if (Char.IsDigit(ch))
                     {
                        result += ch;
                     }
                     else
                     {
                        // Termination of a valid token.
                        
                        done = true;
                     }
                  }
                  else
                  {
                     // Termination of a valid token. 

                     done = true;
                  }
                  
                  break;
               }
            }
         }
         
         return result;
      }
   }
}
