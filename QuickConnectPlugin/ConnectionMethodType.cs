using System;

namespace QuickConnectPlugin {

    public enum ConnectionMethodType {

        Unknown = 0,
        RemoteDesktop = 1,
        RemoteDesktopConsole = 2,
        vSphereClient = 3,
        WindowsTerminalSSH = 4,
        PuttySSH = 5,
        PuttyTelnet = 6,
        WinSCP = 7
    }
}
