namespace QuickConnectPlugin.PasswordChanger {

    public class LinuxPasswordChangerEx : IPasswordChangerEx, IPasswordChangerResultInfo {

        private ILinuxPasswordChangerFactory passwordChangerFactory;
        public string LastOperationDetails { get; private set; }

        public LinuxPasswordChangerEx(ILinuxPasswordChangerFactory passwordChangerFactory) {
            this.passwordChangerFactory = passwordChangerFactory;
        }

        public void ChangePassword(IHostPwEntry hostPwEntry, string newPassword) {
            PuttyOptions options = null;
            bool success = PuttyOptions.TryParse(hostPwEntry.AdditionalOptions, out options);

            var passwordChanger = success && options.Port.HasValue ? this.passwordChangerFactory.Create(options.Port.Value) :
                                                    this.passwordChangerFactory.Create();

            passwordChanger.ChangePassword(hostPwEntry.IPAddress, hostPwEntry.GetUsername(), hostPwEntry.GetPassword(), newPassword);
            var resultInfo = passwordChanger as IPasswordChangerResultInfo;
            this.LastOperationDetails = resultInfo == null ? null : resultInfo.LastOperationDetails;
        }
    }
}
