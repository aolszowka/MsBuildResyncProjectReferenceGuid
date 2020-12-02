// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Ace Olszowka">
//  Copyright (c) Ace Olszowka 2018-2020. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MsBuildResyncProjectReferenceGuid
{
    using System;
    using System.IO;
    using System.Linq;

    using MsBuildResyncProjectReferenceGuid.Properties;

    using NDesk.Options;

    class Program
    {
        static void Main(string[] args)
        {
            string targetDirectory = string.Empty;
            bool validateOnly = false;
            bool showHelp = false;

            OptionSet p = new OptionSet()
            {
                { "<>", Strings.TargetDirectoryDescription, v => targetDirectory = v },
                { "validate", Strings.ValidateDescription, v => validateOnly = v != null },
                { "?|h|help", Strings.HelpDescription, v => showHelp = v != null },
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException)
            {
                Console.WriteLine(Strings.ShortUsageMessage);
                Console.WriteLine($"Try `--help` for more information.");
                Environment.ExitCode = 160;
                return;
            }

            if (showHelp || string.IsNullOrEmpty(targetDirectory))
            {
                ShowUsage(p);
            }
            else if (!Directory.Exists(targetDirectory))
            {
                Console.WriteLine(Strings.InvalidTargetArgument, targetDirectory);
                Environment.ExitCode = 9009;
            }
            else
            {
                Environment.ExitCode = PrintToConsole(targetDirectory, validateOnly == false);
            }
        }

        private static int ShowUsage(OptionSet p)
        {
            Console.WriteLine(Strings.ShortUsageMessage);
            Console.WriteLine();
            Console.WriteLine(Strings.LongDescription);
            Console.WriteLine();
            Console.WriteLine($"               <>            {Strings.TargetDirectoryDescription}");
            p.WriteOptionDescriptions(Console.Out);
            return 160;
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
