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
using System.IO;

namespace Firefly.PipeWrench
{
   /// <summary>
   /// Implements a pipe that can be executed to process text.
   /// </summary>
   public class Pipe
   {
      private static char CommentChar = ';';
         // Commented lines must begin with this character.
      public Engine Eng { get; set; }
         // Reference to the pipe engine.
      private string Script { get; set; }
         // The pipe's script.
      private string Template { get; set; }
         // The template for parsing the pipe's arguments.
      private List<Filter> Filters { get; set; }
         // The pipe's filters.
      private CommandLine CmdLine { get; set; }
         // The pipe's command line.
      public string DebugFile { get; set; }
         // Debugging output is written here.
      public bool Debugging { get; set; }
         // Set true to write debugging output to DebugFile.
      public string Folder { get; set; }
         // The folder where the pipe was opened on disk or ""
         // if the pipe hasn't been saved to disk.
      public string Name { get; set; }
         // The pipe's file name if stored on disk.
      public bool LoggingEnabled { get; set; }
         // Set true to enable logging.
      //public PluginUtils Utils { get; set; }
         // Reference to plug-in related utilities.
      public Stack<string> DivTextStack { get; set; }
         // Stack of text files containing diverted text.
      public CompileErrors Errors { get; set; }
         // The pipe's compile errors are gathered here.

      /// <summary>
      /// Constructs a pipe from a script and references the pipe's file
      /// on disk in order to identify the pipe when generating compile
      /// errors.  If the pipe is not yet saved to disk, specify Folder
      /// and Name as "".
      /// </summary>
      public Pipe(string script, Engine eng, string folder, string name,
      bool loggingEnabled, bool replacingDelims)
      {
         if (eng != null)
         {
            Name = name;
            Eng = eng;
            LoggingEnabled = loggingEnabled;
            Debugging = false;
            DebugFile = null;
            LogWrite("Pipe.Pipe: replacingDelims: " + replacingDelims.ToString());

            if (replacingDelims)
               Script = script.Replace(Engine.PipeDelim.ToString(), System.Environment.NewLine);
            else
               Script = script;

            if (folder != string.Empty)
               Folder = System.IO.Path.GetFullPath(folder);
            else
               Folder = folder;

            DivTextStack = new Stack<string>();
            LogWriteText(Script, "Pipe.Pipe Script... ", "Line[[]]: ");
            LogWrite("Pipe.Pipe: Folder = \"" + folder + "\"");
            LogWrite("Pipe.Pipe: Name = \"" + name + "\"");
            Template = GetKeywordValue("TEMPLATE", Script);
            LogWrite("Pipe.Pipe: Template = \"" + Template + "\"");
            Filters = new List<Filter>();
            CmdLine = new CommandLine(Template, eng.Log, loggingEnabled);
            CmdLine.IgnoringExtraneousText = false;
            Errors = new CompileErrors();
         }
         else
         {
            throw new PipeWrenchEngineException("Engine parameter cannot be null");
         }
      }

      private void LogWrite(string Message)
      {
         if (LoggingEnabled)
         {
            Eng.Log.Write(Message);
         }
      }

      private void LogWriteText(string text, string heading, string prefix)
      {
         if (LoggingEnabled)
         {
            Eng.Log.WriteText(text, heading, prefix);
         }
      }

      /// <summary>
      /// Adds the given filter to the pipe.
      /// </summary>
      private void AddFilter(Filter TheFilter)
      {
         Filters.Add(TheFilter);
      }

      /// <summary>
      /// Returns the next character in TheStr or \0 if no more characters.
      /// </summary>
      private char GetNextChar(string TheStr, ref int CharPos, string Src, ref bool inString)
      {
         char Ch;
         string MethodName = "Pipe.GetNextChar";

         if (CharPos < TheStr.Length)
         {
            Ch = TheStr[CharPos];
            if (Ch == '\'') inString = !inString;
            LogWrite(MethodName + ": " + Src + "[" + CharPos.ToString() + "] = \"" + Ch + "\"");
            CharPos++;
         }
         else
         {
            Ch = '\0';
         }

         return Ch;
      }

