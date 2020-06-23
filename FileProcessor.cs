using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace RobloxFileIO
{
    class FileProcessor
    {
        private const string tempFolderRootName = "RobloxFileIO";
        private const string workspaceFolderName = "Workspace";

        public static void UnzipFiles(string robloxPath, string outputPath)
        {
            XmlDocument originalRobloxFile;
            string tempWorkingFolder = null;
            string originalRobloxFileFileName = null;
            bool tempFolderWasCreated = false; 

            // Create temp folder to unzip files into.
            originalRobloxFileFileName = Path.GetFileNameWithoutExtension(robloxPath);
            tempWorkingFolder = Path.GetTempPath();
            tempWorkingFolder = Path.Combine(tempWorkingFolder, tempFolderRootName);
            tempWorkingFolder = Path.Combine(tempWorkingFolder, Guid.NewGuid().ToString());
            tempWorkingFolder = Path.Combine(tempWorkingFolder, originalRobloxFileFileName);            

            try
            {
                tempFolderWasCreated = true;
                Directory.CreateDirectory(tempWorkingFolder);
                Console.WriteLine("Created folder: " + tempWorkingFolder);

                // Create xml document.
                originalRobloxFile = new XmlDocument();
                originalRobloxFile.LoadXml(File.ReadAllText(robloxPath));

                // Ready now to do the unzip.
                Console.WriteLine("Unzip start into: " + tempWorkingFolder);
                UnzipFiles(originalRobloxFile, tempWorkingFolder);
                Console.WriteLine("Unzip done");

                Console.WriteLine("Copy into destination started: " + outputPath);
                CopyFiles(tempWorkingFolder, outputPath);
                Console.WriteLine("Copy complete");
            }
            finally
            {
                if(
                    (tempWorkingFolder != null)
                    && (tempFolderWasCreated)
                    )
                {   // Tidy-up/delete the temp folder.
                    var directoryInfo = new DirectoryInfo(tempWorkingFolder);
                    foreach (var file in Directory.GetFiles(directoryInfo.ToString()))
                    {
                        try
                        {   
                            File.Delete(file);
                        }catch(Exception ex)
                        {   // ignore
                        }
                    }

                    try
                    {
                        Directory.Delete(tempWorkingFolder);
                    }catch (Exception ex)
                    {   // ignore
                    }
                }
            }
        }

        private static void UnzipFiles(XmlDocument originalRobloxFile, string workingFolderRoot)
        {
            List<String> systemFolders = new List<string>() {
                workspaceFolderName
                , "Players"
                , "Lighting"
                , "ReplicatedFirst"
                , "ReplicatedStorage"
                , "ServerScriptService"
                , "ServerStorage"
                , "StarterGui"
                , "StarterPack"
                , "StarterPlayer"
                , "SoundServer"
                , "Chat"
                , "LocalizationService"
                , "TestService"
            };
            string workingFolderSystemFolder;
            XmlNode parent;
            string subFolderPath;
            string scriptFolder;
            string script;
            string scriptName;
            string scriptId;
            bool isWorkpace;
            string fileExtenstionCode;

            // Find each system folder in roblox file.
            foreach (string systemFolder in systemFolders)
            {   // Create folder in temp folder.
                workingFolderSystemFolder = Path.Combine(workingFolderRoot, systemFolder);
                Directory.CreateDirectory(workingFolderSystemFolder);
                Console.WriteLine("Created folder: " + workingFolderSystemFolder);
                isWorkpace = systemFolder.Equals(workspaceFolderName);

                // Now find node e.g.: <Item class="ReplicatedFirst" .
                var currentSystemFolderNode = originalRobloxFile.SelectSingleNode($"roblox/Item[@class='{systemFolder}']");
                Console.WriteLine("Processing folder: " + systemFolder);

                if (currentSystemFolderNode != null)
                {
                    // Now find all the scripts in this folder.
                    foreach (XmlNode scriptNode in currentSystemFolderNode.SelectNodes(".//Item[@class='Script' or @class='LocalScript' or @class='ModuleScript']"))
                    {   // Now 'walk' up the script's parent's creating any folders until we get to the system folder root.
                        subFolderPath = null;
                        parent = scriptNode.ParentNode;

                        while (
                                (parent != currentSystemFolderNode)
                                && (parent != null)
                                )
                        {
                            if (
                                (parent.Attributes != null)
                                && (parent.Attributes["class"] != null)
                                && (parent.Attributes["class"].Value == "Folder")
                               )
                            {
                                if (subFolderPath == null)
                                {
                                    subFolderPath = parent.SelectSingleNode("./Properties/string").InnerText;
                                }
                                else
                                {
                                    subFolderPath = Path.Combine(parent.SelectSingleNode("./Properties/string").InnerText, subFolderPath);
                                }
                            }
                            parent = parent.ParentNode;
                        }

                        // Now we found all the folders, create the script in the correct folder.
                        if (subFolderPath != null)
                        {
                            scriptFolder = Path.Combine(workingFolderSystemFolder, subFolderPath);
                            Directory.CreateDirectory(scriptFolder);
                            Console.WriteLine("Created folder: " + scriptFolder);
                        }
                        else
                        {
                            scriptFolder = workingFolderSystemFolder;
                        }

                        scriptName = scriptNode.SelectSingleNode("Properties/string[@name='Name']").InnerText;
                        scriptId = scriptNode.SelectSingleNode("Properties/string[@name='ScriptGuid']").InnerText;                                                
                        scriptId = scriptId.Replace("{", "");
                        scriptId = scriptId.Replace("}", "");

                        fileExtenstionCode = GetFileExtenstionCodeFromClassValue(scriptNode.Attributes["class"].Value);
                        script = scriptNode.SelectSingleNode("Properties/ProtectedString").InnerText;
                        script = $"--{scriptId}" + Environment.NewLine + script;

                        Console.WriteLine("Creating script: " + scriptName);
                        if (isWorkpace)
                        {
                            File.WriteAllText(Path.Combine(scriptFolder, $"{scriptName}.{scriptId}.{fileExtenstionCode}lua"), script);
                        }
                        else
                        {                            
                            File.WriteAllText(Path.Combine(scriptFolder, $"{scriptName}.{fileExtenstionCode}lua"), script);
                        }
                    }
                }
            }
        }

        private static string GetFileExtenstionCodeFromClassValue(string classValue)
        {
            switch(classValue)
            {
                case "Script":
                    return String.Empty;

                case "LocalScript":
                    return "loc.";

                case "ModuleScript":
                    return "mod.";

                default:
                    throw new NotImplementedException(classValue);
            }
        }

        private static void CopyFiles(string workingFolderRoot, string destinationRoot)
        {
            string destinationPath;
            string destinationDirectory;

            // Create destination if it doesn't exist.
            Directory.CreateDirectory(destinationRoot);

            // First delete all existing files.
            var destinationDirectoryDeleting = new DirectoryInfo(destinationRoot);
            foreach (var file in Directory.GetFiles(destinationDirectoryDeleting.ToString()))
            {
                try
                {
                    Console.WriteLine("Deleting existing file: " + file);
                    File.Delete(file);
                }
                catch (Exception ex)
                {   // ignore
                }
            }

            // Now copy all the files from temp to destination.
            var workingFolder = new DirectoryInfo(workingFolderRoot);
            foreach (var file in Directory.GetFiles(workingFolder.ToString(), "*.lua", SearchOption.AllDirectories))
            {
                try
                {
                    destinationPath = file.Replace(workingFolderRoot, destinationRoot);
                    destinationDirectory = Path.GetDirectoryName(destinationPath);
                    Directory.CreateDirectory(destinationDirectory);

                    Console.WriteLine("Copying file: " + destinationPath);                    
                    File.Copy(file, destinationPath);
                }catch(Exception ex)
                {
                    throw new Exception("An error occurred copying " + file, ex);
                }
            }
        }
    }
}
