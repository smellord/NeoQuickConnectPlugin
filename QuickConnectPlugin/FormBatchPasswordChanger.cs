using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using QuickConnectPlugin.Commons;
using QuickConnectPlugin.PasswordChanger;
using QuickConnectPlugin.PasswordChanger.Services;

namespace QuickConnectPlugin {

    public partial class FormBatchPasswordChanger : Form {

        private const int DefaultAutomaticPasswordLength = 16;

        private IPasswordChangerServiceFactory pwChangerServiceFactory;
        private BatchPasswordChangerWorker pwChangerWorker;
        private IDictionary<string, ManualPasswordState> manualPasswordStates;

        public bool Changed { get; private set; }

        public FormBatchPasswordChanger(
            IPasswordChangerTreeNode pwChangerTreeNode,
            IPasswordChangerServiceFactory pwChangerServiceFactory
            ) {

            InitializeComponent();

            this.manualPasswordStates = new Dictionary<string, ManualPasswordState>();
            this.pwChangerServiceFactory = pwChangerServiceFactory;

            if (this.FormBorderStyle == FormBorderStyle.Sizable) {
                this.treeView.Dock = DockStyle.Fill;
                this.listView.Dock = DockStyle.Fill;
                this.splitContainer.IsSplitterFixed = false;
            }
            else {
                this.splitContainer.IsSplitterFixed = true;
            }

            this.buttonStartChangePasswords.Enabled = false;
            this.toolStripMenuItemSaveLogAs.Enabled = false;
            this.toolStripMenuItemClearLog.Enabled = false;

            this.comboBoxAutomaticComplexity.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxAutomaticComplexity.Items.Add("Low");
            this.comboBoxAutomaticComplexity.Items.Add("Medium");
            this.comboBoxAutomaticComplexity.Items.Add("High");
            this.comboBoxAutomaticComplexity.SelectedIndex = 2;
            this.numericUpDownAutomaticPasswordLength.Minimum = 4;
            this.numericUpDownAutomaticPasswordLength.Maximum = 128;
            this.numericUpDownAutomaticPasswordLength.Value = DefaultAutomaticPasswordLength;

            this.listView.FullRowSelect = true;

            this.treeView.Nodes.Add(pwChangerTreeNode.Root);
            this.treeView.Nodes[0].Expand();
            this.treeView.AfterSelect += new TreeViewEventHandler(treeViewAfterSelect);

            this.listViewContextMenuStrip = new ContextMenuStrip();
            listViewContextMenuStrip.Items.Add("Select all");
            listViewContextMenuStrip.Items.Add("Deselect all");
            listViewContextMenuStrip.Items[0].Click += new EventHandler(selectAllClick);
            listViewContextMenuStrip.Items[1].Click += new EventHandler(deselectAllClick);
            this.listView.ContextMenuStrip = listViewContextMenuStrip;

            this.toolStripMenuItemSaveLogAs.Click += new EventHandler(saveLogAsClick);
            this.toolStripMenuItemClearLog.Click += new EventHandler(clearLogClick);
            this.buttonStartChangePasswords.Click += new EventHandler(startChangePasswordsClick);
            this.buttonSelectAll.Click += new EventHandler(selectAllClick);
            this.buttonDeselectAll.Click += new EventHandler(deselectAllClick);
            this.richTextBoxLog.TextChanged += new EventHandler(logTextChanged);
            this.radioButtonAutomatic.CheckedChanged += new EventHandler(batchRuleCheckedChanged);
            this.radioButtonManual.CheckedChanged += new EventHandler(batchRuleCheckedChanged);
            this.comboBoxAutomaticComplexity.SelectedIndexChanged += new EventHandler(checkControls);
            this.numericUpDownAutomaticPasswordLength.ValueChanged += new EventHandler(checkControls);
            this.listView.ItemChecked += new ItemCheckedEventHandler(listViewItemChecked);

            this.FormClosing += new FormClosingEventHandler(formClosing);
            this.KeyDown += new KeyEventHandler(form_KeyPress);

            this.treeView.SelectedNode = this.treeView.Nodes[0];
            this.updateRuleModeState();
            this.refreshSelectionSummary();
            this.refreshManualPasswordPanel();
            this.checkControls();
        }

        private bool showPasswordIsChecked() {
            return this.showPasswordsToolStripMenuItem != null && this.showPasswordsToolStripMenuItem.Checked;
        }

