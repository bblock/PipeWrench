// 
// Pyper - automate the transformation of text using "stackable" text filters
// Copyright (C) 2013  Barry Block 
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Firefly.Pyper
{
   public partial class MainForm : Form
   {
      Engine PipeEng;

      string AppName = "Pyper";
         // The app's name, (used to access config files).
      string PipePath = string.Empty; 
         // The path on-disk to an open/saved pipe.
      string CurrentPipeFolder = string.Empty;
         // The last folder visited by a file open/save dialog.
      //const string Version = "0.1";
      string Version = typeof(MainForm).Assembly.GetName().Version.ToString();
      string AssyFolder = string.Empty;
      string homeFolder;
         // User's home folder.
      string applDataFolder;
         // The application's folder beneath the home folder.
      bool CoreLogging = false;
         // Set true (in config file) to log core engine debug messages.
      string DebugFile = string.Empty;
         // File where intermediate text results are written.
      string ConfigFile;
         // The configuration file.
      int cursorLineNo;
         // The text cursor's line number when located in a text memo.
      int cursorColumnNo;
         // The text cursor's column number when located in a text memo.
      bool commandAutoCompletion;
         // Determines whether pipe commands are auto-completed.
      string pipeLine = string.Empty; 
         // The pipe line that was recently clicked on.

      /// <summary>
      /// This constructor is called from Program.cs.
      /// </summary>
      public MainForm(string[] args) :this() 
      {
         CoreLogging = Engine.CoreLogging(args);
      }
      
      /// <summary>
      /// This constructor is called from the other constructor.
      /// </summary>
      public MainForm()
      {
         InitializeComponent();
         PipeEng = new Engine(AppName);
         this.Text = AppName;
         AssyFolder = System.IO.Path.GetDirectoryName(typeof(MainForm).Assembly.Location);
         homeFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
         applDataFolder = homeFolder + System.IO.Path.DirectorySeparatorChar + AppName;
         ConfigFile = AssyFolder + System.IO.Path.DirectorySeparatorChar + AppName + "Conf.xml";
         LoadConfiguration();
         PopulateFilterToolbar();
         this.Text = "new pipe - " + AppName;
         UpdateControls();
      }

      private Font LoadFont(string fontElemName, XmlElement RootElem)
      {
         XmlElement Elem = RootElem[fontElemName]["Name"];
         string fontName = Elem.InnerText;
         Elem = RootElem[fontElemName]["Size"];
         float fontSize = Convert.ToSingle(Elem.InnerText);
         Elem = RootElem[fontElemName]["Style"];
         int fontStyleInt = Convert.ToInt32(Elem.InnerText);
         return new Font(fontName, fontSize, (FontStyle) fontStyleInt);
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
               "  <TextWrapping>False</TextWrapping>" + System.Environment.NewLine +
               "  <CmdAutoCompletion>True</CmdAutoCompletion>" + System.Environment.NewLine +
               "  <PipeFont>" + System.Environment.NewLine +
               "    <Name>Lucida Console</Name>" + System.Environment.NewLine +
               "    <Size>9.75</Size>" + System.Environment.NewLine +
               "    <Style>0</Style>" + System.Environment.NewLine +
               "  </PipeFont>" + System.Environment.NewLine +
               "  <TextFont>" + System.Environment.NewLine +
               "    <Name>Lucida Console</Name>" + System.Environment.NewLine +
               "    <Size>8.25</Size>" + System.Environment.NewLine +
               "    <Style>0</Style>" + System.Environment.NewLine +
               "  </TextFont>" + System.Environment.NewLine +
               "  <ErrorsFont>" + System.Environment.NewLine +
               "    <Name>Lucida Console</Name>" + System.Environment.NewLine +
               "    <Size>9.75</Size>" + System.Environment.NewLine +
               "    <Style>0</Style>" + System.Environment.NewLine +
               "  </ErrorsFont>" + System.Environment.NewLine +
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
            InputTextBox.WordWrap = Elem.InnerText.ToUpper() == "TRUE";
            OutputTextBox.WordWrap = Elem.InnerText.ToUpper() == "TRUE";
         
            // Configure command auto-completion:
         
            Elem = RootElem["CmdAutoCompletion"];
            commandAutoCompletion = Elem.InnerText.ToUpper() == "TRUE";
   
            // Configure fonts:
         
            PipeTextBox.Font = LoadFont("PipeFont", RootElem);
            ArgsTextBox.Font = LoadFont("PipeFont", RootElem);
            InputTextBox.Font = LoadFont("TextFont", RootElem);
            OutputTextBox.Font = LoadFont("TextFont", RootElem);
            ErrorsTextBox.Font = LoadFont("ErrorsFont", RootElem);
         }
         
         catch (Exception)
         {
            throw new PyperEngineException("Error loading app configuration (" + 
            ConfigFile + ").");
         }
      }

      private void SaveFont(string fontElemName, XmlElement rootElem,
      XmlDocument xmlDoc, Font font)
      {
         XmlElement elem = xmlDoc.CreateElement(fontElemName);

         XmlElement nameElem = xmlDoc.CreateElement("Name");
         nameElem.InnerText = font.Name;
         elem.AppendChild(nameElem);

         XmlElement sizeElem = xmlDoc.CreateElement("Size");
         sizeElem.InnerText = font.Size.ToString();
         elem.AppendChild(sizeElem);

         XmlElement styleElem = xmlDoc.CreateElement("Style");
         styleElem.InnerText = ((int)font.Style).ToString();
         elem.AppendChild(styleElem);

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
         Elem.InnerText = InputTextBox.WordWrap.ToString();
         RootElem.AppendChild(Elem);

         Elem = XmlDoc.CreateElement("CmdAutoCompletion");
         Elem.InnerText = (commandAutoCompletion).ToString();
         RootElem.AppendChild(Elem);
         
         SaveFont("PipeFont", RootElem, XmlDoc, PipeTextBox.Font);
         SaveFont("TextFont", RootElem, XmlDoc, InputTextBox.Font);
         SaveFont("ErrorsFont", RootElem, XmlDoc, ErrorsTextBox.Font);

         XmlDoc.AppendChild(RootElem);
         XmlDoc.Save(ConfigFile);
      }

      void Filter_DoubleClick(object sender, EventArgs e)
      {
         ListView listView = (ListView) sender;
         string filterName = listView.SelectedItems[0].Text;
         int newLoc = PipeTextBox.SelectionStart + filterName.Length + 1;
         PipeTextBox.Text = PipeTextBox.Text.Insert(PipeTextBox.SelectionStart, filterName + " ");
         PipeTextBox.Modified = true;

         // Set focus to the pipe:

         PipeTextBox.Focus();
         PipeTextBox.SelectionStart = newLoc;
         PipeTextBox.SelectionLength = 0;

         // Update the status bar:

         PipeTextBox_OnChanged(PipeTextBox, null);
      }

      void Filter_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (e.KeyChar == (char) Keys.Enter)
         {
            ListView listView = (ListView) sender;
            string filterName = listView.SelectedItems[0].Text;
            int newLoc = PipeTextBox.SelectionStart + filterName.Length + 1;
            PipeTextBox.Text = PipeTextBox.Text.Insert(PipeTextBox.SelectionStart, filterName + " ");
            PipeTextBox.Modified = true;
   
            // Set focus to the pipe:
   
            PipeTextBox.Focus();
            PipeTextBox.SelectionStart = newLoc;
            PipeTextBox.SelectionLength = 0;
   
            // Update the status bar:
   
            PipeTextBox_OnChanged(PipeTextBox, null);
         }
      }

      private void filterList_MouseMove(object sender, MouseEventArgs e)
      {
         ListViewItem theItem = this.filterList.GetItemAt(e.X, e.Y);
      
         if (theItem != null)
         {
            int i = PipeEng.CmdSpecs.IndexOf(theItem.Text);
            if (i > -1)
            {
               this.toolTip.SetToolTip(this.filterList, (string) PipeEng.CmdSpecs[i].ShortDesc);
            }
            else
            {
               this.toolTip.SetToolTip(this.filterList, string.Empty);
            }
         }
         else 
         {
            this.toolTip.SetToolTip(this.filterList, string.Empty);
         }
      }
      
      private void PopulateFilterToolbar()
      {
         foreach (CommandSpec CmdSpec in PipeEng.CmdSpecs)
         {
            filterList.Items.Add(CmdSpec.Name);
         }
      }

      /// <summary>
      /// Parses a filter's name from the given line of the pipe's text.
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

         string FilterName = "";
         
         while ((i < text.Length) && (text[i] != ' ') && (text[i] != '\t'))
         {
            FilterName += text[i];
            i++;
         }

         return FilterName;
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

      private void UpdateCmdSyntax()
      {
         // Update command syntax on status bar:

         string text = PipeTextBox.Text + System.Environment.NewLine;
         int startIndex = PipeTextBox.GetFirstCharIndexOfCurrentLine();
         int endIndex = text.IndexOf(System.Environment.NewLine, startIndex);
         int length = endIndex - startIndex;

         if (length > 0)
         {
            text = PipeTextBox.Text.Substring(startIndex, length);
            string filterName = ParseFilterName(text);

            CommandSpec cmdSpec = GetCmdSpec(filterName);

            if (cmdSpec != null)
            {
               mainStatusStripHintLabel.Text = cmdSpec.Name + " " + cmdSpec.Prompt;
            }
            else
            {
               mainStatusStripHintLabel.Text = string.Empty;
            }
         }
         else
            mainStatusStripHintLabel.Text = string.Empty;
      }

      protected void UpdateCursorLocation(TextBox tb)
      {
         cursorLineNo = tb.GetLineFromCharIndex(tb.SelectionStart) + 1;
         cursorColumnNo = tb.SelectionStart - tb.GetFirstCharIndexOfCurrentLine() + 1;

         // Update cursor position on status bar:

         mainStatusStripCursorPosLabel.Text = "(" + cursorLineNo.ToString() + ", " +
         cursorColumnNo.ToString() + ")";
      }

      protected void UpdateControls()
      {
         savePipeToolStripMenuItem.Enabled = PipeTextBox.Modified;
         savePipeToolStripButton.Enabled = PipeTextBox.Modified;

         runToolStripMenuItem.Enabled = PipeTextBox.Text != string.Empty;
         runToolStripButton.Enabled = PipeTextBox.Text != string.Empty;

         runToLineToolStripMenuItem.Enabled = PipeTextBox.Text != string.Empty;
         runToLineToolStripButton.Enabled = PipeTextBox.Text != string.Empty;
      }

      private void NewPipeAction(object sender, EventArgs e)
      {
         if ((!PipeTextBox.Modified) || (MessageBox.Show("Abandon pipe changes?",
         "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes))
         {
            PipePath = string.Empty; 
            PipeTextBox.Text = string.Empty;
            this.Text = "new pipe - " + AppName;
            PipeTextBox.Modified = false;
            UpdateControls();
         }
      }

      private void OpenPipeAction(object sender, EventArgs e)
      {
         if ((!PipeTextBox.Modified) || MessageBox.Show("Abandon pipe changes?",
         "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
         {
            // Display a file open dialog:

            OpenFileDialog OpenFileDlg = new OpenFileDialog();
            OpenFileDlg.Filter = "Pipe files (*.pip)|*.pip|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (CurrentPipeFolder != string.Empty)
            {
               OpenFileDlg.InitialDirectory = CurrentPipeFolder;
            }

            if (OpenFileDlg.ShowDialog() == DialogResult.OK)
            {
               // Open the file for reading:

               StreamReader TheFile =
               File.OpenText(OpenFileDlg.FileName);

               // Copy the contents into the pipe text view control:

               PipeTextBox.Text = TheFile.ReadToEnd();

               // Set the MainWindow Title to the filename
               // and mark the text as "clean":

               PipePath = OpenFileDlg.FileName; 
               this.Text = Path.GetFileName(PipePath) + " - " + AppName; 
               CurrentPipeFolder = OpenFileDlg.InitialDirectory;
               PipeTextBox.Modified = false;
               UpdateControls();

               // Clean up:

               TheFile.Close();
            }
         }
      }

      private void SavePipe(bool SavingAs)
         // Saves the current pipe.
      {
         bool Ok = true;

         if ((PipePath == string.Empty) || SavingAs) 
         {
            // Display a file open dialog:

            SaveFileDialog SaveFileDlg = new SaveFileDialog();
            SaveFileDlg.OverwritePrompt = true;
            SaveFileDlg.Filter = "Pipe files (*.pip)|*.pip|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (CurrentPipeFolder != string.Empty)
            {
               SaveFileDlg.InitialDirectory = CurrentPipeFolder;
            }

            Ok = SaveFileDlg.ShowDialog() == DialogResult.OK;

            if (Ok)
            {
               // Set the name of the current pipe file:

               PipePath = SaveFileDlg.FileName; 
               this.Text = Path.GetFileName(PipePath) + " - " + AppName; 
               CurrentPipeFolder = SaveFileDlg.InitialDirectory;
            }
         }

         if (Ok)
         {
            // Save the pipe:

            StreamWriter TheStream = new StreamWriter(PipePath); 
            TheStream.Write(PipeTextBox.Text);
            TheStream.Flush();
            TheStream.Close();
            PipeTextBox.Modified = false;
            UpdateControls();
         }
      }

      private void SavePipeAction(object sender, EventArgs e)
      {
         SavePipe(false);
      }

      private void SavePipeAsAction(object sender, EventArgs e)
      {
         SavePipe(true);
      }

      /// <summary>
      /// Executes the pipe.
      /// </summary>
      private void ExecutePipe(string pipeText)
      {
         try
         {
            ErrorsTextBox.Text = string.Empty;
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
            ThePipe.Compile(ArgsTextBox.Text, 0, string.Empty, 0);

            if (ThePipe.Errors.Count == 0) 
            {
               if ((!File.Exists(OutputFileTextBox.Text)) ||
               MessageBox.Show("Overwrite output file?", "Warning!", MessageBoxButtons.YesNoCancel,
               MessageBoxIcon.Warning) == DialogResult.Yes)
               {
                  if ((DebugFile != null) && (File.Exists(DebugFile))) 
                  {
                     File.Delete(DebugFile);
                  }
                  
                  string inTempFile = System.IO.Path.GetTempFileName();
                  string outTempFile = System.IO.Path.GetTempFileName();

                  try
                  {
                     if (InputFileTextBox.Text == string.Empty)
                     {
                        File.WriteAllText(inTempFile, InputTextBox.Text);
                     }
                     else
                     {
                        File.Copy(InputFileTextBox.Text, inTempFile, true);
                     }

                     ThePipe.Execute(ref inTempFile, ref outTempFile);

                     if (OutputFileTextBox.Text == string.Empty)
                     {
                        // Unsubscribe from the TextChanged event handler so that 
                        // the updates made to the ouput textbox don't change 
                        // the caret's current location:

                        OutputTextBox.TextChanged -= IOTextBox_OnChanged;

                        // Write changes to the output text box:

                        OutputTextBox.Text = File.ReadAllText(outTempFile);

                        // Re-subscribe to the TextChanged event handler:

                        OutputTextBox.TextChanged += IOTextBox_OnChanged;
                     }
                     else
                     {
                        File.Copy(outTempFile, OutputFileTextBox.Text, true);
                     }
                  }
                  
                  finally
                  {
                     File.Delete(inTempFile);
                     File.Delete(outTempFile);
                  }

                  TextTabControl.SelectedTab = OutputTabPage;
                  PipeTextBox.Focus();
               }
            }
            else
            {
               // Errors.

               ErrorsTextBox.Text = ThePipe.Errors.ToString(); 
               TextTabControl.SelectedTab = ErrorsTabPage;
            }
         }

         catch (PyperExecException ex2)
         {
            // Pipe execution (runtime) exception.
            
            string tempStr = string.Empty;
            string source = (string) ex2.Data["Source"];
            tempStr += source + System.Environment.NewLine;
            string lineNoStr = ((int) ex2.Data["LineNo"]).ToString();
            string cmdLine = (string) ex2.Data["CmdLine"];
            tempStr += "   line " + lineNoStr + ": " + cmdLine + System.Environment.NewLine; 
   
            if (ex2.Data.Contains("CharPos")) 
            {
               int charPos = (int) ex2.Data["CharPos"];
               tempStr += "^".PadLeft(charPos+10+lineNoStr.Length, ' '); 
            }
            
            tempStr += System.Environment.NewLine + "      " + ex2.Message;
            ErrorsTextBox.Text = tempStr;
            TextTabControl.SelectedTab = ErrorsTabPage;
         }
         
         catch (Exception ex3)
         {
            // Anything not already handled...
            
            ErrorsTextBox.Text = ex3.Message;
            TextTabControl.SelectedTab = ErrorsTabPage;
            PipeEng.Log.WriteText(ex3.ToString(), "Fatal error...", "   ");
         }
      }

      private void RunAction(object sender, EventArgs e)
      {
         ExecutePipe(PipeTextBox.Text);
      }

      /// <summary>
      /// Returns all lines of the pipe up to and including 
      /// the given line number, (which is 1-based).
      /// </summary>
      private string TopOfPipe(int cursorLineNo)
      {
         int i = 1;
         string result = string.Empty;

         for (i = 0; (i < cursorLineNo) && (i < PipeTextBox.Lines.Length); i++)
         {
            result += PipeTextBox.Lines[i] + System.Environment.NewLine;
         }

         return result;
      }

      private void RunToLineAction(object sender, EventArgs e)
      {
         int cursorLineNo = PipeTextBox.GetLineFromCharIndex(PipeTextBox.SelectionStart) + 1;
         ExecutePipe(TopOfPipe(cursorLineNo));
      }

      private void PreferencesAction(object sender, EventArgs e)
      {
         PrefWin Pref = new PrefWin();
         Pref.DebugFile = DebugFile;
         Pref.PipeFont = (Font) PipeTextBox.Font.Clone();
         Pref.TextFont = (Font) InputTextBox.Font.Clone();
         Pref.ErrorsFont = (Font) ErrorsTextBox.Font.Clone();
         Pref.TextWrapping = InputTextBox.WordWrap;
         Pref.CommandAutoCompletion = commandAutoCompletion;

         if (Pref.ShowDialog() == DialogResult.OK)
         {
            DebugFile = Pref.DebugFile;
            PipeTextBox.Font = Pref.PipeFont;
            ArgsTextBox.Font = Pref.PipeFont; // shares same font
            InputTextBox.Font = Pref.TextFont;
            OutputTextBox.Font = Pref.TextFont;
            ErrorsTextBox.Font = Pref.ErrorsFont;

            InputTextBox.WordWrap = Pref.TextWrapping;

            if (Pref.TextWrapping)
               InputTextBox.ScrollBars = ScrollBars.Vertical;
            else
               InputTextBox.ScrollBars = ScrollBars.Both;

            OutputTextBox.WordWrap = Pref.TextWrapping;

            if (Pref.TextWrapping)
               OutputTextBox.ScrollBars = ScrollBars.Vertical;
            else
               OutputTextBox.ScrollBars = ScrollBars.Both;
            
            commandAutoCompletion = Pref.CommandAutoCompletion;

            SaveConfiguration();
         }
      }

      /// <summary>
      /// Responds to changes made to the pipe textbox.
      /// </summary>
      private void PipeTextBox_OnChanged(object sender, EventArgs e)
      {
         UpdateCursorLocation((TextBox) sender);
         UpdateCmdSyntax();
         UpdateControls();
      }

      private string CurrentLine()
      {
         int i = PipeTextBox.GetLineFromCharIndex(PipeTextBox.SelectionStart);
         string line = PipeTextBox.Lines[i];
         return line;
      }

      /// <summary>
      /// Returns a matched command suffix, given a prefix.
      /// </summary>
      protected string MatchCommand(string prefix)
      {
         int prefixLen = prefix.Length;

         foreach (CommandSpec CmdSpec in PipeEng.CmdSpecs)
         {
            if (CmdSpec.Name.Length >= prefixLen)
            {
               if (CmdSpec.Name.Substring(0, prefixLen).ToUpper() == prefix.ToUpper())
               {
                  if (prefixLen > 0)
                     return CmdSpec.Name.Substring(prefixLen - 1);
                  else
                     return string.Empty;
               }
            }
         }

         return string.Empty;
      }

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

      private void PipeTextBox_OnKeyUp(object sender, KeyEventArgs e)
      {
         char theChar = (char) e.KeyValue;

         if (commandAutoCompletion)
         {
            if (Char.IsLetter(theChar) && (!e.Control) && (!e.Alt))
            {
               // It's just a letter.

               string theLine = CurrentLine();
               int lineCaretPos = PipeTextBox.SelectionStart - PipeTextBox.GetFirstCharIndexOfCurrentLine();
               string charsToLeftOfCaret = theLine.Substring(0, lineCaretPos).TrimStart();

               if (IsAlphabetic(charsToLeftOfCaret))
               {
                  int savedCaretPos = PipeTextBox.SelectionStart;
                  string suffix = MatchCommand(charsToLeftOfCaret);
                  int i = theLine.IndexOf(' ', lineCaretPos-1);

                  if (i > -1)
                  {
                     // Found the space following the filter name.

                     int noOfChars = i - lineCaretPos + 1;
                     theLine = theLine.Remove(lineCaretPos - 1, noOfChars);
                     theLine = theLine.Insert(lineCaretPos - 1, suffix);
                  }
                  else
                  {
                     // No space was found.

                     theLine = theLine.Remove(lineCaretPos - 1);
                     theLine += suffix;
                  }

                  // Update the pipe textbox:

                  int lineNo = PipeTextBox.GetLineFromCharIndex(PipeTextBox.SelectionStart) + 1;
                  int startIndex = PipeTextBox.GetFirstCharIndexOfCurrentLine();
                  int lineLength = PipeTextBox.Lines[lineNo - 1].Length;
                  PipeTextBox.Text = PipeTextBox.Text.Remove(startIndex, lineLength);
                  PipeTextBox.Text = PipeTextBox.Text.Insert(startIndex, theLine);
                  PipeTextBox.SelectionStart = savedCaretPos;
                  PipeTextBox.Modified = true;
               }
            }
         }

         UpdateCursorLocation((TextBox) sender);
         UpdateCmdSyntax();
         UpdateControls();
      }

      private void OpenFile(string filePath) 
      {
         if ((!PipeTextBox.Modified) || MessageBox.Show("Abandon pipe changes?",
         "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
         {
            // Open the file for reading:

            StreamReader TheFile =  File.OpenText(filePath);

            // Copy the contents into the pipe text view control:

            PipeTextBox.Text = TheFile.ReadToEnd();

            // Set the MainWindow Title to the filename
            // and mark the text as "clean":

            this.Text = Path.GetFileName(filePath) + " - " + AppName;
            CurrentPipeFolder = Path.GetDirectoryName(filePath);
            PipeTextBox.Modified = false;
            UpdateControls();

            // Clean up:

            TheFile.Close();
         }
      }
      
      private void PipeTextBox_MouseDown(object sender, MouseEventArgs args) 
      {
         if (PipeTextBox.Lines.Length > 0)
         {
            int charIndex = PipeTextBox.GetCharIndexFromPosition(args.Location);
            int lineIndex = PipeTextBox.GetLineFromCharIndex(charIndex);
            pipeLine = PipeTextBox.Lines[lineIndex];
         }
      }

      private void PipeCutToolStripMenuItem_Click(object sender, EventArgs e)
      {
         PipeTextBox.Cut();
      }
   
      private void PipeCopyToolStripMenuItem_Click(object sender, EventArgs e)
      {
         PipeTextBox.Copy();
      }

      private void PipePasteToolStripMenuItem_Click(object sender, EventArgs e)
      {
         PipeTextBox.Paste();
      }

      private void PipeDeleteToolStripMenuItem_Click(object sender, EventArgs e)
      {
         PipeTextBox.SelectedText = string.Empty;
         PipeTextBox.Modified = true;
      }

      /// <summary>
      /// Changes state machine's state to the given new state.
      /// </summary>
      private void ToState(ref int State, int NewState)
      {
         State = NewState;
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
      /// This routine searches the source string, "CmdLine" for a string
      /// delimited by single or double quotes, (specify one). Searching begins
      /// at the given character position, "CmdLinePtr".  Embedded delimiters 
      /// are allowed by placing two of them together within the string. 
      /// </summary>
      private string GetString(string CmdLine, ref int CmdLinePtr, char QuoteChar, 
      bool InterpretingQuotes)
      {
         char Ch;
         string Str = string.Empty;
         bool Done = false;
         int State = 0;
         ToState(ref State, 1);
   
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
   
                        ToState(ref State, 2);
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

                           PyperCompileException ex = 
                           new PyperCompileException("Expected beginning of string.");
                           ex.Data.Add("CharPos", CmdLinePtr);
                           ex.Data.Add("CmdLine", CmdLine);
                           throw ex;
                        }
                     }
                  }
                  else
                  {
                     // Reached End-Of-String.  Beginning delimiter not found. 
   
                     PyperCompileException ex = 
                     new PyperCompileException("Expected string not found.");
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
   
                        ToState(ref State, 3);
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
   
                     PyperCompileException ex = 
                     new PyperCompileException("String is not terminated with a quote.");
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
                        ToState(ref State, 2);
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

      private void PipeOpenCalledPipeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         string line = pipeLine.Trim();
         
         if (line.ToUpper().StartsWith("CALL "))
         {
            // It's a CALL filter.
            
            int i = 5;
            
            try
            {
               string filePath = GetString(line, ref i, '\'', true); 
               OpenFile(filePath);
            }
            
            catch (Exception ex)
            {
               ErrorsTextBox.Text = ex.Message;
               TextTabControl.SelectedTab = ErrorsTabPage;
            }
         }
      }

      private void PipeContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
      {
         string line = pipeLine.Trim();
         
         // Enable/disable menu items:
         
         PipeOpenCalledPipeToolStripMenuItem.Enabled = line.ToUpper().StartsWith("CALL ");
         PipeCopyToolStripMenuItem.Enabled = PipeTextBox.SelectedText != string.Empty;
         PipeCutToolStripMenuItem.Enabled = PipeTextBox.SelectedText != string.Empty;
         IDataObject data = Clipboard.GetDataObject();
         PipePasteToolStripMenuItem.Enabled = data.GetDataPresent(DataFormats.Text);
         PipeDeleteToolStripMenuItem.Enabled = PipeTextBox.SelectedText != string.Empty;
         
         e.Cancel = false;
      }
      
      private void PipeTextBox_OnClick(object sender, EventArgs e)
      {
         UpdateCursorLocation((TextBox) sender);
         UpdateCmdSyntax();
      }

      /// <summary>
      /// Responds to changes made to an input/output textbox.
      /// </summary>
      private void IOTextBox_OnChanged(object sender, EventArgs e)
      {
         UpdateCursorLocation((TextBox) sender);
      }

      private void IOTextBox_OnKeyUp(object sender, KeyEventArgs e)
      {
         UpdateCursorLocation((TextBox)sender);
      }

      private void IOTextBox_OnClick(object sender, EventArgs e)
      {
         UpdateCursorLocation((TextBox)sender);
      }

      private void ExitAction(object sender, EventArgs e)
      {
         this.Close();
      }

      private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         if ((PipeTextBox.Modified) && MessageBox.Show("Abandon pipe changes?",
         "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.No)
         {
            e.Cancel = true;
         }
      }

      /// <summary>
      /// Imports the command-line style pipe that's currently on the clipboard.
      /// </summary>
      private void ImportAction(object sender, EventArgs e)
      {
         IDataObject iData = Clipboard.GetDataObject();
         bool HasText = iData.GetDataPresent(DataFormats.Text);

         if (HasText)
         {
            if ((!PipeTextBox.Modified) || MessageBox.Show("Abandon pipe changes?",
            "Warning!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
               string clipboardText = ((String) iData.GetData(DataFormats.Text)).Trim();
               PipeTextBox.Text = Engine.CliPipeToGui(clipboardText);
            }
         }
      }

      /// <summary>
      /// Exports the pipe text to the clipboard as a command-line style pipe.
      /// </summary>
      private void ExportAction(object sender, EventArgs e) 
      {
         string text = PipeTextBox.Text;
         Clipboard.SetDataObject(Engine.GuiPipeToCli(text));
      }

      /// <summary>
      /// Displays an open file dialog to capture the name 
      /// of a file to be used for input or output text.
      /// </summary>
      private string BrowseTextFile(string prompt, string initialFolder)
      {
         OpenFileDialog OpenFileDlg = new OpenFileDialog();
         OpenFileDlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

         OpenFileDlg.Title = prompt;

         string fileName = string.Empty;

         if (initialFolder != string.Empty)
         {
            OpenFileDlg.InitialDirectory = initialFolder;
         }

         if (OpenFileDlg.ShowDialog() == DialogResult.OK)
         {
            fileName = OpenFileDlg.FileName;
         }

         return fileName;
      }

      private string GetOutputName(string inputName)
      {
         string tempStr = string.Empty;

         if (inputName != string.Empty)
         {
            string folder = System.IO.Path.GetDirectoryName(inputName);
            string fileNameMinusExt = System.IO.Path.GetFileNameWithoutExtension(inputName);
            string ext = System.IO.Path.GetExtension(inputName);
            tempStr = folder + System.IO.Path.DirectorySeparatorChar + fileNameMinusExt + " (edited)" + ext;
         }

         return tempStr;
      }
   
      private void InputFileBrowseButton_Click(object sender, EventArgs e)
      {
         string initialPath = string.Empty;
         
         if (InputFileTextBox.Text != string.Empty)
         {
            initialPath = System.IO.Path.GetDirectoryName(InputFileTextBox.Text);
         }
         
         string tempStr = BrowseTextFile("Choose input text file", initialPath);

         if (tempStr != string.Empty)
         {
            InputFileTextBox.Text = tempStr;
            OutputFileTextBox.Text = GetOutputName(tempStr);
         }
      }

      private void OutputFileBrowseButton_Click(object sender, EventArgs e)
      {
         string initialPath = string.Empty;
         
         if (OutputFileTextBox.Text != string.Empty)
         {
            initialPath = System.IO.Path.GetDirectoryName(OutputFileTextBox.Text);
         }
         
         string tempStr = BrowseTextFile("Choose output text file", initialPath);

         if (tempStr != string.Empty)
         {
            OutputFileTextBox.Text = tempStr;
         }
      } 

      private void InputFileClearButton_Click(object sender, EventArgs e)
      {
         InputFileTextBox.Clear();
      }
      
      private void OutputFileClearButton_Click(object sender, EventArgs e)
      {
         OutputFileTextBox.Clear();
      }
      
      private void InputFileTextBox_TextChanged(object sender, EventArgs e)
      {
         InputTextBox.Visible = InputFileTextBox.Text == string.Empty;
         InputTextBypassedLabel.Visible = InputFileTextBox.Text != string.Empty;
         OutputFileTextBox.Text = GetOutputName(InputFileTextBox.Text);
      }

      private void OutputFileTextBox_TextChanged(object sender, EventArgs e)
      {
         OutputTextBox.Visible = OutputFileTextBox.Text == string.Empty;
         OutputTextBypassedLabel.Visible = OutputFileTextBox.Text != string.Empty;
      }

      /// <summary>
      /// Inserts the caret's position for a "text" textbox (that was recently 
      /// clicked on) into the pipe at the pipe's caret position.
      /// </summary>
      private void InsertCursorColAction(object sender, EventArgs e)
      {
         string tempStr = cursorColumnNo.ToString() + " ";
         int start = PipeTextBox.SelectionStart;
         PipeTextBox.Text = PipeTextBox.Text.Insert(start, tempStr);
         PipeTextBox.SelectionStart = start + tempStr.Length;
         PipeTextBox.Focus();
         PipeTextBox.SelectionLength = 0;
      }

      private void CutTextAction(object sender, EventArgs e)
      {
         Control ctrl = VertSplitContainer.ActiveControl;

         if ((ctrl != null) && (ctrl is TextBox))
         {
            ((TextBox) ctrl).Cut();
         }
      }

      private void CopyTextAction(object sender, EventArgs e)
      {
         Control ctrl = VertSplitContainer.ActiveControl;

         if ((ctrl != null) && (ctrl is TextBox))
         {
            ((TextBox) ctrl).Copy();
         }
      }

      private void PasteTextAction(object sender, EventArgs e)
      {
         Control ctrl = VertSplitContainer.ActiveControl;

         if ((ctrl != null) && (ctrl is TextBox))
         {
            ((TextBox) ctrl).Paste();
         }
      }

      private void DeleteTextAction(object sender, EventArgs e)
      {
         Control ctrl = VertSplitContainer.ActiveControl;

         if ((ctrl != null) && (ctrl is TextBox))
         {
            ((TextBox) ctrl).SelectedText = string.Empty;
            ((TextBox) ctrl).Modified = true;
         }
      }
      
      private void SelectAllTextAction(object sender, EventArgs e)
      {
         Control ctrl = VertSplitContainer.ActiveControl;

         if ((ctrl != null) && (ctrl is TextBox))
         {
            ((TextBox) ctrl).SelectAll();
         }
      }
      
      private void AboutAction(object sender, EventArgs e)
      {
         AboutForm about = new AboutForm(AppName, Version);
         about.ShowDialog();
      }
      
      private void UserGuideAction(object sender, EventArgs e)
      {
         MessageBox.Show("Sorry, but the user guide is not yet available.  Please see the README\n" +
         "file for an introduction to using " + AppName + "and the command reference\n" +
         "for specific information regarding " + AppName + " commands.", "Notice", 
         MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      
      private void CommandReferenceAction(object sender, EventArgs e)
      {
         System.Diagnostics.Process.Start("cmdref.htm");
      }
      
      private void DemosAction(object sender, EventArgs e)
      {
         System.Diagnostics.Process.Start("Demos");
      }

// SAVED FOR FUTURE USE
//      private void SupportForumAction(object sender, EventArgs e)
//      {
//         System.Diagnostics.Process.Start("http://somewebsite.com/forum/");
//      }
//      
//      private void QuickStartAction(object sender, EventArgs e)
//      {
//         System.Diagnostics.Process.Start("http://somewebsite.com/pyper/quickstart.htm");
//      }
//      
//      private void VisitHomepageAction(object sender, EventArgs e)
//      {
//         System.Diagnostics.Process.Start("http://somewebsite.com");
//      }
   }
}

