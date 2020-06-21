using System;
using System.Collections.Generic;
using System.Text;

namespace RobloxFileIO
{
    class ArgumentProcessor
    {
        private const string argPathRobloxStudio = "@ROBLOX_STUDIO_FILE";
        private const string argPathManyFilesDirectory = "@MANY_FILES_DIRECTORY";
        private const string argPathDirection = "@DIRECTION";
        private const string argPathDirectionRobloxToManyFiles = "ROBLOX_STUDIO_TO_MANY_FILES";
        private const string argPathDirectionManyFilesToRoblox = "MANY_FILES_TO_ROBLOX_STUDIO";

        public static void ProcessArguments(string[] args)
        {
            string argName;
            string robloxFile = null;
            string manyFileDirectory = null;
            string directionString;
            Direction? direction = null;

            if(args.Length % 2 != 0)
            {   // Non even number of args supplied so couldn't have correct number of args.
                // TODO: Improve this error, it's awful, should provide more clarity. What specifically is missing?
                throw new ArgumentException("Wrong number of arguments supplied. Must be name/value pairs. Please see READ ME.");
            }

            if(args.Length != 6)
            {   // TODO: Again improve improve this error too.
                throw new ArgumentException($"Expected 6 arguments, but received {args.Length}.");
            }

            // Iterate over all args and extract into proper variables.
            for (int argIndex = 0; argIndex < args.Length; argIndex += 2)
            {
                argName = args[argIndex];
                switch (argName.ToUpper())
                {
                    case argPathRobloxStudio:
                        robloxFile = args[argIndex + 1];
                        break;

                    case argPathManyFilesDirectory:
                        manyFileDirectory = args[argIndex + 1];
                        break;

                    case argPathDirection:
                        directionString = args[argIndex + 1];
                        switch(directionString)
                        {
                            case argPathDirectionRobloxToManyFiles:
                                direction = Direction.ROBLOX_STUDIO_TO_MANY_FILES;
                                break;
                            case argPathDirectionManyFilesToRoblox:
                                direction = Direction.MANY_FILES_TO_ROBLOX_STUDIO;
                                break;
                            default:
                                throw new NotSupportedException(directionString);
                        }
                        break;
                }
            }

            // Just some paranoid checks should the above change.
            if(direction == null)
            {   // This should never be reached.
                throw new ArgumentNullException("@DIRECTION not supplied.");
            }

            if(robloxFile == null)
            {   // This should never be reached.
                throw new ArgumentNullException("@ROBLOX_STUDIO_FILE not supplied.");
            }

            if (manyFileDirectory == null)
            {   // This should never be reached.
                throw new ArgumentNullException("@MANY_FILES_DIRECTORY not supplied.");
            }

            // Now forward onto the main logic.
            switch (direction)
            {
                case Direction.ROBLOX_STUDIO_TO_MANY_FILES:
                    FileProcessor.UnzipFiles(robloxFile, manyFileDirectory);
                    break;
                case Direction.MANY_FILES_TO_ROBLOX_STUDIO:
                    throw new NotImplementedException(direction.ToString());
            }            
        }
    }
}
