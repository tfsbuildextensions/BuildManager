//-----------------------------------------------------------------------
// <copyright file="ChangesetChangeDetail.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System.Collections.Generic;

    public class ChangesetChangeDetail
    {
        public string ChangesetChangeId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<string> ChangesetChangeServerPaths { get; set; }
    }
}