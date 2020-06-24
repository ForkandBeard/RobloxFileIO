using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace RobloxFileIO
{
    class RobloxStudioFileUpdator
    {
        public static void UpdateAllFile(string robloxPath, string sourceFilesPath)
        {
            XmlDocument originalRobloxFile;

            // Create xml document.
            originalRobloxFile = new XmlDocument();
            originalRobloxFile.LoadXml(File.ReadAllText(robloxPath));

            var directoryInfo = new DirectoryInfo(sourceFilesPath);
            foreach (var file in Directory.GetFiles(directoryInfo.ToString(), "*.lua", SearchOption.AllDirectories))
            {
                try
                {
                    Console.WriteLine("Updating file: " + file);
                    UpdateFile(originalRobloxFile, file);                    
                }
                catch(Exception ex)
                {
                    throw new Exception("An error occurred Updating from file: " + file, ex);
                }
            }

            Console.WriteLine("Saving RobloxStudio file: " + robloxPath);
            originalRobloxFile.Save(robloxPath.Replace(".rbxlx", "2.rbxlx"));
        }

        private static void UpdateFile(XmlDocument originalRobloxFile, string luaFilePath)
        {
            string newScript;
            string[] scriptAsLines;
            string scriptId;
            string currentScript;

            scriptAsLines = File.ReadAllLines(luaFilePath);
            scriptId = scriptAsLines[0].Replace("--", String.Empty);

            newScript = String.Join(Environment.NewLine, scriptAsLines.Skip(1));

            scriptId = "{" + scriptId + "}";

            var xmlNode =  originalRobloxFile.SelectSingleNode($"//Item/Properties/string[text() = '{scriptId}']");
            if(xmlNode != null)
            {   
                currentScript = xmlNode.ParentNode.SelectSingleNode("ProtectedString").InnerText;
                if (AreScriptsEqual(currentScript, newScript))
                {
                    Console.WriteLine("Not updating, no change detected");
                }
                else
                {
                    Console.WriteLine("Updated RobloxStudio file");
                    xmlNode.ParentNode.SelectSingleNode("ProtectedString").InnerText = newScript;
                }
            }
        }

        private static bool AreScriptsEqual(string script1, string script2)
        {
            // TODO: Fix this method... it always returns false because it doesn't correctly replace the carriage returns.
            return script1.Trim()
                .Replace(@"\r\n", Environment.NewLine)
                .Replace(@"\n", Environment.NewLine)
                .Trim(Environment.NewLine.ToCharArray())
                == script2.Trim()
                .Replace(@"\r\n", Environment.NewLine)
                .Replace(@"\n", Environment.NewLine)
                .Trim(Environment.NewLine.ToCharArray());
        }
    }
}
