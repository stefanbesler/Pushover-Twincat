# PushoverCAT


[Pushover](https://pushover.net/) is a cloud-based service for sending real-time notifications. It allows you to send alerts, reminders, and messages to smartphones and computers. Pushover supports various platforms, including Android, iOS, Windows, and macOS. It provides APIs to send messages programmatically, making it ideal for industrial applications where real-time notifications are crucial.

This project allows integration of Pushover notifications directly from a PLC. It enables you to send real-time alerts and messages from your PLC over Pushover to notify users about important events, alarms, and statuses.
There are two main parts of this project

1. An ADS Server implemented with C#, which sends requests to pushover via the pushover api
1. A PLC Library, which sends requests to the ADS Server

Notifications can be send as follows (also see the Example folder in this repository)

```sti
PROGRAM MAIN
VAR
   _send : BOOL;
   _pushoverClient : PushoverClient('<your_pushover_apptoken>', '<your_pushover_user>');
END_VAR

IF _send
THEN
   _send := FALSE;
   _pushoverClient.SendMessageAsync('Hello World!');
END_IF

_pushoverClient.Cyclic();
```


## Installation

1. Download the [latest release](https://github.com/stefanbesler/pushover-cat/releases/latest)

1. Install the Windows Service by opening a Command Prompt with Administrator privileges and running:
   ```
   <Path to InstallUtil.exe>\InstallUtil.exe PushoverTwincatService.exe
   ```

   `InstallUtil.exe` can be typically found in
    - `C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe (for 32-bit)`
    - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe (for 64-bit)`

1. Start the Windows Service by running the command
   ```
   net start PushoverTwincatService
   ```

  Alternatively you can start the service in the Windows Service Dialog.


## Uninstall

1. Uninstall the Windows Service by opening a Command Prompt with Administrator privileges and running:

   ```
   net stop PushoverTwincatService
    <Path to InstallUtil.exe>\InstallUtil.exe /u PushoverTwincatService.exe
   ```

   `InstallUtil.exe` can be typically found in
    - `C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe (for 32-bit)`
    - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe (for 64-bit)`   
