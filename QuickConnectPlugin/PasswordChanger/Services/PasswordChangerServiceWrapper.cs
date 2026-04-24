using System;
using DisruptiveSoftware.Time.Clocks;

namespace QuickConnectPlugin.PasswordChanger.Services {

    public class PasswordChangerServiceWrapper : IPasswordChangerService {

        private IPasswordDatabase passwordDatabase;
        private IPasswordChanger passwordChanger;
        private IClock clock;

        public string LastOperationDetails { get; private set; }

        public PasswordChangerServiceWrapper(IPasswordDatabase passwordDatabase, IPasswordChanger passwordChanger, IClock clock) {
            this.passwordDatabase = passwordDatabase;
            this.passwordChanger = passwordChanger;
            this.clock = clock;
        }

        public void ChangePassword(IHostPwEntry hostPwEntry, string newPassword) {
            passwordChanger.ChangePassword(hostPwEntry.IPAddress, hostPwEntry.GetUsername(), hostPwEntry.GetPassword(), newPassword);
            this.LastOperationDetails = this.getPasswordChangerOperationDetails();
            this.UpdateEntryPassword(hostPwEntry, newPassword);
        }

        public void UpdateEntryPassword(IHostPwEntry hostPwEntry, string newPassword) {
            hostPwEntry.UpdatePassword(newPassword);
            hostPwEntry.LastModificationTime = this.clock.Now;
            if (String.IsNullOrEmpty(this.LastOperationDetails)) {
                this.LastOperationDetails = "KeePass entry updated successfully.";
            }
        }

        public void SaveDatabase() {
            this.passwordDatabase.Save();
        }

        private string getPasswordChangerOperationDetails() {
            var passwordChangerResultInfo = this.passwordChanger as IPasswordChangerResultInfo;
            if (passwordChangerResultInfo != null) {
                return passwordChangerResultInfo.LastOperationDetails;
            }
            return null;
        }
    }
}
