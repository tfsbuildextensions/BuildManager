//-----------------------------------------------------------------------
// <copyright file="OpenXmlElementDataContext.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    using DocumentFormat.OpenXml;

    /// <summary>
    /// OpenXml element and data context
    /// </summary>
    public class OpenXmlElementDataContext
    {
        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public OpenXmlElement Element { get; set; }

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public object DataContext { get; set; }
    }
}
