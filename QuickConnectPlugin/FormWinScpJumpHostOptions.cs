using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KeePassLib;

namespace QuickConnectPlugin {

    public class FormWinScpJumpHostOptions : Form {

        private readonly ComboBox comboBoxPassphraseEntry;
        private readonly ComboBox comboBoxPassphraseField;
        private readonly RadioButton radioButtonEntryPassword;
        private readonly RadioButton radioButtonPageant;
        private readonly RadioButton radioButtonKeePassEntry;
        private readonly RadioButton radioButtonManual;
        private readonly TextBox textBoxHostName;
        private readonly TextBox textBoxPort;
        private readonly TextBox textBoxUsername;
        private readonly TextBox textBoxPrivateKeyPath;
        private readonly TextBox textBoxManualPassphrase;

        public string JumpHostName { get; private set; }
        public string JumpPort { get; private set; }
        public string JumpUsername { get; private set; }
        public string JumpPrivateKeyPath { get; private set; }
        public string PassphraseSource { get; private set; }
        public string ManualPassphrase { get; private set; }
        public string PassphraseEntryUuid { get; private set; }
        public string PassphraseFieldName { get; private set; }

        public FormWinScpJumpHostOptions(
            IQuickConnectPluginSettings settings,
            PwDatabase database,
            ICollection<string> dbFields)
        {
            this.Text = "WinSCP Jump Host";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.ClientSize = new Size(478, 332);

            Label labelHostName = CreateLabel("Jump host", 18, 22, 95);
            this.textBoxHostName = CreateTextBox(118, 19, 220);
            this.textBoxHostName.Text = settings.WinScpJumpHostName;

            Label labelPort = CreateLabel("Port", 355, 22, 35);
            this.textBoxPort = CreateTextBox(392, 19, 52);
            this.textBoxPort.Text = String.IsNullOrEmpty(settings.WinScpJumpPort)
                ? QuickConnectPluginSettings.DefaultWinScpJumpPort
                : settings.WinScpJumpPort;

            Label labelUsername = CreateLabel("Username", 18, 52, 95);
            this.textBoxUsername = CreateTextBox(118, 49, 326);
            this.textBoxUsername.Text = settings.WinScpJumpUsername;

            Label labelPrivateKey = CreateLabel("PPK key", 18, 82, 95);
            this.textBoxPrivateKeyPath = CreateTextBox(118, 79, 238);
            this.textBoxPrivateKeyPath.Text = settings.WinScpJumpPrivateKeyPath;

            Button buttonBrowseKey = new Button();
            buttonBrowseKey.Text = "Browse...";
            buttonBrowseKey.Location = new Point(362, 77);
            buttonBrowseKey.Size = new Size(82, 24);
            buttonBrowseKey.Click += ButtonBrowseKey_Click;

            GroupBox groupBoxPassphrase = new GroupBox();
            groupBoxPassphrase.Text = "PPK Passphrase";
            groupBoxPassphrase.Location = new Point(12, 114);
            groupBoxPassphrase.Size = new Size(452, 167);

            this.radioButtonEntryPassword = new RadioButton();
            this.radioButtonEntryPassword.Text = "Use current KeePass entry password";
            this.radioButtonEntryPassword.Location = new Point(16, 22);
            this.radioButtonEntryPassword.Size = new Size(230, 20);

            this.radioButtonPageant = new RadioButton();
            this.radioButtonPageant.Text = "Use Pageant / prompt";
            this.radioButtonPageant.Location = new Point(16, 47);
            this.radioButtonPageant.Size = new Size(160, 20);

            this.radioButtonKeePassEntry = new RadioButton();
            this.radioButtonKeePassEntry.Text = "Use KeePass entry";
            this.radioButtonKeePassEntry.Location = new Point(16, 72);
            this.radioButtonKeePassEntry.Size = new Size(145, 20);

            this.comboBoxPassphraseEntry = new ComboBox();
            this.comboBoxPassphraseEntry.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxPassphraseEntry.Location = new Point(166, 71);
            this.comboBoxPassphraseEntry.Size = new Size(264, 21);
            LoadEntries(database, settings.WinScpPassphraseEntryUuid);

            Label labelField = CreateLabel("Field", 43, 101, 42);
            this.comboBoxPassphraseField = new ComboBox();
            this.comboBoxPassphraseField.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxPassphraseField.Location = new Point(166, 98);
            this.comboBoxPassphraseField.Size = new Size(264, 21);
            LoadFields(dbFields, settings.WinScpPassphraseFieldName);

            this.radioButtonManual = new RadioButton();
            this.radioButtonManual.Text = "Manual passphrase";
            this.radioButtonManual.Location = new Point(16, 128);
            this.radioButtonManual.Size = new Size(145, 20);

            this.textBoxManualPassphrase = new TextBox();
            this.textBoxManualPassphrase.Location = new Point(166, 127);
            this.textBoxManualPassphrase.Size = new Size(264, 20);
            this.textBoxManualPassphrase.UseSystemPasswordChar = true;
            this.textBoxManualPassphrase.Text = settings.WinScpManualPassphrase;

            groupBoxPassphrase.Controls.Add(this.radioButtonEntryPassword);
            groupBoxPassphrase.Controls.Add(this.radioButtonPageant);
            groupBoxPassphrase.Controls.Add(this.radioButtonKeePassEntry);
            groupBoxPassphrase.Controls.Add(this.comboBoxPassphraseEntry);
            groupBoxPassphrase.Controls.Add(labelField);
            groupBoxPassphrase.Controls.Add(this.comboBoxPassphraseField);
            groupBoxPassphrase.Controls.Add(this.radioButtonManual);
            groupBoxPassphrase.Controls.Add(this.textBoxManualPassphrase);

            Button buttonOK = new Button();
            buttonOK.Text = "OK";
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(308, 295);
            buttonOK.Size = new Size(75, 23);
            buttonOK.Click += ButtonOK_Click;

            Button buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(389, 295);
            buttonCancel.Size = new Size(75, 23);

            this.Controls.Add(labelHostName);
            this.Controls.Add(this.textBoxHostName);
            this.Controls.Add(labelPort);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(labelUsername);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(labelPrivateKey);
            this.Controls.Add(this.textBoxPrivateKeyPath);
            this.Controls.Add(buttonBrowseKey);
            this.Controls.Add(groupBoxPassphrase);
            this.Controls.Add(buttonOK);
            this.Controls.Add(buttonCancel);

            this.AcceptButton = buttonOK;
            this.CancelButton = buttonCancel;

            SelectPassphraseSource(settings.WinScpPassphraseSource);
            this.radioButtonEntryPassword.CheckedChanged += PassphraseSourceChanged;
            this.radioButtonPageant.CheckedChanged += PassphraseSourceChanged;
            this.radioButtonKeePassEntry.CheckedChanged += PassphraseSourceChanged;
            this.radioButtonManual.CheckedChanged += PassphraseSourceChanged;
            UpdatePassphraseControls();
        }

