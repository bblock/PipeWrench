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
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Firefly.PipeWrench
{
   public class ButtonBar : Panel
   {
      private int Count;

      public ButtonBar(Control parent)
      {
         this.Parent = parent;
         this.Left = 0;
         this.Top = 0;
         this.Dock = DockStyle.Fill;
         this.AutoScroll = true;
         Count = 0;
         
         // The following was done as per suggestion of Scott Waldron (http://www.thewayofcoding.com) 
         // so that ButtonBar (panel) could be scrolled using mouse wheel...
         
         this.Click += OnClick;
      }

//      void OnMouseEnter(object sender, EventArgs e)
//      {
//         this.Focus();      
//      }
            
      private void OnClick(object sender, EventArgs e)
      {
         this.Focus();
      }

//      public void Add(string text)
//      {
//         Button btn = new Button();
//         btn.Text = text;
//         btn.Height = 23;
//         btn.Width = this.Width - 20;
//         int margin = 0;
//         btn.Left = 10;
//         btn.Top = margin + (Count * (btn.Height + 3));
//         btn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
//         this.Controls.Add(btn);
//         Count++;
//      }

//      public void Add(Button btn)
//      {
//         btn.Height = 23;
//         btn.Width = this.Width - 20;
//         int margin = 0;
//         btn.Left = 10;
//         btn.Top = margin + (Count * (btn.Height + 3));
//         btn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
//         this.Controls.Add(btn);
//         Count++;
//      }

      public void Add(Label label)
      {
         //label.TextAlign = ContentAlignment.MiddleCenter;
         label.AutoSize = false; 
         label.Left = 5;
         label.Height = 16;
         label.Top = (label.Height + 3) * Count;
         label.Width = this.Width - (2 * label.Left);
         label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
         this.Controls.Add(label);
         Count++;
      }
   }
}

