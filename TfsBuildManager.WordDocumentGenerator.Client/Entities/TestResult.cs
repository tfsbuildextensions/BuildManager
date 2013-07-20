//-----------------------------------------------------------------------
// <copyright file="TestResult.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client.Entities
{
    public class TestResult
    {
        public string TestResultId { get; set; }

        public string TestResultTitle { get; set; }

        public string TestResultStatus { get; set; }

        public string TestResultTotalTest { get; set; }

        public string TestResultTotalTestCompleted { get; set; }

        public string TestResultTotalTestPassed { get; set; }

        public string TestResultTotalTestFailed { get; set; }

        public string TestResultTotalTestPassRate { get; set; }

        public string TestResultTotalTestInconclusive { get; set; }

        public string TestResultIsAutomated { get; set; }
    }
}