namespace QuickConnectPlugin.PasswordChanger {

    public class BatchPasswordChangeRequest {

        public IHostPwEntry HostPwEntry { get; private set; }
        public string NewPassword { get; private set; }

        public BatchPasswordChangeRequest(IHostPwEntry hostPwEntry, string newPassword) {
            this.HostPwEntry = hostPwEntry;
            this.NewPassword = newPassword;
        }
    }
}