        private void toggleControls(bool state) {
            this.treeView.Enabled = state;
            this.listView.Enabled = state;
            this.buttonSelectAll.Enabled = state;
            this.buttonDeselectAll.Enabled = state;
            this.radioButtonAutomatic.Enabled = state;
            this.radioButtonManual.Enabled = state;
            this.comboBoxAutomaticComplexity.Enabled = state && this.radioButtonAutomatic.Checked;
            this.numericUpDownAutomaticPasswordLength.Enabled = state && this.radioButtonAutomatic.Checked;
            this.flowLayoutPanelManualEntries.Enabled = state && this.radioButtonManual.Checked;
            this.buttonStartChangePasswords.Enabled = state;
            this.showPasswordsToolStripMenuItem.Enabled = state;
        }

        private void batchRuleCheckedChanged(object sender, EventArgs e) {
            this.updateRuleModeState();
            this.checkControls();
        }

        private void updateRuleModeState() {
            this.groupBoxAutomatic.Enabled = this.radioButtonAutomatic.Checked;
            this.groupBoxManual.Enabled = this.radioButtonManual.Checked;
        }

        private void selectAllClick(object sender, EventArgs e) {
            foreach (ListViewItem item in this.listView.Items) {
                item.Checked = true;
            }
        }

        private void deselectAllClick(object sender, EventArgs e) {
            foreach (ListViewItem item in this.listView.Items) {
                item.Checked = false;
            }
        }

        private void showPasswordsClick(object sender, EventArgs e) {
            foreach (var item in this.listView.Items) {
                PwEntryListViewItem pwEntryItem = item as PwEntryListViewItem;
                if (pwEntryItem != null) {
                    pwEntryItem.UpdatePassword(this.showPasswordIsChecked());
                }
            }
        }

        private void treeViewAfterSelect(object sender, TreeViewEventArgs e) {
            IPasswordChangerTreeNode treeNode = e.Node as IPasswordChangerTreeNode;
            if (treeNode != null) {
                bool showPassword = this.showPasswordIsChecked();
                this.listView.Items.Clear();
                foreach (var pwEntry in treeNode.GetEntries(true)) {
                    PwEntryListViewItem item = new PwEntryListViewItem(pwEntry, showPassword);
                    this.listView.Items.Add(item);
                }
            }
            this.refreshSelectionSummary();
            this.refreshManualPasswordPanel();
            this.checkControls();
        }

