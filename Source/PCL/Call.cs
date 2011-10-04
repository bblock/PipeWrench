using System;
using System.IO;

namespace Firefly.Pyper
{
   /// <summary>
   ///    Calls a saved pipe file.
   /// </summary>
   public sealed class Call : FilterPlugin
   {
      private Pipe CalledPipe { get; set; }
         // The pipe that is being called.

      /// <summary>
      /// Compiles the filter.
      /// </summary>
      public override void Compile(string cmdLineText, int initialCharPos)
      {
         // Parse the filter's command line:

         LoggingEnabled = true;
         LogWrite("Call.Compile: begin");
         CmdLine.Text = cmdLineText;
         CmdLine.IgnoringExtraneousText = true;
         CmdLine.Parse(initialCharPos, true);

         // Load and compile the contents of the saved-to-disk pipe:

         string pipePath = (string) CmdLine.GetArg(0).Value;
         LogWrite("Call.Compile: Called pipe: \"" + pipePath + "\"");

         if (System.IO.File.Exists(pipePath))
         {
            // The pipe file exists.  Create a new pipe object from the pipe file's contents:

            string tempStr = System.IO.Path.GetFullPath(pipePath);
            string pipeFolder = System.IO.Path.GetDirectoryName(tempStr);
            string pipeName = System.IO.Path.GetFileName(tempStr);
            string pipeScript = System.IO.File.ReadAllText(pipePath);
            CalledPipe = new Pipe(pipeScript, (Engine) Eng, pipeFolder, pipeName,
            ((Filter) Host).CoreLoggingEnabled, false);
            CalledPipe.DebugFile = ((Filter) Host).DebugFile;
            CalledPipe.Errors = ((Filter) Host).Errors;

            // Save the current folder because Pipe.Compile() may change it:

            string savedFolder = System.Environment.CurrentDirectory;

            // Compile the pipe using the CALL filter's
            // "extraneous" command line parameters:

            CalledPipe.Compile(CmdLine.Text, ((CommandLine) CmdLine).CmdLinePtr, Source, PipeLineNo);

            // Restore the prior-saved current folder:

            Directory.SetCurrentDirectory(savedFolder);
         }
         else
         {
            // Pipe file doesn't exist.

            PyperCompileException ex = new PyperCompileException("Pipe file not found.");
            ex.Data.Add("CharPos", ((CommandLine) CmdLine).CmdLinePtr);
            ex.Data.Add("CmdLine", CmdLine.Text);
            throw ex;
         }

         LogWrite("Call.Compile: end");
      }

      public override void Execute()
      {
         LoggingEnabled = true;
         LogWrite("Call.Execute: begin");

         string inTextFile = ((Filter) Host).InText;
         string outTextFile = ((Filter) Host).OutText;

         LogWrite("Call.Execute: inTextFile = " + inTextFile);
         LogWrite("Call.Execute: outTextFile = " + outTextFile);

         CalledPipe.Debugging = ((Filter) Host).Debugging;
         CalledPipe.Execute(ref inTextFile, ref outTextFile);

         ((Filter) Host).InText = inTextFile;
         ((Filter) Host).OutText = outTextFile;

         LogWrite("Call.Execute: end");
      }

      public Call(IFilter host) : base(host)
      {
         Template = "s";
      }
   }
}
