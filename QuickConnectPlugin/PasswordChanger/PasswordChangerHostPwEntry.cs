using System;
using KeePassLib;

namespace QuickConnectPlugin.PasswordChanger {

    public class PasswordChangerHostPwEntry : HostPwEntry, IPasswordChangerHostPwEntry {

        private PwEntry pwEntry;

        public PasswordChangerHostPwEntry(PwEntry pwEntry, PwDatabase pwDatabase, IFieldMapper fieldMapper)
            : base(pwEntry, pwDatabase, fieldMapper) {
            this.pwEntry = pwEntry;
        }

        public new string Title {
            get { return base.Title; }
        }

        public HostType HostType {
            get {
                return new HostTypeMapper(new HostTypeSafeConverter()).Get(this);
            }
        }

        public string PasswordExpiresIn {
            get {
                if (!this.pwEntry.Expires) {
                    return "Never";
                }

                var today = DateTime.Now.Date;
                var expiryDate = this.pwEntry.ExpiryTime.Date;
                var daysUntilExpiry = (expiryDate - today).Days;

                if (daysUntilExpiry < 0) {
                    return "Expired";
                }

                if (daysUntilExpiry == 0) {
                    return "Today";
                }

                if (daysUntilExpiry == 1) {
                    return "1 day";
                }

                return string.Format("{0} days", daysUntilExpiry);
            }
        }
    }
}
