# ASA Server Manager

<p align="center">
  <img src="https://github.com/Grahamvs/ASA-Server-Manager/blob/main/ScreenShots/Main%20-%20Default.png" alt="ASA Server Manager">
</p>


## Overview
The ASA Server Manager is a Windows-based tool designed to facilitate the management of Ark Survival Ascended servers. It allows users to create and load profiles with server session settings, enabling the operation of multiple sessions from a single ARK Survival Ascended Dedicated Server installation. This feature is especially beneficial for running servers in cluster mode as they can utilize the same .ini files.

The tool provides automatic installation and updates of the server via SteamCMD. However, users can opt for a non-SteamCMD server version, although this mode does not support updates. The ASA Server Manager also allows users to create a custom list of mods, which can be enabled or disabled with a simple checkbox click. Importantly, the tool does not alter the server's .ini files.


## Technology Stack
The ASA Server Manager is developed using C# and WPF using Windows.


## Requirements & Setup
The tool is available in two versions:
- "**Framework**":
    This version is compact but requires the installation of the [.NET 9.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) on the system.

    **Note**: v0.1.10 and below require the [.NET 8.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

- "**Self-contained**".
    This version is larger as it incorporates the .NET runtime, and does not need any additional runtimes to be installed.

The tool is portable, so you just need to unzip and run it.


## Server Configuration
For detailed information on any of the server settings, please visit Ark's [Server Wiki](https://ark.wiki.gg/wiki/Server_configuration).


## Current Limitations
The tool is still under development, so it may contain bugs or lack certain options.

- SteamCMD may encounter issues while attempting to install or update itself or the server if it is located in a OneDrive monitored folder. In rare instances, this may also cause a Blue Screen of Death (BSOD) on Windows


## FAQ
The FAQ has been moved to [here](https://github.com/Grahamvs/ASA-Server-Manager/blob/main/FAQ.md).

## Contributing
Contributions to the project are welcome. You can submit pull requests or bug reports on the project's [GitHub](https://github.com/Grahamvs/ASA-Server-Manager) page. If you're feeling generous, you can also support the project by buying me a coffee via [PayPal.me](https://paypal.me/grahamvs87). There is also a "Donate" menu item under the tool's "Help" menu that will open the PayPal.me link.


## Disclaimer
This software is a work in progress, so use it at your own risk. The developer(s) are not responsible for any loss of data or other damages resulting from the use of this software, nor for any damages caused by SteamCMD or the ASA server itself. The developer(s) are not responsible for any bans or other actions taken by the game's developers or publishers.

The developer(s) are no way affiliated with Steam, SteamCMD, the game's developers or publishers.

The developer(s) are also not responsible in the event you miss important events (such as the birth of your child, your wedding, spouse's birthday, etc), or having your spouse leave you due to you playing too much ASA. Please remember to take frequent breaks and don't forget to feed and spend time with your pets and loved ones.


## License
The project is licensed under the GNU GPL v2 license.


All rights reserved.
