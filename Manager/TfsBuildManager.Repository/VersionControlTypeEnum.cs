//-----------------------------------------------------------------------
// <copyright file="VersionControlTypeEnum.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Repository
{
    public enum VersionControlType
    {
        /// <summary>
        /// Team foundation version control
        /// </summary>
        Tfvc = 0,

        /// <summary>
        /// Git version control
        /// </summary>
        Git = 1
    }
}