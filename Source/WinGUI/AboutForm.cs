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
using System.Windows.Forms;
using System.Drawing;

namespace Firefly.Pyper
{
   public class AboutForm : Form
   {
      private Button okButton;
      private Label nameLabel;
      private Label versionLabel;
      private Label copyrightLabel;
      
      public AboutForm(string prodName, string version)
      {
         this.Height = 200;
         this.Width = 250;
         this.Text = "About";
         this.StartPosition = FormStartPosition.CenterScreen;
         this.FormBorderStyle = FormBorderStyle.FixedDialog;
         this.ControlBox = true;
         
         okButton = new Button();
         okButton.Width = 30;
         okButton.Height = 20;
         okButton.Text = "OK";
         okButton.Left = (this.Width - okButton.Width) / 2;
         okButton.Top = this.Height - okButton.Height - 40;
         okButton.Visible = true;
         okButton.Click += new EventHandler(okButton_Click);
         this.Controls.Add(okButton);
         
         nameLabel = new Label();
         nameLabel.Font = new Font(nameLabel.Font.FontFamily, 18);
         nameLabel.Text = prodName;
         nameLabel.Visible = true;
         nameLabel.Left = 0;
         nameLabel.Width = this.Width;
         nameLabel.Height = 30;
         nameLabel.TextAlign = ContentAlignment.MiddleCenter;
         nameLabel.Top = 10;
         this.Controls.Add(nameLabel);
         
         versionLabel = new Label();
         versionLabel.Font = new Font(versionLabel.Font.FontFamily, 10);
         versionLabel.Text = "Version: " + version;
         versionLabel.Visible = true;
         versionLabel.Left = 0;
         versionLabel.Width = this.Width;
         versionLabel.Height = 30;
         versionLabel.TextAlign = ContentAlignment.MiddleCenter;
         versionLabel.Top = 40;
         this.Controls.Add(versionLabel);
         
         copyrightLabel = new Label();
         copyrightLabel.Font = new Font(copyrightLabel.Font.FontFamily, 8);
         copyrightLabel.Text = "Copyright \u00a9 2010 Firefly Software";
         copyrightLabel.Visible = true;
         copyrightLabel.Left = 0;
         copyrightLabel.Width = this.Width;
         copyrightLabel.Height = 20;
         copyrightLabel.TextAlign = ContentAlignment.MiddleCenter;
         copyrightLabel.Top = 70;
         this.Controls.Add(copyrightLabel);
      }
 
      private void okButton_Click(object sender, EventArgs e)
      {
         this.Close();
      }
   }
}

