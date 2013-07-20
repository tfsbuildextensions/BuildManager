//-----------------------------------------------------------------------
// <copyright file="BuildResourceFilter.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Repository
{
    public class BuildResourceFilter
    {
        public string TeamProject { get; set; }

        public string Controller { get; set; }
    }
}