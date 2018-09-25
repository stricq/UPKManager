# UPKManager
Blade and Soul UPK File Manager

Unreal Engine v3 (UE3) View and Extract Textures and Sounds.  Rebuild UPK files with new textures for use in game.

Extract and re-integrate Texture2D and other object types.

Visual Studio 2017 .Net 4.6.1

### Download
[UPK Manager v1.6.0](https://forums.stricq.com/resources/upk-manager.1/)

### Support
[UPK Manager Forums](https://forums.stricq.com/)

# External NuGet Source
In Visual Studio:

Tools -> Options -> NuGet Package Manager -> Package Sources

Name: STR Programming Services

Source: http://nuget.stricq.com/nuget

# Instructions for Viewing and Saving Textures
### Settings
The first time the program is run, you need to set the paths to the game files and the directory where it will export textures.

On the Main Menu select Settings and then the Settings... item.

For the Path to the Game, you want to choose the directory that contains the 'bns' and 'Local' directories.  This will usually be the 'contents' directory.  You want to click into the contents directory so that you can see the 'bns' and 'Local' directories in the dialog window.

The Export Path can be any location where you want the exported textures to be saved.

### Viewing Textures
In the File Listing tab, you will see all the UPK files.  Only those containing textures will be shown by default.

Click on any file in the list and the program will load and parse all the objects contained in the file.

Now switch to the File Tables tab on the left.

The Export Table tab shows all the objects contained in the file.  You can select any object where the Type Name column is Texture2D.

After you select an object in the Export Table list, the Texture View tab on the right will display the texture.

While viewing a texture, you can save that single texture from the menu by choosing File -> Save Object As...

### Exporting All Textures in a File
To export all objects in a file (to your chosen Export Path) on the File Listing tab, click on the checkbox for any listed file.  You can check as many files as desired.

To export the objects from your checked files, on the menu choose File -> Export Selected Files.

All the texture objects in all the files that are checked will be saved as individual DDS files to the Export Path you choose in the settings.

### Repackaging Modded Textures
After exporting texture files to the chose Export Path, any of the exported textures can be modified or replaced.  Always ensure that you keep the same filename.  The program uses the filename to determine which textures to replace while rebuilding a package file.

All textures must be saved in the DDS file format.

Once you have finished modifying the textures in the Export Path switch to the Rebuild tab.  Any textures listed in the tree that are checked will be rebuilt into a new modified UPK file after selecting Rebuild Selected Exports under the Rebuild menu.  The modified UPK file will automatically be placed in a mod folder with the game files ready to be used by the game.

Any textures not checked will be pulled from the source game file and copied unmodified into the repackaged UPK file.

Any modified UPK files will appear in the Mod Files tab.  These files can be selected and their contents viewed just the same as the game files in the Game Files tab.

# Changes for Release 1.6.1

* Removed all code still referencing the old database server.

# Changes for Release 1.6.0

* A number of bug fixes, but nothing to do with the textures.
* The previous database became corrupted and was not recoverable.  Moved to the latest version of the database software and setup a new server.  The stored data object has been optimized.
* Many bugs relating to scanning new files after a game update have been found and squashed.
* Probably a bunch of other things I have forgotten over the last year.

# Changes for Release 1.5.2

* Fixed another export bug.
* Hide the options to change DDS format when rebuilding a UPK as it doesn't work yet.

# Changes for Release 1.5.1

* Fixed a bug when rebuilding modded upk files.
* Selecting Offline Mode from the main menu will now abort the remote database load.

# Changes for Release 1.5

* Switch to a new database format so, unfortunately, all previous file notes are lost.
* Offline Mode: If there are any connection issues with the remote database, the application will switch to offline mode that will still allow exporting and importing.
* Rearranged the UI a bit to help with workflow and file commenting.

# Changes for Release 1.4.2

* Now uses a different version file if the normal version file cannot be found.
* If there is more than one version of game file, now uses the first file rather than crashing.
* The File Listing can now be filtered on filename, version, and notes.

# Changes for Release 1.4.1

* Translated C++ DXT compression/decompression libraries to C#.  This removes the dependency on the Visual C++ Redistributable package.
* Let me know if you notice any texture quality differences between 1.4 and 1.4.1.

# Changes for Release 1.4

* The DDS graphics library has been replaced with code pulled from Open Paint.NET.
* Exporting textures to a file and rebuilding textures back into a UPK file now support several quality settings controlled from the main menu.  Settings -> Texture Quality.
* The rebuild package process has several changes:
  * Now uses the same DDS format that was in the original UPK file.  This reverses the change made in v1.3.2.
  * Now uses the same minimum mip map size as the unmodified UPK files do which might help with texture streaming crashes.
  * Some specular textures have a missing first mip map and a listed size four times larger than the actual first texture size.  This pattern is now maintained which might help with texture streaming crashes.
* Now has a dependency on the Visual C++ Redistributable for Visual Studio 2015.  Sorry about that.  I will remove it for the next release.
  * https://www.microsoft.com/en-us/download/details.aspx?id=48145

# Changes for Release 1.3.2

* Changes the rebuild package process to use the file format of the saved texture rather than converting it internally to the original format in the game file.
* Now creates the mod directory if it doesn't already exist.
* Other misc issues that were in 1.3.1 that was not officially versioned.

# Changes for Release 1.3

* Exported textures can be edited and re-imported back into the game.
* Added Rebuild tab.  This tab allows you to view exported textures (and sounds) and view them in the Texture Viewer.
* Added Modded Files tab.  This tab allows you to view modded files in the same way was as viewing game files.
* Can now view all the MIPs of loaded textures.
* TextureMovie objects can now be exported as bink files.

# Changes for Release 1.2

* Audio objects (SoundNodeWave) can now be exported (as .ogg) and will play when clicked on in the export object listing.
* Any fatal exceptions will be stored on the remote database to help improve the program.
* Background tasks are now handled in a much better manner.
* New object Viewer: Object Tree.  Export objects show in green and import objects show in blue.  Export objects can be clicked on for viewing as in the Export Table list.
