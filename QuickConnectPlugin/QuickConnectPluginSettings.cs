using System;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;
using QuickConnectPlugin.ShortcutKeys;
using QuickConnectPlugin.KeePass;

namespace QuickConnectPlugin {

    public class QuickConnectPluginSettings : AbstractQuickConnectPluginSettings
    {
        public const Keys DefaultRemoteDesktopShortcutKey = Keys.Control | Keys.Shift | Keys.E;
        public const Keys DefaultPuttyShortcutKey = Keys.Control | Keys.Shift | Keys.Q;
        public const Keys DefaultWinScpShortcutKey = Keys.Control | Keys.Shift | Keys.W;
        public const string DefaultHostAddressMapFieldName = PwDefs.TitleField;
        public const string DefaultConnectionMethodMapFieldName = PwDefs.NotesField;
        public const string DefaultSshConnectionType = SshConnectionTypes.Putty;
        public const bool DefaultEnableSshStartupCommand = false;
        public const string DefaultSshStartupCommand = "sudo -v; exec bash -l";
        public const string DefaultWindowsPasswordResetMethod = WindowsPasswordResetMethods.PsPasswd;
        public const bool DefaultWinScpUseJumpHost = false;
        public const string DefaultWinScpJumpPort = "22";
        public const string DefaultWinScpPassphraseSource = WinScpPassphraseSources.EntryPassword;
        public const string DefaultWinScpPassphraseFieldName = PwDefs.PasswordField;

        private readonly ICustomConfigPropertyNameFormatter formatter;
        private readonly IPluginHost plugin;

        public static QuickConnectPluginSettings Load(IPluginHost pluginHost, ICustomConfigPropertyNameFormatter propertyNameFormatter) {
            QuickConnectPluginSettings options = new QuickConnectPluginSettings(pluginHost, propertyNameFormatter);
            options.Load();
            return options;
        }

        private QuickConnectPluginSettings(IPluginHost pluginHost, ICustomConfigPropertyNameFormatter propertyNameFormatter) {
            this.plugin = pluginHost;
            this.formatter = propertyNameFormatter;
        }

