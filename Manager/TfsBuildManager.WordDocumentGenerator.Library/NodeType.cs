//-----------------------------------------------------------------------
// <copyright file="NodeType.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    /// <summary>
    /// Defines the type of the Node
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Attribute
        /// </summary>
        Attribute = 1,

        /// <summary>
        /// Element
        /// </summary>
        Element = 2
    }
}
