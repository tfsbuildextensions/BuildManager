﻿//-----------------------------------------------------------------------
// <copyright file="ExportedIWorkspaceMapping.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

using Microsoft.TeamFoundation.Build.Client;

namespace TfsBuildManager.Repository
{
    public class ExportedIWorkspaceMapping : IWorkspaceMapping
    {
        public WorkspaceMappingType MappingType { get; set; }

        public string LocalItem { get; set; }

        public string ServerItem { get; set; }

        public WorkspaceMappingDepth Depth { get; set; }
    }
}
