using System.Security.Permissions;

namespace QuickConnectPlugin.PasswordChanger {

    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class WindowsPasswordChanger : IPasswordChanger, IPasswordChangerResultInfo {

        private PsPasswdWrapper wrapper;

        public string LastOperationDetails {
            get {
                return this.wrapper.LastOperationDetails;
            }
        }

        public WindowsPasswordChanger(PsPasswdWrapper psPasswdWrapper) {
            this.wrapper = psPasswdWrapper;
        }

        public void ChangePassword(string host, string username, string password, string newPassword) {
            this.wrapper.ChangePassword(host, username, password, username, newPassword);
        }
    }
}