      private string GetArg(int argIndex, bool inString)
      {
         string tempStr;
         string methodName = "Pipe.GetArg";

         if ((argIndex > -1) && (argIndex < CmdLine.ArgCount))
         {
            // The argument index is in range.

            tempStr = CmdLine.GetArg(argIndex).Value.ToString();

            if (CmdLine.GetArg(argIndex).Value is string)
            {
               // Quote the string:

               if (!inString) tempStr = "'" + tempStr + "'";
            }

            LogWrite(methodName + ": Args[" + argIndex.ToString() + "] = \"" + tempStr + "\"");
         }
         else
         {
            // The argument index is out of range.

            throw new PipeWrenchCompileException("Placeholder index is out of range.");
         }

         return tempStr;
      }

      /// <summary>
      /// Changes state machine's state to the given new state.
      /// </summary>
      private void ToState(ref int State, int NewState, string Source)
      {
         State = NewState;
         LogWrite(Source + ": To state " + State);
      }

      private void ReplacePlaceHolderRange(int FirstIndex, int LastIndex,
      int BegPosOfPlaceHolder, ref int CharPos, ref string CmdLine, bool inString)
      {
         // Insert the passed arguments at "CharPos" which is
         // now equal to "BegPosOfPlaceHolder":

         string ReplStr;
         bool ArgsToRepl;
         string TempStr;
         ReplStr = string.Empty;
         ArgsToRepl = false;

         for (int i = FirstIndex; i <= LastIndex; i++)
         {
            try
            {
               TempStr = GetArg(i, inString);
            }

            catch (PipeWrenchCompileException ex)
            {
               ex.Data.Add("CmdLine", CmdLine);
               ex.Data.Add("CharPos", CharPos);
               throw ex;
            }

            ReplStr += TempStr;
            if (!inString) ReplStr += ' ';

            ArgsToRepl = true;
         }

         if (ArgsToRepl)
         {
            if (!inString)
            {
               // Strip the trailing blank:

               ReplStr = ReplStr.Substring(0,ReplStr.Length-1);
            }

            // Delete the place holder:

            CmdLine = CmdLine.Remove(BegPosOfPlaceHolder, CharPos-BegPosOfPlaceHolder);
            CharPos -= CharPos-BegPosOfPlaceHolder;

            // Insert its replacement string at CharPos ("BegPosOfPlaceHolder"):

            CmdLine = CmdLine.Insert(CharPos, ReplStr);
            CharPos += ReplStr.Length;
         }
      }

