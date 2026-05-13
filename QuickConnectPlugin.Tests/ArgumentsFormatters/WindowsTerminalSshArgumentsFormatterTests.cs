using NUnit.Framework;
using QuickConnectPlugin.ArgumentsFormatters;

namespace QuickConnectPlugin.Tests.ArgumentsFormatters {

    [TestFixture]
    public class WindowsTerminalSshArgumentsFormatterTests {

        [Test]
        public void FormatWithStartupCommandForcesTtyAndRunsCommand() {
            var pwEntry = new InMemoryHostPwEntry() {
                Username = "root",
                IPAddress = "127.0.0.1"
            };

            var argumentsFormatter = new WindowsTerminalSshArgumentsFormatter(
                "wt.exe",
                true,
                QuickConnectPluginSettings.DefaultSshStartupCommand);

            Assert.AreEqual(
                "\"wt.exe\" new-tab -- ssh -t root@127.0.0.1 \"sudo -v; exec bash -l\"",
                argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithEntryCommandDoesNotAppendStartupCommand() {
            var pwEntry = new InMemoryHostPwEntry() {
                Username = "root",
                IPAddress = "127.0.0.1",
                AdditionalOptions = "command:uptime"
            };

            var argumentsFormatter = new WindowsTerminalSshArgumentsFormatter(
                "wt.exe",
                true,
                QuickConnectPluginSettings.DefaultSshStartupCommand);

            Assert.AreEqual(
                "\"wt.exe\" new-tab -- ssh root@127.0.0.1 uptime",
                argumentsFormatter.Format(pwEntry));
        }
    }
}