        private void saveLogAsClick(object sender, EventArgs e) {
            using (var dialog = new SaveFileDialog()) {
                dialog.Title = "Save Log As";
                dialog.Filter = "Log file (*.log)|*.log";
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = true;
                dialog.FileName = String.Format("{0}-{1:yyyyMMdd}.log", AssemblyUtils.GetExecutingAssemblyName(), DateTime.Now);
                dialog.ShowDialog();
                if (dialog.FileName.Length > 0) {
                    try {
                        File.WriteAllText(dialog.FileName, this.richTextBoxLog.Text);
                        MessageBox.Show("The log file was saved successfully.", "Save Log As",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) {
                        MessageBox.Show(String.Format("Error saving log file.\n\nError details: {0}", ex.Message), "Save Log As",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void clearLogClick(object sender, EventArgs e) {
            this.richTextBoxLog.Clear();
        }

        private void formClosing(object sender, FormClosingEventArgs e) {
            if (this.pwChangerWorker != null && this.pwChangerWorker.IsRunning) {
                MessageBox.Show(
                    "Password changing is currently running. Press Stop button and wait for the current task to finish before closing the form.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                e.Cancel = true;
            }
        }

        private void logTextChanged(object sender, EventArgs e) {
            this.toolStripMenuItemSaveLogAs.Enabled = (this.richTextBoxLog.Text.Length > 0);
            this.toolStripMenuItemClearLog.Enabled = (this.richTextBoxLog.Text.Length > 0);
        }

        private void startChangePasswordsClick(object sender, EventArgs e) {
            if (this.buttonStartChangePasswords.Text.Equals("Stop")) {
                if (this.pwChangerWorker != null && this.pwChangerWorker.IsRunning) {
                    this.buttonStartChangePasswords.Text = "Stopping...";
                    this.buttonStartChangePasswords.Enabled = false;
                    this.pwChangerWorker.Cancel();
                }
            }
            else {
                if (this.pwChangerWorker == null || !this.pwChangerWorker.IsRunning) {
                    var requests = this.buildRequests();
                    if (requests.Count == 0) {
                        return;
                    }

                    this.toggleControls(false);
                    this.progressBar.Value = 0;
                    this.progressBar.Maximum = requests.Count;

                    var passwordChangerService = this.pwChangerServiceFactory.Create(new HostTypeMapper(new HostTypeSafeConverter()));
                    this.pwChangerWorker = new BatchPasswordChangerWorker(passwordChangerService, requests);
                    var thread = new Thread(new ThreadStart(() => runBatchPasswordChangerWorker(passwordChangerService)));
                    thread.Name = "FormBatchPasswordChangerThread";
                    thread.IsBackground = true;
                    thread.Start();
                    this.buttonStartChangePasswords.Text = "Stop";
                    this.buttonStartChangePasswords.Enabled = true;

                    this.logStatus(
                        String.Format("Starting batch password change for {0} server(s) using {1}.",
                            requests.Count,
                            this.radioButtonAutomatic.Checked ? "automatic random passwords" : "manual passwords"
                        ),
                        Color.Black
                    );
                }
            }
        }

        private Collection<BatchPasswordChangeRequest> buildRequests() {
            var selectedEntries = this.getSelectedEntries();
            var requests = new Collection<BatchPasswordChangeRequest>();

            if (this.radioButtonAutomatic.Checked) {
                foreach (var entry in selectedEntries) {
                    requests.Add(
                        new BatchPasswordChangeRequest(
                            entry,
                            PasswordGenerator.Generate(
                                Decimal.ToInt32(this.numericUpDownAutomaticPasswordLength.Value),
                                this.getSelectedAutomaticComplexity()
                            )
                        )
                    );
                }
                return requests;
            }

            foreach (var entry in selectedEntries) {
                var state = this.getManualPasswordState(entry);
                requests.Add(new BatchPasswordChangeRequest(entry, state.Password));
            }

            return requests;
        }

        private Collection<IPasswordChangerHostPwEntry> getSelectedEntries() {
            var entries = new Collection<IPasswordChangerHostPwEntry>();
            foreach (ListViewItem item in this.listView.Items) {
                var pwItem = item as PwEntryListViewItem;
                if (pwItem != null && item.Checked) {
                    entries.Add(pwItem.PwEntry);
                }
            }
            return entries;
        }

        private void runBatchPasswordChangerWorker(IPasswordChangerService passwordChangerService) {
            this.pwChangerWorker.SaveDatabaseAfterEachUpdate = true;
            this.pwChangerWorker.Starting += new PasswordChangeStartingEventHandler(batchPasswordChangerWorkerStarting);
            this.pwChangerWorker.Changed += new PasswordChangedEventHandler(batchPasswordChangerWorkerChanged);
            this.pwChangerWorker.Error += new PasswordChangeErrorEventHandler(batchPasswordChangerWorkerError);
            this.pwChangerWorker.Completed += new PasswordChangeCompletedEventHandler(batchPasswordChangerWorkerCompleted);
            this.pwChangerWorker.Run();
        }

        public void batchPasswordChangerWorkerStarting(object sender, BatchPasswordChangerEventArgs e) {
            this.Invoke((MethodInvoker)delegate {
                this.logStatus(
                    String.Format("[RUNNING] Changing password for {0} on host {1} to {2}.",
                        e.HostPwEntry.GetUsername(),
                        e.HostPwEntry.IPAddress,
                        e.NewPassword
                    ),
                    Color.DarkBlue
                );
            });
        }

        public void batchPasswordChangerWorkerChanged(object sender, BatchPasswordChangerEventArgs e) {
            this.Invoke((MethodInvoker)delegate {
                foreach (var item in this.listView.Items) {
                    PwEntryListViewItem pwEntryItem = item as PwEntryListViewItem;
                    if (pwEntryItem != null && e.HostPwEntry.Equals(pwEntryItem.PwEntry)) {
                        pwEntryItem.UpdatePassword(this.showPasswordIsChecked());
                        pwEntryItem.Checked = false;
                    }
                }

                this.logStatus(
                    String.Format("[OK] Password changed for {0} on host {1} and updated in KeePass to {2}.",
                        e.HostPwEntry.GetUsername(),
                        e.HostPwEntry.IPAddress,
                        e.NewPassword
                    ),
                    Color.DarkGreen
                );

                if (!String.IsNullOrEmpty(e.OperationDetails)) {
                    this.logStatus(String.Format("Server response:{0}{1}", Environment.NewLine, e.OperationDetails), Color.DarkSlateGray);
                }

                this.progressBar.Value = e.ProcessedEntries;
                this.Changed = true;
                this.refreshSelectionSummary();
                this.refreshManualPasswordPanel();
                this.checkControls();
            });
        }

        public void batchPasswordChangerWorkerError(object sender, BatchPasswordChangerErrorEventArgs e) {
            this.Invoke((MethodInvoker)delegate {
                this.logStatus(
                    String.Format("[FAILED] Password change failed for {0} on host {1} to {2}.{3}{4}",
                        e.HostPwEntry.GetUsername(),
                        e.HostPwEntry.IPAddress,
                        e.NewPassword,
                        Environment.NewLine,
                        e.Exception
                    ),
                    Color.DarkRed
                );
                this.progressBar.Value = e.ProcessedEntries;
            });
        }

        private void logStatus(string message, Color color) {
            if (this.richTextBoxLog == null) {
                return;
            }

            var timestampedMessage = String.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, message);
            this.richTextBoxLog.SelectionStart = this.richTextBoxLog.TextLength;
            this.richTextBoxLog.SelectionLength = 0;
            this.richTextBoxLog.SelectionColor = color;
            this.richTextBoxLog.AppendText(timestampedMessage + Environment.NewLine);
            this.richTextBoxLog.SelectionColor = this.richTextBoxLog.ForeColor;
            this.richTextBoxLog.ScrollToCaret();
        }

        public void batchPasswordChangerWorkerCompleted(object sender, EventArgs e) {
            this.pwChangerWorker = null;
            this.Invoke((MethodInvoker)delegate {
                this.toggleControls(true);
                this.checkControls();
                this.buttonStartChangePasswords.Text = "Start Change Passwords";
                this.logStatus("Batch password change completed.", Color.Black);
            });
        }

        private void listViewItemChecked(object sender, ItemCheckedEventArgs e) {
            this.refreshSelectionSummary();
            this.refreshManualPasswordPanel();
            this.checkControls();
        }

        private void refreshSelectionSummary() {
            this.labelSelectionSummary.Text = String.Format(
                "Checked servers in current view: {0} / {1}",
                this.getSelectedEntries().Count,
                this.listView.Items.Count
            );
        }

        private void refreshManualPasswordPanel() {
            this.flowLayoutPanelManualEntries.SuspendLayout();
            this.flowLayoutPanelManualEntries.Controls.Clear();

            var selectedEntries = this.getSelectedEntries();
            this.labelManualModeHint.Visible = (selectedEntries.Count == 0);

            foreach (var entry in selectedEntries) {
                var state = this.getManualPasswordState(entry);
                this.flowLayoutPanelManualEntries.Controls.Add(this.createManualPasswordRow(entry, state));
            }

            this.flowLayoutPanelManualEntries.ResumeLayout();
        }

        private Control createManualPasswordRow(IPasswordChangerHostPwEntry entry, ManualPasswordState state) {
            var panel = new Panel();
            panel.Width = this.flowLayoutPanelManualEntries.ClientSize.Width - 28;
            panel.Height = 58;
            panel.Margin = new Padding(3, 3, 3, 6);
            panel.BorderStyle = BorderStyle.FixedSingle;

            var titleLabel = new Label();
            titleLabel.Location = new Point(8, 8);
            titleLabel.Size = new Size(210, 18);
            titleLabel.Text = String.Format("{0} ({1})", entry.Title, entry.IPAddress);
            titleLabel.AutoEllipsis = true;

            var usernameLabel = new Label();
            usernameLabel.Location = new Point(8, 30);
            usernameLabel.Size = new Size(210, 16);
            usernameLabel.Text = String.Format("User: {0}", entry.GetUsername());
            usernameLabel.AutoEllipsis = true;

            var newPasswordTextBox = new MaskedTextBox();
            newPasswordTextBox.Location = new Point(226, 8);
            newPasswordTextBox.Size = new Size(150, 20);
            newPasswordTextBox.UseSystemPasswordChar = !state.ShowPassword;
            newPasswordTextBox.Text = state.Password ?? string.Empty;
            newPasswordTextBox.TextChanged += delegate(object sender, EventArgs e) {
                state.Password = newPasswordTextBox.Text;
                this.checkControls();
            };

            var repeatPasswordTextBox = new MaskedTextBox();
            repeatPasswordTextBox.Location = new Point(226, 30);
            repeatPasswordTextBox.Size = new Size(150, 20);
            repeatPasswordTextBox.UseSystemPasswordChar = !state.ShowPassword;
            repeatPasswordTextBox.Text = state.RepeatPassword ?? string.Empty;
            repeatPasswordTextBox.TextChanged += delegate(object sender, EventArgs e) {
                state.RepeatPassword = repeatPasswordTextBox.Text;
                this.checkControls();
            };

            var showButton = new Button();
            showButton.Location = new Point(384, 8);
            showButton.Size = new Size(32, 20);
            showButton.Text = "...";
            showButton.Click += delegate(object sender, EventArgs e) {
                state.ShowPassword = !state.ShowPassword;
                newPasswordTextBox.UseSystemPasswordChar = !state.ShowPassword;
                repeatPasswordTextBox.UseSystemPasswordChar = !state.ShowPassword;
            };

            var generateButton = new Button();
            generateButton.Location = new Point(422, 8);
            generateButton.Size = new Size(100, 42);
            generateButton.Text = "Generate 16";
            generateButton.FlatStyle = FlatStyle.Popup;
            generateButton.BackColor = Color.Honeydew;
            generateButton.Click += delegate(object sender, EventArgs e) {
                var generatedPassword = PasswordGenerator.Generate(16, PasswordComplexity.High);
                state.Password = generatedPassword;
                state.RepeatPassword = generatedPassword;
                newPasswordTextBox.Text = generatedPassword;
                repeatPasswordTextBox.Text = generatedPassword;
                this.checkControls();
            };

            var newPasswordLabel = new Label();
            newPasswordLabel.Location = new Point(528, 10);
            newPasswordLabel.Size = new Size(40, 13);
            newPasswordLabel.Text = "New:";

            var repeatPasswordLabel = new Label();
            repeatPasswordLabel.Location = new Point(528, 32);
            repeatPasswordLabel.Size = new Size(48, 13);
            repeatPasswordLabel.Text = "Repeat:";

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(usernameLabel);
            panel.Controls.Add(newPasswordTextBox);
            panel.Controls.Add(repeatPasswordTextBox);
            panel.Controls.Add(showButton);
            panel.Controls.Add(generateButton);
            panel.Controls.Add(newPasswordLabel);
            panel.Controls.Add(repeatPasswordLabel);

            return panel;
        }

        private ManualPasswordState getManualPasswordState(IPasswordChangerHostPwEntry entry) {
            var key = this.getEntryKey(entry);
            ManualPasswordState state = null;
            if (!this.manualPasswordStates.TryGetValue(key, out state)) {
                state = new ManualPasswordState();
                this.manualPasswordStates.Add(key, state);
            }
            return state;
        }

        private string getEntryKey(IHostPwEntry entry) {
            return string.Format("{0}|{1}|{2}", entry.IPAddress, entry.GetUsername(), (entry as IHasTitle) != null ? ((IHasTitle)entry).Title : string.Empty);
        }

        private void checkControls(object sender, EventArgs e) {
            this.checkControls();
        }

        private void checkControls() {
            if (this.pwChangerWorker != null && this.pwChangerWorker.IsRunning) {
                return;
            }

            var hasSelectedEntries = this.getSelectedEntries().Count > 0;
            var hasValidPasswordRule = this.radioButtonAutomatic.Checked || this.hasValidManualPasswords();

            this.buttonStartChangePasswords.Enabled = hasSelectedEntries && hasValidPasswordRule;
        }

        private bool hasValidManualPasswords() {
            foreach (var entry in this.getSelectedEntries()) {
                var state = this.getManualPasswordState(entry);
                if (String.IsNullOrEmpty(state.Password) || !state.Password.Equals(state.RepeatPassword)) {
                    return false;
                }
            }

            return true;
        }

        private PasswordComplexity getSelectedAutomaticComplexity() {
            if (this.comboBoxAutomaticComplexity.SelectedIndex == 0) {
                return PasswordComplexity.Low;
            }
            if (this.comboBoxAutomaticComplexity.SelectedIndex == 1) {
                return PasswordComplexity.Medium;
            }
            return PasswordComplexity.High;
        }

        private void form_KeyPress(object sender, KeyEventArgs e) {
            if (this.pwChangerWorker == null || !this.pwChangerWorker.IsRunning) {
                if (e.KeyCode == Keys.Escape) {
                    this.Close();
                }
            }
        }

        private class ManualPasswordState {
            public string Password { get; set; }
            public string RepeatPassword { get; set; }
            public bool ShowPassword { get; set; }
        }
    }
}
