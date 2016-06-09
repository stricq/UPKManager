# UPKManager
Blade and Soul UPK File Manager

Unreal Engine v3 (UE3) View and Extract Textures and Sounds.  Rebuild UPK files with new textures for use in game.

Extract and re-integrate Texture2D and other object types.

Visual Studio 2015 .Net 4.6.1

# External NuGet Source
In Visual Studio:

Tools -> Options -> NuGet Package Manager -> Package Sources

Name: STR Programming Services

Source: http://nuget.stricq.com/nuget

# Download
[UPK Manager v1.3.2](https://bns.stricq.com/Files/UpkManager.Installer_1.3.2.zip)

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

All textures must be saved in the DDS file format.  (JPG and PNG should work, too.  Just as long as the original image dimensions are the same.)

Once you have finished modifying the textures in the Export Path switch to the Rebuild tab.  Any textures listed in the tree that are checked will be rebuilt into a new modified UPK file after selecting Rebuild Selected Exports under the Rebuild menu.  The modified UPK file will automatically be placed in a mod folder with the game files ready to be used by the game.

Any textures not checked will be pulled from the source game file and copied unmodified into the repackaged UPK file.

Any modified UPK files will appear in the Mod Files tab.  These files can be selected and their contents viewed just the same as the game files in the Game Files tab.

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
