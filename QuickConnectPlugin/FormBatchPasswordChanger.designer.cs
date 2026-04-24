using System.Windows.Forms;

namespace QuickConnectPlugin {

    partial class FormBatchPasswordChanger {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.groupBoxRules = new System.Windows.Forms.GroupBox();
            this.groupBoxManual = new System.Windows.Forms.GroupBox();
            this.labelManualModeHint = new System.Windows.Forms.Label();
            this.flowLayoutPanelManualEntries = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.groupBoxAutomatic = new System.Windows.Forms.GroupBox();
            this.numericUpDownAutomaticPasswordLength = new System.Windows.Forms.NumericUpDown();
            this.comboBoxAutomaticComplexity = new System.Windows.Forms.ComboBox();
            this.labelAutomaticLength = new System.Windows.Forms.Label();
            this.labelAutomaticComplexity = new System.Windows.Forms.Label();
            this.radioButtonAutomatic = new System.Windows.Forms.RadioButton();
            this.labelSelectionSummary = new System.Windows.Forms.Label();
            this.buttonDeselectAll = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.username = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.password = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ipAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hostType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.passwordExpiresIn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.buttonStartChangePasswords = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveLogAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClearLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.showPasswordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxRules.SuspendLayout();
            this.groupBoxManual.SuspendLayout();
            this.groupBoxAutomatic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutomaticPasswordLength)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Location = new System.Drawing.Point(12, 27);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBoxRules);
            this.splitContainer.Panel2.Controls.Add(this.labelSelectionSummary);
            this.splitContainer.Panel2.Controls.Add(this.buttonDeselectAll);
            this.splitContainer.Panel2.Controls.Add(this.buttonSelectAll);
            this.splitContainer.Panel2.Controls.Add(this.listView);
            this.splitContainer.Size = new System.Drawing.Size(1076, 438);
            this.splitContainer.SplitterDistance = 249;
            this.splitContainer.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Location = new System.Drawing.Point(3, 3);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(243, 432);
            this.treeView.TabIndex = 0;
            // 
            // groupBoxRules
            // 
            this.groupBoxRules.Controls.Add(this.groupBoxManual);
            this.groupBoxRules.Controls.Add(this.radioButtonManual);
            this.groupBoxRules.Controls.Add(this.groupBoxAutomatic);
            this.groupBoxRules.Controls.Add(this.radioButtonAutomatic);
            this.groupBoxRules.Location = new System.Drawing.Point(3, 205);
            this.groupBoxRules.Name = "groupBoxRules";
            this.groupBoxRules.Size = new System.Drawing.Size(817, 230);
            this.groupBoxRules.TabIndex = 4;
            this.groupBoxRules.TabStop = false;
            this.groupBoxRules.Text = "Batch Change Rules";
            // 
            // groupBoxManual
            // 
            this.groupBoxManual.Controls.Add(this.labelManualModeHint);
            this.groupBoxManual.Controls.Add(this.flowLayoutPanelManualEntries);
            this.groupBoxManual.Location = new System.Drawing.Point(9, 109);
            this.groupBoxManual.Name = "groupBoxManual";
            this.groupBoxManual.Size = new System.Drawing.Size(800, 114);
            this.groupBoxManual.TabIndex = 3;
            this.groupBoxManual.TabStop = false;
            this.groupBoxManual.Text = "Manual Passwords Per Selected Server";
            // 
            // labelManualModeHint
            // 
            this.labelManualModeHint.AutoSize = true;
            this.labelManualModeHint.Location = new System.Drawing.Point(13, 24);
            this.labelManualModeHint.Name = "labelManualModeHint";
            this.labelManualModeHint.Size = new System.Drawing.Size(248, 13);
            this.labelManualModeHint.TabIndex = 1;
            this.labelManualModeHint.Text = "Check one or more servers above to configure them.";
            // 
            // flowLayoutPanelManualEntries
            // 
            this.flowLayoutPanelManualEntries.AutoScroll = true;
            this.flowLayoutPanelManualEntries.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelManualEntries.Location = new System.Drawing.Point(6, 19);
            this.flowLayoutPanelManualEntries.Name = "flowLayoutPanelManualEntries";
            this.flowLayoutPanelManualEntries.Size = new System.Drawing.Size(788, 89);
            this.flowLayoutPanelManualEntries.TabIndex = 0;
            this.flowLayoutPanelManualEntries.WrapContents = false;
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(9, 89);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(135, 17);
            this.radioButtonManual.TabIndex = 2;
            this.radioButtonManual.Text = "Set passwords manually";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // groupBoxAutomatic
            // 
            this.groupBoxAutomatic.Controls.Add(this.numericUpDownAutomaticPasswordLength);
            this.groupBoxAutomatic.Controls.Add(this.comboBoxAutomaticComplexity);
            this.groupBoxAutomatic.Controls.Add(this.labelAutomaticLength);
            this.groupBoxAutomatic.Controls.Add(this.labelAutomaticComplexity);
            this.groupBoxAutomatic.Location = new System.Drawing.Point(9, 33);
            this.groupBoxAutomatic.Name = "groupBoxAutomatic";
            this.groupBoxAutomatic.Size = new System.Drawing.Size(800, 48);
            this.groupBoxAutomatic.TabIndex = 1;
            this.groupBoxAutomatic.TabStop = false;
            this.groupBoxAutomatic.Text = "Automatic Generation Settings";
            // 
            // numericUpDownAutomaticPasswordLength
            // 
            this.numericUpDownAutomaticPasswordLength.Location = new System.Drawing.Point(368, 18);
            this.numericUpDownAutomaticPasswordLength.Name = "numericUpDownAutomaticPasswordLength";
            this.numericUpDownAutomaticPasswordLength.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownAutomaticPasswordLength.TabIndex = 3;
            // 
            // comboBoxAutomaticComplexity
            // 
            this.comboBoxAutomaticComplexity.FormattingEnabled = true;
            this.comboBoxAutomaticComplexity.Location = new System.Drawing.Point(90, 17);
            this.comboBoxAutomaticComplexity.Name = "comboBoxAutomaticComplexity";
            this.comboBoxAutomaticComplexity.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAutomaticComplexity.TabIndex = 1;
            // 
            // labelAutomaticLength
            // 
            this.labelAutomaticLength.AutoSize = true;
            this.labelAutomaticLength.Location = new System.Drawing.Point(259, 20);
            this.labelAutomaticLength.Name = "labelAutomaticLength";
            this.labelAutomaticLength.Size = new System.Drawing.Size(90, 13);
            this.labelAutomaticLength.TabIndex = 2;
            this.labelAutomaticLength.Text = "Password length:";
            // 
            // labelAutomaticComplexity
            // 
            this.labelAutomaticComplexity.AutoSize = true;
            this.labelAutomaticComplexity.Location = new System.Drawing.Point(16, 20);
            this.labelAutomaticComplexity.Name = "labelAutomaticComplexity";
            this.labelAutomaticComplexity.Size = new System.Drawing.Size(60, 13);
            this.labelAutomaticComplexity.TabIndex = 0;
            this.labelAutomaticComplexity.Text = "Complexity:";
            // 
            // radioButtonAutomatic
            // 
            this.radioButtonAutomatic.AutoSize = true;
            this.radioButtonAutomatic.Checked = true;
            this.radioButtonAutomatic.Location = new System.Drawing.Point(9, 16);
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.Size = new System.Drawing.Size(244, 17);
            this.radioButtonAutomatic.TabIndex = 0;
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.Text = "Automatic password change with random passwords";
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
            // 
            // labelSelectionSummary
            // 
            this.labelSelectionSummary.AutoSize = true;
            this.labelSelectionSummary.Location = new System.Drawing.Point(3, 180);
            this.labelSelectionSummary.Name = "labelSelectionSummary";
            this.labelSelectionSummary.Size = new System.Drawing.Size(78, 13);
            this.labelSelectionSummary.TabIndex = 3;
            this.labelSelectionSummary.Text = "Checked: 0 / 0";
            // 
            // buttonDeselectAll
            // 
            this.buttonDeselectAll.Location = new System.Drawing.Point(98, 175);
            this.buttonDeselectAll.Name = "buttonDeselectAll";
            this.buttonDeselectAll.Size = new System.Drawing.Size(90, 23);
            this.buttonDeselectAll.TabIndex = 2;
            this.buttonDeselectAll.Text = "Deselect All";
            this.buttonDeselectAll.UseVisualStyleBackColor = true;
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(3, 175);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(89, 23);
            this.buttonSelectAll.TabIndex = 1;
            this.buttonSelectAll.Text = "Select All";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            // 
            // listView
            // 
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.title,
            this.username,
            this.password,
            this.ipAddress,
            this.hostType,
            this.passwordExpiresIn});
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(3, 3);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(817, 166);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // title
            // 
            this.title.Text = "Title";
            this.title.Width = 140;
            // 
            // username
            // 
            this.username.Text = "Username";
            this.username.Width = 120;
            // 
            // password
            // 
            this.password.Text = "Password";
            this.password.Width = 120;
            // 
            // ipAddress
            // 
            this.ipAddress.Text = "IP Address";
            this.ipAddress.Width = 150;
            // 
            // hostType
            // 
            this.hostType.Text = "Host Type";
            this.hostType.Width = 90;
            // 
            // passwordExpiresIn
            // 
            this.passwordExpiresIn.Text = "Psw Expires In";
            this.passwordExpiresIn.Width = 110;
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Location = new System.Drawing.Point(15, 471);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(1073, 122);
            this.richTextBoxLog.TabIndex = 1;
            this.richTextBoxLog.Text = "";
            // 
            // buttonStartChangePasswords
            // 
            this.buttonStartChangePasswords.Location = new System.Drawing.Point(950, 599);
            this.buttonStartChangePasswords.Name = "buttonStartChangePasswords";
            this.buttonStartChangePasswords.Size = new System.Drawing.Size(138, 25);
            this.buttonStartChangePasswords.TabIndex = 3;
            this.buttonStartChangePasswords.Text = "Start Change Passwords";
            this.buttonStartChangePasswords.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemLog,
            this.toolStripMenuItemView});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1100, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // toolStripMenuItemLog
            // 
            this.toolStripMenuItemLog.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSaveLogAs,
            this.toolStripMenuItemClearLog});
            this.toolStripMenuItemLog.Name = "toolStripMenuItemLog";
            this.toolStripMenuItemLog.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItemLog.Text = "&File";
            // 
            // toolStripMenuItemSaveLogAs
            // 
            this.toolStripMenuItemSaveLogAs.Name = "toolStripMenuItemSaveLogAs";
            this.toolStripMenuItemSaveLogAs.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemSaveLogAs.Text = "&Save Log As...";
            // 
            // toolStripMenuItemClearLog
            // 
            this.toolStripMenuItemClearLog.Name = "toolStripMenuItemClearLog";
            this.toolStripMenuItemClearLog.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemClearLog.Text = "Clear Log";
            // 
            // toolStripMenuItemView
            // 
            this.toolStripMenuItemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showPasswordsToolStripMenuItem});
            this.toolStripMenuItemView.Name = "toolStripMenuItemView";
            this.toolStripMenuItemView.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItemView.Text = "View";
            // 
            // showPasswordsToolStripMenuItem
            // 
            this.showPasswordsToolStripMenuItem.CheckOnClick = true;
            this.showPasswordsToolStripMenuItem.Name = "showPasswordsToolStripMenuItem";
            this.showPasswordsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.showPasswordsToolStripMenuItem.Text = "Show Passwords";
            this.showPasswordsToolStripMenuItem.Click += new System.EventHandler(this.showPasswordsClick);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 599);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(929, 25);
            this.progressBar.TabIndex = 2;
            // 
            // FormBatchPasswordChanger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 636);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonStartChangePasswords);
            this.Controls.Add(this.richTextBoxLog);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBatchPasswordChanger";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Password Changer";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxRules.ResumeLayout(false);
            this.groupBoxRules.PerformLayout();
            this.groupBoxManual.ResumeLayout(false);
            this.groupBoxManual.PerformLayout();
            this.groupBoxAutomatic.ResumeLayout(false);
            this.groupBoxAutomatic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutomaticPasswordLength)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader title;
        private System.Windows.Forms.ColumnHeader username;
        private System.Windows.Forms.ColumnHeader password;
        private System.Windows.Forms.ColumnHeader ipAddress;
        private System.Windows.Forms.ColumnHeader hostType;
        private System.Windows.Forms.ColumnHeader passwordExpiresIn;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Button buttonStartChangePasswords;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLog;
        private ToolStripMenuItem toolStripMenuItemSaveLogAs;
        private ToolStripMenuItem toolStripMenuItemClearLog;
        private ProgressBar progressBar;
        private ToolStripMenuItem toolStripMenuItemView;
        private ToolStripMenuItem showPasswordsToolStripMenuItem;
        private GroupBox groupBoxRules;
        private RadioButton radioButtonAutomatic;
        private GroupBox groupBoxAutomatic;
        private Label labelAutomaticComplexity;
        private Label labelAutomaticLength;
        private ComboBox comboBoxAutomaticComplexity;
        private NumericUpDown numericUpDownAutomaticPasswordLength;
        private RadioButton radioButtonManual;
        private GroupBox groupBoxManual;
        private FlowLayoutPanel flowLayoutPanelManualEntries;
        private Label labelManualModeHint;
        private Button buttonSelectAll;
        private Button buttonDeselectAll;
        private Label labelSelectionSummary;
        private ContextMenuStrip listViewContextMenuStrip;
    }
}
