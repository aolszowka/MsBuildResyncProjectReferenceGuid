// -----------------------------------------------------------------------
// <copyright file="ResyncProjectReferenceGuid.cs" company="Ace Olszowka">
//  Copyright (c) Ace Olszowka 2018-2020. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MsBuildResyncProjectReferenceGuid
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    static class ResyncProjectReferenceGuid
    {
        public static IEnumerable<string> Execute(string targetDirectory, bool saveModifications)
        {
            ConcurrentBag<string> modifiedProjects = new ConcurrentBag<string>();

            IEnumerable<string> projectsToResync = GetProjectsInDirectory(targetDirectory);

            Parallel.ForEach(projectsToResync, projectToResync =>
            {
                if (ResyncProjectReference(projectToResync, saveModifications))
                {
                    modifiedProjects.Add(projectToResync);
                }
            }
            );

            return modifiedProjects;
        }

        static bool ResyncProjectReference(string pathToProject, bool saveModifications)
        {
            bool changesMade = false;

            XDocument projXml = XDocument.Load(pathToProject);

            IEnumerable<XElement> projectReferences = MSBuildUtilities.GetProjectReferenceNodes(projXml);

            string projectDirectory = PathUtilities.AddTrailingSlash(Path.GetDirectoryName(pathToProject));

            foreach (XElement projectReference in projectReferences)
            {
                string existingGuid = MSBuildUtilities.GetProjectReferenceGUID(projectReference, pathToProject);
                string prRelativePath = MSBuildUtilities.GetProjectReferenceIncludeValue(projectReference, pathToProject);
                string prActualPath = PathUtilities.ResolveRelativePath(projectDirectory, prRelativePath);

                // If the referenced project doesn't exist throw an exception
                if (!File.Exists(prActualPath))
                {
                    string exception = $"In Project `{pathToProject}` Reference `{existingGuid}` was expected to exist at `{prRelativePath}` (`{prActualPath}`) but did not exist.";
                    throw new InvalidOperationException(exception);
                }

                // Else get the Guid from that project
                string referencedProjectGuid = MSBuildUtilities.GetMSBuildProjectGuid(prActualPath);

                if (!existingGuid.Equals(referencedProjectGuid, StringComparison.InvariantCultureIgnoreCase))
                {
                    changesMade = true;
                    MSBuildUtilities.SetProjectReferenceGUID(projectReference, referencedProjectGuid);
                }
            }

            if (changesMade && saveModifications)
            {
                try
                {
                    projXml.Save(pathToProject);
                }
                catch
                {
                    // TODO: Race Condition (Consider Project Being Read for its GUID while trying to fix it)
                    // There is a pretty bad race condition bug here
                    // just sleep to forget it for now, no time to fix
                    // this bug.
                    System.Threading.Thread.Sleep(30000);
                    projXml.Save(pathToProject);
                }
            }

            return changesMade;
        }

        /// <summary>
        /// Gets all Project Files that are understood by this
        /// tool from the given directory and all subdirectories.
        /// </summary>
        /// <param name="targetDirectory">The directory to scan for projects.</param>
        /// <returns>All projects that this tool supports.</returns>
        static IEnumerable<string> GetProjectsInDirectory(string targetDirectory)
        {
            HashSet<string> supportedFileExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
            {
                ".csproj",
                ".fsproj",
                ".sqlproj",
                ".synproj",
                ".vbproj",
            };

            return
                Directory
                .EnumerateFiles(targetDirectory, "*proj", SearchOption.AllDirectories)
                .Where(currentFile => supportedFileExtensions.Contains(Path.GetExtension(currentFile)));
        }
    }
}
