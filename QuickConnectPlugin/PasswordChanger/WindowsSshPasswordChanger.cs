using System;
using System.Text;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace QuickConnectPlugin.PasswordChanger {

    public class WindowsSshPasswordChanger : IPasswordChanger, IPasswordChangerResultInfo {

        public const int DefaultSshPort = 22;
        public const int DefaultTimeoutSeconds = 8;

        public int? SshPort { get; set; }
        public string LastOperationDetails { get; private set; }

        public void ChangePassword(string host, string username, string password, string newPassword) {
            if (String.IsNullOrEmpty(host)) {
                throw new ArgumentException("Host cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(username)) {
                throw new ArgumentException("Username cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(password)) {
                throw new ArgumentException("Password cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(newPassword)) {
                throw new ArgumentException("New password cannot be null or an empty string.");
            }

            var connectionHost = host;
            var connectionPort = this.SshPort ?? DefaultSshPort;

            if (host.Contains(":")) {
                var separatorIndex = host.IndexOf(':');
                connectionHost = host.Substring(0, separatorIndex);

                int parsedPort;
                if (int.TryParse(host.Substring(separatorIndex + 1), out parsedPort)) {
                    connectionPort = parsedPort;
                }
            }

            var sanitizedUserName = GetWindowsAccountName(username);
            var commandText = BuildWindowsPasswordCommand(sanitizedUserName, newPassword);

            using (var keyboardInteractiveAuthenticationMethod = new KeyboardInteractiveAuthenticationMethod(username)) {
                keyboardInteractiveAuthenticationMethod.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>((sender, e) => OnAuthenticationPrompt(e, password));

                using (var passwordAuthenticationMethod = new PasswordAuthenticationMethod(username, password)) {
                    var connectionInfo = new ConnectionInfo(
                        connectionHost,
                        connectionPort,
                        username,
                        new AuthenticationMethod[] {
                            keyboardInteractiveAuthenticationMethod,
                            passwordAuthenticationMethod
                        });

                    using (var sshClient = new SshClient(connectionInfo)) {
                        sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds);
                        sshClient.Connect();
                        using (var command = sshClient.CreateCommand(commandText)) {
                            command.CommandTimeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds);
                            command.Execute();

                            var details = new StringBuilder();
                            if (!String.IsNullOrEmpty(command.Result)) {
                                details.AppendLine(command.Result.Trim());
                            }
                            if (!String.IsNullOrEmpty(command.Error)) {
                                details.AppendLine(command.Error.Trim());
                            }

                            this.LastOperationDetails = details.Length == 0
                                ? "Password changed successfully over SSH."
                                : details.ToString().Trim();

                            if (command.ExitStatus != 0) {
                                throw new Exception(this.LastOperationDetails);
                            }
                        }
                    }
                }
            }
        }

        private static string BuildWindowsPasswordCommand(string username, string newPassword) {
            return String.Format(
                "powershell -NoProfile -NonInteractive -Command \"$ErrorActionPreference='Stop'; net user \\\"{0}\\\" \\\"{1}\\\" | Out-String\"",
                EscapeForPowerShellDoubleQuotedString(username),
                EscapeForPowerShellDoubleQuotedString(newPassword));
        }

        private static string GetWindowsAccountName(string username) {
            if (String.IsNullOrEmpty(username)) {
                return username;
            }

            if (username.Contains("\\")) {
                return username.Substring(username.LastIndexOf('\\') + 1);
            }

            if (username.Contains("@")) {
                return username.Substring(0, username.IndexOf('@'));
            }

            return username;
        }

        private static string EscapeForPowerShellDoubleQuotedString(string value) {
            if (value == null) {
                return String.Empty;
            }

            return value.Replace("`", "``").Replace("\"", "`\"");
        }

        private static void OnAuthenticationPrompt(AuthenticationPromptEventArgs e, string password) {
            foreach (AuthenticationPrompt prompt in e.Prompts) {
                if (prompt.Request.StartsWith("Password:", StringComparison.InvariantCultureIgnoreCase)) {
                    prompt.Response = password;
                }
            }
        }
    }
}
