namespace QuickConnectPlugin.ArgumentsFormatters {

    public class WinScpLaunchOptions {

        public bool UseJumpHost { get; set; }

        public string JumpHostName { get; set; }

        public string JumpPort { get; set; }

        public string JumpUsername { get; set; }

        public string JumpPrivateKeyPath { get; set; }

        public string DefaultPrivateKeyPath { get; set; }

        public string PrivateKeyPassphraseFilePath { get; set; }

        public bool UseEntryPasswordAsPrivateKeyPassphrase { get; set; }

        public bool UseSudoSftpServer { get; set; }

        public string SudoSftpServerCommand { get; set; }
    }
}
