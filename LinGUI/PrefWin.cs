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
using Gtk;
using System;

namespace Firefly.Pyper
{
   public partial class PrefWin : Gtk.Dialog
   {
      public PrefWin()
      {
         this.Build();
      }
      
      public string Version { get { return "1.0"; } }
      
      public string DebugFile
      {
         get { return DebugFileEntry.Text; }
         set { DebugFileEntry.Text = value; }
      }

      public bool WrapText
      {
         get { return WrapTextCheckBox.Active; }
         set { WrapTextCheckBox.Active = value; }
      }

      public bool CommandAutoCompletion
      {
         get { return CommandAutoCompletionCheckBox.Active; }
         set { CommandAutoCompletionCheckBox.Active = value; }
      }
      
      public string PipeFont
      {
         get 
         {
            return PipeFontEntry.Style.FontDescription.ToString();
         }

         set
         {
            PipeFontEntry.Text = value;
            PipeFontEntry.ModifyFont(Pango.FontDescription.FromString(value));
         }
      }

      public string TextFont
      {
         get
         {
            return TextFontEntry.Style.FontDescription.ToString();
         }

         set
         {
            TextFontEntry.Text = value;
            TextFontEntry.ModifyFont(Pango.FontDescription.FromString(value));
         }
      }

      public string ErrorsFont
      {
         get
         {
            return ErrorsFontEntry.Style.FontDescription.ToString();
         }

         set
         {
            ErrorsFontEntry.Text = value;
            ErrorsFontEntry.ModifyFont(Pango.FontDescription.FromString(value));
         }
      }

      protected virtual void OnPipeFontBrowseButtonClicked (object sender, System.EventArgs e)
      {
         FontSelectionDialog FontDlg = new FontSelectionDialog("Select Pipe Font");
         FontDlg.SetFontName(PipeFont);
   
         try
         {
            if (FontDlg.Run() == (int) ResponseType.Ok)
            {
               PipeFont = FontDlg.FontName;
            }
         }
         finally
         {
            FontDlg.Destroy();
         }
      }

      protected virtual void OnTextFontBrowseButtonClicked (object sender, System.EventArgs e)
      {
         FontSelectionDialog FontDlg = new FontSelectionDialog("Select I/O Text Font");
         FontDlg.SetFontName(TextFont);
   
         try
         {
            if (FontDlg.Run() == (int) ResponseType.Ok)
            {
               TextFont = FontDlg.FontName;
            }
         }
         finally
         {
            FontDlg.Destroy();
         }
      }

      protected virtual void OnErrorsFontBrowseButtonClicked (object sender, System.EventArgs e)
      {
         FontSelectionDialog FontDlg = new FontSelectionDialog("Select Error Font");
         FontDlg.SetFontName(ErrorsFont);
   
         try
         {
            if (FontDlg.Run() == (int) ResponseType.Ok)
            {
               ErrorsFont = FontDlg.FontName;
            }
         }
         finally
         {
            FontDlg.Destroy();
         }
      }
   }
}