        /// <summary>
        /// Loads the plugin settings from the KeePass configuration file.
        /// </summary>
        public override void Load()
        {
            this.Enabled = this.plugin.CustomConfig.GetBool(this.formatter.Format("Enabled"), true);
            this.CompatibleMode = this.plugin.CustomConfig.GetBool(this.formatter.Format("CompatibleMode"), false);
            this.AddChangePasswordMenuItem = this.plugin.CustomConfig.GetBool(this.formatter.Format("AddChangePasswordMenuItem"), true);
            this.PuttyPath = this.plugin.CustomConfig.GetString(
                this.formatter.Format("SSHClientPath"),
                QuickConnectUtils.NormalizeForStorage(QuickConnectUtils.GetPuttyPath() ?? QuickConnectUtils.DefaultPuttyPath));
            this.WinScpPath = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpPath"),
                QuickConnectUtils.NormalizeForStorage(QuickConnectUtils.GetWinScpPath() ?? QuickConnectUtils.DefaultWinScpPath));
            this.WinScpUseJumpHost = this.plugin.CustomConfig.GetBool(
                this.formatter.Format("WinScpUseJumpHost"),
                DefaultWinScpUseJumpHost);
            this.WinScpJumpHostName = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpJumpHostName"),
                string.Empty);
            this.WinScpJumpPort = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpJumpPort"),
                DefaultWinScpJumpPort);
            this.WinScpJumpUsername = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpJumpUsername"),
                string.Empty);
            this.WinScpJumpPrivateKeyPath = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpJumpPrivateKeyPath"),
                string.Empty);
            this.WinScpPassphraseSource = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpPassphraseSource"),
                DefaultWinScpPassphraseSource);
            this.WinScpManualPassphrase = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpManualPassphrase"),
                string.Empty);
            this.WinScpPassphraseEntryUuid = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpPassphraseEntryUuid"),
                string.Empty);
            this.WinScpPassphraseFieldName = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WinScpPassphraseFieldName"),
                DefaultWinScpPassphraseFieldName);
            this.PsPasswdPath = this.plugin.CustomConfig.GetString(
                this.formatter.Format("PsPasswdPath"),
                QuickConnectUtils.NormalizeForStorage(QuickConnectUtils.GetPsPasswdPath() ?? string.Empty));
            this.SshConnectionType = this.plugin.CustomConfig.GetString(
                this.formatter.Format("SshConnectionType"),
                DefaultSshConnectionType);
            this.ShowAllSshConnectionTypes = this.plugin.CustomConfig.GetBool(
                this.formatter.Format("ShowAllSshConnectionTypes"),
                false);
            this.EnableSshStartupCommand = this.plugin.CustomConfig.GetBool(
                this.formatter.Format("EnableSshStartupCommand"),
                DefaultEnableSshStartupCommand);
            this.SshStartupCommand = this.plugin.CustomConfig.GetString(
                this.formatter.Format("SshStartupCommand"),
                DefaultSshStartupCommand);
            this.WindowsPasswordResetMethod = this.plugin.CustomConfig.GetString(
                this.formatter.Format("WindowsPasswordResetMethod"),
                DefaultWindowsPasswordResetMethod);
            this.HostAddressMapFieldName = this.plugin.CustomConfig.GetString(this.formatter.Format("HostAddressMapFieldName"), DefaultHostAddressMapFieldName);
            this.ConnectionMethodMapFieldName = this.plugin.CustomConfig.GetString(this.formatter.Format("ConnectionMethodMapFieldName"), DefaultConnectionMethodMapFieldName);
            this.AdditionalOptionsMapFieldName = this.plugin.CustomConfig.GetString(this.formatter.Format("AdditionalOptionsMapFieldName"), string.Empty);
            this.DisableCLIPasswordForPutty = this.plugin.CustomConfig.GetBool(this.formatter.Format("DisableCLIPasswordForPutty"), false);

            // Shortcut Keys Settings.
            this.EnableShortcutKeys = GetConfigIfSet<bool?>(this.formatter.Format("EnableShortcutKeys"));

            var remoteDesktopShortcutKey = Keys.None;
            KeysHelper.TryParse(this.plugin.CustomConfig.GetString(this.formatter.Format("RemoteDesktopShortcutKey"), string.Empty), out remoteDesktopShortcutKey);
            this.RemoteDesktopShortcutKey = remoteDesktopShortcutKey;

            var puttyShortcutKey = Keys.None;
            KeysHelper.TryParse(this.plugin.CustomConfig.GetString(this.formatter.Format("PuttyShortcutKey"), string.Empty), out puttyShortcutKey);
            this.PuttyShortcutKey = puttyShortcutKey;

            var winScpShortcutKey = Keys.None;
            KeysHelper.TryParse(this.plugin.CustomConfig.GetString(this.formatter.Format("WinScpShortcutKey"), string.Empty), out winScpShortcutKey);
            this.WinScpShortcutKey = winScpShortcutKey;
        }

        /// <summary>
        /// Saves the plugin settings to the KeePass configuration file.
        /// </summary>
        public override void Save()
        {
            this.plugin.CustomConfig.SetBool(this.formatter.Format("Enabled"), this.Enabled);
            this.plugin.CustomConfig.SetBool(this.formatter.Format("CompatibleMode"), this.CompatibleMode);
            this.plugin.CustomConfig.SetBool(this.formatter.Format("AddChangePasswordMenuItem"), this.AddChangePasswordMenuItem);
            this.plugin.CustomConfig.SetString(this.formatter.Format("SSHClientPath"), this.PuttyPath);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpPath"), this.WinScpPath);
            this.plugin.CustomConfig.SetBool(this.formatter.Format("WinScpUseJumpHost"), this.WinScpUseJumpHost);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpJumpHostName"), this.WinScpJumpHostName);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpJumpPort"), this.WinScpJumpPort);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpJumpUsername"), this.WinScpJumpUsername);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpJumpPrivateKeyPath"), this.WinScpJumpPrivateKeyPath);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpPassphraseSource"), this.WinScpPassphraseSource);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpManualPassphrase"), this.WinScpManualPassphrase);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpPassphraseEntryUuid"), this.WinScpPassphraseEntryUuid);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpPassphraseFieldName"), this.WinScpPassphraseFieldName);
            this.plugin.CustomConfig.SetString(this.formatter.Format("PsPasswdPath"), this.PsPasswdPath);
            this.plugin.CustomConfig.SetString(this.formatter.Format("SshConnectionType"), this.SshConnectionType);
            this.plugin.CustomConfig.SetBool(this.formatter.Format("ShowAllSshConnectionTypes"), this.ShowAllSshConnectionTypes);
            this.plugin.CustomConfig.SetBool(this.formatter.Format("EnableSshStartupCommand"), this.EnableSshStartupCommand);
            this.plugin.CustomConfig.SetString(this.formatter.Format("SshStartupCommand"), this.SshStartupCommand);
            this.plugin.CustomConfig.SetString(this.formatter.Format("WindowsPasswordResetMethod"), this.WindowsPasswordResetMethod);
            this.plugin.CustomConfig.SetString(this.formatter.Format("HostAddressMapFieldName"), this.HostAddressMapFieldName);
            this.plugin.CustomConfig.SetString(this.formatter.Format("ConnectionMethodMapFieldName"), this.ConnectionMethodMapFieldName);
            this.plugin.CustomConfig.SetString(this.formatter.Format("AdditionalOptionsMapFieldName"), this.AdditionalOptionsMapFieldName);
            this.plugin.CustomConfig.SetBool(this.formatter.Format("DisableCLIPasswordForPutty"), this.DisableCLIPasswordForPutty);

            if (this.EnableShortcutKeys.HasValue)
            {
                this.plugin.CustomConfig.SetBool(this.formatter.Format("EnableShortcutKeys"), this.EnableShortcutKeys.Value);
                this.plugin.CustomConfig.SetString(this.formatter.Format("RemoteDesktopShortcutKey"), ((int)this.RemoteDesktopShortcutKey).ToString());
                this.plugin.CustomConfig.SetString(this.formatter.Format("PuttyShortcutKey"), ((int)this.PuttyShortcutKey).ToString());
                this.plugin.CustomConfig.SetString(this.formatter.Format("WinScpShortcutKey"), ((int)this.WinScpShortcutKey).ToString());
            }
        }

        private T GetConfigIfSet<T>(string strID)
        {
            var obj = this.plugin.CustomConfig.GetString(strID, null);

            var underlyingType = Nullable.GetUnderlyingType(typeof(T));

            if (underlyingType != null)
            {
                return (obj == null) ? default(T) : (T)Convert.ChangeType(obj, underlyingType);
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }
    }
}
