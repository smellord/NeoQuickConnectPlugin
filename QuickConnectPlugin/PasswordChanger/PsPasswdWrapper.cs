using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace QuickConnectPlugin.PasswordChanger {

    public class PsPasswdWrapper {

        public const String PsPasswdProductName = "Sysinternals PsPasswd";
        public const int DefaultProcessTimeoutMs = 30000;

        public String Path { get; private set; }

        public bool SuppressLicenseDialog { get; set; }
        public string LastOperationDetails { get; private set; }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
        public PsPasswdWrapper(String psPasswdPath) {
            if (!File.Exists(psPasswdPath)) {
                throw new FileNotFoundException("The specified file was not found.");
            }
            if (!IsPsPasswdUtility(psPasswdPath)) {
                throw new Exception("The specified file is not valid.");
            }
            this.Path = psPasswdPath;
        }

        internal static bool IsPsPasswdUtility(String pspasswdPath) {
            return PsPasswdProductName.Equals(FileVersionInfo.GetVersionInfo(pspasswdPath).ProductName);
        }

        internal void ChangePassword(String host, String username, String password, String account, String newPassword) {
            if (String.IsNullOrEmpty(host)) {
                throw new ArgumentException("Host cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(username)) {
                throw new ArgumentException("Username cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(password)) {
                throw new ArgumentException("Password cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(account)) {
                throw new ArgumentException("Account cannot be null or an empty string.");
            }
            if (String.IsNullOrEmpty(newPassword)) {
                throw new ArgumentException("New password cannot be null or an empty string.");
            }

            var preflightResult = this.CheckRemotePrerequisites(host);
            if (!preflightResult.Success) {
                this.LastOperationDetails = preflightResult.Details;
                throw new Exception(preflightResult.Message);
            }

            String response = this.internalChangePassword(host, username, password, account, newPassword);
            this.LastOperationDetails = response;

            if (response.StartsWith("Error changing password:", StringComparison.OrdinalIgnoreCase)) {
                throw new Exception(response.Substring(response.IndexOf(':') + 1).Trim());
            }
        }

        internal PsPasswdPreflightResult CheckRemotePrerequisites(string host) {
            var normalizedHost = NormalizeHost(host);
            if (String.IsNullOrEmpty(normalizedHost)) {
                return new PsPasswdPreflightResult(false, "Host name is empty.", "No host was provided for PsPasswd.");
            }

            var details = new StringBuilder();

            if (!QuickConnectUtils.IsTcpPortOpen(normalizedHost, 135, QuickConnectUtils.DefaultRemotePortTimeoutMs)) {
                return new PsPasswdPreflightResult(
                    false,
                    "RPC endpoint mapper port 135 is not reachable on the target host.",
                    "PsPasswd pre-check failed: TCP 135 is closed or filtered.");
            }

            details.AppendLine("TCP 135 reachable.");

            if (!QuickConnectUtils.IsTcpPortOpen(normalizedHost, 445, QuickConnectUtils.DefaultRemotePortTimeoutMs)) {
                return new PsPasswdPreflightResult(
                    false,
                    "SMB port 445 is not reachable on the target host.",
                    details.ToString() + Environment.NewLine + "PsPasswd pre-check failed: TCP 445 is closed or filtered.");
            }

            details.AppendLine("TCP 445 reachable.");

            string[] serviceNames = new[] { "RpcSs", "RpcEptMapper", "DcomLaunch" };
            foreach (var serviceName in serviceNames) {
                string serviceOutput;
                if (!RunScQuery(normalizedHost, serviceName, out serviceOutput)) {
                    return new PsPasswdPreflightResult(
                        false,
                        string.Format("Unable to query the remote service '{0}'.", serviceName),
                        details.ToString() + Environment.NewLine + serviceOutput);
                }

                if (serviceOutput.IndexOf("RUNNING", StringComparison.OrdinalIgnoreCase) < 0) {
                    return new PsPasswdPreflightResult(
                        false,
                        string.Format("The remote service '{0}' is not running.", serviceName),
                        details.ToString() + Environment.NewLine + serviceOutput);
                }

                details.AppendLine(string.Format("Service {0}: RUNNING.", serviceName));
            }

            return new PsPasswdPreflightResult(true, null, details.ToString().Trim());
        }

        private String internalChangePassword(String host, String username, String password, String account, String newPassword) {
            StringBuilder arguments = new StringBuilder();
            arguments.AppendFormat("\\\\{0} -u \"{1}\" -p \"{2}\" \"{3}\" \"{4}\"", NormalizeHost(host), username, password, account, newPassword);
            if (this.SuppressLicenseDialog) {
                arguments.Append(" --accepteula");
            }
            using (Process process = new Process()) {
                var standardOutput = new StringBuilder();
                var standardError = new StringBuilder();
                process.StartInfo.FileName = this.Path;
                process.StartInfo.Arguments = arguments.ToString();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data != null) {
                        standardOutput.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data != null) {
                        standardError.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (!process.WaitForExit(DefaultProcessTimeoutMs)) {
                    try {
                        process.Kill();
                    }
                    catch {
                    }

                    throw new Exception("PsPasswd timed out while waiting for the remote host. Check that RPC and SMB are reachable and that the required remote services are running.");
                }
                process.WaitForExit();

                var stdError = standardError.ToString();
                var stdOutput = standardOutput.ToString();

                var outputLines = (stdError + Environment.NewLine + stdOutput)
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (String outputLine in outputLines) {
                    String line = outputLine.Trim();

                    if (line.StartsWith("Error changing password:", StringComparison.OrdinalIgnoreCase)) {
                        return line;
                    }

                    if (line.Equals("Password successfully changed.", StringComparison.OrdinalIgnoreCase) ||
                        (line.StartsWith("Password for", StringComparison.OrdinalIgnoreCase) &&
                        line.EndsWith("successfully changed.", StringComparison.OrdinalIgnoreCase))) {
                        return line;
                    }
                }

                if (process.ExitCode != 0) {
                    throw new Exception(String.Format("PsPasswd exited with code {0}. {1}", process.ExitCode, (stdError + " " + stdOutput).Trim()));
                }

                throw new Exception("An unspecified error has occurred.");
            }
        }

        private static string NormalizeHost(string host) {
            if (String.IsNullOrEmpty(host)) {
                return host;
            }

            var result = host.Trim();
            if (result.StartsWith("\\\\")) {
                result = result.Substring(2);
            }

            var separatorIndex = result.IndexOf(':');
            if (separatorIndex > 0) {
                result = result.Substring(0, separatorIndex);
            }

            return result;
        }

        private static bool RunScQuery(string host, string serviceName, out string output) {
            using (var process = new Process()) {
                var standardOutput = new StringBuilder();
                var standardError = new StringBuilder();
                process.StartInfo.FileName = "sc.exe";
                process.StartInfo.Arguments = String.Format("\\\\{0} query {1}", host, serviceName);
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data != null) {
                        standardOutput.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data != null) {
                        standardError.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit(QuickConnectUtils.DefaultProcessTimeoutMs)) {
                    try {
                        process.Kill();
                    }
                    catch {
                    }
                    output = String.Format("Timed out while querying service '{0}' on host '{1}'.", serviceName, host);
                    return false;
                }

                process.WaitForExit();
                output = (standardError.ToString() + Environment.NewLine + standardOutput.ToString()).Trim();
                return process.ExitCode == 0;
            }
        }
    }

    internal class PsPasswdPreflightResult {

        internal bool Success { get; private set; }
        internal string Message { get; private set; }
        internal string Details { get; private set; }

        internal PsPasswdPreflightResult(bool success, string message, string details) {
            this.Success = success;
            this.Message = message;
            this.Details = details;
        }
    }
}
