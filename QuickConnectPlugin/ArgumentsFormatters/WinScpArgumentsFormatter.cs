using QuickConnectPlugin.Commons;
using System;
using System.Text;

namespace QuickConnectPlugin.ArgumentsFormatters
{
    public class WinScpArgumentsFormatter : IArgumentsFormatter
    {
        public string ExecutablePath { get; private set; }
        public WinScpLaunchOptions LaunchOptions { get; private set; }

        public WinScpArgumentsFormatter(string winScpPath)
        {
            this.ExecutablePath = winScpPath;
            this.LaunchOptions = new WinScpLaunchOptions()
            {
                UseEntryPasswordAsPrivateKeyPassphrase = true
            };
        }

        public WinScpArgumentsFormatter(string winScpPath, WinScpLaunchOptions launchOptions)
        {
            this.ExecutablePath = winScpPath;
            this.LaunchOptions = launchOptions ?? new WinScpLaunchOptions();
        }

        public string Format(IHostPwEntry hostPwEntry)
        {
            WinScpOptions options = null;
            bool success = WinScpOptions.TryParse(hostPwEntry.AdditionalOptions, out options);

            // Try get the protocol if explicitly set, otherwise default to SCP.
            var protocol = (success && options.Protocol.HasValue ? options.Protocol : WinScp.Protocol.Scp).ToString().ToLowerInvariant();
            var privateKeyPath = GetPrivateKeyPath(success ? options : null);

            var stringBuilder = new StringBuilder(string.Format("\"{0}\" {1}://{2}", ExecutablePath, protocol, hostPwEntry.GetUsername()));

            if (String.IsNullOrEmpty(privateKeyPath))
            {
                // See: https://winscp.net/eng/docs/session_url -> Special Characters
                stringBuilder.AppendFormat(":\"{0}\"", HttpUtilityEx.UrlEncodeUpperCase(hostPwEntry.GetPassword()));
            }

            stringBuilder.AppendFormat("@{0}", hostPwEntry.IPAddress);

            if (success && options.Port.HasValue)
            {
                stringBuilder.AppendFormat(":{0}", options.Port);
            }

            if (!String.IsNullOrEmpty(privateKeyPath))
            {
                stringBuilder.AppendFormat(" /privatekey=\"{0}\"", privateKeyPath);
            }

            AppendPrivateKeyPassphrase(stringBuilder, hostPwEntry, privateKeyPath);
            AppendJumpHostSettings(stringBuilder);

            return stringBuilder.ToString();
        }

        private string GetPrivateKeyPath(WinScpOptions options)
        {
            if (options != null && options.HasKeyFile())
            {
                return options.KeyFilePath;
            }

            return this.LaunchOptions.DefaultPrivateKeyPath;
        }

        private void AppendPrivateKeyPassphrase(StringBuilder stringBuilder, IHostPwEntry hostPwEntry, string privateKeyPath)
        {
            var hasPrivateKey = !String.IsNullOrEmpty(privateKeyPath) ||
                (this.LaunchOptions.UseJumpHost && !String.IsNullOrEmpty(this.LaunchOptions.JumpPrivateKeyPath));

            if (!hasPrivateKey)
            {
                return;
            }

            if (!String.IsNullOrEmpty(this.LaunchOptions.PrivateKeyPassphraseFilePath))
            {
                stringBuilder.AppendFormat(" /passwordsfromfiles /passphrase=\"{0}\"", this.LaunchOptions.PrivateKeyPassphraseFilePath);
                return;
            }

            if (this.LaunchOptions.UseEntryPasswordAsPrivateKeyPassphrase && !String.IsNullOrEmpty(privateKeyPath))
            {
                stringBuilder.AppendFormat(" /passphrase=\"{0}\"", hostPwEntry.GetPassword());
            }
        }

        private void AppendJumpHostSettings(StringBuilder stringBuilder)
        {
            if (!this.LaunchOptions.UseJumpHost || String.IsNullOrEmpty(this.LaunchOptions.JumpHostName))
            {
                return;
            }

            stringBuilder.Append(" /rawsettings");
            AppendRawSetting(stringBuilder, "Tunnel", "1");
            AppendRawSetting(stringBuilder, "TunnelHostName", this.LaunchOptions.JumpHostName);
            AppendRawSetting(stringBuilder, "TunnelPortNumber", this.LaunchOptions.JumpPort);
            AppendRawSetting(stringBuilder, "TunnelUserName", this.LaunchOptions.JumpUsername);
            AppendRawSetting(stringBuilder, "TunnelPublicKeyFile", this.LaunchOptions.JumpPrivateKeyPath);
        }

        private static void AppendRawSetting(StringBuilder stringBuilder, string name, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            stringBuilder.AppendFormat(" \"{0}={1}\"", name, value.Replace(@"""", @"\"""));
        }
    }
}
