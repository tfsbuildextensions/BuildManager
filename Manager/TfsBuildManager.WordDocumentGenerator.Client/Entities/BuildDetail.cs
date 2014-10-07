//-----------------------------------------------------------------------
// <copyright file="BuildDetail.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    using System.Collections.Generic;

    public class BuildDetail
    {
        public DocumentDetail DocumentDetail { get; set; }

        public Build Build { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<BuildConfiguration> BuildConfigurations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<BuildConfigurationSolution> BuildConfigurationSolutions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<WorkItem> WorkItems { get; set; }

        public string WorkItemIntroduction { get; set; }

        public string ChangesetTotalCount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<Changeset> Changesets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<ChangesetChangeDetail> ChangesetChangeDetails { get; set; }

        public TestResult TestResult { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<TestResultPassedItem> TestResultPassedItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<TestResultFailedItem> TestResultFailedItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Don't have a fix for this just now")]
        public IList<TestResultInconclusiveItem> TestResultInconclusiveItems { get; set; }
    }
}
