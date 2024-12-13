# PushoverCAT

## Installation

1. Download the [latest release](https://github.com/stefanbesler/pushover-cat/releases/latest)
1. Locate InstallUtil.exe:
   It is typically located in:
   - `C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe (for 32-bit)`
   - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe (for 64-bit)`
1. Install the Windows Service by opening a Command Prompt with Administrator privileges and running:
   ```
   <Path to InstallUtil.exe>\InstallUtil.exe PushoverCat.exe
   ```

1. Start the Windows Service by running the command
   ```
   net start PushoverCat
   ```

  Alternatively you can start the service in the Windows Service Dialog.


## Uninstall

1. Locate InstallUtil.exe:
   It is typically located in:
   - `C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe (for 32-bit)`
   - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe (for 64-bit)`
1. Uninstall the Windows Service by opening a Command Prompt with Administrator privileges and running:

   ```
   net stop PushoverCat
    <Path to InstallUtil.exe>\InstallUtil.exe /u PushoverCat.exe
   ```
