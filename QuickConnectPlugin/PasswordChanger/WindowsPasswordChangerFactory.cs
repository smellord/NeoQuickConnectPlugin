using System;

namespace QuickConnectPlugin.PasswordChanger {

    public class WindowsPasswordChangerFactory : IPasswordChangerGenericFactory<IPasswordChanger> {

        private IPasswordChanger passwordChanger;

        public WindowsPasswordChangerFactory(IPasswordChanger passwordChanger) {
            this.passwordChanger = passwordChanger;
        }

        public IPasswordChanger Create() {
            return this.passwordChanger;
        }
    }
}
