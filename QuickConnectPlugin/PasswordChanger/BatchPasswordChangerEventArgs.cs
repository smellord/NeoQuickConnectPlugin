using System;

namespace QuickConnectPlugin.PasswordChanger {

    public class BatchPasswordChangerEventArgs : EventArgs {

        public IHostPwEntry HostPwEntry { get; private set; }
        public string NewPassword { get; set; }
        public string OperationDetails { get; set; }
        public int ProcessedEntries { get; set; }
        public int TotalEntries { get; set; }

        public BatchPasswordChangerEventArgs(IHostPwEntry hostPwEntry) {
            this.HostPwEntry = hostPwEntry;
        }
    }
}
