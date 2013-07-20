//-----------------------------------------------------------------------
// <copyright file="DocumentGenerationInfo.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Library
{
    using System;
    using System.Collections.Generic;

    public class DocumentGenerationInfo
    {
        /// <summary>
        /// Namespace Uri for CustomXML part
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Don't have a fix for this just now")]
        public static Uri NamespaceUri = new Uri(@"http://schemas.WordDocumentGenerator.com/DocumentGeneration");

        /// <summary>
        /// Gets or sets the place holder tag to type collection.
        /// </summary>
        /// <value>
        /// The place holder tag to type collection.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public Dictionary<string, PlaceholderType> PlaceholderTagToTypeCollection { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public DocumentMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the template data.
        /// </summary>
        /// <value>
        /// The template data.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't have a fix for this just now")]
        public byte[] TemplateData { get; set; }

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public object DataContext { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is data bound controls.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is data bound controls; otherwise, <c>false</c>.
        /// </value>
        public bool IsDataBoundControls { get; set; }
    }
}
