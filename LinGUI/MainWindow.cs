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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Gtk;
using System.Xml;
using Firefly.PipeWrench;

public partial class MainWindow: Gtk.Window
{
   Engine PipeEng;
      // The pipe engine.
   string AppName = "PipeWrench"; 
      // The app's name, (used to access config files).
   string PipePath = string.Empty; 
      // The path on-disk to an open/saved pipe.
   string CurrentPipeFolder = string.Empty;
      // The last folder visited by a file open/save dialog.
   Clipboard TheClipboard;
   //const string Version = "0.1";
   string Version = typeof(MainWindow).Assembly.GetName().Version.ToString();
   string AssyFolder = string.Empty;
   string homeFolder;
      // User's home folder.
   string applDataFolder;
      // The application's folder beneath the home folder.
   string ConfigFile;
      // The configuration file.
   bool CoreLogging = false;
      // Set true (from command line switch) to log core engine debug messages.
   string DebugFile = string.Empty;
      // File where intermediate text results are written.
   int cursorLineNo;
      // The text cursor's line number when located in a text memo.
   int cursorColumnNo;
      // The text cursor's column number when located in a text memo.
   string savedInputText;
      // Text saved from the input text box during use of input file.
   string savedOutputText;
      // Text saved from the output text box during use of output file.
   bool commandAutoCompletion;
      // Determines whether pipe commands are auto-completed.
   
   /// <summary>
   /// This constructor is called from Main.cs.
   /// </summary>
   public MainWindow(string[] args) :this() 
   {
      CoreLogging = Engine.CoreLogging(args);
   }
   
