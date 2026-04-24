using System;
using DisruptiveSoftware.Time.Clocks;

namespace QuickConnectPlugin.PasswordChanger.Services {

    public class PasswordChangerService : IPasswordChangerService {

        private IPasswordDatabase passwordDatabase;
        private IPasswordChangerExFactory passwordChangerFactory;
        private IHostTypeMapper hostTypeMapper;
        private IClock clock;

        public string LastOperationDetails { get; private set; }

        public PasswordChangerService(IPasswordDatabase passwordDatabase,
            IPasswordChangerExFactory passwordChangerFactory,
            IHostTypeMapper hostTypeMapper, IClock clock) {
            this.passwordDatabase = passwordDatabase;
            this.passwordChangerFactory = passwordChangerFactory;
            this.hostTypeMapper = hostTypeMapper;
            this.clock = clock;
        }

        public void ChangePassword(IHostPwEntry hostPwEntry, string newPassword) {
            var passwordChanger = passwordChangerFactory.Create(this.hostTypeMapper.Get(hostPwEntry));
            passwordChanger.ChangePassword(hostPwEntry, newPassword);
            this.LastOperationDetails = this.getPasswordChangerOperationDetails(passwordChanger);
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

        private string getPasswordChangerOperationDetails(IPasswordChangerEx passwordChanger) {
            var passwordChangerResultInfo = passwordChanger as IPasswordChangerResultInfo;
            if (passwordChangerResultInfo != null) {
                return passwordChangerResultInfo.LastOperationDetails;
            }
            return null;
        }
    }
}
