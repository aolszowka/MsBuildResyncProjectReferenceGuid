// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Ace Olszowka">
//  Copyright (c) Ace Olszowka 2018. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MsBuildResyncProjectReferenceGuid
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            int errorCode = 0;

            if (args.Any())
            {
                string command = args.First().ToLowerInvariant();

                if (command.Equals("-?") || command.Equals("/?") || command.Equals("-help") || command.Equals("/help"))
                {
                    errorCode = ShowUsage();
                }
                else if (command.Equals("validatedirectory"))
                {
                    if (args.Length < 2)
                    {
                        string error = string.Format("You must provide a directory as a second argument to use validatedirectory");
                        Console.WriteLine(error);
                        errorCode = 1;
                    }
                    else
                    {
                        // The second argument is a directory
                        string directoryArgument = args[1];

                        if (Directory.Exists(directoryArgument))
                        {
                            errorCode = PrintToConsole(args[1], false);
                        }
                        else
                        {
                            string error = string.Format("The provided directory `{0}` is invalid.", directoryArgument);
                            errorCode = 9009;
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(command))
                    {
                        string targetPath = command;
                        PrintToConsole(targetPath, true);
                        errorCode = 0;
                    }
                    else
                    {
                        string error = string.Format("The specified path `{0}` is not valid.", command);
                        Console.WriteLine(error);
                        errorCode = 1;
                    }
                }
            }
            else
            {
                // This was a bad command
                errorCode = ShowUsage();
            }

            Environment.Exit(errorCode);
        }

        private static int ShowUsage()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Scans given directory for MsBuild Projects; Resycing the ProjectReference Project Guid to the referenced Project.");
            message.AppendLine("Invalid Command/Arguments. Valid commands are:");
            message.AppendLine();
            message.AppendLine("[directory]                   - [MODIFIES] Spins through the specified directory\n" +
                               "                                and all subdirectories for Project files resetting\n" +
                               "                                the ProjectReference Project tag. Prints modified\n" +
                               "                                paths. ALWAYS Returns 0.");
            message.AppendLine("validatedirectory [directory] - [READS] Spins through the specified directory\n" +
                               "                                and all subdirectories for Project files prints\n" +
                               "                                all projects whose ProjectReference Project tags\n" +
                               "                                should be updated. Returns the number of invalid projects.");
            Console.WriteLine(message);
            return 21;
        }

        static int PrintToConsole(string targetDirectory, bool saveModifications)
        {
            string[] modifiedProjects = ResyncProjectReferenceGuid.Execute(targetDirectory, saveModifications).ToArray();

            foreach (string modifiedProject in modifiedProjects)
            {
                Console.WriteLine(modifiedProject);
            }

            return modifiedProjects.Length;
        }
    }
}