      /// <summary>
      /// This routine replaces all place-holders in the given command line string.
      /// </summary>
      private string ReplacePlaceholders(string CmdLineText)
      {
         string MethodName = "Pipe.ReplacePlaceholders";
         char Ch;
         int CharPos = 0;
         int BegPosOfPlaceHolder = 0;
         int PosOfNum = 0;
         string FilterSwitchID = string.Empty;
         string PipeSwitchID = string.Empty;
         int FirstIndex = 0;
         int LastIndex = 0;
         bool Done = false;
         string TempStr;
         LogWrite(MethodName + ": Raw Cmdline = \"" + CmdLineText + "\"");
         int State = 0;
         bool inString = false;
         ToState(ref State, 10, MethodName);

         do
         {
            Ch = GetNextChar(CmdLineText, ref CharPos, "CmdLine", ref inString);

            switch (State)
            {
               case 10:

                  // Looking for opening bracket "[".

                  if (Ch == '[')
                  {
                     BegPosOfPlaceHolder = CharPos - 1;
                     ToState(ref State, 20, MethodName);
                  }
                  else
                  {
                     if (Ch == '\0')
                     {
                        Done = true;
                     }
                  }

                  break;

               case 20:

                  // Looking either for a positional index's first digit
                  // or a slash (/).

                  if (Char.IsDigit(Ch))
                  {
                     // Must be a positional placeholder.

                     PosOfNum = CharPos - 1;
                     ToState(ref State, 30, MethodName);
                  }
                  else
                  {
                     if (Ch == '/')
                     {
                        // Must be a switch placeholder.

                        ToState(ref State, 60, MethodName);
                     }
                     else
                     {
                        if (Ch == '\0')
                        {
                           Done = true;
                        }
                        else
                        {
                           // Character not expected.  Backtrack a character:

                           CharPos--;
                           ToState(ref State, 10, MethodName);
                        }
                     }
                  }

                  break;

               case 30:

                  // Looking for the index's remaining digits.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Char.IsDigit(Ch))
                     {
                        // Do nothing
                     }
                     else
                     {
                        if (Ch == ']')
                        {
                           // Located a simple positional place holder.  Get the index:

                           FirstIndex = Int32.Parse(CmdLineText.Substring(PosOfNum, CharPos-PosOfNum-1)) - 1;

                           // Get the argument:

                           try
                           {
                              TempStr = GetArg(FirstIndex, inString);
                           }

                           catch (PipeWrenchCompileException ex)
                           {
                              ex.Data.Add("CmdLine", CmdLineText);
                              ex.Data.Add("CharPos", CharPos);
                              throw ex;
                           }

                           // Delete the place holder:

                           CmdLineText = CmdLineText.Remove(BegPosOfPlaceHolder, CharPos-BegPosOfPlaceHolder);
                           CharPos -= CharPos-BegPosOfPlaceHolder;

                           // Insert the argument:

                           CmdLineText = CmdLineText.Insert(BegPosOfPlaceHolder, TempStr);
                           CharPos += TempStr.Length;
                           ToState(ref State, 10, MethodName);
                        }
                        else
                        {
                           if (Ch == '-')
                           {
                              // Matching a positional "range" placeholder.  Get the first index:

                              FirstIndex = Int32.Parse(CmdLineText.Substring(PosOfNum, CharPos-PosOfNum-1)) - 1;
                              ToState(ref State, 40, MethodName);
                           }
                           else
                           {
                              // Character not expected.  Backtrack a character:

                              CharPos--;
                              ToState(ref State, 10, MethodName);
                           }
                        }
                     }
                  }

                  break;

               case 40:

                  // Looking for last index's first digit.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Ch == ']')
                     {
                        // Located an infinite range place holder.  Set
                        // the last index to indicate an infinite range:

                        LastIndex = CmdLine.ArgCount - 1;

                        // Process the place holder:

                        ReplacePlaceHolderRange(FirstIndex, LastIndex, BegPosOfPlaceHolder,
                        ref CharPos, ref CmdLineText, inString);
                        ToState(ref State, 10, MethodName);
                     }
                     else
                     {
                        if (Char.IsDigit(Ch))
                        {
                           PosOfNum = CharPos-1;
                           ToState(ref State, 50, MethodName);
                        }
                        else
                        {
                           // Character not expected.  Backtrack a character:

                           CharPos--;
                           ToState(ref State, 10, MethodName);
                        }
                     }
                  }

                  break;

               case 50:

                  // Looking for last index's remaining digits.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Ch == ']')
                     {
                        // Located a closed range place holder.  Get the last index:

                        LastIndex = Int32.Parse(CmdLineText.Substring(PosOfNum, CharPos-PosOfNum-1)) - 1;

                        // Process the place holder:

                        ReplacePlaceHolderRange(FirstIndex, LastIndex, BegPosOfPlaceHolder,
                        ref CharPos, ref CmdLineText, inString);
                        ToState(ref State, 10, MethodName);
                     }
                     else
                     {
                        if (Char.IsDigit(Ch))
                        {
                           // Do nothing
                        }
                        else
                        {
                           // Character not expected.  Backtrack a character:

                           CharPos--;
                           ToState(ref State, 10, MethodName);
                        }
                     }
                  }

                  break;

               case 60:

                  // Looking for the filter's switch identifier.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Char.IsLetter(Ch))
                     {
                        // Got the switch ID.

                        FilterSwitchID = Ch.ToString();
                        ToState(ref State, 70, MethodName);
                     }
                     else
                     {
                        // Character not expected.  Backtrack a character:

                        CharPos--;
                        ToState(ref State, 10, MethodName);
                     }
                  }

                  break;

               case 70:

