//-----------------------------------------------------------------------
// <copyright file="DocumentMetadata.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    /// <summary>
    /// Defines the metadata for a Word document
    /// </summary>
    public class DocumentMetadata
    {
        #region Members

        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        public string DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the document version.
        /// </summary>
        /// <value>
        /// The document version.
        /// </value>
        public string DocumentVersion { get; set; }

        #endregion
    }
}
