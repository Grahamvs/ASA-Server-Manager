# ASA Server Manager

## Description
ASA Server Manager is a simple Windows application designed to help users manage their Ark Survival Ascended servers. With the ability to create and load profiles containing server session settings, users can easily run multiple sessions off a single ARK Survival Ascended Dedicated Server installation. This is particularly useful for running servers in cluster mode as they can share the same .ini files.

The application supports automatic installation and updates of the server using SteamCMD. Alternatively, users can choose to use a non-SteamCMD version of the server (note: updates are not available in this mode). ASA Server Manager also supports a custom list of mods that users can create. Enabling or disabling a mod is as simple as clicking a checkbox. Importantly, the application does not override the server's .ini files.

## Technologies
The application is built using C# and WPF.

## Prerequisites
The [.NET 8 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is required for operation.

## Installation & Execution
The application is portable, so simply unzip and execute.

## Server Settings
For further information on any of the settings, please visit Ark's [Server Wiki](https://ark.wiki.gg/wiki/Server_configuration).

## Known Issues
As a work in progress, the application may have some bugs or missing options.

- SteamCMD may fail while trying to install / update itself or the server if it is inside a OneDrive monitored folder.
    - In rare cases, this may also result in a Blue Screen of Death (BSOD) on Windows.

## Contributing
Contributions to the project are welcome. You can submit pull requests or bug reports on the project's [GitHub](https://github.com/Grahamvs/ASA-Server-Manager) page. If you're feeling generous, you can also support the project by buying me a coffee via [PayPal](https://paypal.me/grahamvs87).

## Disclaimer
This software is a work in progress, so use it at your own risk. The developer(s) are not responsible for any loss of data or other damages resulting from the use of this software.

## License
The project is licensed under the GNU GPL v2 license.

All rights reserved.