   /// <summary>
   /// This constructor is called from the other constructor.
   /// </summary>
   public MainWindow(): base (Gtk.WindowType.Toplevel)
   {
      Build();
      PipeEng = new Engine(AppName); 
      this.Title = AppName;
      AssyFolder = System.IO.Path.GetDirectoryName(typeof(MainWindow).Assembly.Location);
      homeFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
      applDataFolder = homeFolder + System.IO.Path.DirectorySeparatorChar + "." + AppName.ToLower();
      ConfigFile = applDataFolder + System.IO.Path.DirectorySeparatorChar + AppName + "Conf.xml";

      LoadConfiguration();
      
      ErrorsTextView.WrapMode = WrapMode.Word; 
      TheClipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));

      PipeTextView.Buffer.Changed += PipeTextBuffer_OnChanged;
      InputTextView.Buffer.Changed += IOTextBuffer_OnChanged;
      OutputTextView.Buffer.Changed += IOTextBuffer_OnChanged;

      TextNotebook.SetTabLabelText(TextNotebook.Children[0],"Input");
      TextNotebook.SetTabLabelText(TextNotebook.Children[1],"Output");
      TextNotebook.SetTabLabelText(TextNotebook.Children[2],"Errors");

      PopulateFilterToolbar();
      
      this.Title = "new pipe - " + AppName;
      UpdateControls();
   }

   private string LoadFont(string fontElemName, XmlElement RootElem)
   {
      XmlElement Elem = RootElem[fontElemName];
      return Elem.InnerText;
   }

   /// <summary>
   /// Configures the application.
   /// </summary>
   private void LoadConfiguration() 
   {
      try
      {
         if (!File.Exists(ConfigFile))
         {
            // Configuration file doesn't exist. Create it:
            
            string configContents = 
            "<Config>" + System.Environment.NewLine +
            "  <DebugFile>" + applDataFolder + System.IO.Path.DirectorySeparatorChar + "Debug.txt</DebugFile>" + System.Environment.NewLine +
            "  <TextWrapping>True</TextWrapping>" + System.Environment.NewLine +
            "  <CmdAutoCompletion>True</CmdAutoCompletion>" + System.Environment.NewLine +
            "  <PipeFont>FreeMono Bold 10</PipeFont>" + System.Environment.NewLine +
            "  <TextFont>FreeMono 10</TextFont>" + System.Environment.NewLine +
            "  <ErrorFont>FreeMono 10</ErrorFont>" + System.Environment.NewLine +
            "</Config>" + System.Environment.NewLine;
   
            File.WriteAllText(ConfigFile, configContents);
         }
      
         // Load the configuration file:
         
         XmlDocument XmlDoc = new XmlDocument();
         XmlDoc.Load(ConfigFile);
         XmlElement RootElem = XmlDoc["Config"];
      
         // Configure debugging:
         
         XmlElement Elem = RootElem["DebugFile"];
         DebugFile = Elem.InnerText;
   
         // Configure text wrapping:
   
         Elem = RootElem["TextWrapping"];
         
         if (Elem.InnerText.ToUpper() == "TRUE")
         {
            InputTextView.WrapMode = WrapMode.Word;
            OutputTextView.WrapMode = WrapMode.Word;
         }
         else
         {
            InputTextView.WrapMode = WrapMode.None;
            OutputTextView.WrapMode = WrapMode.None;
         }
         
         // Configure command auto-completion:
         
         Elem = RootElem["CmdAutoCompletion"];
         commandAutoCompletion = Elem.InnerText.ToUpper() == "TRUE";
         
         // Configure fonts:
         
         string tempStr = LoadFont("PipeFont", RootElem);
         PipeTextView.ModifyFont(Pango.FontDescription.FromString(tempStr));
         ArgsEntry.ModifyFont(Pango.FontDescription.FromString(tempStr));
         InputTextView.ModifyFont(Pango.FontDescription.FromString(LoadFont("TextFont", RootElem)));
         OutputTextView.ModifyFont(Pango.FontDescription.FromString(LoadFont("TextFont", RootElem)));
         ErrorsTextView.ModifyFont(Pango.FontDescription.FromString(LoadFont("ErrorFont", RootElem)));
      }
      
      catch (Exception)
      {
         throw new PipeWrenchEngineException("Could not create/load app. configuration (" + 
         ConfigFile + ").");
      }
   }
   
   private void SaveFont(string fontElemName, XmlElement rootElem,
   XmlDocument xmlDoc, string fontDesc)
   {
      XmlElement elem = xmlDoc.CreateElement(fontElemName);
      elem.InnerText = fontDesc;
      rootElem.AppendChild(elem);
   }

   private void SaveConfiguration()
   {
      XmlElement Elem;

      XmlDocument XmlDoc = new XmlDocument();
      XmlElement RootElem = XmlDoc.CreateElement("Config");

      Elem = XmlDoc.CreateElement("DebugFile");
      Elem.InnerText = DebugFile;
      RootElem.AppendChild(Elem);

      Elem = XmlDoc.CreateElement("TextWrapping");
      Elem.InnerText = (InputTextView.WrapMode != WrapMode.None).ToString();
      RootElem.AppendChild(Elem);
      
      Elem = XmlDoc.CreateElement("CmdAutoCompletion");
      Elem.InnerText = (commandAutoCompletion).ToString();
      RootElem.AppendChild(Elem);
      
      SaveFont("PipeFont", RootElem, XmlDoc, PipeTextView.Style.FontDescription.ToString());
      SaveFont("TextFont", RootElem, XmlDoc, InputTextView.Style.FontDescription.ToString());
      SaveFont("ErrorFont", RootElem, XmlDoc, ErrorsTextView.Style.FontDescription.ToString());

      XmlDoc.AppendChild(RootElem);
      XmlDoc.Save(ConfigFile);
   }

   protected void OnDeleteEvent (object sender, DeleteEventArgs a)
   {
      MessageDialog WarningMesgDlg = new MessageDialog (this, 
      DialogFlags.DestroyWithParent, MessageType.Warning, 
      ButtonsType.YesNo, "Abandon pipe changes?");

      try
      {
         if ((!PipeTextView.Buffer.Modified) || 
         (WarningMesgDlg.Run() == (int) ResponseType.Yes))
         {
            a.RetVal = false;
            Application.Quit ();
         }
         else
            a.RetVal = true;
      }
      
      finally
      {
         WarningMesgDlg.Destroy();
      }
   }

   protected void DispMesg(string mesg)
   {
      MessageDialog notifyMesgDlg = new MessageDialog (this, 
      DialogFlags.DestroyWithParent, MessageType.Info, 
      ButtonsType.Ok, mesg);
      try
      {
         notifyMesgDlg.Run();
      }
      
      finally
      {
         notifyMesgDlg.Destroy();
      }
   }

   /// <summary>
   /// Parses a filter's name from the given text line.  
   /// </summary>
   private string ParseFilterName(string text)
   {
      int i = 0;

      // Scan over any initial whitespace:
      
      while ((i < text.Length) && ((text[i] == ' ') || (text[i] == '\t')))
      {
         i++;
      }
      
      // Scan the filter name:
      
      string FilterName = string.Empty;
      
      while ((i < text.Length) && (text[i] != ' ') && (text[i] != '\t'))
      {
         FilterName += text[i];
         i++;
      }
      
      return FilterName;
   }
      
   private void UpdateCmdSyntax()
   {
      // Update command syntax on status bar:
      
      TextIter ti = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
      TextIter begIter = PipeTextView.Buffer.GetIterAtLine(ti.Line);
      TextIter endIter = PipeTextView.Buffer.GetIterAtLine(ti.Line); 
      endIter.ForwardLine();
      string text = PipeTextView.Buffer.GetText(begIter, endIter, 
      false).Replace(System.Environment.NewLine, string.Empty);
      string filterName = ParseFilterName(text);
      CommandSpec cmdSpec = GetCmdSpec(filterName);

      if (cmdSpec != null)
      {
         StatusBarTextLabel.Text = cmdSpec.Name + " " + cmdSpec.Prompt;
      }
      else
      {
         StatusBarTextLabel.Text = string.Empty;
      }
   }
   
   protected void UpdCursorLocation(TextBuffer tb)
   {
      TextIter ti = tb.GetIterAtMark(tb.InsertMark);

      cursorLineNo = ti.Line + 1;
      cursorColumnNo = ti.LineOffset + 1;

      // Update cursor position on status bar:
      
      StatusBarCursorPosLabel.Text = "(" + cursorLineNo.ToString() + ", " + 
      cursorColumnNo.ToString() + ")";
   }
   
   private void UpdateControls()
   {
      RunAction.Sensitive = PipeTextView.Buffer.Text != string.Empty;
      RunToLineAction.Sensitive = PipeTextView.Buffer.Text != string.Empty;
      SaveAction.Sensitive = PipeTextView.Buffer.Modified;
   }

   protected void PopulateFilterToolbar()
   {
      int i = 0;
      
      foreach (CommandSpec CmdSpec in PipeEng.CmdSpecs)
      {
         ToolButton TheButton = new ToolButton(CmdSpec.IconName);
         TheButton.Name = CmdSpec.Name + "Button";
         TheButton.Label = CmdSpec.Name;

         Tooltips TT = new Tooltips();
         TheButton.SetTooltip (TT, CmdSpec.ShortDesc, CmdSpec.ShortDesc);
         
         TheButton.Sensitive = true;
         TheButton.Clicked += FilterButton_OnActivated;
         TheButton.Show();
         CommandsToolbar.Insert(TheButton,i++);
      }
   }

   /// <summary>
   /// Responds to changes made to the pipe textview's buffer.
   /// </summary>
   protected virtual void PipeTextBuffer_OnChanged (object sender, System.EventArgs e)
   {
      UpdCursorLocation((TextBuffer) sender);
      UpdateCmdSyntax();
      UpdateControls();
   }

   /// <summary>
   /// Responds to changes made to an I/O textview's buffer.
   /// </summary>
   protected virtual void IOTextBuffer_OnChanged (object sender, System.EventArgs e)
   {
      // Update the cursor position (line,Col) on the statusbar:
      
      UpdCursorLocation((TextBuffer) sender);
   }

   protected virtual void FilterButton_OnActivated (object sender, System.EventArgs e)
   {
      TextIter InsertIter = PipeTextView.Buffer.GetIterAtMark(
      PipeTextView.Buffer.InsertMark);

      string filterName = ((ToolButton) sender).Label;
      
      PipeTextView.Buffer.Insert(ref InsertIter, filterName + " ");
         
      // Update the status bar:  
    
      CommandSpec CmdSpec = GetCmdSpec(filterName);
      StatusBarTextLabel.Text = CmdSpec.Name + " " + CmdSpec.Prompt;

      // Set focus to the pipe:

      PipeTextView.GrabFocus();
   }

