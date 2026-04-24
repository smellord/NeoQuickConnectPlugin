namespace QuickConnectPlugin {
    partial class FormPasswordChangeSuccess {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.labelMessage = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonToggleDetails = new System.Windows.Forms.Button();
            this.textBoxDetails = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.Location = new System.Drawing.Point(12, 9);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(380, 53);
            this.labelMessage.TabIndex = 0;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(317, 74);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOkClick);
            // 
            // buttonToggleDetails
            // 
            this.buttonToggleDetails.Location = new System.Drawing.Point(12, 74);
            this.buttonToggleDetails.Name = "buttonToggleDetails";
            this.buttonToggleDetails.Size = new System.Drawing.Size(127, 23);
            this.buttonToggleDetails.TabIndex = 2;
            this.buttonToggleDetails.Text = "> View server log";
            this.buttonToggleDetails.UseVisualStyleBackColor = true;
            this.buttonToggleDetails.Click += new System.EventHandler(this.buttonToggleDetailsClick);
            // 
            // textBoxDetails
            // 
            this.textBoxDetails.Location = new System.Drawing.Point(12, 112);
            this.textBoxDetails.Multiline = true;
            this.textBoxDetails.Name = "textBoxDetails";
            this.textBoxDetails.ReadOnly = true;
            this.textBoxDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDetails.Size = new System.Drawing.Size(380, 137);
            this.textBoxDetails.TabIndex = 3;
            // 
            // FormPasswordChangeSuccess
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 261);
            this.Controls.Add(this.textBoxDetails);
            this.Controls.Add(this.buttonToggleDetails);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPasswordChangeSuccess";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Password Changed";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonToggleDetails;
        private System.Windows.Forms.TextBox textBoxDetails;
    }
}
