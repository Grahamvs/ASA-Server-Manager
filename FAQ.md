# FAQ

## How do I install the Ark Survival Ascended dedicated server?
When you launch the tool for the first time, an options window will appear. Choose "SteamCMD" as the server type and click "Install". A warning will appear, notifying you that proceeding will initiate the SteamCMD installation to download the server, which requires at least 11GB of free space (this requirement may change in the future). If you agree to proceed, you will be asked to select the installation location for SteamCMD. Choose your preferred directory and wait for the tool to automatically install SteamCMD and the ASA server. Once the installation is complete, the tool will automatically populate the necessary server paths. After verifying the settings, click "Save" to store the settings and close the window.

## What if I already have an existing SteamCMD version of the Ark Survival Ascended server installed?
If you already have the server installed using an existing SteamCMD installation, ensure you have the server type set to "SteamCMD", then use the browse button (the "..." button next to the "Install" button) and navigate to the server's directory. Once you've selected the SteamCMD executable, click on the "open" button.

## What if I don't want to use SteamCMD?
If you prefer not to use SteamCMD, you can select the "Standalone" option in the options window, then use the "ASA Server"'s browse button (the "..." button) to navigate to the server's directory. Once you've selected the server's executable, click on the "open" button.

This option is useful if you have a server that is not controlled by SteamCMD, however, updates through ASA Server Manager are not available in this mode.

## How do I update the Ark Survival Ascended server?
To update the server, open the "Server" menu in the main window and select the "Update" option. The tool will automatically instruct SteamCMD to update the server if necessary.

Notes:
- You can also enable automatic updates on first run in the options window.
- Updates are only available for servers installed using SteamCMD.
    
## What is a profile?
A profile is a set of server settings that can be saved and loaded at any time. This is useful for running multiple servers off a single installation.
See "[Can I run multiple servers at once?](#can-i-run-multiple-servers-at-once)" for more information.

## Can I run multiple servers at once?
Yes, you can run multiple servers at once. To do this, create a profile for each server you want to run, then load the profile and start the server. Once the server is running, you can load a different profile and start another server. For best results, ensure you have 12 - 16GB of RAM available per server, and that each profile has a unique Port and QueryPort assigned to it.