        private static Label CreateLabel(string text, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(width, 18);
            label.TextAlign = ContentAlignment.MiddleRight;
            return label;
        }

        private static TextBox CreateTextBox(int x, int y, int width)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(width, 20);
            return textBox;
        }

        private void LoadEntries(PwDatabase database, string selectedUuid)
        {
            if (database == null || !database.IsOpen)
            {
                return;
            }

            foreach (PwEntry entry in database.RootGroup.GetEntries(true))
            {
                var reference = new KeePassEntryReference(entry);
                this.comboBoxPassphraseEntry.Items.Add(reference);

                if (String.Equals(reference.UuidHex, selectedUuid, StringComparison.OrdinalIgnoreCase))
                {
                    this.comboBoxPassphraseEntry.SelectedItem = reference;
                }
            }
        }

        private void LoadFields(ICollection<string> dbFields, string selectedField)
        {
            this.comboBoxPassphraseField.Items.Add(PwDefs.PasswordField);

            if (dbFields != null)
            {
                foreach (var field in dbFields)
                {
                    if (!this.comboBoxPassphraseField.Items.Contains(field))
                    {
                        this.comboBoxPassphraseField.Items.Add(field);
                    }
                }
            }

            var fieldName = String.IsNullOrEmpty(selectedField)
                ? QuickConnectPluginSettings.DefaultWinScpPassphraseFieldName
                : selectedField;
            var index = this.comboBoxPassphraseField.FindStringExact(fieldName);
            this.comboBoxPassphraseField.SelectedIndex = index >= 0 ? index : 0;
        }

