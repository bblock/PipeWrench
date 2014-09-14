namespace Firefly.PipeWrench
{
   partial class PrefWin
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.prefTabControl = new System.Windows.Forms.TabControl();
         this.PipeTabPage = new System.Windows.Forms.TabPage();
         this.pipeFontBrowseButton = new System.Windows.Forms.Button();
         this.pipeFontTextBox = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.IOTextTabPage = new System.Windows.Forms.TabPage();
         this.textWrappingCheckBox = new System.Windows.Forms.CheckBox();
         this.commandAutoCompletionCheckBox = new System.Windows.Forms.CheckBox();
         this.textFontTextBox = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.textFontBrowseButton = new System.Windows.Forms.Button();
         this.ErrorsTabPage = new System.Windows.Forms.TabPage();
         this.errorsFontBrowseButton = new System.Windows.Forms.Button();
         this.errorsFontTextBox = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.debugTabPage = new System.Windows.Forms.TabPage();
         this.debugFileTextBox = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         //this.debuggingCheckBox = new System.Windows.Forms.CheckBox();
         this.cancelButton = new System.Windows.Forms.Button();
         this.okButton = new System.Windows.Forms.Button();
         this.prefTabControl.SuspendLayout();
         this.PipeTabPage.SuspendLayout();
         this.IOTextTabPage.SuspendLayout();
         this.ErrorsTabPage.SuspendLayout();
         this.debugTabPage.SuspendLayout();
         this.SuspendLayout();
         // 
         // prefTabControl
         // 
         this.prefTabControl.Controls.Add(this.PipeTabPage);
         this.prefTabControl.Controls.Add(this.IOTextTabPage);
         this.prefTabControl.Controls.Add(this.ErrorsTabPage);
         this.prefTabControl.Controls.Add(this.debugTabPage);
         this.prefTabControl.Dock = System.Windows.Forms.DockStyle.Top;
         this.prefTabControl.Location = new System.Drawing.Point(0, 0);
         this.prefTabControl.Name = "prefTabControl";
         this.prefTabControl.SelectedIndex = 0;
         this.prefTabControl.Size = new System.Drawing.Size(292, 230);
         this.prefTabControl.TabIndex = 0;
         // 
         // PipeTabPage
         // 
         this.PipeTabPage.BackColor = System.Drawing.SystemColors.Control;
         this.PipeTabPage.Controls.Add(this.pipeFontBrowseButton);
         this.PipeTabPage.Controls.Add(this.pipeFontTextBox);
         this.PipeTabPage.Controls.Add(this.label2);
         this.PipeTabPage.Controls.Add(this.commandAutoCompletionCheckBox);
         this.PipeTabPage.Location = new System.Drawing.Point(4, 22);
         this.PipeTabPage.Name = "PipeTabPage";
         this.PipeTabPage.Padding = new System.Windows.Forms.Padding(3);
         this.PipeTabPage.Size = new System.Drawing.Size(284, 204);
         this.PipeTabPage.TabIndex = 3;
         this.PipeTabPage.Text = "Pipe";
         this.PipeTabPage.UseVisualStyleBackColor = false;
         // 
         // pipeFontBrowseButton
         // 
         this.pipeFontBrowseButton.Location = new System.Drawing.Point(245, 19);
         this.pipeFontBrowseButton.Name = "pipeFontBrowseButton";
         this.pipeFontBrowseButton.Size = new System.Drawing.Size(25, 23);
         this.pipeFontBrowseButton.TabIndex = 18;
         this.pipeFontBrowseButton.Text = "...";
         this.pipeFontBrowseButton.UseVisualStyleBackColor = true;
         this.pipeFontBrowseButton.Click += new System.EventHandler(this.pipeFontBrowseButton_Click);
         // 
         // pipeFontTextBox
         // 
         this.pipeFontTextBox.Location = new System.Drawing.Point(90, 21);
         this.pipeFontTextBox.Name = "pipeFontTextBox";
         this.pipeFontTextBox.Size = new System.Drawing.Size(149, 20);
         this.pipeFontTextBox.TabIndex = 17;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(13, 24);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(55, 13);
         this.label2.TabIndex = 16;
         this.label2.Text = "Text Font";
         // 
         // commandAutoCompletionCheckBox
         // 
         this.commandAutoCompletionCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.commandAutoCompletionCheckBox.Location = new System.Drawing.Point(13, 48);
         this.commandAutoCompletionCheckBox.Name = "commandAutoCompletionCheckBox";
         this.commandAutoCompletionCheckBox.Size = new System.Drawing.Size(257, 24);
         this.commandAutoCompletionCheckBox.TabIndex = 19;
         this.commandAutoCompletionCheckBox.Text = "Auto-complete pipe commands";
         this.commandAutoCompletionCheckBox.UseVisualStyleBackColor = true;
         // 
         // IOTextTabPage
         // 
         this.IOTextTabPage.BackColor = System.Drawing.SystemColors.Control;
         this.IOTextTabPage.Controls.Add(this.textWrappingCheckBox);
         this.IOTextTabPage.Controls.Add(this.textFontTextBox);
         this.IOTextTabPage.Controls.Add(this.label3);
         this.IOTextTabPage.Controls.Add(this.textFontBrowseButton);
         this.IOTextTabPage.Location = new System.Drawing.Point(4, 22);
         this.IOTextTabPage.Name = "IOTextTabPage";
         this.IOTextTabPage.Padding = new System.Windows.Forms.Padding(3);
         this.IOTextTabPage.Size = new System.Drawing.Size(284, 204);
         this.IOTextTabPage.TabIndex = 2;
         this.IOTextTabPage.Text = "I/O Text";
         this.IOTextTabPage.UseVisualStyleBackColor = false;
         // 
         // textWrappingCheckBox
         // 
         this.textWrappingCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
         this.textWrappingCheckBox.Location = new System.Drawing.Point(13, 48);
         this.textWrappingCheckBox.Name = "textWrappingCheckBox";
         this.textWrappingCheckBox.Size = new System.Drawing.Size(257, 24);
         this.textWrappingCheckBox.TabIndex = 20;
         this.textWrappingCheckBox.Text = "Wrap Text";
         this.textWrappingCheckBox.UseVisualStyleBackColor = true;
         // 
         // textFontTextBox
         // 
         this.textFontTextBox.Location = new System.Drawing.Point(90, 21);
         this.textFontTextBox.Name = "textFontTextBox";
         this.textFontTextBox.Size = new System.Drawing.Size(149, 20);
         this.textFontTextBox.TabIndex = 13;
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(13, 24);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(55, 13);
         this.label3.TabIndex = 10;
         this.label3.Text = "Text Font";
         // 
         // textFontBrowseButton
         // 
         this.textFontBrowseButton.Location = new System.Drawing.Point(245, 19);
         this.textFontBrowseButton.Name = "textFontBrowseButton";
         this.textFontBrowseButton.Size = new System.Drawing.Size(25, 23);
         this.textFontBrowseButton.TabIndex = 16;
         this.textFontBrowseButton.Text = "...";
         this.textFontBrowseButton.UseVisualStyleBackColor = true;
         this.textFontBrowseButton.Click += new System.EventHandler(this.textFontBrowseButton_Click);
         // 
         // ErrorsTabPage
         // 
         this.ErrorsTabPage.BackColor = System.Drawing.SystemColors.Control;
         this.ErrorsTabPage.Controls.Add(this.errorsFontBrowseButton);
         this.ErrorsTabPage.Controls.Add(this.errorsFontTextBox);
         this.ErrorsTabPage.Controls.Add(this.label4);
         this.ErrorsTabPage.Location = new System.Drawing.Point(4, 22);
         this.ErrorsTabPage.Name = "ErrorsTabPage";
         this.ErrorsTabPage.Padding = new System.Windows.Forms.Padding(3);
         this.ErrorsTabPage.Size = new System.Drawing.Size(284, 204);
         this.ErrorsTabPage.TabIndex = 4;
         this.ErrorsTabPage.Text = "Errors";
         this.ErrorsTabPage.UseVisualStyleBackColor = false;
         // 
         // errorsFontBrowseButton
         // 
         this.errorsFontBrowseButton.Location = new System.Drawing.Point(245, 19);
         this.errorsFontBrowseButton.Name = "errorsFontBrowseButton";
         this.errorsFontBrowseButton.Size = new System.Drawing.Size(25, 23);
         this.errorsFontBrowseButton.TabIndex = 20;
         this.errorsFontBrowseButton.Text = "...";
         this.errorsFontBrowseButton.UseVisualStyleBackColor = true;
         this.errorsFontBrowseButton.Click += new System.EventHandler(this.errorsFontBrowseButton_Click);
         // 
         // errorsFontTextBox
         // 
         this.errorsFontTextBox.Location = new System.Drawing.Point(90, 21);
         this.errorsFontTextBox.Name = "errorsFontTextBox";
         this.errorsFontTextBox.Size = new System.Drawing.Size(149, 20);
         this.errorsFontTextBox.TabIndex = 19;
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(13, 24);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(61, 13);
         this.label4.TabIndex = 18;
         this.label4.Text = "Errors Font";
         // 
         // debugTabPage
         // 
         this.debugTabPage.BackColor = System.Drawing.SystemColors.Control;
         this.debugTabPage.Controls.Add(this.debugFileTextBox);
         this.debugTabPage.Controls.Add(this.label1);
         //this.debugTabPage.Controls.Add(this.debuggingCheckBox);
         this.debugTabPage.Location = new System.Drawing.Point(4, 22);
         this.debugTabPage.Name = "debugTabPage";
         this.debugTabPage.Padding = new System.Windows.Forms.Padding(3);
         this.debugTabPage.Size = new System.Drawing.Size(284, 204);
         this.debugTabPage.TabIndex = 1;
         this.debugTabPage.Text = "Debug";
         this.debugTabPage.UseVisualStyleBackColor = false;
         // 
         // debugFileTextBox
         // 
         this.debugFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         //this.debugFileTextBox.Location = new System.Drawing.Point(90, 46);
         this.debugFileTextBox.Location = new System.Drawing.Point(90, 21);
         this.debugFileTextBox.Name = "debugFileTextBox";
         this.debugFileTextBox.Size = new System.Drawing.Size(175, 20);
         this.debugFileTextBox.TabIndex = 8;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         //this.label1.Location = new System.Drawing.Point(13, 49);
         this.label1.Location = new System.Drawing.Point(13, 24);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(61, 13);
         this.label1.TabIndex = 7;
         this.label1.Text = "Debug File";
//         // 
//         // debuggingCheckBox
//         // 
//         this.debuggingCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
//         this.debuggingCheckBox.Location = new System.Drawing.Point(13, 21);
//         this.debuggingCheckBox.Name = "debuggingCheckBox";
//         this.debuggingCheckBox.Size = new System.Drawing.Size(252, 20);
//         this.debuggingCheckBox.TabIndex = 6;
//         this.debuggingCheckBox.Text = "Debugging";
//         this.debuggingCheckBox.UseVisualStyleBackColor = true;
         // 
         // cancelButton
         // 
         this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cancelButton.Location = new System.Drawing.Point(129, 236);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 24);
         this.cancelButton.TabIndex = 1;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         // 
         // okButton
         // 
         this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.okButton.Location = new System.Drawing.Point(210, 236);
         this.okButton.Name = "okButton";
         this.okButton.Size = new System.Drawing.Size(75, 24);
         this.okButton.TabIndex = 2;
         this.okButton.Text = "Ok";
         this.okButton.UseVisualStyleBackColor = true;
         // 
         // PrefWin
         // 
         this.AcceptButton = this.okButton;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.cancelButton;
         this.ClientSize = new System.Drawing.Size(292, 266);
         this.Controls.Add(this.okButton);
         this.Controls.Add(this.cancelButton);
         this.Controls.Add(this.prefTabControl);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "PrefWin";
         this.Text = "Edit Preferences";
         this.prefTabControl.ResumeLayout(false);
         this.PipeTabPage.ResumeLayout(false);
         this.PipeTabPage.PerformLayout();
         this.IOTextTabPage.ResumeLayout(false);
         this.IOTextTabPage.PerformLayout();
         this.ErrorsTabPage.ResumeLayout(false);
         this.ErrorsTabPage.PerformLayout();
         this.debugTabPage.ResumeLayout(false);
         this.debugTabPage.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TabControl prefTabControl;
      private System.Windows.Forms.TabPage debugTabPage;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.Button okButton;
      private System.Windows.Forms.TextBox debugFileTextBox;
      private System.Windows.Forms.Label label1;
      //private System.Windows.Forms.CheckBox debuggingCheckBox;
      private System.Windows.Forms.TabPage IOTextTabPage;
      private System.Windows.Forms.Button textFontBrowseButton;
      private System.Windows.Forms.TextBox textFontTextBox;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TabPage PipeTabPage;
      private System.Windows.Forms.Button pipeFontBrowseButton;
      private System.Windows.Forms.TextBox pipeFontTextBox;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TabPage ErrorsTabPage;
      private System.Windows.Forms.Button errorsFontBrowseButton;
      private System.Windows.Forms.TextBox errorsFontTextBox;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.CheckBox textWrappingCheckBox;
      private System.Windows.Forms.CheckBox commandAutoCompletionCheckBox; 
   }
}

