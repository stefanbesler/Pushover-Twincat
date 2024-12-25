# Pushover Twincat

[Pushover](https://pushover.net/) is a cloud-based service for sending real-time notifications. It allows you to send alerts, reminders, and messages to smartphones and computers. Pushover supports various platforms, including Android, iOS, Windows, and macOS. It provides APIs to send messages programmatically, making it ideal for industrial applications where real-time notifications are crucial.

This project allows integration of Pushover notifications directly from a PLC. It enables you to send real-time alerts and messages from your PLC over Pushover to notify users about important events, alarms, and statuses.
There are two main parts of this project

1. An ADS Server implemented with C#, which sends requests to pushover via the pushover api
1. A PLC Library, which sends requests to the ADS Server


## Installation (Windows)

1. Download the [latest release](https://github.com/stefanbesler/pushover-cat/releases/latest)

1. Install and start the Windows Service by opening a Command Prompt with **Administrator privileges** and running:


   ```
   sc create PushoverService binpath="<ABSOLUTE_PATH_TO_PushoverService.exe>"
   sc start PushoverService
   ```
   
   For instance, the commands may look like
   ```
   sc create PushoverService binpath="C:\Users\Stefan\Downloads\PushoverTwincat\PushoverService.exe"
   sc start PushoverService
   
   ```
   
## Installation (Linux, BSD)

1. Download the [latest release](https://github.com/stefanbesler/pushover-cat/releases/latest)

1. Create a service file `/etc/systemd/system/pushoverservice.service`, which looks similar to

    ```
    [Unit]
    Description=Pushover TwinCAT Service

    [Service]
    ExecStart=/usr/bin/dotnet </path/to/PushoverService.dll>/PushoverService.dll
    Restart=always
    User=<youruser>
    Environment=DOTNET_ENVIRONMENT=Production

    [Install]
    WantedBy=multi-user.target
    ```

    and adapt the placeholders `</path/to/PushoverService.dll>` and `<youruser>`
  
1. Start the service by calling


   ```
   sudo systemctl enable pushoverservice
   sudo systemctl start pushoverservice
   ```


## Usage

1. Reference the `Pushover` PLC Library with the [Twinpack](https://github.com/Zeugwerk/Twinpack) Package Manager. Alternatively, get it from the [latest GitHub release](https://github.com/stefanbesler/pushover-cat/releases/latest), install and reference it manually with TwinCAT.
   
2. Use the following code for sending push notifications

   ```sti
   PROGRAM MAIN
   VAR
      _send : BOOL;
      _pushoverClient : Pushover.PushoverClient('<your_pushover_apptoken>', '<your_pushover_user>');
   END_VAR
   
   IF _send
   THEN
      _send := FALSE;
      _pushoverClient.SendMessageAsync('Hello World!');
   END_IF
   
   _pushoverClient.Cyclic();
   ```


## Uninstall (Windows)

1. Uninstall the Windows Service by opening a Command Prompt with Administrator privileges and running:

   ```
   sc stop PushoverService
   sc delete PushoverService
   ```

## Uninstall (Linux, BSD)

1. Stop the service by calling

   ```
   sudo systemctl stop pushoverservice
   ```   
1. Delete the service file previously created in `/etc/systemd/system/pushoverservice.service`


