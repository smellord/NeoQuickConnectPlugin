namespace QuickConnectPlugin {
    partial class FormPasswordChanger {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.numericUpDownPasswordLength = new System.Windows.Forms.NumericUpDown();
            this.comboBoxPasswordComplexity = new System.Windows.Forms.ComboBox();
            this.labelPasswordLength = new System.Windows.Forms.Label();
            this.labelPasswordComplexity = new System.Windows.Forms.Label();
            this.buttonGeneratePassword = new System.Windows.Forms.Button();
            this.buttonShowHidePassword = new System.Windows.Forms.Button();
            this.maskedTextBoxRepeatNewPassword = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxNewPassword = new System.Windows.Forms.MaskedTextBox();
            this.labelRepeatNewPassword = new System.Windows.Forms.Label();
            this.labelNewPassword = new System.Windows.Forms.Label();
            this.buttonSaveToEntry = new System.Windows.Forms.Button();
            this.buttonChangePassword = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPasswordLength)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.numericUpDownPasswordLength);
            this.groupBox.Controls.Add(this.comboBoxPasswordComplexity);
            this.groupBox.Controls.Add(this.labelPasswordLength);
            this.groupBox.Controls.Add(this.labelPasswordComplexity);
            this.groupBox.Controls.Add(this.buttonGeneratePassword);
            this.groupBox.Controls.Add(this.buttonShowHidePassword);
            this.groupBox.Controls.Add(this.maskedTextBoxRepeatNewPassword);
            this.groupBox.Controls.Add(this.maskedTextBoxNewPassword);
            this.groupBox.Controls.Add(this.labelRepeatNewPassword);
            this.groupBox.Controls.Add(this.labelNewPassword);
            this.groupBox.Location = new System.Drawing.Point(11, 12);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(409, 139);
            this.groupBox.TabIndex = 5;
            this.groupBox.TabStop = false;
            // 
            // numericUpDownPasswordLength
            // 
            this.numericUpDownPasswordLength.Location = new System.Drawing.Point(131, 106);
            this.numericUpDownPasswordLength.Name = "numericUpDownPasswordLength";
            this.numericUpDownPasswordLength.Size = new System.Drawing.Size(60, 20);
            this.numericUpDownPasswordLength.TabIndex = 11;
            // 
            // comboBoxPasswordComplexity
            // 
            this.comboBoxPasswordComplexity.FormattingEnabled = true;
            this.comboBoxPasswordComplexity.Location = new System.Drawing.Point(131, 78);
            this.comboBoxPasswordComplexity.Name = "comboBoxPasswordComplexity";
            this.comboBoxPasswordComplexity.Size = new System.Drawing.Size(143, 21);
            this.comboBoxPasswordComplexity.TabIndex = 9;
            // 
            // labelPasswordLength
            // 
            this.labelPasswordLength.AutoSize = true;
            this.labelPasswordLength.Location = new System.Drawing.Point(35, 108);
            this.labelPasswordLength.Name = "labelPasswordLength";
            this.labelPasswordLength.Size = new System.Drawing.Size(90, 13);
            this.labelPasswordLength.TabIndex = 10;
            this.labelPasswordLength.Text = "Password length:";
            // 
            // labelPasswordComplexity
            // 
            this.labelPasswordComplexity.AutoSize = true;
            this.labelPasswordComplexity.Location = new System.Drawing.Point(25, 81);
            this.labelPasswordComplexity.Name = "labelPasswordComplexity";
            this.labelPasswordComplexity.Size = new System.Drawing.Size(100, 13);
            this.labelPasswordComplexity.TabIndex = 8;
            this.labelPasswordComplexity.Text = "Complexity grade:";
            // 
            // buttonGeneratePassword
            // 
            this.buttonGeneratePassword.Location = new System.Drawing.Point(280, 78);
            this.buttonGeneratePassword.Name = "buttonGeneratePassword";
            this.buttonGeneratePassword.Size = new System.Drawing.Size(114, 48);
            this.buttonGeneratePassword.TabIndex = 12;
            this.buttonGeneratePassword.Text = "Generate Password";
            this.buttonGeneratePassword.UseVisualStyleBackColor = true;
            this.buttonGeneratePassword.Click += new System.EventHandler(this.generatePasswordClick);
            // 
            // buttonShowHidePassword
            // 
            this.buttonShowHidePassword.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonShowHidePassword.Location = new System.Drawing.Point(280, 19);
            this.buttonShowHidePassword.Name = "buttonShowHidePassword";
            this.buttonShowHidePassword.Size = new System.Drawing.Size(34, 20);
            this.buttonShowHidePassword.TabIndex = 12;
            this.buttonShowHidePassword.Text = "···";
            this.buttonShowHidePassword.UseVisualStyleBackColor = true;
            this.buttonShowHidePassword.Click += new System.EventHandler(this.buttonShowHidePasswordClick);
            // 
            // maskedTextBoxRepeatNewPassword
            // 
            this.maskedTextBoxRepeatNewPassword.Location = new System.Drawing.Point(131, 45);
            this.maskedTextBoxRepeatNewPassword.Name = "maskedTextBoxRepeatNewPassword";
            this.maskedTextBoxRepeatNewPassword.Size = new System.Drawing.Size(143, 20);
            this.maskedTextBoxRepeatNewPassword.TabIndex = 7;
            this.maskedTextBoxRepeatNewPassword.UseSystemPasswordChar = true;
            // 
            // maskedTextBoxNewPassword
            // 
            this.maskedTextBoxNewPassword.Location = new System.Drawing.Point(131, 19);
            this.maskedTextBoxNewPassword.Name = "maskedTextBoxNewPassword";
            this.maskedTextBoxNewPassword.Size = new System.Drawing.Size(143, 20);
            this.maskedTextBoxNewPassword.TabIndex = 6;
            this.maskedTextBoxNewPassword.UseSystemPasswordChar = true;
            // 
            // labelRepeatNewPassword
            // 
            this.labelRepeatNewPassword.AutoSize = true;
            this.labelRepeatNewPassword.Location = new System.Drawing.Point(9, 48);
            this.labelRepeatNewPassword.Name = "labelRepeatNewPassword";
            this.labelRepeatNewPassword.Size = new System.Drawing.Size(116, 13);
            this.labelRepeatNewPassword.TabIndex = 5;
            this.labelRepeatNewPassword.Text = "Repeat new password:";
            // 
            // labelNewPassword
            // 
            this.labelNewPassword.AutoSize = true;
            this.labelNewPassword.Location = new System.Drawing.Point(45, 22);
            this.labelNewPassword.Name = "labelNewPassword";
            this.labelNewPassword.Size = new System.Drawing.Size(80, 13);
            this.labelNewPassword.TabIndex = 4;
            this.labelNewPassword.Text = "New password:";
            // 
            // buttonSaveToEntry
            // 
            this.buttonSaveToEntry.Location = new System.Drawing.Point(220, 157);
            this.buttonSaveToEntry.Name = "buttonSaveToEntry";
            this.buttonSaveToEntry.Size = new System.Drawing.Size(110, 25);
            this.buttonSaveToEntry.TabIndex = 13;
            this.buttonSaveToEntry.Text = "Save To Entry";
            this.buttonSaveToEntry.UseVisualStyleBackColor = true;
            this.buttonSaveToEntry.Click += new System.EventHandler(this.savePasswordToEntryClick);
            // 
            // buttonChangePassword
            // 
            this.buttonChangePassword.Location = new System.Drawing.Point(101, 157);
            this.buttonChangePassword.Name = "buttonChangePassword";
            this.buttonChangePassword.Size = new System.Drawing.Size(110, 25);
            this.buttonChangePassword.TabIndex = 14;
            this.buttonChangePassword.Text = "Change Password";
            this.buttonChangePassword.UseVisualStyleBackColor = true;
            this.buttonChangePassword.Click += new System.EventHandler(this.changePasswordClick);
            // 
            // FormPasswordChanger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 193);
            this.Controls.Add(this.buttonSaveToEntry);
            this.Controls.Add(this.buttonChangePassword);
            this.Controls.Add(this.groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPasswordChanger";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Password ({})";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPasswordLength)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button buttonShowHidePassword;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxRepeatNewPassword;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxNewPassword;
        private System.Windows.Forms.Label labelRepeatNewPassword;
        private System.Windows.Forms.Label labelNewPassword;
        private System.Windows.Forms.Button buttonGeneratePassword;
        private System.Windows.Forms.Button buttonSaveToEntry;
        private System.Windows.Forms.Button buttonChangePassword;
        private System.Windows.Forms.ComboBox comboBoxPasswordComplexity;
        private System.Windows.Forms.Label labelPasswordComplexity;
        private System.Windows.Forms.Label labelPasswordLength;
        private System.Windows.Forms.NumericUpDown numericUpDownPasswordLength;
    }
}
