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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Firefly.Pyper
{
   public partial class PrefWin : Form
   {
      public string Version { get { return "1.0"; } }
      
      public string DebugFile
      {
         get { return debugFileTextBox.Text; }
         set { debugFileTextBox.Text = value; }
      }

      public bool TextWrapping
      {
         get { return textWrappingCheckBox.Checked; }
         set { textWrappingCheckBox.Checked = value; }
      }

      public bool CommandAutoCompletion
      {
         get { return commandAutoCompletionCheckBox.Checked; }
         set { commandAutoCompletionCheckBox.Checked = value; }
      }
      
      public Font PipeFont
      {
         get 
         {
            return new Font(pipeFontTextBox.Font, pipeFontTextBox.Font.Style);
         }

         set
         {
            pipeFontTextBox.Font = value;
            pipeFontTextBox.Text = value.Name;
         }
      }

      public Font TextFont
      {
         get
         {
            return new Font(textFontTextBox.Font, textFontTextBox.Font.Style);
         }

         set
         {
            textFontTextBox.Font = value;
            textFontTextBox.Text = value.Name;
         }
      }

      public Font ErrorsFont
      {
         get
         {
            return new Font(errorsFontTextBox.Font, errorsFontTextBox.Font.Style);
         }

         set
         {
            errorsFontTextBox.Font = value;
            errorsFontTextBox.Text = value.Name;
         }
      }

      public PrefWin()
      {
         InitializeComponent();
      }

      private void pipeFontBrowseButton_Click(object sender, EventArgs e)
      {
         FontDialog FontDlg = new FontDialog();
         FontDlg.Font = PipeFont;

         if (FontDlg.ShowDialog() == DialogResult.OK)
         {
            PipeFont = FontDlg.Font;
         }
      }

      private void textFontBrowseButton_Click(object sender, EventArgs e)
      {
         FontDialog FontDlg = new FontDialog();
         FontDlg.Font = TextFont;

         if (FontDlg.ShowDialog() == DialogResult.OK)
         {
            TextFont = FontDlg.Font;
         }
      }

      private void errorsFontBrowseButton_Click(object sender, EventArgs e)
      {
         FontDialog FontDlg = new FontDialog();
         FontDlg.Font = ErrorsFont;

         if (FontDlg.ShowDialog() == DialogResult.OK)
         {
            ErrorsFont = FontDlg.Font;
         }
      }
   }
}