                  // Looking for an equal sign character.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Ch == '=')
                     {
                        // Got the equal sign character.

                        ToState(ref State, 80, MethodName);
                     }
                     else
                     {
                        // Character not expected.  Backtrack a character:

                        CharPos--;
                        ToState(ref State, 10, MethodName);
                     }
                  }

                  break;

               case 80:

                  // Looking for either a pipe's switch identifier or the
                  // first digit of a positional parameter index (a number).

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Char.IsLetter(Ch))
                     {
                        // Got the switch ID.

                        PipeSwitchID = Ch.ToString();
                        ToState(ref State, 90, MethodName);
                     }
                     else
                     {
                        if (Char.IsDigit(Ch))
                        {
                           // Got the first digit of a positional index.

                           PosOfNum = CharPos - 1;
                           ToState(ref State, 100, MethodName);
                        }
                        else
                        {
                           // Character not expected.  Backtrack a character:

                           CharPos--;
                           ToState(ref State, 10, MethodName);
                        }
                     }
                  }

                  break;

               case 90:

                  // Looking for closing bracket character.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Ch == ']')
                     {
                        // Located a switch placeholder mapped to a switch parameter.
                        // Delete the place holder:

                        CmdLineText = CmdLineText.Remove(BegPosOfPlaceHolder, CharPos-BegPosOfPlaceHolder);
                        CharPos -= CharPos - BegPosOfPlaceHolder;

                        int i = CmdLine.GetSwitchIndex("/" + PipeSwitchID);

                        if (i > -1)
                        {
                           // The switch was passed to the pipe.

                           TempStr = string.Empty;

                           if (CmdLine.GetSwitch(i).Value != null)
                           {
                              // Is numeric or string switch.  Insert the
                              // switch's value into the command line:

                              TempStr = CmdLine.GetSwitch(i).Value.ToString();

                              if (CmdLine.GetSwitch(i).Value is string)
                              {
                                 // Quote the string:

                                 TempStr = "'" + TempStr + "'";
                              }
                           }

                           // Insert the replacement text:

                           CmdLineText = CmdLineText.Insert(BegPosOfPlaceHolder, "/" + FilterSwitchID + TempStr);
                           CharPos += TempStr.Length + 2;
                        }

                        ToState(ref State, 10, MethodName);
                     }
                     else
                     {
                        // Character not expected.  Backtrack a character:

                        CharPos--;
                        ToState(ref State, 10, MethodName);
                     }
                  }

                  break;

               case 100:

                  // Looking for the remaining digits of a positional index.

                  if (Ch == '\0')
                  {
                     Done = true;
                  }
                  else
                  {
                     if (Char.IsDigit(Ch))
                     {
                        // Do nothing
                     }
                     else
                     {
                        if (Ch == ']')
                        {
                           // Located a switch placeholder mapped to
                           // a positional parameter.  Get the index:

                           FirstIndex = Int32.Parse(CmdLineText.Substring(PosOfNum, CharPos-PosOfNum-1)) - 1;

                           // Get the argument:

                           try
                           {
                              TempStr = GetArg(FirstIndex, inString);
                           }

                           catch (PipeWrenchCompileException ex)
                           {
                              ex.Data.Add("CmdLine", CmdLineText);
                              ex.Data.Add("CharPos", CharPos);
                              throw ex;
                           }

                           // Delete the place holder:

                           CmdLineText = CmdLineText.Remove(BegPosOfPlaceHolder, CharPos-BegPosOfPlaceHolder);
                           CharPos -= CharPos-BegPosOfPlaceHolder;

                           // Insert the argument:

                           CmdLineText = CmdLineText.Insert(BegPosOfPlaceHolder, "/" + FilterSwitchID + TempStr);
                           CharPos += TempStr.Length;
                           ToState(ref State, 10, MethodName);
                        }
                        else
                        {
                           // Character not expected.  Backtrack a character:

                           CharPos--;
                           ToState(ref State, 10, MethodName);
                        }
                     }
                  }

                  break;
            }
         }
         while (!Done);

         LogWrite(MethodName + ": Translated Cmdline = \"" + CmdLineText + "\"");
         return CmdLineText;
      }

      /// <summary>
      /// Parses a filter's name from its command line.
      /// </summary>
      private string ParseFilterName(string CmdLine, out int CmdLinePtr)
      {
         string MethodName = "Pipe.ParseFilterName";
         CmdLinePtr = 0;

         // Scan over any initial whitespace:

         while ((CmdLinePtr < CmdLine.Length) && ((CmdLine[CmdLinePtr] == ' ') ||
         (CmdLine[CmdLinePtr] == '\t')))
         {
            CmdLinePtr++;
         }

         // Scan the filter name:

         string FilterName = string.Empty;

         while ((CmdLinePtr < CmdLine.Length) && (CmdLine[CmdLinePtr] != ' ') && (CmdLine[CmdLinePtr] != '\t'))
         {
            FilterName += CmdLine[CmdLinePtr];
            CmdLinePtr++;
         }

         LogWrite(MethodName + ": FilterName = \"" + FilterName + "\"");
         return FilterName;
      }

      /// <summary>
      /// Compiles the pipe given its command line.  Normally, InitialCharPos is set to 0 unless
      /// the CALL filter is compiling a pipe in which case the CALL filter's original command
      /// line is passed in and InitialCharPos is passed a value > 0 so that command line parsing
      /// can begin where the CALL filter's command line parsing left off.
      /// </summary>
      public void Compile(string PipeArgs, int InitialCharPos, string ParentSource, int ParentLineNo)
      {
         int CmdLinePtr;
         string MethodName = "Pipe.Compile";
         int LineNo = 0;
         string TempStr;

         LogWrite(MethodName + ": begin");

         if (this.Folder != string.Empty)
         {
            // Set the currently logged folder to this pipe's folder:

            Directory.SetCurrentDirectory(this.Folder);
         }

         CmdLine.Text = PipeArgs;

         try
         {
            // Parse the pipe's arguments (note: a pipe's string
            // arguments should never be interpreted, hence the
            // xlatStrings parameter is passed as false):

            CmdLine.Parse(InitialCharPos, false);

            // Process each script line:

            string[] ScriptLines = Script.Split(new string[] { System.Environment.NewLine },
            StringSplitOptions.None);

            foreach (string ScriptLn in ScriptLines)
            {
               LineNo++;
               LogWrite(MethodName + ": Raw Script Line = \"" + ScriptLn + "\"");
               TempStr = ScriptLn.Trim();

               if ((TempStr != String.Empty) && (TempStr[0] != CommentChar))
               {
                  // The line isn't blank or a comment.

                  string ScriptLine = ScriptLn;

                  if (Template != string.Empty)
                  {
                     // Replace all placeholders in the line
                     // with their respective pipe arguments:

                     try
                     {
                        ScriptLine = ReplacePlaceholders(ScriptLine);
                     }

                     catch (PipeWrenchCompileException ex)
                     {
                        ex.Data.Add("Source", this.Name);
                        ex.Data.Add("LineNo", LineNo);
                        Errors.Add(ex);
                     }
                  }

                  // Get the filter's name:

                  string FilterName = ParseFilterName(ScriptLine, out CmdLinePtr);

                  // Determine if the filter is registered:

                  int i = Eng.CmdSpecs.IndexOf(FilterName);

                  if (i > -1)
                  {
                     // The filter is registered.

                     string TypeName = Eng.CmdSpecs[i].TypeName;
                     LogWrite(MethodName + ": Filter TypeName: \"" + TypeName + "\"");
                     string AssyPath = Eng.CmdSpecs[i].AssyPath;
                     LogWrite(MethodName + ": Filter AssyPath: \"" + AssyPath + "\"");

                     Filter TheFilter = new Filter(FilterName, TypeName, AssyPath, this, LineNo);

                     try
                     {
                        // "Compile" the filter:

                        TheFilter.Compile(ScriptLine, CmdLinePtr);
                     }

                     catch (PipeWrenchCompileException ex)
                     {
                        ex.Data.Add("Source", this.Name);
                        ex.Data.Add("LineNo", LineNo);
                        Errors.Add(ex);
                     }

                     // Add the filter container to the pipe:

                     AddFilter(TheFilter);
                  }
                  else
                  {
                     // The filter is not registered.

                     PipeWrenchCompileException ex = new PipeWrenchCompileException("Filter " +
                     FilterName + " is unknown.");
                     ex.Data.Add("CmdLine", ScriptLine);
                     ex.Data.Add("CharPos", CmdLinePtr);
                     ex.Data.Add("LineNo", LineNo);
                     ex.Data.Add("Source", this.Name);
                     Errors.Add(ex);
                  }
               }
            }
         }

         catch (PipeWrenchCompileException ex)
         {
            ex.Data.Add("Source", ParentSource);
            ex.Data.Add("LineNo", ParentLineNo);
            Errors.Add(ex);
         }

         LogWrite(MethodName + ": end");
      }

      /// <summary>
      /// This routine returns the "value" for the given
      /// keyword (tag) in the pipe header where the pipe
      /// is provided by TheList.
      /// </summary>
      private string GetKeywordValue(string Keyword, string Script)
      {
         int i = 0;
         string Line;
         int ThePos;
         string Result = string.Empty;
         bool Done = false;
         string MethodName = "Pipe.GetKeywordValue";

         string[] TheList = Script.Split(new string[] { System.Environment.NewLine },
         StringSplitOptions.None);

         LogWrite(MethodName + ": Keyword = " + Keyword);
         LogWrite(MethodName + ": TheList.Length = " + TheList.Length);

         while ((i < TheList.Length) && (!Done))
         {
            Line = TheList[i].Trim();
            LogWrite(MethodName + ": Line[" + i.ToString() + "] = \"" + TheList[i] + "\"");
            bool LineIsCommented = (Line.Length > 0) && (Line[0] == ';');

            if (LineIsCommented)
            {
               ThePos = Line.ToUpper().IndexOf(Keyword.ToUpper() + ':');

               if (ThePos > -1)
               {
                  Result = Line.Substring(ThePos + Keyword.Length + 1).Trim();
                  Done = true;
               }
            }
            else
            {
               Done = true;
            }

            i++;
         }

         return Result;
      }

      /// <summary>
      /// Executes the pipe against the input text and returns the edited (output) text.
      /// </summary>
      public void Execute(ref string inTextFile, ref string outTextFile)
      {
         LogWrite("Pipe.Execute: begin");
         LogWrite("Pipe.Execute: inTextFile = " + inTextFile);
         LogWrite("Pipe.Execute: outTextFile = " + outTextFile);

         if (Filters.Count > 0)
         {
            // The pipe has filters.  Execute the pipe:

            int i = 1;

            foreach (Filter filter in Filters)
            {
               // Execute the filter:

               filter.Execute(ref inTextFile, ref outTextFile);

               if (i < Filters.Count)
               {
                  // Not the last filter in the pipe.  Swap the text objects:

                  string temp = inTextFile;
                  inTextFile = outTextFile;
                  outTextFile = temp;
               }

               i++;
            }
         }
         else
         {
            // Executing a pipe that has no filters.  Transfer
            // the input text to the output unchanged:

            string temp = inTextFile;
            inTextFile = outTextFile;
            outTextFile = temp;
         }

         LogWrite("Pipe.Execute: end");
      }

      /// <summary>
      /// Executes the pipe against the contents of an existing text file
      /// (without affecting it) and outputs the results to a second text file.
      /// </summary>
      public void Execute(string inTextFile, string outTextFile)
      {
         LogWrite("Pipe.Execute: BEGIN");
         LogWrite("Pipe.Execute: inTextFile = " + inTextFile);
         LogWrite("Pipe.Execute: outTextFile = " + outTextFile);

         if (File.Exists(inTextFile))
         {
            // Set up the temp files:

            string inText = Path.GetTempFileName();
            File.Copy(inTextFile, inText, true);
            string outText = Path.GetTempFileName();

            // Execute the pipe:

            this.Execute(ref inText, ref outText);

            // Copy the edited text to the output text file:

            File.Copy(outText, outTextFile, true);

            // Delete the two temp files:

            File.Delete(inText);
            File.Delete(outText);
         }
         else
         {
            throw new PipeWrenchEngineException("Could not open input text file."); ///
         }

         LogWrite("Pipe.Execute: END");
      }
   }
}
