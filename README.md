# RobloxFileIO
A utility for parsing the xml file created by Roblox Studio so lua scripts can be added to source control individually.

# How to use?
To turn a single Roblox Studio file into many Lua scripts:
```
RobloxFileIO.exe @ROBLOX_STUDIO_FILE "C:\..\YourGame.rbxlx" @MANY_FILES_DIRECTORY "C:\..\Target" @DIRECTION ROBLOX_STUDIO_TO_MANY_FILES
```
# What happens?
This app will then parse the .rbxlx file (must be saved in Roblox Studio as XML) and then for every script file it finds it will create a seperate .lua file in the target folder.

# Why?
This allows a user to upload the individual .lua files to Git or the source control of their choice and have version control history/backups at script level.

# Can I update the Roblox Studio file with any changes?
To update the Roblox Studio file with any changes made in those many Lua scripts use:
```
RobloxFileIO.exe @ROBLOX_STUDIO_FILE "C:\..\YourGame.rbxlx" @MANY_FILES_DIRECTORY "C:\..\Target" @DIRECTION MANY_FILES_TO_ROBLOX_STUDIO
```
# What happens then?
This app will then iterate through each .lua script and (extract the script guid from the first line of the file put there by the previous step) and then simply update the Roblox Studio file with the new script.

# Tip
The command line args can be added to a .bat file in the folder where the .lua scripts are saved and then the .bat can be run whenever the .lua files are needed to be updated.
