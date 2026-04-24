using QuickConnectPlugin.PasswordChanger;
using QuickConnectPlugin.ShortcutKeys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

using HotKeyControlEx = QuickConnect.KeePass.UI.HotKeyControlEx;

namespace QuickConnectPlugin
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class FormOptions : Form
    {
        private const string PuttyDownloadUrl = "https://www.putty.org/";
        private const string WinScpDownloadUrl = "https://winscp.net/eng/download.php";
        private const string PsPasswdDownloadUrl = "https://learn.microsoft.com/en-us/sysinternals/downloads/pspasswd";

        private readonly IQuickConnectPluginSettings settings;

        private readonly HotKeyControlEx shortcutKeyControlRemoteDesktop;
        private readonly HotKeyControlEx shortcutKeyControlPutty;
        private readonly HotKeyControlEx shortcutKeyControlWinScp;

        private bool shortcutKeysSettingWasChanged;

        public FormOptions(string pluginName, IQuickConnectPluginSettings settings, ICollection<string> dbFields)
        {
            InitializeComponent();

            this.settings = settings;

            this.Text = this.Text.Replace("{title}", pluginName);

            this.checkBoxEnable.Checked = settings.Enabled;
            this.checkBoxCompatibleMode.Checked = settings.CompatibleMode;
            this.checkBoxAddChangePasswordItem.Checked = settings.AddChangePasswordMenuItem;
            this.checkBoxDisableCLIPasswordForPutty.Checked = settings.DisableCLIPasswordForPutty;
            this.checkBoxShowAllSshOptions.Checked = settings.ShowAllSshConnectionTypes;

            this.textBoxPuttyPath.Text = settings.PuttyPath;
            this.textBoxPuttyPath.Select(this.textBoxPuttyPath.Text.Length, 0);

            this.textBoxWinScpPath.Text = settings.WinScpPath;
            this.textBoxWinScpPath.Select(this.textBoxWinScpPath.Text.Length, 0);

            this.textBoxPsPasswdPath.Text = settings.PsPasswdPath;
            this.textBoxPsPasswdPath.Select(this.textBoxPsPasswdPath.Text.Length, 0);

            this.comboBoxSshConnectionType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxSshConnectionType.Items.Add(SshConnectionTypes.WindowsTerminalSsh);
            this.comboBoxSshConnectionType.Items.Add(SshConnectionTypes.WindowsTerminalPlink);
            this.comboBoxSshConnectionType.Items.Add(SshConnectionTypes.Putty);
            var sshConnectionType = QuickConnectUtils.IsSshConnectionTypeValid(settings.SshConnectionType)
                ? settings.SshConnectionType
                : QuickConnectPluginSettings.DefaultSshConnectionType;
            this.comboBoxSshConnectionType.SelectedIndex = this.comboBoxSshConnectionType.FindStringExact(sshConnectionType);
            if (this.comboBoxSshConnectionType.SelectedIndex < 0)
            {
                this.comboBoxSshConnectionType.SelectedIndex = 0;
            }
            this.comboBoxSshConnectionType.Enabled = !this.checkBoxShowAllSshOptions.Checked;

            this.comboBoxWindowsPasswordResetMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxWindowsPasswordResetMethod.Items.Add(WindowsPasswordResetMethods.PsPasswd);
            this.comboBoxWindowsPasswordResetMethod.Items.Add(WindowsPasswordResetMethods.Ssh);
            var selectedMethod = QuickConnectUtils.IsWindowsPasswordResetMethodValid(settings.WindowsPasswordResetMethod)
                ? settings.WindowsPasswordResetMethod
                : QuickConnectPluginSettings.DefaultWindowsPasswordResetMethod;
            this.comboBoxWindowsPasswordResetMethod.SelectedIndex = this.comboBoxWindowsPasswordResetMethod.FindStringExact(selectedMethod);
            if (this.comboBoxWindowsPasswordResetMethod.SelectedIndex < 0)
            {
                this.comboBoxWindowsPasswordResetMethod.SelectedIndex = 0;
            }

            // Always add empty items.
            this.comboBoxHostAddressMapFieldName.Items.Add(string.Empty);
            this.comboBoxConnectionMethodMapFieldName.Items.Add(string.Empty);
            this.comboBoxAdditionalOptionsMapFieldName.Items.Add(string.Empty);

            if (dbFields == null)
            {
                this.labelWarningMessage.Visible = true;
                this.comboBoxHostAddressMapFieldName.Enabled = false;
                this.comboBoxConnectionMethodMapFieldName.Enabled = false;
                this.comboBoxAdditionalOptionsMapFieldName.Enabled = false;

                if (!string.IsNullOrEmpty(settings.HostAddressMapFieldName))
                {
                    this.comboBoxHostAddressMapFieldName.Items.Add(settings.HostAddressMapFieldName);
                }
                if (!string.IsNullOrEmpty(settings.ConnectionMethodMapFieldName))
                {
                    this.comboBoxConnectionMethodMapFieldName.Items.Add(settings.ConnectionMethodMapFieldName);
                }
                if (!string.IsNullOrEmpty(settings.AdditionalOptionsMapFieldName))
                {
                    this.comboBoxAdditionalOptionsMapFieldName.Items.Add(settings.AdditionalOptionsMapFieldName);
                }

                this.comboBoxHostAddressMapFieldName.SelectedValue = settings.HostAddressMapFieldName;
                this.comboBoxConnectionMethodMapFieldName.SelectedValue = settings.ConnectionMethodMapFieldName;
                this.comboBoxAdditionalOptionsMapFieldName.SelectedValue = settings.AdditionalOptionsMapFieldName;
            }
            else
            {
                this.labelWarningMessage.Visible = false;

                foreach (var field in dbFields)
                {
                    this.comboBoxHostAddressMapFieldName.Items.Add(field);
                    this.comboBoxConnectionMethodMapFieldName.Items.Add(field);
                    this.comboBoxAdditionalOptionsMapFieldName.Items.Add(field);
                }
            }

            if (string.IsNullOrEmpty(settings.HostAddressMapFieldName))
            {
                this.comboBoxHostAddressMapFieldName.SelectedIndex = this.comboBoxHostAddressMapFieldName.FindStringExact(string.Empty);
            }
            else
            {
                this.comboBoxHostAddressMapFieldName.SelectedIndex = this.comboBoxHostAddressMapFieldName.FindStringExact(this.settings.HostAddressMapFieldName);
            }

            if (string.IsNullOrEmpty(settings.ConnectionMethodMapFieldName))
            {
                this.comboBoxConnectionMethodMapFieldName.SelectedIndex = this.comboBoxConnectionMethodMapFieldName.FindStringExact(string.Empty);
            }
            else
            {
                this.comboBoxConnectionMethodMapFieldName.SelectedIndex = this.comboBoxConnectionMethodMapFieldName.FindStringExact(this.settings.ConnectionMethodMapFieldName);
            }

            if (string.IsNullOrEmpty(settings.AdditionalOptionsMapFieldName))
            {
                this.comboBoxAdditionalOptionsMapFieldName.SelectedIndex = this.comboBoxAdditionalOptionsMapFieldName.FindStringExact(string.Empty);
            }
            else
            {
                this.comboBoxAdditionalOptionsMapFieldName.SelectedIndex = this.comboBoxAdditionalOptionsMapFieldName.FindStringExact(this.settings.AdditionalOptionsMapFieldName);
            }

            // Shortcut Keys.
            checkBoxEnableShortcutKeys.Checked = settings.EnableShortcutKeys ?? false;
            shortcutKeysSettingWasChanged = settings.EnableShortcutKeys ?? false;

            // KeePass v2.42 API breaking change.
            shortcutKeyControlRemoteDesktop = HotKeyControlEx.ReplaceTextBox(this.textBoxRDShortcutKey.Parent, this.textBoxRDShortcutKey, false);
            shortcutKeyControlRemoteDesktop.HotKey = (settings.EnableShortcutKeys.HasValue ? settings.RemoteDesktopShortcutKey : QuickConnectPluginSettings.DefaultRemoteDesktopShortcutKey) & Keys.KeyCode;
            shortcutKeyControlRemoteDesktop.HotKeyModifiers = (settings.EnableShortcutKeys.HasValue ? settings.RemoteDesktopShortcutKey : QuickConnectPluginSettings.DefaultRemoteDesktopShortcutKey) & Keys.Modifiers;
            shortcutKeyControlRemoteDesktop.RenderHotKey();
            shortcutKeyControlRemoteDesktop.Enabled = settings.EnableShortcutKeys ?? false;
            shortcutKeyControlRemoteDesktop.Show();

            shortcutKeyControlPutty = HotKeyControlEx.ReplaceTextBox(this.textBoxPuttyShortcutKey.Parent, this.textBoxPuttyShortcutKey, false);
            shortcutKeyControlPutty.HotKey = (settings.EnableShortcutKeys.HasValue ? settings.PuttyShortcutKey : QuickConnectPluginSettings.DefaultPuttyShortcutKey) & Keys.KeyCode;
            shortcutKeyControlPutty.HotKeyModifiers = (settings.EnableShortcutKeys.HasValue ? settings.PuttyShortcutKey : QuickConnectPluginSettings.DefaultPuttyShortcutKey) & Keys.Modifiers;
            shortcutKeyControlPutty.RenderHotKey();
            shortcutKeyControlPutty.Enabled = settings.EnableShortcutKeys ?? false;
            shortcutKeyControlPutty.Show();

            shortcutKeyControlWinScp = HotKeyControlEx.ReplaceTextBox(this.textBoxWinScpShortcutKey.Parent, this.textBoxWinScpShortcutKey, false);
            shortcutKeyControlWinScp.HotKey = (settings.EnableShortcutKeys.HasValue ? settings.WinScpShortcutKey : QuickConnectPluginSettings.DefaultWinScpShortcutKey) & Keys.KeyCode;
            shortcutKeyControlWinScp.HotKeyModifiers = (settings.EnableShortcutKeys.HasValue ? settings.WinScpShortcutKey : QuickConnectPluginSettings.DefaultWinScpShortcutKey) & Keys.Modifiers;
            shortcutKeyControlWinScp.RenderHotKey();
            shortcutKeyControlWinScp.Enabled = settings.EnableShortcutKeys ?? false;
            shortcutKeyControlWinScp.Show();

            // Add handlers.
            this.checkBoxEnable.CheckedChanged += new EventHandler(SettingsChanged);
            this.checkBoxCompatibleMode.CheckedChanged += new EventHandler(SettingsChanged);
            this.checkBoxAddChangePasswordItem.CheckedChanged += new EventHandler(SettingsChanged);
            this.checkBoxDisableCLIPasswordForPutty.CheckedChanged += new EventHandler(SettingsChanged);
            this.checkBoxShowAllSshOptions.CheckedChanged += new EventHandler(ShowAllSshOptionsChanged);
            this.textBoxPuttyPath.TextChanged += new EventHandler(SettingsChanged);
            this.textBoxWinScpPath.TextChanged += new EventHandler(SettingsChanged);
            this.textBoxPsPasswdPath.TextChanged += new EventHandler(SettingsChanged);
            this.comboBoxSshConnectionType.SelectedIndexChanged += new EventHandler(SettingsChanged);
            this.comboBoxWindowsPasswordResetMethod.SelectedIndexChanged += new EventHandler(SettingsChanged);
            this.comboBoxHostAddressMapFieldName.SelectedIndexChanged += new EventHandler(SettingsChanged);
            this.comboBoxConnectionMethodMapFieldName.SelectedIndexChanged += new EventHandler(SettingsChanged);
            this.comboBoxAdditionalOptionsMapFieldName.SelectedIndexChanged += new EventHandler(SettingsChanged);

            this.checkBoxEnableShortcutKeys.CheckedChanged += new EventHandler(SettingsChanged);
            this.checkBoxEnableShortcutKeys.CheckedChanged += (o, e) => { shortcutKeysSettingWasChanged = true; };
            this.shortcutKeyControlRemoteDesktop.KeyUp += (s, e) => { SettingsChanged(s, e); };
            this.shortcutKeyControlPutty.KeyUp += (s, e) => { SettingsChanged(s, e); };
            this.shortcutKeyControlWinScp.KeyUp += (s, e) => { SettingsChanged(s, e); };

            this.buttonApply.Enabled = false;

            // Check if VMware VSphere PowerCLI is installed.
            this.CheckVSpherePowerCLIStatus();

            // Force settings validation.
            this.ValidateSettings();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            this.SaveSettings();
            this.buttonApply.Enabled = false;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (this.buttonApply.Enabled)
            {
                this.SaveSettings();
            }

            this.Close();
        }

        private void SaveSettings()
        {
            this.settings.Enabled = this.checkBoxEnable.Checked;
            this.settings.CompatibleMode = this.checkBoxCompatibleMode.Checked;
            this.settings.DisableCLIPasswordForPutty = this.checkBoxDisableCLIPasswordForPutty.Checked;
            this.settings.AddChangePasswordMenuItem = this.checkBoxAddChangePasswordItem.Checked;
            this.settings.PuttyPath = this.textBoxPuttyPath.Text;
            this.settings.WinScpPath = this.textBoxWinScpPath.Text;
            this.settings.PsPasswdPath = this.textBoxPsPasswdPath.Text;
            this.settings.ShowAllSshConnectionTypes = this.checkBoxShowAllSshOptions.Checked;
            this.settings.SshConnectionType = this.comboBoxSshConnectionType.SelectedItem == null
                ? QuickConnectPluginSettings.DefaultSshConnectionType
                : this.comboBoxSshConnectionType.SelectedItem.ToString();
            this.settings.WindowsPasswordResetMethod = this.comboBoxWindowsPasswordResetMethod.SelectedItem == null
                ? QuickConnectPluginSettings.DefaultWindowsPasswordResetMethod
                : this.comboBoxWindowsPasswordResetMethod.SelectedItem.ToString();
            this.settings.HostAddressMapFieldName = (string)this.comboBoxHostAddressMapFieldName.SelectedItem;
            this.settings.ConnectionMethodMapFieldName = (string)this.comboBoxConnectionMethodMapFieldName.SelectedItem;
            this.settings.AdditionalOptionsMapFieldName = (string)this.comboBoxAdditionalOptionsMapFieldName.SelectedItem;

            if (shortcutKeysSettingWasChanged)
            {
                this.settings.EnableShortcutKeys = this.checkBoxEnableShortcutKeys.Checked;
                this.settings.RemoteDesktopShortcutKey = (this.shortcutKeyControlRemoteDesktop.HotKey | this.shortcutKeyControlRemoteDesktop.HotKeyModifiers);
                this.settings.PuttyShortcutKey = (this.shortcutKeyControlPutty.HotKey | this.shortcutKeyControlPutty.HotKeyModifiers);
                this.settings.WinScpShortcutKey = (this.shortcutKeyControlWinScp.HotKey | this.shortcutKeyControlWinScp.HotKeyModifiers);
            }

            this.settings.Save();
        }

        private bool IsPuttyPathValid()
        {
            if (this.textBoxPuttyPath.Text.Length == 0 || !QuickConnectUtils.FileExists(this.textBoxPuttyPath.Text))
            {
                return (this.textBoxPuttyPath.Text.Length == 0);
            }
            else
            {
                this.textBoxPuttyPath.BackColor = default(Color);
                return true;
            }
        }

        private bool IsWinScpPathValid()
        {
            if (this.textBoxWinScpPath.Text.Length == 0 || !QuickConnectUtils.FileExists(this.textBoxWinScpPath.Text))
            {
                return (this.textBoxWinScpPath.Text.Length == 0); // Allow empty path.
            }
            else
            {
                this.textBoxWinScpPath.BackColor = default(Color);
                return true;
            }
        }

        private bool IsPsPasswdPathValid()
        {
            if (this.comboBoxWindowsPasswordResetMethod.SelectedItem != null &&
                String.Equals(this.comboBoxWindowsPasswordResetMethod.SelectedItem.ToString(), WindowsPasswordResetMethods.Ssh, StringComparison.OrdinalIgnoreCase))
            {
                this.pictureBoxPsPasswdPathWarningIcon.Visible = false;
                this.labelPsPasswdPathWarningMessage.Visible = false;
                this.textBoxPsPasswdPath.BackColor = default(Color);
                return true;
            }

            if (this.textBoxPsPasswdPath.Text.Length == 0)
            {
                this.pictureBoxPsPasswdPathWarningIcon.Visible = false;
                this.labelPsPasswdPathWarningMessage.Visible = false;
                this.textBoxPsPasswdPath.BackColor = default(Color);

                return true;
            }
            else
            {
                this.pictureBoxPsPasswdPathWarningIcon.Visible = true;
                this.labelPsPasswdPathWarningMessage.Visible = true;

                var psPasswdPath = QuickConnectUtils.ResolvePath(this.textBoxPsPasswdPath.Text);
                if (File.Exists(psPasswdPath))
                {
                    if (!PsPasswdWrapper.IsPsPasswdUtility(psPasswdPath))
                    {
                        this.labelPsPasswdPathWarningMessage.Text = "Specified file is not valid.";
                        return false;
                    }
                    else
                    {
                        this.pictureBoxPsPasswdPathWarningIcon.Visible = false;
                        this.labelPsPasswdPathWarningMessage.Visible = false;
                        return true;
                    }
                }
                else
                {
                    this.pictureBoxPsPasswdPathWarningIcon.Image = global::QuickConnectPlugin.Properties.Resources.important;
                    this.labelPsPasswdPathWarningMessage.Text = "Specified path does not exists.";
                    return false;
                }
            }
        }

        private void SettingsChanged(Object sender, EventArgs e)
        {
            this.buttonApply.Enabled = ValidateSettings();
        }

        private void ShowAllSshOptionsChanged(object sender, EventArgs e)
        {
            this.comboBoxSshConnectionType.Enabled = !this.checkBoxShowAllSshOptions.Checked;
            this.SettingsChanged(sender, e);
        }

        private bool ValidateSettings()
        {
            bool isValidPuttyPath = IsPuttyPathValid();
            this.pictureBoxPuttyPathWarningIcon.Visible = !isValidPuttyPath;
            this.labelPuttyPathWarningMessage.Visible = !isValidPuttyPath;

            bool isValidWinScpPath = IsWinScpPathValid();
            this.pictureBoxWinScpPathWarningIcon.Visible = !isValidWinScpPath;
            this.labelWinScpPathWarningMessage.Visible = !isValidWinScpPath;

            this.shortcutKeyControlRemoteDesktop.Enabled = this.checkBoxEnableShortcutKeys.Checked;
            this.shortcutKeyControlPutty.Enabled = this.checkBoxEnableShortcutKeys.Checked;
            this.shortcutKeyControlWinScp.Enabled = this.checkBoxEnableShortcutKeys.Checked;

            return isValidPuttyPath && isValidWinScpPath && IsPsPasswdPathValid() && HasNoConflictingShortcutKeys();
        }

        private void ButtonConfigurePuttyPath_Click(object sender, EventArgs e)
        {
            this.ConfigureExecutablePath(this.textBoxPuttyPath, "PuTTY executable (*.exe)|*.exe|All files (*.*)|*.*", "Select PuTTY Path");
        }

        private void ButtonAutoSetPuttyPath_Click(object sender, EventArgs e)
        {
            this.AutoSetExecutablePath(this.textBoxPuttyPath, QuickConnectUtils.GetPuttyPath(), QuickConnectUtils.DefaultPuttyPath, "PuTTY");
        }

        private void ButtonInstallPutty_Click(object sender, EventArgs e)
        {
            InstallWithWinGet("PuTTY.PuTTY", "PuTTY", PuttyDownloadUrl, this.textBoxPuttyPath, QuickConnectUtils.GetPuttyPath, QuickConnectUtils.DefaultPuttyPath);
        }

        private void ButtonConfigureWinScpPath_Click(object sender, EventArgs e)
        {
            this.ConfigureExecutablePath(this.textBoxWinScpPath, "WinSCP executable (*.exe)|*.exe|All files (*.*)|*.*", "Select WinSCP Path");
        }

        private void ButtonAutoSetWinScpPath_Click(object sender, EventArgs e)
        {
            this.AutoSetExecutablePath(this.textBoxWinScpPath, QuickConnectUtils.GetWinScpPath(), QuickConnectUtils.DefaultWinScpPath, "WinSCP");
        }

        private void ButtonInstallWinScp_Click(object sender, EventArgs e)
        {
            InstallWithWinGet("WinSCP.WinSCP", "WinSCP", WinScpDownloadUrl, this.textBoxWinScpPath, QuickConnectUtils.GetWinScpPath, QuickConnectUtils.DefaultWinScpPath);
        }

        private void ButtonConfigurePsPasswdPath_Click(object sender, EventArgs e)
        {
            this.ConfigureExecutablePath(this.textBoxPsPasswdPath, "PsPasswd executable (*.exe)|*.exe|All files (*.*)|*.*", "Select PsPasswd Path");
        }

        private void ButtonDownloadPsPasswd_Click(object sender, EventArgs e)
        {
            InstallWithWinGet("Microsoft.Sysinternals.PsTools", "PsTools", PsPasswdDownloadUrl, this.textBoxPsPasswdPath, QuickConnectUtils.GetPsPasswdPath, string.Empty);
        }

        private void ConfigureExecutablePath(TextBox textBox, string filter, string title)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = false;

                var resolvedPath = QuickConnectUtils.ResolvePath(textBox.Text);
                if (File.Exists(resolvedPath))
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(resolvedPath);
                    openFileDialog.FileName = Path.GetFileName(resolvedPath);
                }

                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Filter = filter;
                openFileDialog.Title = title;

                var result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    textBox.Text = QuickConnectUtils.NormalizeForStorage(openFileDialog.FileName);
                    textBox.Select(textBox.Text.Length, 0);
                }
            }
        }

        private void AutoSetExecutablePath(TextBox textBox, string detectedPath, string fallbackPath, string applicationName)
        {
            textBox.Text = !string.IsNullOrEmpty(detectedPath)
                ? QuickConnectUtils.NormalizeForStorage(detectedPath)
                : fallbackPath;
            textBox.Select(textBox.Text.Length, 0);

            if (string.IsNullOrEmpty(detectedPath))
            {
                var message = !string.IsNullOrEmpty(fallbackPath)
                    ? string.Format("{0} was not detected automatically. The default path has been filled in and can be adjusted manually if needed.", applicationName)
                    : string.Format("{0} was not detected automatically. Please set the path manually if it is still missing.", applicationName);

                MessageBox.Show(
                    message,
                    "Auto Set",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private static void OpenDownloadPage(string url, string applicationName)
        {
            try
            {
                QuickConnectUtils.OpenUrl(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(
                    string.Format("Unable to open the {0} download page.\n\n{1}", applicationName, ex.Message),
                    "Open Download Page",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InstallWithWinGet(string packageId, string applicationName, string fallbackUrl, TextBox targetTextBox, Func<string> detectPathFunc, string fallbackPath)
        {
            try
            {
                if (String.Equals(packageId, "Microsoft.Sysinternals.PsTools", StringComparison.OrdinalIgnoreCase))
                {
                    var preparedPsPasswdPath = QuickConnectUtils.PreparePsPasswdPath();
                    if (!string.IsNullOrEmpty(preparedPsPasswdPath))
                    {
                        targetTextBox.Text = QuickConnectUtils.NormalizeForStorage(preparedPsPasswdPath);
                        targetTextBox.Select(targetTextBox.Text.Length, 0);
                        PersistPsPasswdPath(targetTextBox);

                        MessageBox.Show(
                            this,
                            "PsTools is already installed. I've set the path to pspasswd for you.",
                            "PsTools Ready",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                if (!QuickConnectUtils.EnsureWinGetAvailable(this))
                {
                    return;
                }

                UseWaitCursor = true;
                Enabled = false;

                string errorMessage;
                bool alreadyInstalled;
                if (!QuickConnectUtils.InstallPackageWithWinGet(packageId, out errorMessage, out alreadyInstalled))
                {
                    throw new Exception(string.IsNullOrEmpty(errorMessage) ? "The package installation did not complete successfully." : errorMessage);
                }

                var detectedPath = detectPathFunc();
                if (String.Equals(packageId, "Microsoft.Sysinternals.PsTools", StringComparison.OrdinalIgnoreCase))
                {
                    detectedPath = QuickConnectUtils.PreparePsPasswdPath();
                }

                if (!string.IsNullOrEmpty(detectedPath) || !string.IsNullOrEmpty(fallbackPath))
                {
                    this.AutoSetExecutablePath(targetTextBox, detectedPath, fallbackPath, applicationName);
                    PersistPsPasswdPath(targetTextBox);
                }

                MessageBox.Show(
                    this,
                    alreadyInstalled
                        ? string.Format("{0} is already installed. The path has been refreshed automatically.", applicationName)
                        : string.Format("{0} installation completed. The path has been refreshed automatically.", applicationName),
                    alreadyInstalled ? "Already Installed" : "Installation Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                var result = MessageBox.Show(
                    this,
                    string.Format("Unable to start the {0} installation with WinGet.\n\nDo you want to open the direct download page instead?\n\n{1}", applicationName, ex.Message),
                    "Install Failed",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    OpenDownloadPage(fallbackUrl, applicationName);
                }
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void PersistPsPasswdPath(TextBox targetTextBox)
        {
            if (!object.ReferenceEquals(targetTextBox, this.textBoxPsPasswdPath))
            {
                return;
            }

            this.settings.PsPasswdPath = this.textBoxPsPasswdPath.Text;
            this.settings.Save();
        }

        private void CheckVSpherePowerCLIStatus()
        {
            var status = QuickConnectUtils.IsVSpherePowerCLIInstalled();

            this.labelVSpherePowerCLIStatusMessage.Text = this.labelVSpherePowerCLIStatusMessage.Text.Replace("{status}",
                status ? "installed" : "not installed"
            );

            if (status)
            {
                this.pictureBoxVSpherePowerCLIStatusIcon.Image = global::QuickConnectPlugin.Properties.Resources.success;
            }
        }

        private bool HasNoConflictingShortcutKeys()
        {
            if (checkBoxEnableShortcutKeys.Checked)
            {
                var remoteDesktopShortcutKey = (this.shortcutKeyControlRemoteDesktop.HotKey | this.shortcutKeyControlRemoteDesktop.HotKeyModifiers);
                var puttyShortcutKey = (this.shortcutKeyControlPutty.HotKey | this.shortcutKeyControlPutty.HotKeyModifiers);
                var winScpShortcutKey = (this.shortcutKeyControlWinScp.HotKey | this.shortcutKeyControlWinScp.HotKeyModifiers);

                bool conflictsWithKeePass = false;

                conflictsWithKeePass |= CheckRemoteDesktopShortcutKey(remoteDesktopShortcutKey);
                conflictsWithKeePass |= CheckPuttyShortcutKey(puttyShortcutKey);
                conflictsWithKeePass |= CheckWinScpShortcutKey(winScpShortcutKey);

                // Reset warning text.
                labelShortcutKeysWarning.Text = Properties.Resources.ShortcutKeysConflictsWithKeePassConfiguration;

                var configuredShortcuts = new List<Keys>
                {
                    remoteDesktopShortcutKey,
                    puttyShortcutKey,
                    winScpShortcutKey
                };

                var conflictsWithSelf = configuredShortcuts.Where(x => x != Keys.None).GroupBy(x => x).Any(g => g.Count() > 1);
                var hasConflicts = conflictsWithKeePass || conflictsWithSelf;

                if (!conflictsWithKeePass && conflictsWithSelf)
                {
                    labelShortcutKeysWarning.Text = Properties.Resources.ShortcutKeysCannotAssignToMultiplePrograms;
                }

                pictureBoxShortcutKeysWarning.Visible = hasConflicts;
                labelShortcutKeysWarning.Visible = hasConflicts;

                return !hasConflicts;
            }
            else
            {
                pictureBoxShortcutKeysWarning.Visible = false;
                labelShortcutKeysWarning.Visible = false;

                return true;
            }
        }

        private bool CheckRemoteDesktopShortcutKey(Keys remoteDesktopShortcutKeys)
        {
            var conflictsWithKeePass = remoteDesktopShortcutKeys != Keys.None && KeysHelper.ConflictsWithKeePassShortcutKeys(remoteDesktopShortcutKeys);
            this.shortcutKeyControlRemoteDesktop.BackColor = conflictsWithKeePass ? ColorTranslator.FromHtml("#FFC0C0") : default(Color);

            return conflictsWithKeePass;
        }

        private bool CheckPuttyShortcutKey(Keys puttyShortcutKeys)
        {
            var conflictsWithKeePass = puttyShortcutKeys != Keys.None && KeysHelper.ConflictsWithKeePassShortcutKeys(puttyShortcutKeys);
            this.shortcutKeyControlPutty.BackColor = conflictsWithKeePass ? ColorTranslator.FromHtml("#FFC0C0") : default(Color);

            return conflictsWithKeePass;
        }

        private bool CheckWinScpShortcutKey(Keys winScpShortcutKeys)
        {
            var conflictsWithKeePass = winScpShortcutKeys != Keys.None && KeysHelper.ConflictsWithKeePassShortcutKeys(winScpShortcutKeys);
            this.shortcutKeyControlWinScp.BackColor = conflictsWithKeePass ? ColorTranslator.FromHtml("#FFC0C0") : default(Color);

            return conflictsWithKeePass;
        }
    }
}
