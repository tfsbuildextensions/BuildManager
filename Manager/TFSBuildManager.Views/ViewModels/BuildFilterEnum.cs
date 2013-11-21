//-----------------------------------------------------------------------
// <copyright file="BuildFilterEnum.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    public enum BuildFilter
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Latest
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Queued
        /// </summary>
        Queued = 2
    }
}