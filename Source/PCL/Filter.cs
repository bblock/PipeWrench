// 
// Pyper - automate the transformation of text using "stackable" text filters
// Copyright (C) 2011  Barry Block 
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
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   /// Implements a filter.
   /// </summary>
   public class Filter : IFilter
   {
      public string Name { get; set; }
         // The filter's name.
      public object Eng { get; set; }
         // Reference to the pipe engine.
      public ICommandLine CmdLine { get; set; }
         // The filter's command line.
      public ILogger Log { get; set; }
         // Reference to logger for use by the plugin.  This
         // is merely a reference to the engine's logger and
         // not the parent's (pipe's) logger.
      private FilterPlugin Plugin { get; set; }
         // The filter's processing core plugin.
      private Pipe Parent { get; set; }
         // The filter's parent pipe.
      private bool coreLoggingEnabled;
         // Set to true to enable logging of CORE messages.
      private Assembly AssyRef { get; set; }
         // Holds reference to loaded assembly.
      public string InText { get; set; }
         // Reference to the text being edited.
      public string OutText { get; set; }
         // Reference to the edited text.
      //public IPluginUtils Utils { get; set; }
         // Reference to common plug-in utilities.
      public Stack<string> DivTextStack { get; set; }
         // Stack of text objects containing diverted text.
      private int textLineNo;
      private StreamReader TheReader { get; set; }
      private StreamWriter TheWriter { get; set; }
      private int pipeLineNo;
         // The filter's line number in the pipe.

      /// <summary>
      /// Constructor.
      /// </summary>
      public Filter(string Name, string TypeName, string AssyPath, Pipe Parent, int pipeLineNo)
      {
         this.Parent = Parent;
         this.Name = Name;
         this.Eng = Parent.Eng;
         this.Log = ((Engine) Eng).Log;
         this.coreLoggingEnabled = Parent.LoggingEnabled;
         this.DivTextStack = Parent.DivTextStack;
         this.pipeLineNo = pipeLineNo;

         // Instantiate the plug-in type:

         AssyRef = Assembly.LoadFrom(AssyPath);
         Type type = AssyRef.GetType(TypeName);

         if (type != null)
         {
            object TheObj = Activator.CreateInstance(type, new object[] {this});

            if (TheObj is FilterPlugin)
            {
               // The type is a plugin.

               Plugin = (FilterPlugin) TheObj;
               CmdLine = new CommandLine(Plugin.Template, Parent.Eng.Log, CoreLoggingEnabled);
            }
            else
            {
               // The type isn't a plugin.

               throw new PyperEngineException("Type isn't a plugin.\nAssembly: " +
               AssyPath + "\nType: " + TypeName); ///
            }
         }
         else
         {
            // Unable to retrieve plugin type.

            throw new PyperEngineException("Plugin type not found.\nAssembly: " +
            AssyPath + "\nType: " + TypeName); ///
         }
      }

      /// <summary>
      /// Logs the given message.
      /// </summary>
      private void LogWrite(string Message)
      {
         if (CoreLoggingEnabled)
         {
            Log.Write(Message);
         }
      }

      /// <summary>
      /// Logs the given text.
      /// </summary>
      private void LogWriteText(string text, string heading, string prefix)
      {
         if (CoreLoggingEnabled)
         {
            Log.WriteText(text, heading, prefix);
         }
      }

      public int PipeLineNo
      {
         get { return pipeLineNo; }
      }

      public string Source
      {
         get
         {
            return Parent.Name;
         }
      }

      public bool CoreLoggingEnabled
      {
         get { return coreLoggingEnabled; }
      }

      public CompileErrors Errors
      {
         get { return Parent.Errors; }
      }

      public string DebugFile
      {
         get { return Parent.DebugFile; }
      }

      public bool Debugging
      {
         get { return Parent.Debugging; }
         set { Parent.Debugging = value; }
      }

      public int TextLineCount
      {
         get
         {
            int count = 0;

            StreamReader TheReader = new StreamReader(InText);

            while (!TheReader.EndOfStream)
            {
               TheReader.ReadLine();
               count++;
            }

            TheReader.Close();

            return count;
         }
      }

      public int TextLineNo
      {
         get { return textLineNo; }
      }

      /// <summary>
      /// Resets the reading of the text.
      /// </summary>
      public void Reset()
      {
         CloseRead();
         OpenRead();
      }

      //// <summary>
      /// Returns true if there is no more text to process.
      /// </summary>
      public bool EndOfText
      {
         get
         {
            return TheReader.EndOfStream;
         }
      }

      /// <summary>
      /// Returns the next line of text.
      /// </summary>
      public string ReadLine()
      {
         textLineNo++;
         return TheReader.ReadLine();
      }

      /// <summary>
      /// Writes one or more lines of text to the output with a trailing newline.
      /// </summary>
      public void WriteText(string text)
      {
         TheWriter.WriteLine(text);

         if (Debugging && (DebugFile != null) && (DebugFile != string.Empty))
         {
            File.AppendAllText(DebugFile, text + System.Environment.NewLine);
         }
      }

      /// <summary>
      /// Writes one or more lines of text to the output without a trailing newline.
      /// </summary>
      public void Write(string text)
      {
         TheWriter.Write(text);

         if (Debugging && (DebugFile != null) && (DebugFile != string.Empty))
         {
            File.AppendAllText(DebugFile, text);
         }
      }

      /// <summary>
      /// Writes all of the input text to output unchanged.
      /// </summary>
      public void WriteAllText()
      {
         string tempText = InText;
         InText = OutText;
         OutText = tempText;
      }

      /// <summary>
      /// Compiles the filter.
      /// </summary>
      public void Compile(string CmdLineText, int InitialCharPos)
      {
         Plugin.Compile(CmdLineText, InitialCharPos);
      }

      /// <summary>
      /// Executes the filter against the given text.
      /// </summary>
      public void Execute(ref string inTextFile, ref string outTextFile)
      {
         LogWrite("Filter.Execute: begin");
         LogWrite("Filter.Execute: inTextFile = " + inTextFile);
         LogWrite("Filter.Execute: outTextFile = " + outTextFile);
         LogWrite("Filter.Execute: Executing " + this.Name + "...");

         InText = inTextFile;
         OutText = outTextFile;

         if (Debugging)
         {
            string tempStr = Name.ToUpper();

            if ((tempStr != "SETDEBUGON") && (tempStr != "SETDEBUGOFF") && (tempStr != "CALL"))
            {
               File.AppendAllText(DebugFile, System.Environment.NewLine +
               this.CmdLine.Text + System.Environment.NewLine +
               System.Environment.NewLine);
            }
         }

         // Execute the filter's plug-in:

         Plugin.Execute();

         // Back-assign the file references (this is necessary as
         // any CALLed pipe will cause the references to be swapped
         // one or more times):

         inTextFile = InText;
         outTextFile = OutText;

         LogWrite("Filter.Execute: end");
      }

      private void OpenRead()
      {
         textLineNo = 0;
         TheReader = File.OpenText(InText);
      }

      private void OpenWrite()
      {
         TheWriter = File.CreateText(OutText);
      }

      private void CloseRead()
      {
         TheReader.Close();
      }

      private void CloseWrite()
      {
         TheWriter.Close();
      }

      public void Open()
      {
         OpenRead();
         OpenWrite();
      }

      public void Close()
      {
         CloseRead();
         CloseWrite();
      }
   }
}