## Can I run multiple cluster servers at once?
Yes, but you will need to set the ClusterID for each profile. All servers in the cluster should have the same ClusterID. See "[Can I run multiple servers at once?](#can-i-run-multiple-servers-at-once)" for more information, as well as the Cluster section in the [Ark Server Configuration Wiki](https://ark.wiki.gg/wiki/Server_configuration#More_About_Cluster_Files_and_Running_Multiple_Servers) for more information on cluster servers.

## Are passwords stored in plain text?
No, passwords are stored in a custom encrypted format, and are never shown unencrypted.

This might change in the future to a more secure method, however, the current method is secure enough for now as this is just for a game server.

## What are mods, and how do I add them?
Mods are user-created content that can enhance the gaming experience. The ASA Server Manager provides a user-created list of available mods that all profiles share. Each profile can be configured to enable any or all of the mods for the server.

To add a new mod, open the "Options" menu and click on the "Available Mods" option. In the window that appears, you can choose to add or remove mods from the list. To add a mod, double-click the last row in the grid and set the ID and name. The ID is usually a 6 to 7 digit numerical value, which can be found on [CursedForge.com](https://www.curseforge.com/ark-survival-ascended). The description is optional, but can be used to add keywords to help when filtering. Once you've updated the mods list, click the "Save" button to save the changes and close the window.

Note: Mods will be loaded in the order they appear in the available mods list. To change the load order, open the "Available Mods" window, select the desired mod, then use the up and down buttons to change its position in the list. Finally, click save.

## How do I use a mod from the list for my profile?
To use a mod from the Available Mods list, go to the Mods group, find the desired mod, and click either the "Enabled" or "Passive" toggle..

## What is the difference between "Enabled" and "Passive"?
- Enabled: The mod will be loaded by the server and will be active.
- Passive: The mod's assets will be loaded by the server but the rest of the mod will not be active. (Note: Currently, only "Love Ascended" and "Winter Wonderland" utilize this state.)

## Can I filter the mod list?
Yes, you can filter the mod list by typing in the filter box. The list will automatically update to show only the mods that match the filter. The filter is not case-sensitive, and checks the mod's name, ID and description.

## Can I filter the mod list to only show disabled, enabled, or passive mods?
Yes. You can use the words "enabled", "disabled", or "passive" in the filter box to show only mods that match the filter.

## When I set a mod to "Passive", why does it also show as "Enabled" in the list?
In order to use a mod in "Passive" mode, it must also be enabled. This is because the server needs to load the mod's assets in order to use it in "Passive" mode.

## How do I update the mods to the latest version?
To update the mods, simply run the server with the mods enabled. The server will automatically check for updates and download them if necessary.

## I renamed the executable, and now the application has lost my settings. How do I fix this?
This issue occurs because the tool searches for its config files based on the executable's name. To resolve this, either close the tool and rename the executable back to its original name, or rename the ".config" and ".mods" file to match.

## There's a new / custom map I want to use, but it's not showing in the list, how do I add it?
On the main window, click the "Options" menu and select the "Custom Maps" option. In the window that appears, you can add the map name to the list. An ID is required. Once you've finished, click the "Save" button to save the changes and close the window.

## Why can't I update the server?
Firstly, ensure there is no instances of the server running. Secondly, check the server path options are correct, and the server type is set to SteamCMD.

## Why can't I start the server?
Firstly, ensure the server is not currently updating. Secondly, check the server path options are correct.

## Why can't I see the server in the server list?
Ensure the server is running and the ports are open on your network. You may need to visit your router's manufacturer's website for instructions on how to open ports.

For any other assistance, please refer to the [Ark Server Configuration Wiki](https://ark.wiki.gg/wiki/Server_configuration).

## How do I update ASA Server Manager?
You will need to download the latest version from [GitHub](https://github.com/Grahamvs/ASA-Server-Manager/releases). Once downloaded, unzip the executable and overwrite the previous version.

## How do I know if there is a new version available?
The tool will automatically check for updates based on your update settings (the default is only on start up). If a new version is available, a notification will appear in the bottom right corner of the window. You can also check for updates manually by clicking the "Check for Updates" option in the "Help" menu.

## I don't like the new version, how do I go back to the previous version?
You can download the a previous version, then you can either disable update notifications entirely, or simply set the latest version to be ignored.

## How do I disable update notifications?
You can disable update notifications by clicking the "Options" menu, then setting the "Check for App Updates" option to none. Finally, click the "Save" button to save the changes and close the window.

## How do I ignore the latest version?
When the update notification appears, click the Close button on the notification. A dialogue box will appear asking if you want to ignore the specific version. Click "Yes" to ignore the version.

Note: Ignoring the latest version will only ignore the current version. If a new version is released, you will be notified of that version.

## I previously ignored the latest version, how do I update to it now?
You can force a check for the latest version by clicking the "Check for Updates" option in the "Help" menu. The tool will check for updates and notify you if a new version is available.

## Can I run ASA Server Manager on a Linux or Mac system?
Maybe. The tool is developed for Windows, however, there are ways to run Windows applications on Linux and Mac systems. One such way is to use [Wine](https://www.winehq.org/). Please note that the developer(s) have not tested the tool on Linux or Mac systems, so it may not work as expected.

## Which mods do you recommend?
The mods you choose to use are entirely up to you, however, I personally recommended the following:

- Server tools (Must haves):
  - Admin Panel (ID: 929868) see [link](https://www.curseforge.com/ark-survival-ascended/mods/admin-panel).
  - Lily's Tweaker (ID: 939688) see [link](https://www.curseforge.com/ark-survival-ascended/mods/lilys-tweaker).

- Simple Quality of Life changes:
    - Custom Dino Levels (ID: 928708) see [link](https://www.curseforge.com/ark-survival-ascended/mods/custom-dino-levels).
    - AutoDoors (ID: 931047) see [link](https://www.curseforge.com/ark-survival-ascended/mods/autodoors).
    - SilentStructures (ID: 930381) see [link](https://www.curseforge.com/ark-survival-ascended/mods/silent-structures).
    - TG Stacking Mod 1000-90 (ID: 929713) see [link](https://www.curseforge.com/ark-survival-ascended/mods/tg-stacking-mod-1000-90).
    - Utilities Plus (ID: 928621) see [link](https://www.curseforge.com/ark-survival-ascended/mods/utilities-plus).
    - Explorer Notes Plus (ID: 946237) see [link](https://www.curseforge.com/ark-survival-ascended/mods/expnplus)

- Big Quality of Life changes:
    - Cybers Structures QoL+ (ID: 940975) see [link](https://www.curseforge.com/ark-survival-ascended/mods/cybers-structures).
    - Der Dino Finder (ID: 935408) see [link](https://www.curseforge.com/ark-survival-ascended/mods/der-dino-finder).      
    - Awesome Spyglass! (ID: 947033) see [link](https://www.curseforge.com/ark-survival-ascended/mods/awesomespyglass).
    - Awesome Teleporters! (ID: 950914) see [link](https://www.curseforge.com/ark-survival-ascended/mods/awesometeleporters).
 
- Enhanced Cryopod mods (only use one):
    - Super Cryo Storage (ID: 933099) see [link](https://www.curseforge.com/ark-survival-ascended/mods/super-cryo-storage).
    - Dino Depot (ID: 942024) see [link](https://www.curseforge.com/ark-survival-ascended/mods/dino-depot).
  
 Super Cryo Storage and Dino Depot are very similar. The key differnces are as follows:
- Dino Depot: Has more configurable settings, but most settings a set in the .ini file.
- Super Cryo Storage: Has a few more features, and all settings are set in the mod's UI.
 
Note: It is strongly recommended to only use one cryopod mod at a time! Also, when uninstalling a cryopod mod, ensure all dinos are transfered into vanilla cryopods prior to uninstalling the mod, as the dinos will be lost if they are not transfered.
