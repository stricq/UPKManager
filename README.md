# UPKManager
Blade and Soul UPK File Manager

Extract and re-integrate Texture2D and other object types.

Visual Studio 2015 .Net 4.6.1

# External NuGet Source
In Visual Studio:

Tools -> Options -> NuGet Package Manager -> Package Sources

Name: STR Programming Services

Source: http://nuget.stricq.com/nuget

# Download
[UPK Manager v1.0](https://bns.stricq.com/Files/UpkManager.Installer.msi)

# Instructions for Viewing and Saving Textures
### Settings
The first time the program is run, you need to set the paths to the game files and the directory where it will export textures.

On the Main Menu select Settings and then the Settings... item.

For the Path to the Game, you want to choose the directory that contains the 'bns' and 'Local' directories.  This will usually be the 'contents' directory.  You want to click into the contents directory so that you can see the 'bns' and 'Local' directories in the dialog window.

The Export Path can be any location where you want the exported textures to be saved.

### File Listing
In the File Listing tab, you will see all the UPK files.  Only those containing textures will be shown by default.

Click on any file in the list and the program will load and parse all the objects contained in the file.

Now switch to the File Tables tab.

The Export Table tab shows all the objects contained in the file.  You can select any object where the Type Name column is Texture2D.

After you select an object in the Export Table list, the Texture View tab on the left will display the texture.

While viewing a texture, you can save that single texture from the menu by choosing File -> Save Object As...

### Exporting All Textures in a File
To export all objects in a file (to your chosen Export Path) on the File Listing tab, click on the checkbox for any listed file.  You can check as many files as desired.

To export the objects from your checked files, on the menu choose File -> Export Selected Files.

All the texture objects in all the files that are checked will be saved as individual DDS files to the Export Path you choose in the settings.
