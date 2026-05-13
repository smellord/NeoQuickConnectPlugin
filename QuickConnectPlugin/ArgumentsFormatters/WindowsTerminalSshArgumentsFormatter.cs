using System;
using System.Text;

namespace QuickConnectPlugin.ArgumentsFormatters {

    public class WindowsTerminalSshArgumentsFormatter : IArgumentsFormatter {

        public String ExecutablePath { get; private set; }
        public bool EnableStartupCommand { get; private set; }
        public string StartupCommand { get; private set; }

        public WindowsTerminalSshArgumentsFormatter(String executablePath) {
            this.ExecutablePath = executablePath;
        }

        public WindowsTerminalSshArgumentsFormatter(String executablePath, bool enableStartupCommand, string startupCommand) {
            this.ExecutablePath = executablePath;
            this.EnableStartupCommand = enableStartupCommand;
            this.StartupCommand = startupCommand;
        }

        public String Format(IHostPwEntry hostPwEntry) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\"{0}\"", this.ExecutablePath);
            sb.Append(" new-tab -- ssh");

            string ipAddress = null;
            string port = null;

            if (hostPwEntry.IPAddress.Contains(":")) {
                ipAddress = hostPwEntry.IPAddress.Substring(0, hostPwEntry.IPAddress.IndexOf(':'));
                port = hostPwEntry.IPAddress.Substring(hostPwEntry.IPAddress.IndexOf(':') + 1);
            }
            else {
                ipAddress = hostPwEntry.IPAddress;
            }

            PuttyOptions options = null;
            bool success = PuttyOptions.TryParse(hostPwEntry.AdditionalOptions, out options);
            string startupCommand = null;
            var hasStartupCommand = SshStartupCommandFormatter.TryGetStartupCommand(
                success ? options : null,
                this.EnableStartupCommand,
                this.StartupCommand,
                out startupCommand);

            if (hasStartupCommand) {
                sb.Append(" -t");
            }

            if (!String.IsNullOrEmpty(port)) {
                sb.AppendFormat(" -p {0}", port);
            }
            else if (success && options.Port.HasValue) {
                sb.AppendFormat(" -p {0}", options.Port.Value);
            }

            if (success && options.HasKeyFile()) {
                sb.AppendFormat(" -i \"{0}\"", options.KeyFilePath);
            }

            sb.AppendFormat(" {0}@{1}", hostPwEntry.GetUsername(), ipAddress);

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
