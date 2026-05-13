using NUnit.Framework;
using QuickConnectPlugin.ArgumentsFormatters;

namespace QuickConnectPlugin.Tests.ArgumentsFormatters {

    [TestFixture]
    public class WindowsTerminalArgumentsFormatterTests {

        [Test]
        public void FormatWithStartupCommandForcesTtyAndRunsCommand() {
            var pwEntry = new InMemoryHostPwEntry() {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1"
            };

            var argumentsFormatter = new WindowsTerminalArgumentsFormatter(
                "wt.exe",
                "plink.exe",
                true,
                true,
                QuickConnectPluginSettings.DefaultSshStartupCommand);

            Assert.AreEqual(
                "\"wt.exe\" new-tab -- \"plink.exe\" -t -ssh -P 22 -l \"root\" -pw \"12345678\" 127.0.0.1 \"sudo -v; exec bash -l\"",
                argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithEntryCommandDoesNotAppendStartupCommand() {
            var pwEntry = new InMemoryHostPwEntry() {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1",
                AdditionalOptions = "command:uptime"
            };

            var argumentsFormatter = new WindowsTerminalArgumentsFormatter(
                "wt.exe",
                "plink.exe",
                true,
                true,
                QuickConnectPluginSettings.DefaultSshStartupCommand);

            Assert.AreEqual(
                "\"wt.exe\" new-tab -- \"plink.exe\" -batch -ssh -P 22 -l \"root\" -pw \"12345678\" 127.0.0.1 uptime",
                argumentsFormatter.Format(pwEntry));
        }
    }
}
