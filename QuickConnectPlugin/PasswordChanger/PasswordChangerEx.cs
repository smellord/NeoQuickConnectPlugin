namespace QuickConnectPlugin.PasswordChanger {

    public class PasswordChangerEx : IPasswordChangerEx, IPasswordChangerResultInfo {

        private IPasswordChanger passwordChanger;

        public string LastOperationDetails {
            get {
                var resultInfo = this.passwordChanger as IPasswordChangerResultInfo;
                return resultInfo == null ? null : resultInfo.LastOperationDetails;
            }
        }

        public PasswordChangerEx(IPasswordChanger passwordChanger) {
            this.passwordChanger = passwordChanger;
        }

        public void ChangePassword(IHostPwEntry hostPwEntry, string newPassword) {
            this.passwordChanger.ChangePassword(
                hostPwEntry.IPAddress,
                hostPwEntry.GetUsername(),
                hostPwEntry.GetPassword(),
                newPassword
            );
        }
    }
}
