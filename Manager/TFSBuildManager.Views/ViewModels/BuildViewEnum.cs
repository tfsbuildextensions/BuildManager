//-----------------------------------------------------------------------
// <copyright file="BuildViewEnum.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    public enum BuildView
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// BuildDefinitions
        /// </summary>
        BuildDefinitions = 1,

        /// <summary>
        /// Builds
        /// </summary>
        Builds = 2,

        /// <summary>
        /// Build Process Template
        /// </summary>
        BuildProcessTemplates = 3,

        /// <summary>
        /// Build Resources
        /// </summary>
        BuildResources = 4,
    }

    public enum DateFilter
    {
        /// <summary>
        /// None
        /// </summary>
        Today = 0,

        /// <summary>
        /// BuildDefinitions
        /// </summary>
        OneDay = 1,

        /// <summary>
        /// Builds
        /// </summary>
        TwoDays = 2,

        /// <summary>
        /// Builds
        /// </summary>
        OneWeek = 3,

        /// <summary>
        /// Builds
        /// </summary>
        TwoWeeks = 4,

        /// <summary>
        /// Builds
        /// </summary>
        FourWeeks = 5,

        /// <summary>
        /// Builds
        /// </summary>
        Anytime = 6
    }
}