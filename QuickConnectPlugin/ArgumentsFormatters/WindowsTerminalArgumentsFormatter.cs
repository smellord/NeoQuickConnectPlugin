using System;
using System.Text;

namespace QuickConnectPlugin.ArgumentsFormatters {

    public class WindowsTerminalArgumentsFormatter : IArgumentsFormatter {

        public String ExecutablePath { get; private set; }
        public String PlinkPath { get; private set; }
        public bool UseBatchMode { get; private set; }
        public bool EnableStartupCommand { get; private set; }
        public string StartupCommand { get; private set; }

        public WindowsTerminalArgumentsFormatter(String executablePath, String plinkPath, bool useBatchMode) {
            this.ExecutablePath = executablePath;
            this.PlinkPath = plinkPath;
            this.UseBatchMode = useBatchMode;
        }

        public WindowsTerminalArgumentsFormatter(String executablePath, String plinkPath, bool useBatchMode, bool enableStartupCommand, string startupCommand) {
            this.ExecutablePath = executablePath;
            this.PlinkPath = plinkPath;
            this.UseBatchMode = useBatchMode;
            this.EnableStartupCommand = enableStartupCommand;
            this.StartupCommand = startupCommand;
        }

        public String Format(IHostPwEntry hostPwEntry) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\"{0}\"", this.ExecutablePath);
            sb.AppendFormat(" new-tab -- \"{0}\"", this.PlinkPath);

            PuttyOptions options = null;
            bool success = PuttyOptions.TryParse(hostPwEntry.AdditionalOptions, out options);
            string startupCommand = null;
            var hasStartupCommand = SshStartupCommandFormatter.TryGetStartupCommand(
                success ? options : null,
                this.EnableStartupCommand,
                this.StartupCommand,
                out startupCommand);

            if (this.UseBatchMode && !hasStartupCommand) {
                sb.Append(" -batch");
            }

            if (hasStartupCommand) {
                sb.Append(" -t");
            }

            sb.Append(" -ssh");

            string ipAddress = null;
            string port = null;

            if (hostPwEntry.IPAddress.Contains(":")) {
                ipAddress = hostPwEntry.IPAddress.Substring(0, hostPwEntry.IPAddress.IndexOf(':'));
                port = hostPwEntry.IPAddress.Substring(hostPwEntry.IPAddress.IndexOf(':') + 1);
            }
            else {
                ipAddress = hostPwEntry.IPAddress;
            }

            if (!String.IsNullOrEmpty(port)) {
                sb.AppendFormat(" -P {0}", port);
            }
            else if (success && options.Port.HasValue) {
                sb.AppendFormat(" -P {0}", options.Port.Value);
            }
            else {
                sb.Append(" -P 22");
            }

            if (success && options.HasKeyFile()) {
                sb.AppendFormat(" -i \"{0}\"", options.KeyFilePath);
            }

            sb.AppendFormat(" -l \"{0}\"", hostPwEntry.GetUsername());

            var password = hostPwEntry.GetPassword();
            if (password != null) {
                if (password.Length == 0) {
                    password = " ";
                }

                if (password.Contains(@"""")) {
                    sb.AppendFormat(" -pw \"{0}\"", password.Replace(@"""", @"\"""));
                }
                else {
                    sb.AppendFormat(" -pw \"{0}\"", password);
                }
            }

            sb.AppendFormat(" {0}", ipAddress);

            if (success && options.HasCommand()) {
                sb.AppendFormat(" {0}", options.Command);
            }
            else if (hasStartupCommand) {
                sb.AppendFormat(" {0}", SshStartupCommandFormatter.QuoteRemoteCommand(startupCommand));
            }

            return sb.ToString();
        }
    }
}
