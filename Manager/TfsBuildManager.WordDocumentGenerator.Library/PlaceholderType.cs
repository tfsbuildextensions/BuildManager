//-----------------------------------------------------------------------
// <copyright file="PlaceholderType.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    /// <summary>
    /// Defines the type of the PlaceHolder
    /// </summary>
    public enum PlaceholderType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Recursive
        /// </summary>
        Recursive = 1,

        /// <summary>
        /// NonRecursive
        /// </summary>
        NonRecursive = 2,

        /// <summary>
        /// Ignore
        /// </summary>
        Ignore = 3,

        /// <summary>
        /// Container
        /// </summary>
        Container = 4,

        /// <summary>
        /// Picture Container
        /// </summary>
        PictureContainer = 5
    }
}
