# Modified Fork Notes

This branch is prepared as a clean, source-first fork of `cristianst85/QuickConnectPlugin`.

## Upstream Base

- Upstream repository: https://github.com/cristianst85/QuickConnectPlugin
- Upstream base tag: `0.6.1`
- Upstream base commit used locally: `813389b4aa12fc9ab91a8ebfeb6c699052b10662`
- Local branch name: `gianluca-enhancements-from-0.6.1`
- Comparison date: 2026-04-24

## Cleanup Applied Before Comparison

The local source snapshot was copied onto a fresh upstream clone, then obvious machine/build artifacts were excluded:

- `.vs/`
- `build/`
- `packages/`
- `snapshots/`
- nested `bin/` and `obj/`
- generated binaries such as `*.dll`, `*.pdb`, `*.exe`, `*.plgx`, and `*.nupkg`

After removing line-ending-only noise, the meaningful delta is 43 modified tracked files plus new source/script files.

## Main Additions

### Project/runtime

- Moves the plugin project from .NET Framework 4.0 to .NET Framework 4.8.
- Updates MSBuild project metadata and disables `Prefer32Bit` for Debug and Release.
- Adds new source files to the main plugin and test project files.

### SSH launch workflow

- Adds a preferred SSH connection type setting:
  - `Putty`
  - `WindowsTerminalSsh`
  - `WindowsTerminalPlink`
- Adds a "show all SSH options" setting so users can expose all SSH launchers in the KeePass entry menu.
- Adds Windows Terminal launch support through:
  - `QuickConnectPlugin/ArgumentsFormatters/WindowsTerminalSshArgumentsFormatter.cs`
  - `QuickConnectPlugin/ArgumentsFormatters/WindowsTerminalArgumentsFormatter.cs`
- Keeps PuTTY support as the default/fallback behavior.

### Tool detection and install helpers

- Adds environment-variable-aware path resolution and storage normalization.
- Adds automatic discovery for PuTTY, plink, WinSCP, Windows Terminal, and PsPasswd.
- Adds option-dialog buttons for auto-setting and installing PuTTY, WinSCP, and PsTools through WinGet.
- Adds `scripts/Ensure-WinGet.ps1` to help enable or repair WinGet/App Installer.
- Copies PsPasswd into a KeePass-local tools folder when possible.

### Windows password changing

- Adds a Windows password reset method setting:
  - `PsPasswd`
  - `Ssh`
- Keeps PsPasswd support, but adds preflight checks for RPC/SMB reachability and required Windows services.
- Removes the hard requirement for exactly PsPasswd `1.22`, while still validating the executable product name.
- Adds Windows-over-SSH password changing through SSH.NET:
  - `QuickConnectPlugin/PasswordChanger/WindowsSshPasswordChanger.cs`
  - `QuickConnectPlugin/PasswordChanger/WindowsSshPasswordChangerEx.cs`
  - `QuickConnectPlugin/PasswordChanger/WindowsSshPasswordChangerExFactory.cs`

### Batch password changer

- Reworks the batch password changer UI and worker flow.
- Supports automatic per-host password generation.
- Supports manual per-host password entry.
- Adds per-entry request objects instead of a single shared password:
  - `QuickConnectPlugin/PasswordChanger/BatchPasswordChangeRequest.cs`
- Adds richer operation logging, including start/success/failure messages and backend operation details.
- Saves the KeePass database after each successful update when configured by the worker.

### Password generation

- Adds a cryptographic random password generator:
  - `QuickConnectPlugin/PasswordChanger/PasswordGenerator.cs`
  - `QuickConnectPlugin/PasswordChanger/PasswordComplexity.cs`
- Adds test coverage:
  - `QuickConnectPlugin.Tests/PasswordChanger/PasswordGeneratorTests.cs`

### Result details and reliability

- Adds `IPasswordChangerResultInfo` so password changers can report backend details.
- Propagates operation details through password changer services and batch events.
- Improves Linux password-changing prompt matching and captures server-side response details.

## New Files

- `QuickConnectPlugin/ArgumentsFormatters/WindowsTerminalArgumentsFormatter.cs`
- `QuickConnectPlugin/ArgumentsFormatters/WindowsTerminalSshArgumentsFormatter.cs`
- `QuickConnectPlugin/FormPasswordChangeSuccess.cs`
- `QuickConnectPlugin/FormPasswordChangeSuccess.Designer.cs`
- `QuickConnectPlugin/PasswordChanger/BatchPasswordChangeRequest.cs`
- `QuickConnectPlugin/PasswordChanger/IPasswordChangerResultInfo.cs`
- `QuickConnectPlugin/PasswordChanger/PasswordComplexity.cs`
- `QuickConnectPlugin/PasswordChanger/PasswordGenerator.cs`
- `QuickConnectPlugin/PasswordChanger/WindowsSshPasswordChanger.cs`
- `QuickConnectPlugin/PasswordChanger/WindowsSshPasswordChangerEx.cs`
- `QuickConnectPlugin/PasswordChanger/WindowsSshPasswordChangerExFactory.cs`
- `QuickConnectPlugin/SshConnectionTypes.cs`
- `QuickConnectPlugin/WindowsPasswordResetMethods.cs`
- `QuickConnectPlugin.Tests/PasswordChanger/PasswordGeneratorTests.cs`
- `scripts/Ensure-WinGet.ps1`

## Publishing Checklist

This project is GPL-2.0-or-later upstream. This is not legal advice, but the practical compliance checklist for a public fork is:

- Keep `LICENSE` and `COPYING`.
- Keep existing copyright and third-party license notices.
- Mark the repository clearly as a modified fork, not the official upstream project.
- Publish the complete corresponding source for any `.plgx`, `.dll`, or installer release you distribute.
- Keep third-party license files under `libs/`.
- Do not use upstream release badges or download links in a way that makes modified binaries look official.
- Mention the upstream base tag and the modified branch/commit in every release note.

## Suggested GitHub Workflow

For your own public fork:

```powershell
git remote rename origin upstream
git remote add origin https://github.com/<your-user>/QuickConnectPlugin.git
git push -u origin gianluca-enhancements-from-0.6.1
```

For a contribution back to upstream:

```powershell
git fetch upstream
git switch -c feature/windows-terminal-password-workflows upstream/master
# cherry-pick or split the local feature changes into smaller reviewable commits
git push -u origin feature/windows-terminal-password-workflows
```

Maintainers usually prefer smaller PRs. A good split would be:

1. Project/runtime update to .NET Framework 4.8.
2. Tool path discovery and WinGet helpers.
3. Windows Terminal SSH launcher support.
4. Windows password reset method selection and SSH reset implementation.
5. Batch password changer UI and per-entry password requests.
6. Password generator and tests.

## Suggested Release Description

This fork builds on QuickConnectPlugin `0.6.1` and adds Windows Terminal SSH launch support, automatic tool detection and installation helpers, Windows password reset over SSH, improved PsPasswd diagnostics, and a more capable batch password changer with per-host generated or manual passwords.

It remains licensed under GPL-2.0-or-later and retains the original upstream copyright and license notices.
