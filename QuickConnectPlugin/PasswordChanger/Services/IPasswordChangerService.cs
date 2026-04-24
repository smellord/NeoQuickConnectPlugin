using System;

namespace QuickConnectPlugin.PasswordChanger.Services {

    public interface IPasswordChangerService {

        string LastOperationDetails { get; }

        void ChangePassword(IHostPwEntry hostPwEntry, String newPassword);

        void UpdateEntryPassword(IHostPwEntry hostPwEntry, String newPassword);

        void SaveDatabase();
    }
}
