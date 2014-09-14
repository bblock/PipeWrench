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

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Implements a command line and its associated parser.
   /// </summary>
   public class CommandLine : ICommandLine
   {
      public string Text { get; set; }
         // The command line text.
      public string Template { get; set; }
         // The template that determines command line parsing.
      private List<IArgument> Args { get; set; }
         // The parsed command line arguments.
      private List<Switch> Switches { get; set; }
         // The parsed command line switches.
      public bool IgnoringExtraneousText { get; set; }
         // Determines whether extraneous text is allowed
         // beyond that expected by the template.
      public Logger Log { get; set; }
         // Performs logging functions.
      private int iCmdLinePtr;
         // Pointer to current character position during parsing.
      private bool LoggingEnabled;
         // set true to enable logging.

      /// <summary>
      /// Constructor.
      /// </summary>
      public CommandLine(string Template, Logger Log, bool LoggingEnabled)
      {
         this.Text = null;
         this.Template = Template;
         Args = new List<IArgument>();
         Switches = new List<Switch>();
         IgnoringExtraneousText = false;
         this.Log = Log;
         this.LoggingEnabled = LoggingEnabled;
      }

      public void LogWrite(string message)
      {
         if (LoggingEnabled)
         {
            Log.Write(message);
         }
      }

      public void LogWriteText(string text, string heading, string prefix)
      {
         if (LoggingEnabled)
         {
            Log.WriteText(text, heading, prefix);
         }
      }

      public int CmdLinePtr
      {
         get { return iCmdLinePtr; }
         set { iCmdLinePtr = value; }
      }

      public int ArgCount
      {
         get { return Args.Count; }
      }

      /// <summary>
      /// Returns a switch's index.
      /// </summary>
      public int GetSwitchIndex(string iD)
      {
         int i = -1;
         int result = -1;

         foreach (Switch sw in Switches)
         {
            i++;
            if (sw.ID.ToUpper() == iD.ToUpper())
            {
               result = i;
               break;
            }
         }

         return result;
      }

      /// <summary>
      /// Returns a boolean switch's value.
      /// </summary>
      public bool GetBooleanSwitch(string iD)
      {
         return (GetSwitchIndex(iD) > -1);
      }

      /// <summary>
      /// Returns an integer switch's value.
      /// </summary>
      public int GetIntSwitch(string iD, int defaultValue)
      {
         int Result;
         int i = GetSwitchIndex(iD);

         if (i > -1)
         {
            Result = (int) Switches[i].Value;
         }
         else
         {
            Result = defaultValue;
         }

         return Result;
      }

      /// <summary>
      /// Returns a switch's command line position.
      /// </summary>
      public int GetSwitchPos(string iD)
      {
         int i = GetSwitchIndex(iD);

         if (i > -1)
         {
            return Switches[i].CharPos;
         }
         else
         {
            return 0;
         }
      }

      /// <summary>
      /// Returns a string switch's value.
      /// </summary>
      public string GetStrSwitch(string iD, string defaultValue)
      {
         string Result;
         int i = GetSwitchIndex(iD);

         if (i > -1)
         {
            Result = (string) Switches[i].Value;
         }
         else
         {
            Result = defaultValue;
         }

         return Result;
      }

      /// <summary>
      /// Returns the command line argument indexed by the given 0-based index.
      /// </summary>
      public IArgument GetArg(int argIndex)
      {
         return Args[argIndex];
      }

      /// <summary>
      /// Returns the command line switch indexed by the given 0-based index.
      /// </summary>
      public Switch GetSwitch(int switchIndex)
      {
         return Switches[switchIndex];
      }

      /// <summary>
      /// Returns parser error formatted for display.
      /// </summary>
      public string FormatParserError(string CmdLine, int CmdLinePtr, string Message)
      {
         return CmdLine + System.Environment.NewLine + "^".PadLeft(CmdLinePtr) +
         System.Environment.NewLine + "   " + Message  + System.Environment.NewLine +
         System.Environment.NewLine;
      }

      /// <summary>
      /// Returns the next character in TheStr or \0 if no more characters.
      /// </summary>
      private char GetNextChar(string TheStr, ref int CharPos)
      {
         char Ch;

         if (CharPos < TheStr.Length)
         {
            Ch = TheStr[CharPos];
            CharPos++;
         }
         else
         {
            Ch = '\0';
         }

         return Ch;
      }

      /// <summary>
      /// Translates any escape sequences in the given string.
      /// </summary>
      public string XlatEscapes(string theStr)
      {
         // Translate pound sign escape sequences:

         theStr = XlatBackslashes(theStr);
         return XlatPoundSigns(theStr);
      }

      private bool IsHexDigit(char Ch)
      {
         return "0123456789ABCDEF".Contains(Ch.ToString().ToUpper());
      }

      /// <summary>
      /// Changes state machine's state to the given new state.
      /// </summary>
      private void ToState(ref int State, int NewState, string Source)
      {
         State = NewState;
         LogWrite(Source + ": State = " + State.ToString());
      }

      /// <summary>
      /// This routine interprets all backslash
      /// escape sequences in the given string.
      /// </summary>
      public string XlatBackslashes(string TheStr)
      {
         string methodName = "CommandLine.XlatBackslashes";
         LogWrite(methodName + ": Start");
         LogWrite(methodName + ": Input string: " + TheStr);
         char backslashChar = '\\';
         int charPos = 0;
         string result = string.Empty;
         bool done = false;
         char ch = '\0';
         string hexStr = string.Empty;
         int state = 0;
         ToState(ref state, 1, methodName);

         // Translate the string:

         while (!done)
         {
            switch (state)
            {
               case 1:
                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character.

                     if (ch == backslashChar)
                        ToState(ref state, 2, methodName);
                     else
                        result += ch;
                  }
                  else
                  {
                     // String processed successfully.

                     done = true;
                  }

                  break;

               case 2:
                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character following a backslash.

                     switch (ch.ToString().ToUpper())
                     {
                        case "0":
                           result += '\0';
                           ToState(ref state, 1, methodName);
                           break;

                        case "A":
                           result += '\a';
                           ToState(ref state, 1, methodName);
                           break;

                        case "B":
                           result += '\b';
                           ToState(ref state, 1, methodName);
                           break;

                        case "E":

                           // This escape isn't recognized as a standard. I've added it in order
                           // to represent an end-of-line sequence regardless of what system PipeWrench
                           // is executing on.

                           result += System.Environment.NewLine;
                           ToState(ref state, 1, methodName);
                           break;

                        case "F":
                           result += '\f';
                           ToState(ref state, 1, methodName);
                           break;

                        case "N":
                           result += '\n';
                           ToState(ref state, 1, methodName);
                           break;

                        case "R":
                           result += '\r';
                           ToState(ref state, 1, methodName);
                           break;

                        case "T":
                           result += '\t';
                           ToState(ref state, 1, methodName);
                           break;

                        case "V":
                           result += '\v';
                           ToState(ref state, 1, methodName);
                           break;

                        case "\'":
                           result += '\'';
                           ToState(ref state, 1, methodName);
                           break;

                        case "\"":
                           result += '"';
                           ToState(ref state, 1, methodName);
                           break;

                        case "\\":
                           result += '\\';
                           ToState(ref state, 1, methodName);
                           break;

                        case "X":
                           ToState(ref state, 3, methodName);
                           break;

                        default:
                           result += ch;
                           ToState(ref state, 1, methodName);
                           break;
                     }
                  }
                  else
                  {
                     // Abrupt end-of-data.

                     PipeWrenchCompileException ex = new PipeWrenchCompileException(
                     "Abrupt end-of-data.");
                     ex.Data.Add("Offset", charPos-1);
                     throw ex;
                  }

                  break;

               case 3:

                  // Have "\x".

                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character.

                     if (IsHexDigit(ch))
                     {
                        // First hex digit scanned.

                        hexStr = ch.ToString();
                        ToState(ref state, 4, methodName);
                     }
                     else
                     {
                        // Character is not a hex digit.

                        PipeWrenchCompileException ex = new PipeWrenchCompileException(
                        "Hex digit expected.");
                        ex.Data.Add("Offset", charPos-1);
                        throw ex;
                     }
                  }
                  else
                  {
                     // Abrupt end-of-data.

                     PipeWrenchCompileException ex = new PipeWrenchCompileException(
                     "Abrupt end-of-data.");
                     ex.Data.Add("Offset", charPos-1);
                     throw ex;
                  }

                  break;

               case 4:

                  // Have "\xn".

                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character.

                     if (IsHexDigit(ch))
                     {
                        // 2nd hex digit scanned.  Insert the actual represented
                        // character and continue scanning the string:

                        hexStr += ch;
                        result += (char) Convert.ToInt32(hexStr, 16);
                        ToState(ref state, 1, methodName);
                     }
                     else
                     {
                        // Character is not a hex digit.

                        PipeWrenchCompileException ex = new PipeWrenchCompileException(
                        "Hex digit expected.");
                        ex.Data.Add("Offset", charPos-1);
                        throw ex;
                     }
                  }
                  else
                  {
                     // Abrupt end-of-data.

                     PipeWrenchCompileException ex = new PipeWrenchCompileException(
                     "Abrupt end-of-data.");
                     ex.Data.Add("Offset", charPos-1);
                     throw ex;
                  }

                  break;
            }
         }

         LogWrite(methodName + ": Result string: " + result);
         LogWrite(methodName + ": Finish");
         return result;
      }

      /// <summary>
      /// This routine converts all 2-digit hexadecimal numbers preceded
      /// by a pound character found in the given string into the characters
      /// represented by each three-character sequence. Two pound characters
      /// found together are interpreted as meaning a single one.
      /// </summary>
      public string XlatPoundSigns(string TheStr)
      {
         string methodName = "CommandLine.XlatPoundSigns";
         LogWrite(methodName + ": Start");
         LogWrite(methodName + ": Input string: " + TheStr);
         char keyChar = '#';
         int charPos = 0;
         string result = string.Empty;
         bool done = false;
         char ch;
         string hexStr = string.Empty;
         int state = 0;
         ToState(ref state, 1, methodName);

         // Translate the string:

         while (!done)
         {
            switch (state)
            {
               case 1:
                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character.

                     if (ch == keyChar)
                        ToState(ref state, 2, methodName);
                     else
                        result += ch;
                  }
                  else
                  {
                     // String processed successfully.

                     done = true;
                  }
                  break;

               case 2:
                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character.

                     if (IsHexDigit(ch))
                     {
                        // First hex digit scanned.

                        hexStr = ch.ToString();
                        ToState(ref state, 3, methodName);
                     }
                     else
                     {
                        if (ch == keyChar)
                        {
                           // Insert a # character:

                           result += keyChar;
                           ToState(ref state, 1, methodName);
                        }
                        else
                        {
                           // # or hex digit expected.

                           PipeWrenchCompileException ex = new PipeWrenchCompileException("# or hex digit expected.");
                           ex.Data.Add("Offset", charPos-1);
                           throw ex;
                        }
                     }
                  }
                  else
                  {
                     // Abrupt end-of-data.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("# or hex digit expected but end-of-string encountered.");
                     ex.Data.Add("Offset", charPos);
                     throw ex;
                  }

                  break;

               case 3:
                  ch = GetNextChar(TheStr, ref charPos);

                  if (ch != '\0')
                  {
                     // Got a character.

                     if (IsHexDigit(ch))
                     {
                        // 2nd hex digit scanned.  Insert
                        // the actual represented character:

                        hexStr += ch;
                        result += (char) Convert.ToInt32(hexStr, 16);
                        ToState(ref state, 1, methodName);
                     }
                     else
                     {
                        // Hex digit expected.

                        PipeWrenchCompileException ex = new PipeWrenchCompileException("Hex digit expected.");
                        ex.Data.Add("Offset", charPos-1);
                        throw ex;
                     }
                  }
                  else
                  {
                     // Abrupt end-of-data.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("Hex digit expected but end-of-string encountered.");
                     ex.Data.Add("Offset", charPos);
                     throw ex;
                  }
                  break;
            }
         }

         LogWrite(methodName + ": Result string: " + result);
         LogWrite(methodName + ": Finish");
         return result;
      }

      /// <summary>
      /// This routine searches the source string, "CmdLine" for a string
      /// delimited by single or double quotes, (specify one).  Searching begins
      /// at the given character position, "CmdLinePtr".  If a complete delimited
      /// string is not found, then "RCode" is returned non-zero, (see below).
      /// Embedded delimiters are allowed by placing two of them together within
      /// the string.
      /// </summary>
      private string GetString(string CmdLine, ref int CmdLinePtr, char QuoteChar,
      bool InterpretingQuotes)
      {
         string MethodName = "CommandLine.GetString";
         char Ch;
         string Str = string.Empty;
         bool Done = false;
         int State = 0;
         ToState(ref State, 1, MethodName);

         // Parse the string:

         do
         {
            switch (State)
            {
               case 1:
                  Ch = GetNextChar(CmdLine, ref CmdLinePtr);

                  if (Ch != '\0')
                  {
                     if (Ch == QuoteChar)
                     {
                        // Found beginning delimiter.

                        ToState(ref State, 2, MethodName);
                     }
                     else
                     {
                        if ((Ch == ' ') || (Ch == '\t'))
                        {
                           // Whitespace character.  Remain in this state.
                        }
                        else
                        {
                           // First non-whitespace character is not a delimiter.

                           PipeWrenchCompileException ex =
                           new PipeWrenchCompileException("Expected beginning of string.");
                           ex.Data.Add("CharPos", CmdLinePtr);
                           ex.Data.Add("CmdLine", CmdLine);
                           throw ex;
                        }
                     }
                  }
                  else
                  {
                     // Reached End-Of-String.  Beginning delimiter not found.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("Expected string not found.");
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("CmdLine", CmdLine);
                     throw ex;
                  }
                  break;

               case 2:
                  Ch = GetNextChar(CmdLine, ref CmdLinePtr);

                  if (Ch != '\0')
                  {
                     if (Ch == QuoteChar)
                     {
                        // Found another delimiter.

                        ToState(ref State, 3, MethodName);
                     }
                     else
                     {
                        // Remain in this state.

                        Str += Ch;
                     }
                  }
                  else
                  {
                     // Reached End-Of-String.  Ending delimiter not found.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("String is not terminated with a quote.");
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("CmdLine", CmdLine);
                     throw ex;
                  }
                  break;

               case 3:
                  Ch = GetNextChar(CmdLine, ref CmdLinePtr);

                  if (Ch != '\0')
                  {
                     if (Ch == QuoteChar)
                     {
                        // This delimiter is part of the
                        // string.  Add it to the string:

                        Str += Ch;
                        if (!InterpretingQuotes) Str += Ch;
                        ToState(ref State, 2, MethodName);
                     }
                     else
                     {
                        // Next char is not an ajacent delimiter;
                        // therefore, we're done.  Back up:

                        CmdLinePtr--;
                        Done = true;
                     }
                  }
                  else
                  {
                     // Done.  Reached End-Of-String.

                     Done = true;
                  }
                  break;
            }
         }
         while (!Done);

         return Str;
      }

      /// <summary>
      /// Parses a switch.
      /// </summary>
      private void ParseSwitch(string SwitchID, ref int CmdLinePtr, ref int TemplPtr, bool xlatStrings)
      {
         string MethodName = "CommandLine.ParseSwitch";
         LogWrite(MethodName + ": SwitchID = " + SwitchID);
         char SwitchType;
         int i = Template.ToUpper().IndexOf(SwitchID.ToUpper());

         if (i > -1)
         {
            // The switch is expected.  Set the template
            // pointer to point to the switch's type:

            TemplPtr = i + 2;

            // Determine the type of switch:

            if (TemplPtr < Template.Length)
            {
               // Not yet to end-of-template.

               if ((Template[TemplPtr] == 'n') || (Template[TemplPtr] == 's'))
               {
                  SwitchType = Template[TemplPtr];
               }
               else
               {
                  SwitchType = 'b';
               }
            }
            else
            {
               // Reached the end of the template.

               SwitchType = 'b';
            }

            // Parse the switch's setting: }

            LogWrite(MethodName + ": SwitchType = \"" + SwitchType + "\"");
            switch (SwitchType)
            {
               case 'n':
                  if ((CmdLinePtr < this.Text.Length) && ("0123456789-+".IndexOf(this.Text[CmdLinePtr]) != -1))
                  {
                     // The first character is part of an integer.  Parse it:

                     string Token = "";

                     while ((CmdLinePtr < this.Text.Length) && (this.Text[CmdLinePtr] != ' ') &&
                     (this.Text[CmdLinePtr] != '\t'))
                     {
                        Token += this.Text[CmdLinePtr];
                        CmdLinePtr++;
                     }

                     try
                     {
                        // Convert the token to an integer value:

                        int Value = Convert.ToInt32(Token);
                        Switch TheSwitch = new Switch(SwitchID, Value, CmdLinePtr);
                        LogWrite(MethodName + ": IntSwitch value = " + Value.ToString());
                        Switches.Add(TheSwitch);
                     }
                     catch (Exception)
                     {
                        PipeWrenchCompileException ex =
                        new PipeWrenchCompileException("Integer value is invalid");
                        ex.Data.Add("CharPos", CmdLinePtr);
                        ex.Data.Add("CmdLine", this.Text);
                        throw ex;
                     }
                  }
                  else
                  {
                     // The first character is not part of an integer.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("Expected an integer immediately following this switch.");
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("CmdLine", this.Text);
                     throw ex;
                  }

                  break;

               case 's':
                  if ((CmdLinePtr < this.Text.Length) && (this.Text[CmdLinePtr] == '\''))
                  {
                     // The first character is a quote.  Parse the string:

                     string Value;

                     if (xlatStrings)
                     {
                        string tempStr = GetString(this.Text, ref CmdLinePtr, '\'', true);

                        try
                        {
                           Value = XlatEscapes(tempStr);
                        }
                        catch (PipeWrenchCompileException ex)
                        {
                           ex.Data.Add("CmdLine", this.Text);
                           ex.Data["CharPos"] = CmdLinePtr - tempStr.Length + (int) ex.Data["Offset"];
                           throw ex;
                        }
                     }
                     else
                     {
                        Value = GetString(this.Text, ref CmdLinePtr, '\'', false);
                     }

                     Switch TheSwitch = new Switch(SwitchID, Value, CmdLinePtr);
                     LogWrite(MethodName + ": StrSwitch value = " + Value);
                     Switches.Add(TheSwitch);
                  }
                  else
                  {
                     // The first character is not a quote.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("Expected a string immediately following this switch.");
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("CmdLine", this.Text);
                     throw ex;
                  }

                  break;

               case 'b':
                  bool SwitchIsValid = (CmdLinePtr == this.Text.Length) ||
                  (this.Text[CmdLinePtr] == ' ') || (this.Text[CmdLinePtr] == '\t');

                  if (SwitchIsValid)
                  {
                     Switch TheSwitch = new Switch(SwitchID, null, CmdLinePtr);
                     LogWrite(MethodName + ": BoolSwitch value = true");
                     Switches.Add(TheSwitch);
                  }
                  else
                  {
                     // Switch is invalid.

                     PipeWrenchCompileException ex = new PipeWrenchCompileException("Boolean switch is invalid.");
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("CmdLine", this.Text);
                     throw ex;
                  }

                  break;
            }
         }
         else
         {
            // The switch was not expected.

            PipeWrenchCompileException ex = new PipeWrenchCompileException("Switch is unexpected.");
            ex.Data.Add("CharPos", CmdLinePtr);
            ex.Data.Add("CmdLine", this.Text);
            throw ex;
         }
      }

      /// <summary>
      /// Parses the command line arguments.
      /// </summary>
      public void Parse(int initPos, bool xlatStrings)
      {
         string MethodName = "CommandLine.Parse";
         bool Done = false;
         char Ch;
         bool ParamIsOptional = false;
         int TemplPtr = 0;
         CmdLinePtr = initPos;
         LogWrite(MethodName + ": Parsing \"" + this.Text + "\" using template, \"" + Template +
         "\" starting at CharPos " + initPos.ToString() + ".");
         LogWrite(MethodName + ": IgnoringExtraneousText = " + IgnoringExtraneousText);
         LogWrite(MethodName + ": ParamIsOptional = " + ParamIsOptional);
         int State = 0;
         ToState(ref State, 1, MethodName);

         while (!Done)
         {
            switch (State)
            {
               case 1:
                  Ch = GetNextChar(Template, ref TemplPtr);

                  switch (Ch)
                  {
                     case 'n':
                        ToState(ref State, 2, MethodName);
                        break;

                     case 's':
                        ToState(ref State, 3, MethodName);
                        break;

                     case '[':
                        ParamIsOptional = true;
                        LogWrite(MethodName + ": ParamIsOptional = " + ParamIsOptional);
                        break;

                     case ']':

                        // do nothing

                        break;

                     case '.':
                        ToState(ref State, 4, MethodName);
                        break;

                     case '/':

                        // switch encountered

                        ToState(ref State, 7, MethodName);
                        break;

                     case '\0':

                        // end of template encountered

                        ToState(ref State, 5, MethodName);
                        break;

                     case ' ':

                        // Ignore and keep scanning...

                        break;

                     case '\t':

                        // Ignore and keep scanning...

                        break;

                     default:

                        // Invalid template character.

                        throw new PipeWrenchEngineException("Template is invalid.");
                  }
                  break;

               case 2:

                  // An integer is expected according to the template.

                  Ch = GetNextChar(this.Text, ref iCmdLinePtr);

                  if ("0123456789-+".IndexOf(Ch) != -1)
                  {
                     // Found an integer.  Back up a character and parse it:

                     CmdLinePtr--;
                     string Token = "";

                     while ((CmdLinePtr < this.Text.Length) && (this.Text[CmdLinePtr] != ' ') &&
                     (this.Text[CmdLinePtr] != '\t'))
                     {
                        Token += this.Text[CmdLinePtr];
                        CmdLinePtr++;
                     }

                     try
                     {
                        // Convert the token to an integer value:

                        int Value = Convert.ToInt32(Token);
                        LogWrite(MethodName + ": IntArg value = " + Value.ToString());
                        Argument arg = new Argument(Value, CmdLinePtr);
                        Args.Add(arg);
                     }
                     catch (Exception)
                     {
                        PipeWrenchCompileException ex = new PipeWrenchCompileException("Integer value is invalid.");
                        ex.Data.Add("CharPos", CmdLinePtr);
                        ex.Data.Add("CmdLine", this.Text);
                        throw ex;
                     }

                     if (ParamIsOptional)
                     {
                        // Found first parameter in optional
                        // sequence.  Now all other parameters
                        // to end of sequence are required.

                        ParamIsOptional = false;
                        LogWrite(MethodName + ": ParamIsOptional = " + ParamIsOptional);
                     }

                     ToState(ref State, 1, MethodName);
                  }
                  else
                  {
                     if (Ch == '\'')
                     {
                        // String encountered instead of an integer.

                        PipeWrenchCompileException ex = new PipeWrenchCompileException("Integer expected for this parameter.");
                        ex.Data.Add("CharPos", CmdLinePtr);
                        ex.Data.Add("CmdLine", this.Text);
                        throw ex;
                     }
                     else
                     {
                        if (Ch == '/')
                        {
                           // Switch encountered instead of an integer.

                           if (ParamIsOptional)
                           {
                              ToState(ref State, 6, MethodName);
                           }
                           else
                           {
                              // Expected integer value was not found.

                              PipeWrenchCompileException ex =
                              new PipeWrenchCompileException("Expected an integer parameter but switch was encountered.");
                              ex.Data.Add("CharPos", CmdLinePtr);
                              ex.Data.Add("CmdLine", this.Text);
                              throw ex;
                           }
                        }
                        else
                        {
                           if (Ch == '\0')
                           {
                              // Reached the end of the command line.

                              if (ParamIsOptional)
                              {
                                 Done = true;
                              }
                              else
                              {
                                 // Expected integer value was not found.

                                 PipeWrenchCompileException ex =
                                 new PipeWrenchCompileException("Expected an integer parameter but end-of-line was encountered.");
                                 ex.Data.Add("CharPos", CmdLinePtr);
                                 ex.Data.Add("CmdLine", this.Text);
                                 throw ex;
                              }
                           }
                           else
                           {
                              if ((Ch == ' ') || (Ch == '\t'))
                              {
                                 // It's a whitespace character.  Just ignore it and keep scanning...
                              }
                              else
                              {
                                 // Expected integer value was not found.

                                 PipeWrenchCompileException ex =
                                 new PipeWrenchCompileException("Integer expected for this parameter.");
                                 ex.Data.Add("CharPos", CmdLinePtr);
                                 ex.Data.Add("CmdLine", this.Text);
                                 throw ex;
                              }
                           }
                        }
                     }
                  }

                  break;

               case 3:

                  // A string is expected according to the template.

                  Ch = GetNextChar(this.Text, ref iCmdLinePtr);

                  if (Ch == '\'')
                  {
                     // Found a string.  Back up and parse it:

                     CmdLinePtr--;
                     string Token;

                     if (xlatStrings)
                     {
                        string tempStr = GetString(this.Text, ref iCmdLinePtr, '\'', true);

                        try
                        {
                           Token = XlatEscapes(tempStr);
                        }

                        catch (PipeWrenchCompileException ex)
                        {
                           ex.Data.Add("CmdLine", this.Text);
                           ex.Data["CharPos"] = CmdLinePtr - tempStr.Length + (int) ex.Data["Offset"];
                           throw ex;
                        }
                     }
                     else
                     {
                        Token = GetString(this.Text, ref iCmdLinePtr, '\'', false);
                     }

                     Argument arg = new Argument(Token, iCmdLinePtr);
                     Args.Add(arg);

                     if (ParamIsOptional)
                     {
                        // Found first parameter in optional sequence.  Now all other
                        // parameters to end of sequence are required.

                        ParamIsOptional = false;
                        LogWrite(MethodName + ": ParamIsOptional = " + ParamIsOptional);
                     }

                     ToState(ref State, 1, MethodName);
                  }
                  else
                  {
                     if ("0123456789-+".IndexOf(Ch) != -1)
                     {
                        // An integer was encountered.

                        PipeWrenchCompileException ex =
                        new PipeWrenchCompileException("String expected for this parameter.");
                        ex.Data.Add("CharPos", CmdLinePtr);
                        ex.Data.Add("CmdLine", this.Text);
                        throw ex;
                     }
                     else
                     {
                        if (Ch == '/')
                        {
                           // A switch was encountered.

                           if (ParamIsOptional)
                           {
                              ToState(ref State, 6, MethodName);
                           }
                           else
                           {
                              PipeWrenchCompileException ex =
                              new PipeWrenchCompileException("Expected a string parameter but switch was encountered.");
                              ex.Data.Add("CharPos", CmdLinePtr);
                              ex.Data.Add("CmdLine", this.Text);
                              throw ex;
                           }
                        }
                        else
                        {
                           if (Ch == '\0')
                           {
                              // Reached the end of the command line.

                              if (ParamIsOptional)
                              {
                                 Done = true;
                              }
                              else
                              {
                                 // Expected string value was not found.

                                 PipeWrenchCompileException ex =
                                 new PipeWrenchCompileException("Expected a string parameter but end-of-line was encountered.");
                                 ex.Data.Add("CharPos", CmdLinePtr);
                                 ex.Data.Add("CmdLine", this.Text);
                                 throw ex;
                              }
                           }
                           else
                           {
                              if ((Ch == ' ') || (Ch == '\t'))
                              {
                                 // It's a whitespace character.  Just ignore it and keep scanning...
                              }
                              else
                              {
                                 // Expected string value was not found.

                                 PipeWrenchCompileException ex =
                                 new PipeWrenchCompileException("String expected for this parameter.");
                                 ex.Data.Add("CharPos", CmdLinePtr);
                                 ex.Data.Add("CmdLine", this.Text);
                                 throw ex;
                              }
                           }
                        }
                     }
                  }

                  break;

               case 4:

                  // Backtracking through optional sequence.

                  int savedTemplPtr = TemplPtr;

                  try
                  {
                     while (Template[TemplPtr] != '[')
                     {
                        TemplPtr--;
                     }
                  }

                  catch (Exception)
                  {
                     PipeWrenchTemplateException ex =
                     new PipeWrenchTemplateException(
                     "Template error: Repeating group (...) is only allowed inside \"[]\".");
                     ex.Data.Add("CharPos", savedTemplPtr);
                     ex.Data.Add("Template", Template);
                     throw ex;
                  }

                  TemplPtr++;
                  ParamIsOptional = true;
                  LogWrite(MethodName + ": ParamIsOptional = " + ParamIsOptional);
                  ToState(ref State, 1, MethodName);
                  break;

               case 5:

                  // End of template encountered.

                  if (!IgnoringExtraneousText)
                  {
                     // Be sure that remainder of command line is clear
                     // as additional parameters, (including switches)
                     // are not expected:

                     Ch = GetNextChar(this.Text, ref iCmdLinePtr);

                     if (Ch == '\0')
                     {
                        Done = true;
                     }
                     else
                     {
                        if ((Ch == ' ') || (Ch == '\t'))
                        {
                           // It's a whitespace character.  Just ignore it and keep scanning...
                        }
                        else
                        {
                           PipeWrenchCompileException ex =
                           new PipeWrenchCompileException("Parameter or switch is extraneous.");
                           ex.Data.Add("CharPos", CmdLinePtr);
                           ex.Data.Add("CmdLine", this.Text);
                           throw ex;
                        }
                     }
                  }
                  else
                  {
                     Done = true;
                  }

                  break;

               case 6:

                  // Switch encountered.

                  if (CmdLinePtr < this.Text.Length)
                  {
                     // Not yet to the end of the command line.

                     string SwitchID = "/" + Char.ToUpper(this.Text[CmdLinePtr]);
                     CmdLinePtr++;
                     ParseSwitch(SwitchID, ref iCmdLinePtr, ref TemplPtr, xlatStrings);
                     ToState(ref State, 7, MethodName);
                  }
                  else
                  {
                     // End of command line was encountered.  Switch
                     // character, (/) was found without a succeeding
                     // ID character.

                     PipeWrenchCompileException ex =
                     new PipeWrenchCompileException("Switch is incomplete.");
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("CmdLine", this.Text);
                     throw ex;
                  }

                  break;

               case 7:

                  // Locate the next switch on the command line:

                  Ch = GetNextChar(this.Text, ref iCmdLinePtr);

                  switch (Ch)
                  {
                     case '/':

                        // Found it.

                        ToState(ref State, 6, MethodName);
                        break;

                     case '\0':
                        Done = true;
                        break;

                     case ' ':

                        // Whitespace character.  Just ignore it and keep scanning...

                        break;

                     case '\t':

                        // Whitespace character.  Just ignore it and keep scanning...

                        break;

                     default:
                        PipeWrenchCompileException ex =
                        new PipeWrenchCompileException("A switch was expected.");
                        ex.Data.Add("CharPos", CmdLinePtr);
                        ex.Data.Add("CmdLine", this.Text);
                        throw ex; 
                  }

                  break;

               default:
                  throw new PipeWrenchEngineException(MethodName +
                  ": Invalid state (" + State.ToString() + ")."); 
            }
         }
      }
   }
}
