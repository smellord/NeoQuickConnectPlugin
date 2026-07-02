using NUnit.Framework;
using QuickConnectPlugin.ArgumentsFormatters;

namespace QuickConnectPlugin.Tests.ArgumentsFormatters
{
    [TestFixture]
    public class WinScpArgumentsFormatterTests
    {
        [Test]
        public void Format()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter("WinSCP.exe");
            Assert.AreEqual("\"WinSCP.exe\" scp://root:\"12345678\"@127.0.0.1", argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithCustomPort()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1",
                AdditionalOptions = "port:50000"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter("WinSCP.exe");
            Assert.AreEqual("\"WinSCP.exe\" scp://root:\"12345678\"@127.0.0.1:50000", argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithCustomPortAndCustomProtocol()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1",
                AdditionalOptions = "port:50000;protocol:sftp"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter("WinSCP.exe");
            Assert.AreEqual("\"WinSCP.exe\" sftp://root:\"12345678\"@127.0.0.1:50000", argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithKeyFile()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1",
                AdditionalOptions = "key:\"C:\\Key Files\\PrivateKey.ppk\""
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter("WinSCP.exe");
            Assert.AreEqual("\"WinSCP.exe\" scp://root@127.0.0.1 /privatekey=\"C:\\Key Files\\PrivateKey.ppk\" /passphrase=\"12345678\"", argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithJumpHost()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "gmelis",
                Password = "12345678",
                IPAddress = "s99-eanvapp1",
                AdditionalOptions = "protocol:sftp"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter(
                "WinSCP.exe",
                new WinScpLaunchOptions()
                {
                    UseJumpHost = true,
                    JumpHostName = "s-1564-ew-test",
                    JumpPort = "22",
                    JumpUsername = "gianluca.melis",
                    JumpPrivateKeyPath = "C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk",
                    DefaultPrivateKeyPath = "C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk",
                    PrivateKeyPassphraseFilePath = "C:\\Temp\\winscp-passphrase.txt"
                });

            Assert.AreEqual(
                "\"WinSCP.exe\" sftp://gmelis@s99-eanvapp1 /privatekey=\"C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk\" /passwordsfromfiles /passphrase=\"C:\\Temp\\winscp-passphrase.txt\" /rawsettings \"Tunnel=1\" \"TunnelHostName=s-1564-ew-test\" \"TunnelPortNumber=22\" \"TunnelUserName=gianluca.melis\" \"TunnelPublicKeyFile=C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk\"",
                argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithJumpHostKeepsEntryKeyPriority()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "gmelis",
                Password = "12345678",
                IPAddress = "s99-eanvapp1",
                AdditionalOptions = "protocol:sftp;key:\"C:\\Entry\\entry.ppk\""
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter(
                "WinSCP.exe",
                new WinScpLaunchOptions()
                {
                    UseJumpHost = true,
                    JumpHostName = "s-1564-ew-test",
                    JumpPort = "22",
                    JumpUsername = "gianluca.melis",
                    JumpPrivateKeyPath = "C:\\Tunnel\\tunnel.ppk",
                    DefaultPrivateKeyPath = "C:\\Fallback\\fallback.ppk",
                    UseEntryPasswordAsPrivateKeyPassphrase = false
                });

            Assert.AreEqual(
                "\"WinSCP.exe\" sftp://gmelis@s99-eanvapp1 /privatekey=\"C:\\Entry\\entry.ppk\" /rawsettings \"Tunnel=1\" \"TunnelHostName=s-1564-ew-test\" \"TunnelPortNumber=22\" \"TunnelUserName=gianluca.melis\" \"TunnelPublicKeyFile=C:\\Tunnel\\tunnel.ppk\"",
                argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithSudoSftpServer()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "gmelis",
                Password = "12345678",
                IPAddress = "s99-ew-mgmt",
                AdditionalOptions = "protocol:sftp"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter(
                "WinSCP.exe",
                new WinScpLaunchOptions()
                {
                    UseJumpHost = true,
                    JumpHostName = "s-1564-ew-test",
                    JumpPort = "22",
                    JumpUsername = "gianluca.melis",
                    JumpPrivateKeyPath = "C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk",
                    DefaultPrivateKeyPath = "C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk",
                    UseSudoSftpServer = true,
                    SudoSftpServerCommand = QuickConnectPluginSettings.DefaultWinScpSudoSftpServerCommand
                });

            Assert.AreEqual(
                "\"WinSCP.exe\" sftp://gmelis@s99-ew-mgmt /privatekey=\"C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk\" /rawsettings \"Tunnel=1\" \"TunnelHostName=s-1564-ew-test\" \"TunnelPortNumber=22\" \"TunnelUserName=gianluca.melis\" \"TunnelPublicKeyFile=C:\\Users\\gianluca.melis\\.ssh\\id_ed25519.ppk\" \"SftpServer=sudo -n /usr/lib/openssh/sftp-server\"",
                argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithSudoSftpServerDoesNotAffectScp()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "gmelis",
                Password = "12345678",
                IPAddress = "s99-ew-mgmt"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter(
                "WinSCP.exe",
                new WinScpLaunchOptions()
                {
                    UseSudoSftpServer = true,
                    SudoSftpServerCommand = QuickConnectPluginSettings.DefaultWinScpSudoSftpServerCommand
                });

            Assert.AreEqual(
                "\"WinSCP.exe\" scp://gmelis:\"12345678\"@s99-ew-mgmt",
                argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithPortFromHostAddress()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "root",
                Password = "12345678",
                IPAddress = "127.0.0.1:2222"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter("WinSCP.exe");
            Assert.AreEqual("\"WinSCP.exe\" scp://root:\"12345678\"@127.0.0.1:2222", argumentsFormatter.Format(pwEntry));
        }

        [Test]
        public void FormatWithSpecialCharsInPassword()
        {
            InMemoryHostPwEntry pwEntry = new InMemoryHostPwEntry()
            {
                Username = "root",
                Password = "#m+y/p@ssw0rd",
                IPAddress = "127.0.0.1:2222"
            };

            WinScpArgumentsFormatter argumentsFormatter = new WinScpArgumentsFormatter("WinSCP.exe");
            Assert.AreEqual("\"WinSCP.exe\" scp://root:\"%23m%2By%2Fp%40ssw0rd\"@127.0.0.1:2222", argumentsFormatter.Format(pwEntry));
        }
    }
}
