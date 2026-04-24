using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using QuickConnectPlugin.ArgumentsFormatters;

namespace QuickConnectPlugin {

    internal static class QuickConnectUtils {
        internal const string DefaultPuttyPath = @"%SYSTEMDRIVE%\Program Files\PuTTY\putty.exe";
        internal const string DefaultPlinkPath = @"%SYSTEMDRIVE%\Program Files\PuTTY\plink.exe";
        internal const string DefaultWinScpPath = @"%SYSTEMDRIVE%\Program Files\WinSCP\WinSCP.exe";
        internal const string DefaultWindowsTerminalPath = @"%LOCALAPPDATA%\Microsoft\WindowsApps\wt.exe";
        internal const string WinGetInstallHelpUrl = "https://learn.microsoft.com/en-us/windows/package-manager/winget/";
        internal const string WinGetScriptPath = @"scripts\Ensure-WinGet.ps1";
        internal const string PsPasswdFileName = "pspasswd.exe";
        internal const int DefaultRemotePortTimeoutMs = 2500;
        internal const int DefaultProcessTimeoutMs = 8000;

        internal static string ResolvePath(string path) {
            return String.IsNullOrWhiteSpace(path) ? path : Environment.ExpandEnvironmentVariables(path);
        }

        internal static bool FileExists(string path) {
            return !String.IsNullOrWhiteSpace(path) && File.Exists(ResolvePath(path));
        }

        internal static string NormalizeForStorage(string path) {
            if (String.IsNullOrWhiteSpace(path)) {
                return path;
            }

            var systemDrive = Environment.GetEnvironmentVariable("SYSTEMDRIVE");

            if (!String.IsNullOrEmpty(systemDrive) &&
                path.StartsWith(systemDrive + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) {
                return "%SYSTEMDRIVE%" + path.Substring(systemDrive.Length);
            }

            return path;
        }

        internal static void OpenUrl(string url) {
            Process.Start(url);
        }

        internal static string GetKeePassToolsDirectory() {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "KeePass",
                "Tools");
        }

        internal static string GetLocalPsPasswdPath() {
            return Path.Combine(GetKeePassToolsDirectory(), PsPasswdFileName);
        }