        private void SelectPassphraseSource(string source)
        {
            if (String.Equals(source, WinScpPassphraseSources.Pageant, StringComparison.OrdinalIgnoreCase))
            {
                this.radioButtonPageant.Checked = true;
            }
            else if (String.Equals(source, WinScpPassphraseSources.KeePassEntry, StringComparison.OrdinalIgnoreCase))
            {
                this.radioButtonKeePassEntry.Checked = true;
            }
            else if (String.Equals(source, WinScpPassphraseSources.Manual, StringComparison.OrdinalIgnoreCase))
            {
                this.radioButtonManual.Checked = true;
            }
            else
            {
                this.radioButtonEntryPassword.Checked = true;
            }
        }

        private void PassphraseSourceChanged(object sender, EventArgs e)
        {
            UpdatePassphraseControls();
        }

        private void UpdatePassphraseControls()
        {
            this.comboBoxPassphraseEntry.Enabled = this.radioButtonKeePassEntry.Checked && this.comboBoxPassphraseEntry.Items.Count > 0;
            this.comboBoxPassphraseField.Enabled = this.radioButtonKeePassEntry.Checked;
            this.textBoxManualPassphrase.Enabled = this.radioButtonManual.Checked;
        }

        private void ButtonBrowseKey_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = false;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Filter = "PuTTY private key (*.ppk)|*.ppk|All files (*.*)|*.*";
                openFileDialog.Title = "Select WinSCP PPK Key";

                var resolvedPath = QuickConnectUtils.ResolvePath(this.textBoxPrivateKeyPath.Text);
                if (File.Exists(resolvedPath))
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(resolvedPath);
                    openFileDialog.FileName = Path.GetFileName(resolvedPath);
                }

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.textBoxPrivateKeyPath.Text = QuickConnectUtils.NormalizeForStorage(openFileDialog.FileName);
                }
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.JumpHostName = this.textBoxHostName.Text.Trim();
            this.JumpPort = String.IsNullOrEmpty(this.textBoxPort.Text.Trim())
                ? QuickConnectPluginSettings.DefaultWinScpJumpPort
                : this.textBoxPort.Text.Trim();
            this.JumpUsername = this.textBoxUsername.Text.Trim();
            this.JumpPrivateKeyPath = this.textBoxPrivateKeyPath.Text.Trim();
            this.ManualPassphrase = this.textBoxManualPassphrase.Text;
            this.PassphraseFieldName = this.comboBoxPassphraseField.SelectedItem == null
                ? QuickConnectPluginSettings.DefaultWinScpPassphraseFieldName
                : this.comboBoxPassphraseField.SelectedItem.ToString();

            var selectedEntry = this.comboBoxPassphraseEntry.SelectedItem as KeePassEntryReference;
            this.PassphraseEntryUuid = selectedEntry == null ? string.Empty : selectedEntry.UuidHex;

            if (this.radioButtonPageant.Checked)
            {
                this.PassphraseSource = WinScpPassphraseSources.Pageant;
            }
            else if (this.radioButtonKeePassEntry.Checked)
            {
                this.PassphraseSource = WinScpPassphraseSources.KeePassEntry;
            }
            else if (this.radioButtonManual.Checked)
            {
                this.PassphraseSource = WinScpPassphraseSources.Manual;
            }
            else
            {
                this.PassphraseSource = WinScpPassphraseSources.EntryPassword;
            }
        }

        private class KeePassEntryReference {

            public string UuidHex { get; private set; }

            private readonly string displayName;

            public KeePassEntryReference(PwEntry entry)
            {
                this.UuidHex = entry.Uuid.ToHexString();

                var title = entry.Strings.ReadSafe(PwDefs.TitleField);
                var username = entry.Strings.ReadSafe(PwDefs.UserNameField);

                if (String.IsNullOrEmpty(username))
                {
                    this.displayName = title;
                }
                else
                {
                    this.displayName = String.Format("{0} ({1})", title, username);
                }
            }

            public override string ToString()
            {
                return this.displayName;
            }
        }
    }
}