//////////////////////// Auto-Subscribed Event Handlers //////////////////////////

   /// <summary>
   /// Responds to any change in cursor position in a TextView.
   /// </summary>
   protected virtual void TextView_OnMoveCursor (object o, Gtk.MoveCursorArgs args)
   {
      // Update the cursor position (line,Col) on the statusbar:

      UpdCursorLocation(((TextView) o).Buffer);
   }

   /// <summary>
   /// Responds to mouse button clicks in a TextView.
   /// </summary>
   [GLib.ConnectBefore]
   protected virtual void TextView_OnButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
   {
      UpdCursorLocation(((TextView) o).Buffer);
      
      if ((TextView) o == PipeTextView)
      {
         UpdateCmdSyntax();
      }
   }

   [GLib.ConnectBefore]
   protected virtual void TextView_OnFocused (object o, Gtk.FocusedArgs args)
   {
      // Update the cursor position (line,Col) on the statusbar:

      UpdCursorLocation(((TextView) o).Buffer);
   }

   protected virtual void OnExit (object sender, System.EventArgs e)
   {
      Application.Quit();
   }

   protected virtual void NewAction_OnActivated (object sender, System.EventArgs e)
   {
      MessageDialog WarningMesgDlg = new MessageDialog (this, 
      DialogFlags.DestroyWithParent, MessageType.Warning, 
      ButtonsType.YesNo, "Abandon pipe changes?");

      try
      {
         if ((!PipeTextView.Buffer.Modified) || (WarningMesgDlg.Run() == (int) ResponseType.Yes))
         {
            PipePath = string.Empty; 
            PipeTextView.Buffer.Text = string.Empty;
            this.Title = "new pipe - " + AppName;
            PipeTextView.Buffer.Modified = false;
            UpdateControls();
         }
      }
      
      finally
      {
         WarningMesgDlg.Destroy();
      }
   }

   protected virtual void OpenAction_OnActivated (object sender, System.EventArgs e)
   {
      // Construct an "overwrite warning" dialog:

      MessageDialog WarningMesgDlg = new MessageDialog (this, 
      DialogFlags.DestroyWithParent, MessageType.Question, 
      ButtonsType.YesNo, "Abandon pipe changes?");

      try
      {
         if ((!PipeTextView.Buffer.Modified) || WarningMesgDlg.Run() == (int) ResponseType.Yes)
         {
            // Display a file open dialog:

            FileChooserDialog FileOpenDlg = new FileChooserDialog(
            "Choose pipe file to edit", this,FileChooserAction.Open,
            "Cancel", ResponseType.Cancel,"Open", ResponseType.Accept);
     
            FileFilter filter = new FileFilter();
            filter.Name = AppName + " files (*.pip)";
            filter.AddMimeType("text/pip");
            filter.AddPattern("*.pip");
            FileOpenDlg.AddFilter(filter);

            filter = new FileFilter();
            filter.Name = "All files (*.*)";
            filter.AddPattern("*.*");
            FileOpenDlg.AddFilter(filter);

            if (CurrentPipeFolder != string.Empty)
            {
               FileOpenDlg.SetCurrentFolder(CurrentPipeFolder);
            }

            try
            {
               if(FileOpenDlg.Run() == (int) ResponseType.Accept)
               {
                  // Open the file for reading:

                  System.IO.StreamReader TheFile =
                  System.IO.File.OpenText(FileOpenDlg.Filename);
      
                  // Copy the contents into the pipe text view control:

                  PipeTextView.Buffer.Text = TheFile.ReadToEnd();
     
                  // Set the MainWindow Title to the filename
                  // and mark the text as "clean":

                  PipePath = FileOpenDlg.Filename; 
                  this.Title = System.IO.Path.GetFileName(PipePath) + " - " + AppName; 
                  CurrentPipeFolder = FileOpenDlg.CurrentFolder;
                  PipeTextView.Buffer.Modified = false;
                  UpdateControls();

                  // Clean up:

                  TheFile.Close();
               }
            }
            
            finally
            {
               FileOpenDlg.Destroy();
            }
         }
      }
      
      finally
      {
         WarningMesgDlg.Destroy();
      }
   }

   /// <summary>
   /// Saves the current pipe.
   /// </summary>
   protected void SavePipe(bool SavingAs)
   {
      bool Ok = true;

      if ((PipePath == string.Empty) || SavingAs) 
      {
         // Display a file open dialog:

         FileChooserDialog FileSaveDlg = new FileChooserDialog(
         "Choose file to save pipe to", this, FileChooserAction.Save,
         "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
         
         FileSaveDlg.DoOverwriteConfirmation = true;
     
         FileFilter filter = new FileFilter();
         filter.Name = AppName + " files (*.pip)";
         filter.AddMimeType("text/pip");
         filter.AddPattern("*.pip");
         FileSaveDlg.AddFilter(filter);
         
         filter = new FileFilter();
         filter.Name = "All files (*.*)";
         filter.AddPattern("*.*");
         FileSaveDlg.AddFilter(filter);

         if (CurrentPipeFolder != string.Empty)
         {
            FileSaveDlg.SetCurrentFolder(CurrentPipeFolder);
         }

         try
         {
            Ok = FileSaveDlg.Run() == (int) ResponseType.Accept;
            
            if (Ok)
            {
               // Set the name of the current pipe file:
   
               PipePath = FileSaveDlg.Filename; 
               this.Title = System.IO.Path.GetFileName(PipePath) + " - " + AppName; 
               CurrentPipeFolder = FileSaveDlg.CurrentFolder;
            }
         }
         
         finally
         {
            FileSaveDlg.Destroy();
         }
      }
      
      if (Ok)
      {
         // Save the pipe:

         StreamWriter TheStream = new StreamWriter(PipePath); 
         TheStream.Write(PipeTextView.Buffer.Text);
         TheStream.Flush();
         TheStream.Close();
         PipeTextView.Buffer.Modified = false;
         UpdateControls();
      }
   }

   protected virtual void SaveAction_OnActivated (object sender, System.EventArgs e)
   {
      SavePipe(false);
   }

   protected virtual void SaveAsAction_OnActivated (object sender, System.EventArgs e)
   {
      SavePipe(true);
   }

   protected void CopyClipboardToPipe(Clipboard CB, string text)
   {
      PipeTextView.Buffer.Text = Engine.CliPipeToGui(text);
   }

   protected virtual void ImportAction_OnActivated(object sender, System.EventArgs e)
   {
      MessageDialog WarningMesgDlg = new MessageDialog (this, 
      DialogFlags.DestroyWithParent, MessageType.Warning, 
      ButtonsType.YesNo, "Abandon pipe changes?");

      try
      {
         if ((!PipeTextView.Buffer.Modified) || (WarningMesgDlg.Run() == (int) ResponseType.Yes))
         {
            TheClipboard.RequestText(new ClipboardTextReceivedFunc(CopyClipboardToPipe));
            PipeTextView.Buffer.Modified = false;
            UpdateControls();
         }
      }
      
      finally
      {
         WarningMesgDlg.Destroy();
      }
   }

   protected virtual void ExportAction_OnActivated(object sender, System.EventArgs e)
   {
      string text = PipeTextView.Buffer.GetText(
      PipeTextView.Buffer.StartIter, PipeTextView.Buffer.EndIter, false);

      try
      {
         TheClipboard.Text = Engine.GuiPipeToCli(text);
      }
      
      catch (PipeWrenchConvException ex)
      {
         MessageDialog WarningMesgDlg = new MessageDialog (this, 
         DialogFlags.DestroyWithParent, MessageType.Warning, 
         ButtonsType.Ok, ex.Message);
         
         try
         {
            WarningMesgDlg.Run();
         }
         
         finally
         {
            WarningMesgDlg.Destroy();
         }
      }
   }

   /// <summary>
   /// Executes the pipe.
   /// </summary>
   private void ExecutePipe(string pipeText)
   {
      try
      {
         ErrorsTextView.Buffer.Text = string.Empty; 
         string pipeFolder = string.Empty;
         string pipeName = "<new pipe>"; 
         
         if (PipePath != string.Empty)
         {
            string tempStr = System.IO.Path.GetFullPath(PipePath); 
            pipeFolder = System.IO.Path.GetDirectoryName(tempStr); 
            pipeName = System.IO.Path.GetFileName(tempStr); 
         }
         
         Pipe ThePipe = new Pipe(pipeText, PipeEng, pipeFolder, pipeName, CoreLogging, false);
         ThePipe.DebugFile = DebugFile;
         ThePipe.Compile(ArgsEntry.Text, 0, string.Empty, 0);

         if (ThePipe.Errors.Count == 0) 
         {
            MessageDialog WarningMesgDlg = new MessageDialog (this, 
            DialogFlags.DestroyWithParent, MessageType.Warning, 
            ButtonsType.YesNo, "Overwrite output file?");

            try
            {
               if ((!File.Exists(OutputFileEntry.Text)) || (WarningMesgDlg.Run() == (int) ResponseType.Yes))
               {
                  if ((DebugFile != null) && (File.Exists(DebugFile))) 
                  {
                     File.Delete(DebugFile);
                  }
                  
                  string inTempFile = System.IO.Path.GetTempFileName();
                  string outTempFile = System.IO.Path.GetTempFileName();
   
                  try
                  {
                     if (InputFileEntry.Text == string.Empty)
                     {
                        File.WriteAllText(inTempFile, InputTextView.Buffer.Text);
                     }
                     else
                     {
                        File.Copy(InputFileEntry.Text, inTempFile, true);
                     }
   
                     ThePipe.Execute(ref inTempFile, ref outTempFile);
   
                     if (OutputFileEntry.Text == string.Empty)
                     {
                        // Unsubscribe from the TextChanged event handler so that 
                        // the updates made to the ouput textbox don't change 
                        // the caret's current location:
   
                        OutputTextView.Buffer.Changed -= IOTextBuffer_OnChanged;
   
                        // Write changes to the output text box:
   
                        OutputTextView.Buffer.Text = File.ReadAllText(outTempFile);
   
                        // Re-subscribe to the TextChanged event handler:
   
                        OutputTextView.Buffer.Changed += IOTextBuffer_OnChanged;
                     }
                     else
                     {
                        File.Copy(outTempFile, OutputFileEntry.Text, true);
                     }
                  }
   
                  finally
                  {
                     File.Delete(inTempFile);
                     File.Delete(outTempFile);
                  }
   
                  TextNotebook.CurrentPage = 1;
                  PipeTextView.GrabFocus();
               }
            }
            
            finally
            {
               WarningMesgDlg.Destroy();
            }
         }
         else
         {
            // The pipe had compile (syntax) errors.

            ErrorsTextView.Buffer.Text = ThePipe.Errors.ToString();
            TextNotebook.CurrentPage = 2;
         }
      }
      
      catch (PipeWrenchTemplateException ex)
      {
         string tempStr = (string) ex.Data["Template"] + System.Environment.NewLine;
         
         if (ex.Data.Contains("CharPos")) 
         {
            int charPos = (int) ex.Data["CharPos"];
            tempStr += "^".PadLeft(charPos, ' '); 
         }
         
         tempStr += System.Environment.NewLine + "   " + ex.Message;
         
         ErrorsTextView.Buffer.Text = tempStr;
         TextNotebook.CurrentPage = 2;
      }
      
      catch (PipeWrenchExecException ex)
      {
         // Pipe execution (runtime) exception.
         
         string tempStr = string.Empty;
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
         ErrorsTextView.Buffer.Text = tempStr;
         TextNotebook.CurrentPage = 2;
      }
      
      catch (Exception ex)
      {
         // Anything not already handled...
         
         ErrorsTextView.Buffer.Text = ex.Message;
         TextNotebook.CurrentPage = 2;
         PipeEng.Log.WriteText(ex.ToString(), "Fatal error...", "   ");
      }
   }

   protected virtual void RunAction_OnActivated (object sender, System.EventArgs e) 
   {
      ExecutePipe(PipeTextView.Buffer.Text);
   }
   
   /// <summary>
   /// Returns the given number of lines from the top of the pipe text.
   /// </summary>
   private string TopOfPipe(int noOfLines)
   {
      string pipeText = PipeTextView.Buffer.Text;
      string[] pipeLines = pipeText.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);
      string result = string.Empty;
      
      for (int i = 0; (i < noOfLines) && (i < pipeLines.Length); i++)
      {
         result += pipeLines[i] + System.Environment.NewLine;
      }
      
      return result;
   }

   protected virtual void RunToLineAction_OnActivated (object sender, System.EventArgs e)
   {
      ExecutePipe(TopOfPipe(CaretLineNo()));
   }
   
   protected virtual void About_OnActivated (object sender, System.EventArgs e)
   {
      AboutDialog ab = new AboutDialog();
      
      try
      {
         ab.ProgramName = AppName;
         ab.Copyright = "Copyright \u00a9 2013 Firefly Software";
         ab.Version = string.Empty;
         ab.Comments = "Version: " + Version;
         
         try
         {
            ab.Logo = null;
         }
         
         catch(Exception Mye)
         {
            DispMesg(Mye.Message);
         }
         
         ab.Run();
      }
      
      finally
      {
         ab.Destroy();
      }
   }

   /// <summary>
   /// Returns a matched command suffix, given a prefix.
   /// </summary>
   protected string MatchCommand(string Prefix)
   {
      int PrefixLen = Prefix.Length;

      foreach (CommandSpec CmdSpec in PipeEng.CmdSpecs)
      {
         if (CmdSpec.Name.Length >= PrefixLen)
         {
            if (CmdSpec.Name.Substring(0,PrefixLen).ToUpper() == Prefix.ToUpper())
            {
               if (PrefixLen > 0)
                  return CmdSpec.Name.Substring(PrefixLen-1);
               else
                  return string.Empty;
            }
         }
      }

      return string.Empty;
   }
   
   /// <summary>
   /// Returns a CommandSpec object given a filter name.
   /// </summary>
   private CommandSpec GetCmdSpec(string filterName)
   {
      CommandSpec result = null;
      
      foreach (CommandSpec cmdSpec in PipeEng.CmdSpecs)
      {
         if (filterName.ToUpper() == cmdSpec.Name.ToUpper())
         {
            result = cmdSpec;
            break;
         }
      }
      
      return result;
   }
   
   /// <summary>
   /// Returns true if the given string contains all alphabetic characters.
   /// </summary>
   private bool IsAlphabetic(string st)
   {
      foreach (char ch in st)
      {
         if (!Char.IsLetter(ch))
         {
            return false;
         }
      }

      return true;
   }

   /// <summary>
   /// Returns the line of text from the pipe textview that contains the caret.
   /// </summary>
   private string CurrentLine()
   {
      TextIter begIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
      TextIter endIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
      begIter.BackwardChars(begIter.LineOffset);
      endIter.BackwardCursorPosition(); 
      endIter.ForwardToLineEnd();
      string line = PipeTextView.Buffer.GetText(begIter, endIter, false);
      return line;
   }
   
   /// <summary>
   /// Returns the 1-based line number in the pipe textview of the line containing the caret.
   /// </summary>
   private int CaretLineNo()
   {
      TextIter theIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
      int result = theIter.Line + 1;
      return result;
   }
   
   /// <summary>
   /// Returns the character position offset of the caret in the pipe textview.
   /// </summary>
   private int CaretPos()
   {
      TextIter theIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
      return theIter.LineOffset;
   }
   
   /// <summary>
   /// Moves the pipe textview's caret to the given character position on the current line.
   /// </summary>
   private void MoveCaret(int charPos)
   {
      TextIter theIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
      theIter.BackwardChars(theIter.LineOffset);
      theIter.ForwardChars(charPos);
      PipeTextView.Buffer.PlaceCursor(theIter);
   }

   protected virtual void PipeTextView_OnKeyReleaseEvent(object o, Gtk.KeyReleaseEventArgs e)
   {
      Gdk.Key theKey = e.Event.Key;
      char theChar = (char) e.Event.KeyValue;

      if (commandAutoCompletion)
      {
         bool controlPressed = (e.Event.State & Gdk.ModifierType.ControlMask) != 0;
         bool altPressed = (e.Event.State & Gdk.ModifierType.Mod1Mask) != 0;
         
         if (Char.IsLetter(theChar) && ((theKey != Gdk.Key.Home) && 
         (theKey != Gdk.Key.Left) && (theKey != Gdk.Key.Right) && 
         (theKey != Gdk.Key.Up) && (theKey != Gdk.Key.Down) && 
         (theKey != Gdk.Key.Return) && (theKey != Gdk.Key.Delete) && 
         (theKey != Gdk.Key.BackSpace) && (theKey != Gdk.Key.End)) && 
         (!controlPressed) && (!altPressed))
         {
            // The character typed is just a letter.

            PipeTextView.Buffer.Changed -= PipeTextBuffer_OnChanged;

            string theLine = CurrentLine();
            
            int lineCaretPos = CaretPos();
            string prefix = theLine.Substring(0, lineCaretPos).TrimStart();

            if (IsAlphabetic(prefix))
            {
               // The "prefix" is all alphabetic.
               
               string suffix = MatchCommand(prefix);
               int i = theLine.IndexOf(' ', lineCaretPos-1);

               if (i > -1)
               {
                  // Found the space following the filter name.

                  int noOfChars = i - lineCaretPos + 1;
                  theLine = theLine.Remove(lineCaretPos-1, noOfChars);
                  theLine = theLine.Insert(lineCaretPos-1, suffix);
               }
               else
               {
                  // No space was found.

                  theLine = theLine.Remove(lineCaretPos-1);
                  theLine += suffix;
               }

               // Update the textview's text buffer:

               TextIter begIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
               TextIter endIter = PipeTextView.Buffer.GetIterAtMark(PipeTextView.Buffer.InsertMark);
               begIter.BackwardChars(begIter.LineOffset);
               endIter.BackwardCursorPosition();
               endIter.ForwardToLineEnd();
               PipeTextView.Buffer.Delete(ref begIter, ref endIter);
               PipeTextView.Buffer.Insert(ref begIter, theLine);
               
               if (suffix != string.Empty) 
               {
                  // Restore the caret's original position:
               
                  MoveCaret(lineCaretPos);
               }
            }
            
            PipeTextView.Buffer.Changed += PipeTextBuffer_OnChanged;
         }
      }
     
      UpdCursorLocation(PipeTextView.Buffer);
      UpdateCmdSyntax();
      UpdateControls();
   }
   
   /// <summary>
   /// Returns the control that currently has selected text.
   /// </summary>
   private object ControlWithSelection()
   {
      object control = null;
      
      if (PipeTextView.Buffer.HasSelection)
      {
         control = PipeTextView;
      }
      
      if (InputTextView.Buffer.HasSelection)
      {
         control = InputTextView;
      }
      
      if (OutputTextView.Buffer.HasSelection)
      {
         control = OutputTextView;
      }
      
      int start; 
      int end;
      
      if (ArgsEntry.GetSelectionBounds(out start, out end))
      {
         control = ArgsEntry;
      }
      
      if (InputFileEntry.GetSelectionBounds(out start, out end))
      {
         control = InputFileEntry;
      }
      
      if (OutputFileEntry.GetSelectionBounds(out start, out end))
      {
         control = OutputFileEntry;
      }
      
      return control;
   }
   
   /// <summary>
   /// Returns the control that currently has focus.
   /// </summary>
   private object ControlHasFocus()
   {
      object control = null;
      
      if (PipeTextView.HasFocus)
      {
         control = PipeTextView;
      }
      
      if (InputTextView.HasFocus)
      {
         control = InputTextView;
      }
      
      if (OutputTextView.HasFocus)
      {
         control = OutputTextView;
      }
      
      if (ArgsEntry.HasFocus)
      {
         control = ArgsEntry;
      }
      
      if (InputFileEntry.HasFocus)
      {
         control = InputFileEntry;
      }
      
      if (OutputFileEntry.HasFocus)
      {
         control = OutputFileEntry;
      }
      
      return control;
   }
   
   protected virtual void Cut_OnActivated (object sender, System.EventArgs e)
   {
      object control = ControlWithSelection();
      
      if (control != null)
      {
         if (control is TextView)
         {
            Clipboard cb = ((TextView) control).GetClipboard(Gdk.Selection.Clipboard);
            ((TextView) control).Buffer.CutClipboard(cb, true);
         }
         
         if (control is Entry)
         {
            ((Entry) control).CutClipboard(); 
         }
      }
   }

   protected virtual void Copy_OnActivated (object sender, System.EventArgs e)
   {
      object control = ControlWithSelection();
      
      if (control != null)
      {
         if (control is TextView)
         {
            Clipboard cb = ((TextView) control).GetClipboard(Gdk.Selection.Clipboard);
            ((TextView) control).Buffer.CopyClipboard(cb);
         }
         
         if (control is Entry)
         {
            ((Entry) control).CopyClipboard();  
         }
      }
   }

   protected virtual void Paste_OnActivated (object sender, System.EventArgs e)
   {
      object control = ControlHasFocus();
      
      if (control != null)
      {
         if (control is TextView)
         {
            Clipboard cb = ((TextView) control).GetClipboard(Gdk.Selection.Clipboard);
            ((TextView) control).Buffer.PasteClipboard(cb);
         }
         
         if (control is Entry)
         {
            ((Entry) control).PasteClipboard();
         }
      }
   }

   protected virtual void Delete_OnActivated (object sender, System.EventArgs e)
   {
      object control = ControlWithSelection();
      
      if (control != null)
      {
         if (control is TextView)
         {
            ((TextView) control).Buffer.DeleteSelection(true, true);
         }
         
         if (control is Entry)
         {
            ((Entry) control).DeleteSelection();  
         }
      }
   }

   protected virtual void SelectAll_OnActivated (object sender, System.EventArgs e)
   {
      object control = ControlHasFocus();
      
      if (control != null)
      {
         if (control is TextView)
         {
            ((TextView) control).Buffer.SelectRange(((TextView) control).Buffer.StartIter, 
            ((TextView) control).Buffer.EndIter);
         }
         
         if (control is Entry)
         {
            ((Entry) control).SelectRegion(0, ((Entry) control).Text.Length);
         }
      }
   }

   protected virtual void Preferences_OnActivated (object sender, System.EventArgs e)
   {
      PrefWin Pref = new PrefWin();
      
      Pref.DebugFile = DebugFile;
      
      Pref.PipeFont = PipeTextView.Style.FontDescription.ToString();
      Pref.TextFont = InputTextView.Style.FontDescription.ToString();
      Pref.ErrorsFont = ErrorsTextView.Style.FontDescription.ToString();
      
      Pref.WrapText = InputTextView.WrapMode != WrapMode.None;
      Pref.CommandAutoCompletion = commandAutoCompletion;
      
      try
      {
         if (Pref.Run() == (int) ResponseType.Ok)
         {
            DebugFile = Pref.DebugFile;
            
            PipeTextView.ModifyFont(Pango.FontDescription.FromString(Pref.PipeFont));
            ArgsEntry.ModifyFont(Pango.FontDescription.FromString(Pref.PipeFont));
            InputTextView.ModifyFont(Pango.FontDescription.FromString(Pref.TextFont));
            OutputTextView.ModifyFont(Pango.FontDescription.FromString(Pref.TextFont));
            ErrorsTextView.ModifyFont(Pango.FontDescription.FromString(Pref.ErrorsFont));

            if (Pref.WrapText)
            {
               InputTextView.WrapMode = WrapMode.Word;
               OutputTextView.WrapMode = WrapMode.Word;
            }
            else
            {
               InputTextView.WrapMode = WrapMode.None;
               OutputTextView.WrapMode = WrapMode.None;
            }
            
            commandAutoCompletion = Pref.CommandAutoCompletion;
            
            SaveConfiguration();
         }
      }
      
      finally
      {
         Pref.Destroy();
      }
   }

   protected virtual void UserGuide_OnActivated (object sender, System.EventArgs e)
   {
      ProcessStartInfo startInfo = new ProcessStartInfo("userguide.pdf");
      startInfo.WorkingDirectory = AssyFolder;
      Process.Start(startInfo);
   }

   protected virtual void Demos_OnActivated (object sender, System.EventArgs e)
   {
      ProcessStartInfo startInfo = new ProcessStartInfo("Demos");
      startInfo.WorkingDirectory = AssyFolder;
      Process.Start(startInfo);
   }

