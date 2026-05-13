using System;
using System.IO;
using System.Text;

namespace QuickConnectPlugin.ArgumentsFormatters {

    internal static class SshStartupCommandFormatter {

        public static bool TryGetStartupCommand(PuttyOptions options, bool enabled, string command, out string startupCommand) {
            startupCommand = null;

            if (options != null && options.HasCommand()) {
                return false;
            }

            if (!enabled || String.IsNullOrEmpty(command) || command.Trim().Length == 0) {
                return false;
            }

            startupCommand = command.Trim();
            return true;
        }

        public static string QuoteRemoteCommand(string command) {
            return String.Format("\"{0}\"", command.Replace(@"""", @"\"""));
        }

        public static string CreatePuttyCommandFile(string command) {
            var directoryPath = Path.Combine(Path.GetTempPath(), "NeoQuickConnectPlugin");
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, String.Format("ssh-startup-{0}.cmd", Guid.NewGuid().ToString("N")));
            File.WriteAllText(filePath, command + Environment.NewLine, new UTF8Encoding(false));

            return filePath;
        }
    }
}
