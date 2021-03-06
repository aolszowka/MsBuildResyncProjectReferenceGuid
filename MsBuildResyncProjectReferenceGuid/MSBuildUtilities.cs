﻿// -----------------------------------------------------------------------
// <copyright file="MSBuildUtilities.cs" company="Ace Olszowka">
//  Copyright (c) Ace Olszowka 2018-2020. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MsBuildResyncProjectReferenceGuid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class MSBuildUtilities
    {
        internal static XNamespace msbuildNS = @"http://schemas.microsoft.com/developer/msbuild/2003";

        /// <summary>
        /// Extracts the Project GUID from the specified proj File.
        /// </summary>
        /// <param name="pathToProjFile">The proj File to extract the Project GUID from.</param>
        /// <returns>The specified proj File's Project GUID.</returns>
        public static string GetMSBuildProjectGuid(string pathToProjFile)
        {
            XDocument projFile = XDocument.Load(pathToProjFile);
            XElement projectGuid = projFile.Descendants(msbuildNS + "ProjectGuid").FirstOrDefault();

            if (projectGuid == null)
            {
                string exception = $"Project {pathToProjFile} did not contain a ProjectGuid.";
                throw new InvalidOperationException(pathToProjFile);
            }

            return projectGuid.Value;
        }

        public static IEnumerable<XElement> GetProjectReferenceNodes(XDocument projXml)
        {
            return projXml.Descendants(msbuildNS + "ProjectReference");
        }

        public static string GetOrCreateProjectReferenceGUID(XElement projectReference, string projectPath)
        {
            // Get the existing Project Reference GUID
            XElement projectReferenceGuidElement = projectReference.Descendants(msbuildNS + "Project").FirstOrDefault();
            if (projectReferenceGuidElement == null)
            {
                projectReferenceGuidElement = new XElement(msbuildNS + "Project", string.Empty);
                projectReference.Add(projectReferenceGuidElement);
            }

            // This is the referenced project
            string projectReferenceGuid = projectReferenceGuidElement.Value;

            return projectReferenceGuid;
        }

        public static void SetProjectReferenceGUID(XElement projectReference, string projectGuid)
        {
            string cleanProjectGuid = Guid.Parse(projectGuid).ToString("B").ToUpperInvariant();
            projectReference.Descendants(msbuildNS + "Project").First().SetValue(cleanProjectGuid);
        }

        public static string GetProjectReferenceIncludeValue(XElement projectReference, string projectPath)
        {
            // Get the existing Project Reference Include Value
            XAttribute projectReferenceIncludeAttribute = projectReference.Attribute("Include");

            if (projectReferenceIncludeAttribute == null)
            {
                string exception = $"A ProjectReference in {projectPath} does not contain an Include Attribute on it; this is invalid.";
                throw new InvalidOperationException(exception);
            }

            // This is the referenced project
            string projectReferenceInclude = projectReferenceIncludeAttribute.Value;

            return projectReferenceInclude;
        }

    }
}