// SAVED FOR FUTURE USE
//   protected virtual void SupportForum_OnActivated (object sender, System.EventArgs e)
//   {
//      System.Diagnostics.Process.Start("http://somewebsite.com/forum/");
//   }
//
//   protected virtual void QuickStartTutorial_OnActivated (object sender, System.EventArgs e)
//   {
//      System.Diagnostics.Process.Start("http://somewebsite.com/PipeWrench/quickstart.htm");
//   }
//
//   protected virtual void VisitOurHomepage_OnActivated (object sender, System.EventArgs e)
//   {
//      System.Diagnostics.Process.Start("http://somewebsite.com");
//   }

   /// <summary>
   /// Displays an open file dialog to capture the name 
   /// of a file to be used for input or output text.
   /// </summary>
   private string BrowseTextFile(string prompt, string initialFolder)
   {
      // Display a file open dialog:

      FileChooserDialog FileOpenDlg = new FileChooserDialog(
      prompt, this, FileChooserAction.Open, "Cancel", 
      ResponseType.Cancel,"Open", ResponseType.Accept);

      FileFilter filter = new FileFilter();
      filter.Name = "text files (*.txt)";
      filter.AddMimeType("text");
      filter.AddPattern("*.txt");
      FileOpenDlg.AddFilter(filter);

      filter = new FileFilter();
      filter.Name = "All files (*.*)";
      filter.AddPattern("*.*");
      FileOpenDlg.AddFilter(filter);
      
      if (initialFolder != string.Empty)
      {
         FileOpenDlg.SetCurrentFolder(initialFolder);
      }

      string fileName = string.Empty;
      
      try
      {
         if(FileOpenDlg.Run() == (int) ResponseType.Accept)
         {
            // Copy the contents into the pipe text view control:

            fileName = FileOpenDlg.Filename;
         }
      }
      
      finally
      {
         FileOpenDlg.Destroy();
      }
      
      return fileName;
   }
   
   private string GetOutputName(string InputName)
   {
      string Folder = System.IO.Path.GetDirectoryName(InputName);
      string FileNameMinusExt = System.IO.Path.GetFileNameWithoutExtension(InputName);
      string Ext = System.IO.Path.GetExtension(InputName);
      return Folder + System.IO.Path.DirectorySeparatorChar + FileNameMinusExt + " (edited)" + Ext;
   }
   
   protected virtual void InputFileBrowseButton_OnClicked (object sender, System.EventArgs e)
   {      
      string initialPath = string.Empty;
      
      if (InputFileEntry.Text != string.Empty)
      {
         initialPath = System.IO.Path.GetDirectoryName(InputFileEntry.Text);
      }
      
      string TempStr = BrowseTextFile("Choose input text file", initialPath);
      
      if (TempStr != string.Empty)
      {
         InputFileEntry.Text = TempStr;
         OutputFileEntry.Text = GetOutputName(TempStr);
      }
   }

   protected virtual void OutputFileBrowseButton_OnClicked (object sender, System.EventArgs e)
   {
      string initialPath = string.Empty;
      
      if (OutputFileEntry.Text != string.Empty)
      {
         initialPath = System.IO.Path.GetDirectoryName(OutputFileEntry.Text);
      }
      
      string TempStr = BrowseTextFile("Choose output text file", initialPath);
      
      if (TempStr != string.Empty)
      {
         OutputFileEntry.Text = TempStr;
      }
   }

   protected virtual void InputFileEntry_OnChanged (object sender, System.EventArgs e)
   {
      // Note: Editing is disabled for this control.  User must use 
      // the browse or clear buttons to alter its contents.
      
      bool newState = InputFileEntry.Text == string.Empty;
      
      if (newState != InputTextView.Sensitive)
      {
         // The state changed.
         
         InputTextView.Sensitive = newState;
         
         if (!InputTextView.Sensitive)
         {
            savedInputText = InputTextView.Buffer.Text;
            InputTextView.Buffer.Text = "Inputting from file listed below";
         }
         else
         {
            InputTextView.Buffer.Text = savedInputText;
         }
      }
      
      try
      {
         OutputFileEntry.Text = GetOutputName(InputFileEntry.Text);
      }
      
      catch (Exception)
      {
         OutputFileEntry.Text = InputFileEntry.Text;
      }
   }
   
   protected virtual void InputFileClearButton_Clicked (object sender, System.EventArgs e)
   {
      InputFileEntry.Text = string.Empty;
   }  

   protected virtual void OutputFileEntry_OnChanged (object sender, System.EventArgs e)
   {
      // Note: Editing is disabled for this control.  User must use 
      // the browse or clear buttons to alter its contents.
      
      bool newState = OutputFileEntry.Text == string.Empty;
      
      if (newState != OutputTextView.Sensitive)
      {
         // The state changed.
         
         OutputTextView.Sensitive = newState;
         
         if (!OutputTextView.Sensitive)
         {
            savedOutputText = OutputTextView.Buffer.Text;
            OutputTextView.Buffer.Text = "Outputting to file listed below";
         }
         else
         {
            OutputTextView.Buffer.Text = savedOutputText;
         }
      }
   }

   protected virtual void OutputFileClearButton_Clicked(object sender, System.EventArgs e)
   {
      OutputFileEntry.Text = string.Empty;
   }
   
   protected virtual void InsertCursorCol_OnActivated(object sender, System.EventArgs e)
   {
      TextIter InsertIter = PipeTextView.Buffer.GetIterAtMark(
      PipeTextView.Buffer.InsertMark);
      PipeTextView.Buffer.Insert(ref InsertIter, cursorColumnNo.ToString() + " ");
   }
}

