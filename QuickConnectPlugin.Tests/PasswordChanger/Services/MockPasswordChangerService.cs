using QuickConnectPlugin.PasswordChanger.Services;

namespace QuickConnectPlugin.Tests.PasswordChanger.Services {

    public class MockPasswordChangerService : IPasswordChangerService {

        public string LastOperationDetails { get; set; }
        public int ChangePasswordCount { get; private set; }
        public int UpdateEntryPasswordCount { get; private set; }
        public int SaveDatabaseCount { get; private set; }

        public void ChangePassword(IHostPwEntry hostPwEntry, string newPassword) {
            this.ChangePasswordCount++;
        }

        public void UpdateEntryPassword(IHostPwEntry hostPwEntry, string newPassword) {
            this.UpdateEntryPasswordCount++;
        }

        public void SaveDatabase() {
            this.SaveDatabaseCount++;
        }
    }
}
