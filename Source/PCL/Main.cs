// pyper / PCL
//
// This is the command line version of Pyper.

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Firefly.Pyper
{
   class Program
   {
      static string version = typeof(Program).Assembly.GetName().Version.ToString();
      static string appName = "Pyper"; 
         // The app's name, (used to access config files).
      static string cmdLineAppName = "pcl";
         // The command-line program's name.
      static int consoleRows = Console.WindowHeight;
      static int consoleCols = Console.WindowWidth;
      static Engine pipeEng = new Engine(appName);
      static bool isLinux = System.IO.Path.DirectorySeparatorChar == '/';
      
      /// <summary>
      /// Returns the number of newlines in the given string.
      /// </summary>
      static int CountOfNewlines(string theStr)
      {
         int count = 0;
         
         for (int i=0; i < theStr.Length; i++)
         {
            if (theStr[i] == '\n') count++;
         }
         
         return count;
      }
      
      /// <summary>
      /// Given the combination of strings that will be written to 
      /// the console, this routine returns a string containing 
      /// sufficient newlines to pad the console window to the end.
      /// </summary>
      static string PadLines(string theStr)
      {
         int newlineCount = CountOfNewlines(theStr);
         int padCount = consoleRows - newlineCount - 1;
         string tempStr = string.Empty;
         if (padCount > 0) tempStr = new string('\n', padCount);
         return tempStr;
      }
      
      static string GetFilters()
      {
         string result = string.Empty;
         foreach (CommandSpec cmdSpec in pipeEng.CmdSpecs)
         {
            string filterName = cmdSpec.Name;
            if (!cmdSpec.IsCore) filterName += '*';
            result += filterName + " ";
         }
         
         return result;
      }
      
      static void PaginateText(string text, string headerStr, string footerStr)
      {
         int headerLines = CountOfNewlines(headerStr);
         int footerLines = CountOfNewlines(footerStr);
         string[] lines = text.Split(new char[] {'\n'}, StringSplitOptions.None);
         int lineIndex = 0;
         
         while (lineIndex < lines.Length)
         {
            string bodyStr = "Registered filters (* denotes add-in):\n\n";
            int bodyLines = 2;
            
            while ((lineIndex < lines.Length) && (bodyLines < consoleRows - 
            headerLines - footerLines - 1))
            {
               bodyStr += lines[lineIndex] + '\n';
               lineIndex++;
               bodyLines++;
            }
            
            int lineCount = headerLines + footerLines + bodyLines + 1;
            
            int padCount = consoleRows - lineCount;
            string padStr = string.Empty;
            if (padCount > 0) padStr = new string('\n', padCount);
            
            string tempStr = headerStr + bodyStr + padStr + footerStr;
            Console.Error.Write(tempStr);
            Console.ReadKey();
         }
      }
      
      /// <summary>
      /// Returns the argument specified by its positional number (1-n) on 
      /// the command line while ignoring any switches on the command line
      /// (those arguments that begin with a "--"). If the specified argument
      /// isn't found, an empty string is returned.
      /// </summary>
      static string GetPositionalArg(int argNo, string[] args)
      {
         int argIndex = 0;
         int posCount = 0;
         string theArg = string.Empty;
         
         while ((argIndex < args.Length) && (posCount < argNo))
         {
            if ((args[argIndex].Length < 2) || (args[argIndex].Substring(0,2) != "--"))
            {
               // Its a positional argument.
               
               posCount++;
            }
            
            argIndex++;
         }
         
         if (posCount == argNo) theArg = args[argIndex-1];
         
         return theArg;
      }
      
      static int Main(string[] args)
      {
         int exitCode = 0;
         string homeFolder;
            // User's home folder.
         string applDataFolder;
            // The application's folder beneath the home folder.
         string tempStr = string.Empty;
         string debugFile;

         if (args.Length != 0) 
         {
            // Executing a pipe.
            
            try
            {
               // Load configuration parameters:
               
               if (isLinux)
               {
                  homeFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                  applDataFolder = homeFolder + System.IO.Path.DirectorySeparatorChar + "." + appName.ToLower();
               }
               else
               {   
                  homeFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                  applDataFolder = homeFolder + System.IO.Path.DirectorySeparatorChar + appName;
               }
               
               string configFile = applDataFolder + System.IO.Path.DirectorySeparatorChar + appName + "Conf.xml";
      
               if (File.Exists(configFile))
               {
                  // Configuration file found.
                  
                  XmlDocument xmlDoc = new XmlDocument();
                  xmlDoc.Load(configFile);
                  XmlElement rootElem = xmlDoc["Config"];
                  XmlElement elem = rootElem["DebugFile"];
                  debugFile = elem.InnerText;
               }
               else
               {
                  // Configuration file doesn't exist. Default the debug file:
                  
                  debugFile = applDataFolder + System.IO.Path.DirectorySeparatorChar + "Debug.txt";
               }

               // Create a pipe object:
               
               string currFolder = System.Environment.CurrentDirectory;
               string script = GetPositionalArg(1, args);
               Pipe thePipe = new Pipe(script, pipeEng, currFolder, string.Empty, 
               Engine.CoreLogging(args), true);
               thePipe.DebugFile = debugFile;
               string pipeArgs = GetPositionalArg(2, args);
               
               // Compile the pipe:
               
               thePipe.Compile(pipeArgs, 0, string.Empty, 0);
               
               if (thePipe.Errors.Count == 0) 
               {
                  // Create the input text file:
                  
                  string inText = Path.GetTempFileName();
                  StreamWriter inTextWriter = new StreamWriter(inText);
   
                  // Transfer the text from standard input to the text file object:
                  
                  while ((tempStr = Console.ReadLine()) != null)
                  {
                     inTextWriter.WriteLine(tempStr);
                  }
                  
                  inTextWriter.Close();
                  
                  // Create the output text file:
                  
                  string outText = Path.GetTempFileName();
                  
                  // Execute the pipe:
                  
                  thePipe.Execute(ref inText, ref outText);
                  
                  // Output the text results to standard output:
                  
                  StreamReader outTextReader = new StreamReader(outText);
                  
                  while (!outTextReader.EndOfStream)
                  {
                     Console.WriteLine(outTextReader.ReadLine());
                  }
               }
               else 
               {
                  Console.Error.WriteLine(System.Environment.NewLine + 
                  thePipe.Errors.ToString()); 
               }
            }
            
            catch (PyperExecException ex)
            {
               // Pipe execution (runtime) exception.
               
               tempStr = string.Empty;
               string source = (string) ex.Data["Source"];
               tempStr += source + System.Environment.NewLine;
               string lineNoStr = ((int) ex.Data["LineNo"]).ToString();
               string cmdLine = (string) ex.Data["CmdLine"];
               tempStr += "   line " + lineNoStr + ": " + cmdLine + 
               System.Environment.NewLine; 
      
               if (ex.Data.Contains("CharPos")) 
               {
                  int charPos = (int) ex.Data["CharPos"];
                  tempStr += "^".PadLeft(charPos+10+lineNoStr.Length, ' '); 
               }
               
               tempStr += System.Environment.NewLine + "      " + ex.Message;
               Console.Error.WriteLine(tempStr);
               exitCode = -1;
            }
            
            catch (Exception ex)
            {
               // Anything not already handled...
               
               exitCode = -2;
               Console.Error.WriteLine(ex.Message);
               pipeEng.Log.WriteText(ex.ToString(), "Fatal error...", "   ");
            }
         }
         else
         {
            try
            {
               string headerLeftStr = cmdLineAppName + " " + version;
               string headerRightStr;
               
               if (isLinux)
                  headerRightStr = "Copyright \u00a9 2010 Firefly Software";
               else
                  headerRightStr = "Copyright (C) 2010 Firefly Software";
               
               string headerStr = headerLeftStr + headerRightStr.PadLeft(consoleCols - 
               headerLeftStr.Length - 1) + "\n\n";
               string footerStr = "\n<Press ENTER to Continue>";
               
               // Display the help screen:
               
               tempStr = cmdLineAppName + " is the command-line compliment of " + appName + 
               ", the pipe-based text processing GUI that allows you to morph text into other forms " + 
               "by simply combining filters. " + appName + "'s " + pipeEng.CoreFilters + 
               " built-in text translation filters allow you to accomplish all sorts of text editing " + 
               "tasks. You can search and replace text, convert between CSV, comma-delimited, " + 
               "tab-delimited and fixed width data, extract logfile data, manipulate XML, convert " + 
               "line endings, format source code and more by simply combining filters." + appName + 
               " can be found on the web at www.fireflysoftware.com.";
               
               string wrapToWidthPipe = "WrapText " + consoleCols.ToString();
               string bodyStr = pipeEng.RunPipe("wrapToWidthPipe", wrapToWidthPipe, "", tempStr, false, false);
               
               Console.Error.Write(headerStr + bodyStr + PadLines(headerStr + bodyStr + footerStr) + footerStr);
               Console.ReadKey();
               
               bodyStr = "Syntax: " + cmdLineAppName + " <pipe> [<arguments>]\n\n";
               bodyStr += "Where:\n\n";
               
               string wrapToWidthIndentedPipe = "WrapText " + (consoleCols-3).ToString() + "|InsStr 1 '   '";
               bodyStr += pipeEng.RunPipe("wrapToWidthIndentedPipe", wrapToWidthIndentedPipe, "", 
               "<pipe> is a command line argument containing one or more Pyper commands separated " +
               "by \"" + Engine.PipeDelim + "\".", false, true) + "\n\n";
               
               bodyStr += pipeEng.RunPipe("wrapToWidthIndentedPipe", wrapToWidthIndentedPipe, "", 
               "<arguments> is a command line argument containing one or more pipe arguments.", false, true) + "\n\n";
               
               bodyStr += pipeEng.RunPipe("wrapToWidthPipe", wrapToWidthPipe, "", appName.ToLower() + 
               "'s I/O can be redirected. Following are some examples:", false, true) + "\n\n";
                  
               if (isLinux)
               {
                  bodyStr += 
                  "   LOGNAME=$(echo \"$FILE\" | " + cmdLineAppName + " \"AppendStr '.log'\")\n" +
                  "   echo hello | " + cmdLineAppName + " \"UpperCase " + Engine.PipeDelim + " AppendStr ' THERE'\"\n" +
                  "   cat the.txt | " + cmdLineAppName + " \"SortLines " + Engine.PipeDelim + " DelDuplLines\" >out.txt\n" +
                  "   find . -type f -printf '%P\\n' | " + cmdLineAppName + " \"InsStr 1 '#22' " + Engine.PipeDelim + " \n" +
                  "      AppendStr '#22'\" | xargs md5sum > \"$MD5SUMSFILE\"\n" +
                  "   echo \"$FILELIST\" | " + cmdLineAppName + " \"call '$RENAMEFILESPIPE'\" >\"$TEMPSCRIPT\"\n";
               }
               else
               {
                  bodyStr += 
                  "   echo hello | " + cmdLineAppName + " \"UpperCase " + Engine.PipeDelim + " AppendStr ' THERE'\"\n" +
                  "   type the.txt | " + cmdLineAppName + " \"SortLines " + Engine.PipeDelim + " DelDuplLines\" >out.txt\n" +
                  "   echo \"%FILELIST%\" | " + cmdLineAppName + " \"call '%RENAMEFILESPIPE%'\" >\"%TEMPBATCH%\"\n" +
                  "   type template.txt | " + cmdLineAppName + " \"ReplStr '<KEY>' '%KEY%'\" >page.htm";
               }
                  
               Console.Error.Write(headerStr + bodyStr + PadLines(headerStr + bodyStr + footerStr) + footerStr);
               Console.ReadKey();
               
               string filterList = GetFilters();
               
               string displayFiltersPipe = "ParseWords|SortLines|PadLinesRight '~'|InsStr 1 '~'|" +
               "ReplStr '~(\\\\w+)\\\\*' '*$1~' /r|AppendStr ' '|JoinLines|WrapText " + 
               consoleCols.ToString() + " /j50|ReplStr '~' ' '";
               
               tempStr = pipeEng.RunPipe("displayFiltersPipe", displayFiltersPipe, "", filterList, false, true);
               PaginateText(tempStr, headerStr, footerStr);
            }
            
            catch (PyperCompileException ex)
            {
               Console.Error.WriteLine(ex.Message);
            }
      
            catch (PyperExecException ex)
            {
               // Pipe execution (runtime) exception.
               
               tempStr = string.Empty;
               string source = (string) ex.Data["Source"];
               tempStr += source + System.Environment.NewLine;
               string lineNoStr = ((int) ex.Data["LineNo"]).ToString();
               string cmdLine = (string) ex.Data["CmdLine"];
               tempStr += "   line " + lineNoStr + ": " + cmdLine + System.Environment.NewLine; 
      
               if (ex.Data.Contains("CharPos")) 
               {
                  int charPos = (int) ex.Data["CharPos"];
                  tempStr += "^".PadLeft(charPos+10+lineNoStr.Length, ' '); 
               }
               
               tempStr += System.Environment.NewLine + "      " + ex.Message;
               Console.Error.WriteLine(tempStr);
            }
      
            catch (Exception ex)
            {
               // Anything not already handled...
               
               pipeEng.Log.WriteText(ex.ToString(), "Fatal error...", "   ");
               Console.Error.WriteLine(ex.Message);
            }
         }
         
         return exitCode;  
      }
   }
}

