using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using QuickConnectPlugin.PasswordChanger.Services;
using QuickConnectPlugin.Workers;

namespace QuickConnectPlugin.PasswordChanger {

    public delegate void PasswordChangeStartingEventHandler(object sender, BatchPasswordChangerEventArgs e);
    public delegate void PasswordChangedEventHandler(object sender, BatchPasswordChangerEventArgs e);
    public delegate void PasswordChangeErrorEventHandler(object sender, BatchPasswordChangerErrorEventArgs e);
    public delegate void PasswordChangeCompletedEventHandler(object sender, EventArgs e);

    public class BatchPasswordChangerWorker : CancelableWorker {

        public bool SaveDatabaseAfterEachUpdate { get; set; }

        public event PasswordChangeStartingEventHandler Starting;
        public event PasswordChangedEventHandler Changed;
        public event PasswordChangeErrorEventHandler Error;
        public event PasswordChangeCompletedEventHandler Completed;

        private IPasswordChangerService service;
        private IList<BatchPasswordChangeRequest> requests;

        public BatchPasswordChangerWorker(IPasswordChangerService service, ICollection<IHostPwEntry> entries, String newPassword)
            : this(service, buildRequests(entries, newPassword)) {
        }

        public BatchPasswordChangerWorker(IPasswordChangerService service, ICollection<BatchPasswordChangeRequest> requests) {
            this.service = service;
            this.requests = new List<BatchPasswordChangeRequest>(requests);
        }

        protected override void OnRun() {
            bool databaseIsDirty = false;
            for (int i = 0; i < this.requests.Count; i++) {
                var request = this.requests[i];
                try {
                    this.OnPasswordChangeStarting(request, i);
                    this.service.ChangePassword(request.HostPwEntry, request.NewPassword);
                    if (this.SaveDatabaseAfterEachUpdate) {
                        this.service.SaveDatabase();
                    }
                    else {
                        databaseIsDirty = true;
                    }
                    this.OnPasswordChanged(request, i);
                }
                catch (Exception ex) {
                    this.OnPasswordChangeError(request, ex, i);
                }

                if (this.WasCanceled) {
                    break;
                }
            }

            if (databaseIsDirty) {
                this.service.SaveDatabase();
            }

            this.OnCompleted();
        }

        protected virtual void OnPasswordChangeStarting(BatchPasswordChangeRequest request, int index) {
            PasswordChangeStartingEventHandler handler = Starting;
            if (handler != null) {
                handler(this, new BatchPasswordChangerEventArgs(request.HostPwEntry) {
                    NewPassword = request.NewPassword,
                    ProcessedEntries = index,
                    TotalEntries = this.requests.Count
                });
            }
        }

        protected virtual void OnPasswordChanged(BatchPasswordChangeRequest request, int index) {
            PasswordChangedEventHandler handler = Changed;
            if (handler != null) {
                handler(this, new BatchPasswordChangerEventArgs(request.HostPwEntry) {
                    NewPassword = request.NewPassword,
                    OperationDetails = this.service.LastOperationDetails,
                    ProcessedEntries = index + 1,
                    TotalEntries = this.requests.Count
                });
            }
        }

        protected virtual void OnPasswordChangeError(BatchPasswordChangeRequest request, Exception ex, int index) {
            PasswordChangeErrorEventHandler handler = Error;
            if (handler != null) {
                handler(this, new BatchPasswordChangerErrorEventArgs(request.HostPwEntry, ex) {
                    NewPassword = request.NewPassword,
                    ProcessedEntries = index + 1,
                    TotalEntries = this.requests.Count
                });
            }
        }

        protected virtual void OnCompleted() {
            PasswordChangeCompletedEventHandler handler = Completed;
            if (handler != null) {
                handler(this, new EventArgs());
            }
        }

        private static ICollection<BatchPasswordChangeRequest> buildRequests(ICollection<IHostPwEntry> entries, string newPassword) {
            var results = new Collection<BatchPasswordChangeRequest>();
            foreach (var entry in entries) {
                results.Add(new BatchPasswordChangeRequest(entry, newPassword));
            }
            return results;
        }
    }
}
