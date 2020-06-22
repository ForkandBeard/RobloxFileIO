# RobloxFileIO
A utility for parsing the xml file created by Roblox Studio so lua scripts can be added to source control individually.

# How to Use
RobloxFileIO.exe @ROBLOX_STUDIO_FILE "C:\..\YourGame.rbxlx" @MANY_FILES_DIRECTORY "C:\..\Target" @DIRECTION ROBLOX_STUDIO_TO_MANY_FILES

# What Happens
This app will then parse the .rbxlx file (must be saved in Roblox Studio as XML) and then for every script file it finds it will create a seperate .lua file in the target folder.

# Why?
This allows a user to then upload the individual .lua files to Git or the source control of their choice and have version control history/backups at script level.

# Tip
The command line args can be added to a .bat file in the folder where the .lua scripts are saved and then the .bat can be run whenever the .lua files are needed to be updated.
