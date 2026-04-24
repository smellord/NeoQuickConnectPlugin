namespace QuickConnectPlugin.PasswordChanger {

    public class WindowsSshPasswordChangerEx : IPasswordChangerEx, IPasswordChangerResultInfo {

        public string LastOperationDetails { get; private set; }

        public void ChangePassword(IHostPwEntry hostPwEntry, string newPassword) {
            PuttyOptions options = null;
            bool success = PuttyOptions.TryParse(hostPwEntry.AdditionalOptions, out options);

            var passwordChanger = new WindowsSshPasswordChanger();
            if (success && options.Port.HasValue) {
                passwordChanger.SshPort = options.Port.Value;
            }

            passwordChanger.ChangePassword(
                hostPwEntry.IPAddress,
                hostPwEntry.GetUsername(),
                hostPwEntry.GetPassword(),
                newPassword);
            this.LastOperationDetails = passwordChanger.LastOperationDetails;
        }
    }
}
