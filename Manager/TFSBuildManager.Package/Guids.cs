//-----------------------------------------------------------------------
// <copyright file="Guids.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager
{
    using System;

    public static class GuidList
    {
        public const string GuidTfsBuildManagerPackageString = "6fadf005-94b3-46d1-b509-fd8ba6d1ec00";

        public const string GuidTfsBuildManagerPackageCmdSetString = "b3516385-098d-4066-89e5-439bea58700d";

        public const string GuidToolWindowPersistenceString = "9f6c976b-ea0b-4d40-b942-dd4a2785aa50";

        public static readonly Guid GuidTfsBuildManagerPackageCmdSet = new Guid(GuidTfsBuildManagerPackageCmdSetString);
    }
}