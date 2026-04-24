namespace QuickConnectPlugin.PasswordChanger {

    public class WindowsSshPasswordChangerExFactory : IPasswordChangerExGenericFactory<IPasswordChangerEx> {

        public IPasswordChangerEx Create() {
            return new WindowsSshPasswordChangerEx();
        }
    }
}