        internal static bool EnsureWinGetAvailable(IWin32Window owner) {
            if (IsWinGetAvailable()) {
                return true;
            }

            var message = new StringBuilder();
            message.AppendLine("WinGet is required to install packages from inside QuickConnect.");
            message.AppendLine();
            message.AppendLine("Do you want QuickConnect to try enabling/installing WinGet for you?");
            message.AppendLine();
            message.AppendLine("Yes: try to enable or repair WinGet automatically.");
            message.AppendLine("No: open the official WinGet install page.");
            message.AppendLine("Cancel: do nothing.");

            var result = MessageBox.Show(owner,
                message.ToString(),
                "WinGet Required",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes) {
                TryInstallOrRepairWinGet();

                if (IsWinGetAvailable()) {
                    return true;
                }

                MessageBox.Show(owner,
                    "WinGet is still not available. The official install page will now be opened.",
                    "WinGet Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                OpenUrl(WinGetInstallHelpUrl);
                return false;
            }

            if (result == DialogResult.No) {
                OpenUrl(WinGetInstallHelpUrl);
            }

            return false;
        }

        internal static bool IsWinGetAvailable() {
            try {
                using (var process = new Process()) {
                    process.StartInfo.FileName = "winget";
                    process.StartInfo.Arguments = "--version";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit(5000);
                    return process.ExitCode == 0;
                }
            }
            catch {
                return false;
            }
        }

        internal static bool IsWinGetPackageInstalled(string packageId) {
            var arguments = String.Format("list --id {0} --exact --accept-source-agreements", packageId);
            string output;
            int exitCode;
            if (!RunProcessForTextOutput("winget", arguments, 15000, out output, out exitCode)) {
                return false;
            }

            return exitCode == 0 &&
                !String.IsNullOrEmpty(output) &&
                output.IndexOf(packageId, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool InstallPackageWithWinGet(string packageId, out string errorMessage, out bool alreadyInstalled) {
            var arguments = String.Format("install --id {0} --exact --source winget --accept-source-agreements --accept-package-agreements --disable-interactivity", packageId);
            errorMessage = null;
            alreadyInstalled = false;

            try {
                string commandOutput;
                int exitCode;
                if (!RunProcessForTextOutput("winget", arguments, 300000, out commandOutput, out exitCode)) {
                    errorMessage = "The WinGet process did not complete.";
                    return false;
                }

                if (commandOutput.IndexOf("already installed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    commandOutput.IndexOf("No available upgrade found", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    commandOutput.IndexOf("No newer package versions are available", StringComparison.OrdinalIgnoreCase) >= 0) {
                    alreadyInstalled = true;
                    return true;
                }

                if (exitCode == 0) {
                    return true;
                }

                errorMessage = SanitizeConsoleOutput(commandOutput);
                return false;
            }
            catch (Exception ex) {
                errorMessage = ex.Message;
                return false;
            }
        }

        internal static string PreparePsPasswdPath() {
            var localPath = GetLocalPsPasswdPath();
            if (File.Exists(localPath) && PasswordChanger.PsPasswdWrapper.IsPsPasswdUtility(localPath)) {
                return localPath;
            }

            var detectedPath = GetPsPasswdPath();
            if (String.IsNullOrEmpty(detectedPath)) {
                return null;
            }

            return CopyPsPasswdToKeePassTools(detectedPath);
        }

        internal static string CopyPsPasswdToKeePassTools(string sourcePath) {
            var resolvedPath = ResolvePath(sourcePath);
            if (String.IsNullOrEmpty(resolvedPath) || !File.Exists(resolvedPath)) {
                return null;
            }

            var targetDirectory = GetKeePassToolsDirectory();
            Directory.CreateDirectory(targetDirectory);
            var targetPath = Path.Combine(targetDirectory, PsPasswdFileName);

            if (!String.Equals(resolvedPath, targetPath, StringComparison.OrdinalIgnoreCase)) {
                File.Copy(resolvedPath, targetPath, true);
            }

            return targetPath;
        }

        internal static bool IsTcpPortOpen(string host, int port, int timeoutMs) {
            if (String.IsNullOrEmpty(host)) {
                return false;
            }

            try {
                using (var client = new TcpClient()) {
                    var asyncResult = client.BeginConnect(host, port, null, null);
                    try {
                        if (!asyncResult.AsyncWaitHandle.WaitOne(timeoutMs, false)) {
                            return false;
                        }

                        client.EndConnect(asyncResult);
                        return true;
                    }
                    finally {
                        asyncResult.AsyncWaitHandle.Close();
                    }
                }
            }
            catch {
                return false;
            }
        }

        internal static bool IsWindowsPasswordResetMethodValid(string method) {
            return String.Equals(method, WindowsPasswordResetMethods.PsPasswd, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(method, WindowsPasswordResetMethods.Ssh, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsSshConnectionTypeValid(string method) {
            return String.Equals(method, SshConnectionTypes.WindowsTerminalSsh, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(method, SshConnectionTypes.WindowsTerminalPlink, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(method, SshConnectionTypes.Putty, StringComparison.OrdinalIgnoreCase);
        }

        internal static void TryInstallOrRepairWinGet() {
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WinGetScriptPath);

            if (File.Exists(scriptPath)) {
                var process = Process.Start(new ProcessStartInfo {
                    FileName = "powershell.exe",
                    Arguments = String.Format("-ExecutionPolicy Bypass -File \"{0}\" -AttemptInstall", scriptPath),
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(scriptPath)
                });

                if (process != null) {
                    process.WaitForExit();
                }
                return;
            }

            try {
                var process = Process.Start(new ProcessStartInfo {
                    FileName = "powershell.exe",
                    Arguments = "-ExecutionPolicy Bypass -Command \"Add-AppxPackage -RegisterByFamilyName -MainPackage Microsoft.DesktopAppInstaller_8wekyb3d8bbwe\"",
                    UseShellExecute = true
                });

                if (process != null) {
                    process.WaitForExit();
                }
            }
            catch {
            }
        }

        internal static String GetVSphereClientPath() {
            RegistryKey regKey = null;
            try {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\");
                if (regKey == null) {
                    regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\");
                }
                if (regKey != null) {
                    using (RegistryKey subRegKey = regKey.OpenSubKey("VMware, Inc.\\VMware Virtual Infrastructure Client\\")) {
                        if (subRegKey != null) {
                            String path = (String)subRegKey.GetValue("LauncherPath", null);
                            return Path.Combine(path, "VpxClient.exe");
                        }
                        return null;
                    }
                }
                return null;
            }
            catch {
                throw;
            }
            finally {
                if (regKey != null) {
                    regKey.Close();
                }
            }
        }

        internal static bool IsVSpherePowerCLIInstalled() {
            return !String.IsNullOrEmpty(GetVSpherePowerCLIPath());
        }

        internal static string GetVSpherePowerCLIPath() {
            RegistryKey regKey = null;
            try {
                regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\");
                if (regKey == null) {
                    regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\");
                }
                if (regKey != null) {
                    using (RegistryKey subRegKey = regKey.OpenSubKey("VMware, Inc.\\VMware vSphere PowerCLI\\")) {
                        if (subRegKey != null) {
                            return (String)subRegKey.GetValue("InstallPath", null);
                        }
                        return null;
                    }
                }
                return null;
            }
            catch {
                throw;
            }
            finally {
                if (regKey != null) {
                    regKey.Close();
                }
            }
        }

        internal static bool IsAtLeastRDCVersion61() {
            return new Version("6.0.6001") <= new Version(GetRDCVersion().ProductVersion);
        }

        private static FileVersionInfo GetRDCVersion() {
            return FileVersionInfo.GetVersionInfo(RemoteDesktopArgumentsFormatter.RemoteDesktopClientPath);
        }

        internal static String GetPuttyPath() {
            return FindExecutablePath("putty.exe",
                DefaultPuttyPath,
                @"%SYSTEMDRIVE%\Program Files (x86)\PuTTY\putty.exe",
                @"%LOCALAPPDATA%\Programs\PuTTY\putty.exe");
        }

        internal static String GetPlinkPath(IQuickConnectPluginSettings settings) {
            if (settings != null && FileExists(settings.PuttyPath)) {
                var candidate = Path.Combine(Path.GetDirectoryName(ResolvePath(settings.PuttyPath)), "plink.exe");
                if (File.Exists(candidate)) {
                    return candidate;
                }
            }

            return FindExecutablePath("plink.exe",
                DefaultPlinkPath,
                @"%SYSTEMDRIVE%\Program Files (x86)\PuTTY\plink.exe",
                @"%LOCALAPPDATA%\Programs\PuTTY\plink.exe");
        }

        internal static String GetWindowsTerminalPath() {
            return FindExecutablePath("wt.exe",
                DefaultWindowsTerminalPath);
        }

        internal static String GetWinScpPath() {
            return FindExecutablePath("winscp.exe",
                DefaultWinScpPath,
                @"%SYSTEMDRIVE%\Program Files (x86)\WinSCP\WinSCP.exe",
                @"%LOCALAPPDATA%\Programs\WinSCP\WinSCP.exe");
        }

        internal static String GetPsPasswdPath() {
            var localPath = GetLocalPsPasswdPath();
            if (File.Exists(localPath)) {
                return localPath;
            }

            var directPath = FindExecutablePath("pspasswd.exe",
                @"%SYSTEMDRIVE%\Program Files\SysinternalsSuite\pspasswd.exe",
                @"%SYSTEMDRIVE%\Program Files (x86)\SysinternalsSuite\pspasswd.exe");

            if (!String.IsNullOrEmpty(directPath)) {
                return directPath;
            }

            var searchRoots = new[] {
                @"%LOCALAPPDATA%\Microsoft\WinGet\Packages",
                @"%LOCALAPPDATA%\Microsoft\WinGet\Links",
                @"%LOCALAPPDATA%\Programs",
                @"%SYSTEMDRIVE%\Program Files\SysinternalsSuite",
                @"%SYSTEMDRIVE%\Program Files (x86)\SysinternalsSuite"
            };

            foreach (var searchRoot in searchRoots) {
                var path = TryFindExecutableUnderRoot(searchRoot, "pspasswd.exe");
                if (!String.IsNullOrEmpty(path)) {
                    return path;
                }
            }

            return null;
        }

        internal static bool CanOpenPutty(IQuickConnectPluginSettings settings, IHostPwEntry hostPwEntry, out string puttyPath)
        {
            puttyPath = FileExists(settings.PuttyPath) ? ResolvePath(settings.PuttyPath) : GetPuttyPath();
            return hostPwEntry.HasIPAddress && !String.IsNullOrEmpty(puttyPath);
        }

        internal static bool CanOpenWindowsTerminal(IQuickConnectPluginSettings settings, IHostPwEntry hostPwEntry, out string windowsTerminalPath, out string plinkPath)
        {
            windowsTerminalPath = GetWindowsTerminalPath();
            plinkPath = GetPlinkPath(settings);
            return hostPwEntry.HasIPAddress && !String.IsNullOrEmpty(windowsTerminalPath) && !String.IsNullOrEmpty(plinkPath);
        }

        internal static bool IsPuttyHostKeyCached(string host, int port, Services.IRegistryService registryService) {
            if (String.IsNullOrEmpty(host) || registryService == null) {
                return false;
            }

            var normalizedHost = host.Trim().ToLowerInvariant();
            var normalizedSuffix = String.Format("@{0}:{1}", port, normalizedHost);

            foreach (var valueName in registryService.GetPuttyHostKeys()) {
                if (valueName != null && valueName.ToLowerInvariant().EndsWith(normalizedSuffix, StringComparison.Ordinal)) {
                    return true;
                }
            }

            return false;
        }

        internal static bool CanOpenWinScp(IQuickConnectPluginSettings settings, IHostPwEntry hostPwEntry, out string winScpPath)
        {
            winScpPath = FileExists(settings.WinScpPath) ? ResolvePath(settings.WinScpPath) : GetWinScpPath();
            return hostPwEntry.HasIPAddress && !String.IsNullOrEmpty(winScpPath);
        }

        private static string FindExecutablePath(string executableName, params string[] fallbackCandidates) {
            var candidates = GetAppPaths(executableName).Concat(fallbackCandidates ?? Enumerable.Empty<string>());

            foreach (var candidate in candidates.Where(x => !String.IsNullOrWhiteSpace(x))) {
                var resolvedPath = ResolvePath(candidate.Trim('"'));
                if (File.Exists(resolvedPath)) {
                    return resolvedPath;
                }
            }

            return null;
        }

        private static string TryFindExecutableUnderRoot(string rootPath, string executableName) {
            try {
                var resolvedRootPath = ResolvePath(rootPath);

                if (String.IsNullOrEmpty(resolvedRootPath) || !Directory.Exists(resolvedRootPath)) {
                    return null;
                }

                var matches = Directory.GetFiles(resolvedRootPath, executableName, SearchOption.AllDirectories);
                return matches.FirstOrDefault();
            }
            catch {
                return null;
            }
        }

        private static string[] GetAppPaths(string executableName) {
            var registryViews = new[] { RegistryView.Registry64, RegistryView.Registry32 };
            var hives = new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine };
            var keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + executableName;

            return hives
                .SelectMany(hive => registryViews.Select(view => GetAppPathValue(hive, view, keyPath)))
                .Where(path => !String.IsNullOrWhiteSpace(path))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string GetAppPathValue(RegistryHive hive, RegistryView view, string keyPath) {
            try {
                using (var baseKey = RegistryKey.OpenBaseKey(hive, view))
                using (var appPathKey = baseKey.OpenSubKey(keyPath)) {
                    return appPathKey == null ? null : appPathKey.GetValue(string.Empty) as string;
                }
            }
            catch {
                return null;
            }
        }

        private static bool RunProcessForTextOutput(string fileName, string arguments, int timeoutMs, out string output, out int exitCode) {
            output = null;
            exitCode = -1;

            using (var process = new Process()) {
                var standardOutput = new StringBuilder();
                var standardError = new StringBuilder();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data != null) {
                        standardOutput.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data != null) {
                        standardError.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit(timeoutMs)) {
                    try {
                        process.Kill();
                    }
                    catch {
                    }
                    output = SanitizeConsoleOutput(standardError.ToString() + Environment.NewLine + standardOutput.ToString());
                    return false;
                }

                process.WaitForExit();
                exitCode = process.ExitCode;
                output = SanitizeConsoleOutput(standardError.ToString() + Environment.NewLine + standardOutput.ToString());
                return true;
            }
        }

        private static string SanitizeConsoleOutput(string text) {
            if (String.IsNullOrEmpty(text)) {
                return text;
            }

            var builder = new StringBuilder();
            foreach (var ch in text) {
                if (ch == '\r' || ch == '\n' || ch == '\t' || !Char.IsControl(ch)) {
                    builder.Append(ch);
                }
            }

            var lines = builder.ToString()
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .Distinct()
                .ToArray();

            return String.Join(Environment.NewLine, lines);
        }
    }
}
