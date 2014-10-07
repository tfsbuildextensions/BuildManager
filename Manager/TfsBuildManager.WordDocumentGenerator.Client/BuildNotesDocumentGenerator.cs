//-----------------------------------------------------------------------
// <copyright file="BuildNotesDocumentGenerator.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace WordDocumentGenerator.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using DocumentFormat.OpenXml.Drawing.Charts;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using WordDocumentGenerator.Client.Entities;
    using WordDocumentGenerator.Library;

    /// <summary>
    /// Document generator for template having table and charts
    /// </summary>
    public class BuildNotesDocumentGenerator : DocumentGenerator
    {
        // Content Control Tags - Table tags are different. Other Tags are same so reusing the base class's code
        protected const string DocumentTitle = "DocumentTitle";
        protected const string DocumentCreatedOn = "DocumentCreatedOn";
        protected const string DocumentCreatedBy = "DocumentCreatedBy";

        protected const string BuildNumber = "BuildNumber";
        protected const string BuildState = "BuildState";
        protected const string BuildReason = "BuildReason";
        protected const string BuildQuality = "BuildQuality";
        protected const string BuildRequestedBy = "BuildRequestedBy";
        protected const string BuildDefinitionName = "BuildDefinitionName";
        protected const string BuildTeamProject = "BuildTeamProject";
        protected const string BuildTotalExecutionTimeInSeconds = "BuildTotalExecutionTimeInSeconds";
        protected const string BuildController = "BuildController";
        protected const string BuildCompletedNoOfDaysAgo = "BuildCompletedNoOfDaysAgo";
        protected const string BuildUri = "BuildUri";
        protected const string BuildRetainIndefinitely = "BuildRetainIndefinitely";
        protected const string BuildSourceGetVersion = "BuildSourceGetVersion";
        protected const string BuildShelvesetName = "BuildShelvesetName";
        protected const string BuildLabelName = "BuildLabelName";
        protected const string BuildLogLocation = "BuildLogLocation";
        protected const string BuildDropLocation = "BuildDropLocation";

        protected const string BuildLastModifiedBy = "BuildLastModifiedBy";
        protected const string BuildLastModifiedOn = "BuildLastModifiedOn";
        protected const string BuildLastModifiedNoOfDaysAgo = "BuildLastModifiedNoOfDaysAgo";

        protected const string BuildConfigurationSolutionContentControlRow = "BuildConfigurationSolutionContentControlRow";
        protected const string BuildRootNodeSolutionServerPath = "BuildRootNodeSolutionServerPath";
        protected const string BuildRootNodeErrorCount = "BuildRootNodeErrorCount";
        protected const string BuildRootNodeWarningCount = "BuildRootNodeWarningCount";
        protected const string BuildRootNodeLogFile = "BuildRootNodeLogFile";
        protected const string BuildConfigurationContentControlRow = "BuildConfigurationContentControlRow";
        protected const string BuildPlatform = "BuildPlatform";
        protected const string BuildFlavor = "BuildFlavor";
        protected const string BuildConfigTotalErrors = "BuildConfigTotalErrors";
        protected const string BuildConfigTotalWarnings = "BuildConfigTotalWarnings";

        protected const string ChangesetContentControlRow = "ChangesetContentControlRow";
        protected const string ChangesetId = "ChangesetId";
        protected const string ChangesetComment = "ChangesetComment";
        protected const string ChangesetCommittedOn = "ChangesetCommittedOn";
        protected const string ChangesetCommittedBy = "ChangesetCommittedBy";
        protected const string ChangesetTotalCount = "ChangesetTotalCount";

        protected const string ChangesetChangeContentControlRow = "ChangesetChangeContentControlRow";
        protected const string CheckInInformationImg = "CheckInInformationImg";
        protected const string ChangesetChangeId = "ChangesetChangeId";
        protected const string ChangesetChangeServerPaths = "ChangesetChangeServerPaths";
        protected const string ChangesetChangeServerPath = "ChangesetChangeServerPath";

        protected const string WorkItemContentControlRow = "WorkItemContentControlRow";
        protected const string WorkItemId = "WorkItemId";
        protected const string WorkItemTitle = "WorkItemTitle";
        protected const string WorkItemType = "WorkItemType";
        protected const string WorkItemState = "WorkItemState";
        protected const string WorkItemAssignedTo = "WorkItemAssignedTo";
        protected const string WorkItemIteration = "WorkItemIteration";
        protected const string WorkItemAreaPath = "WorkItemAreaPath";
        protected const string WorkItemIntroduction = "WorkItemIntroduction";

        protected const string TestResultId = "TestResultId";
        protected const string TestResultTitle = "TestResultTitle";
        protected const string TestResultStatus = "TestResultStatus";
        protected const string TestResultTotalTest = "TestResultTotalTest";
        protected const string TestResultTotalTestCompleted = "TestResultTotalTestCompleted";
        protected const string TestResultTotalTestPassed = "TestResultTotalTestPassed";
        protected const string TestResultTotalTestFailed = "TestResultTotalTestFailed";
        protected const string TestResultTotalTestPassRate = "TestResultTotalTestPassRate";
        protected const string TestResultTotalTestInconclusive = "TestResultTotalTestInconclusive";
        protected const string TestResultIsAutomated = "TestResultIsAutomated";
        protected const string TestResultDetailsImage = "TestResultDetailsImage";

        protected const string TestResultPassedItemDetailContentControlRow = "TestResultPassedItemDetailContentControlRow";
        protected const string TestResultPassedListTitle = "TestResultPassedListTitle";
        protected const string TestResultPassedListStatus = "TestResultPassedListStatus";

        protected const string TestResultFailedItemDetailContentControlRow = "TestResultFailedItemDetailContentControlRow";
        protected const string TestResultFailedListTitle = "TestResultFailedListTitle";
        protected const string TestResultFailedListStatus = "TestResultFailedListStatus";
        protected const string TestResultFailedListReason = "TestResultFailedListReason";

        protected const string TestResultInconclusiveItemDetailContentControlRow = "TestResultInconclusiveItemDetailContentControlRow";
        protected const string TestResultInconclusiveListTitle = "TestResultInconclusiveListTitle";
        protected const string TestResultInconclusiveListStatus = "TestResultInconclusiveListStatus";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Do not have a fix for it yet!")]
        public BuildNotesDocumentGenerator(DocumentGenerationInfo generationInfo, List<string> exclusions) : base(generationInfo, exclusions)
        {
        }

        /// <summary>
        /// Gets the place holder tag to type collection.
        /// </summary>
        /// <returns>Returns a dictionary of the placeholder name and the place holder type</returns>
        protected override Dictionary<string, PlaceholderType> GetPlaceholderTagToTypeCollection()
        {
            Dictionary<string, PlaceholderType> placeHolderTagToTypeCollection = new Dictionary<string, PlaceholderType>();

            // Handle recursive placeholders            
            placeHolderTagToTypeCollection.Add(BuildConfigurationSolutionContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(BuildConfigurationContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(ChangesetContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(WorkItemContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(TestResultPassedItemDetailContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(TestResultFailedItemDetailContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(TestResultInconclusiveItemDetailContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(ChangesetChangeContentControlRow, PlaceholderType.Recursive);
            placeHolderTagToTypeCollection.Add(ChangesetChangeServerPaths, PlaceholderType.Recursive);

            // Handle non recursive placeholders
            // placeHolderTagToTypeCollection.Add(VendorId, PlaceholderType.NonRecursive);
            // placeHolderTagToTypeCollection.Add(VendorName, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(DocumentCreatedOn, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(DocumentCreatedBy, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildNumber, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildState, PlaceholderType.PictureContainer);
            placeHolderTagToTypeCollection.Add(BuildReason, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildQuality, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildRequestedBy, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildDefinitionName, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildTeamProject, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildTotalExecutionTimeInSeconds, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildController, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildCompletedNoOfDaysAgo, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildUri, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildRetainIndefinitely, PlaceholderType.PictureContainer);
            placeHolderTagToTypeCollection.Add(BuildSourceGetVersion, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildShelvesetName, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildLabelName, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildLogLocation, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildDropLocation, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(BuildLastModifiedBy, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildLastModifiedOn, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildLastModifiedNoOfDaysAgo, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(BuildRootNodeSolutionServerPath, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildRootNodeErrorCount, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildRootNodeWarningCount, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildRootNodeLogFile, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(BuildPlatform, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildFlavor, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildConfigTotalErrors, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(BuildConfigTotalWarnings, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(ChangesetId, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(ChangesetComment, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(ChangesetCommittedBy, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(ChangesetCommittedOn, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(ChangesetTotalCount, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(CheckInInformationImg, PlaceholderType.PictureContainer);
            placeHolderTagToTypeCollection.Add(ChangesetChangeId, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(ChangesetChangeServerPath, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(WorkItemId, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemTitle, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemType, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemAssignedTo, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemState, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemIteration, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemAreaPath, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(WorkItemIntroduction, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(TestResultId, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTitle, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultStatus, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTotalTest, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTotalTestCompleted, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTotalTestPassed, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTotalTestFailed, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTotalTestPassRate, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultTotalTestInconclusive, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultIsAutomated, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultDetailsImage, PlaceholderType.PictureContainer);

            placeHolderTagToTypeCollection.Add(TestResultPassedListTitle, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultPassedListStatus, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(TestResultFailedListTitle, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultFailedListStatus, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultFailedListReason, PlaceholderType.NonRecursive);

            placeHolderTagToTypeCollection.Add(TestResultInconclusiveListTitle, PlaceholderType.NonRecursive);
            placeHolderTagToTypeCollection.Add(TestResultInconclusiveListStatus, PlaceholderType.NonRecursive);

            return placeHolderTagToTypeCollection;
        }

        /// <summary>
        /// Refreshes the charts.
        /// </summary>
        /// <param name="mainDocumentPart">The main document part.</param>
        protected override void RefreshCharts(MainDocumentPart mainDocumentPart)
        {
            if (mainDocumentPart != null)
            {
                foreach (ChartPart chartPart in mainDocumentPart.ChartParts)
                {
                    Chart chart = chartPart.ChartSpace.Elements<Chart>().FirstOrDefault();

                    if (chart != null)
                    {
                    }

                    chartPart.ChartSpace.Save();
                }
            }
        }

        /// <summary>
        /// Non recursive placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Don't have a fix for this just now"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Don't have a fix for this just now")]
        protected override void NonRecursivePlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (openXmlElementDataContext == null || openXmlElementDataContext.Element == null || openXmlElementDataContext.DataContext == null)
            {
                return;
            }

            string tagPlaceHolderValue;
            string tagGuidPart;
            DocumentGenerator.GetTagValue(openXmlElementDataContext.Element as SdtElement, out tagPlaceHolderValue, out tagGuidPart);

            string tagValue = string.Empty;
            string content = string.Empty;

            // Reuse base class code and handle only tags unavailable in base class
            bool bubblePlaceHolder = true;

            switch (tagPlaceHolderValue)
            {
                case DocumentCreatedOn:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).DocumentDetail.CreatedOn;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).DocumentDetail.CreatedOn;
                    break;
                case DocumentCreatedBy:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).DocumentDetail.CreatedBy;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).DocumentDetail.CreatedBy;
                    break;
                case BuildNumber:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildNumber;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildNumber;
                    break;
                case BuildReason:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildReason;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildReason;
                    break;
                case BuildQuality:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildQuality;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildQuality;
                    break;
                case BuildRequestedBy:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildRequestedBy;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildRequestedBy;
                    break;
                case BuildDefinitionName:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildDefinition;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildDefinition;
                    break;
                case BuildTeamProject:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildTeamProject;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildTeamProject;
                    break;
                case BuildTotalExecutionTimeInSeconds:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildTotalExecutionTimeInSeconds;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildTotalExecutionTimeInSeconds;
                    break;
                case BuildController:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildController;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildController;
                    break;
                case BuildUri:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildUri.ToString();
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildUri.ToString();
                    break;
                case BuildSourceGetVersion:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildSourceGetVersion;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildSourceGetVersion;
                    break;
                case BuildShelvesetName:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildShelvesetName;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildShelvesetName;
                    break;
                case BuildLabelName:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLabelName;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLabelName;
                    break;
                case BuildLogLocation:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLogLocation;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLogLocation;
                    break;
                case BuildDropLocation:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildDropLocation;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildDropLocation;
                    break;
                case BuildLastModifiedBy:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLastChangedBy;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLastChangedBy;
                    break;
                case BuildLastModifiedOn:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLastChangedOn;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLastChangedOn;
                    break;
                case BuildLastModifiedNoOfDaysAgo:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLastModifiedNoOfDaysAgo;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildLastModifiedNoOfDaysAgo;
                    break;
                case BuildCompletedNoOfDaysAgo:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildCompletedNoOfDaysAgo;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildCompletedNoOfDaysAgo;
                    break;
                case BuildRootNodeSolutionServerPath:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeSolutionServerPath;
                    content = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeSolutionServerPath;
                    break;
                case BuildRootNodeErrorCount:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeErrorCount;
                    content = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeErrorCount;
                    break;
                case BuildRootNodeWarningCount:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeWarningCount;
                    content = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeWarningCount;
                    break;
                case BuildRootNodeLogFile:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeLogFile;
                    content = (openXmlElementDataContext.DataContext as BuildConfigurationSolution).BuildRootNodeLogFile;
                    break;
                case BuildPlatform:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigPlatform;
                    content = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigPlatform;
                    break;
                case BuildFlavor:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigFlavor;
                    content = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigFlavor;
                    break;
                case BuildConfigTotalErrors:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigTotalErrors;
                    content = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigTotalErrors;
                    break;
                case BuildConfigTotalWarnings:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigTotalWarnings;
                    content = (openXmlElementDataContext.DataContext as BuildConfiguration).BuildConfigTotalWarnings;
                    break;
                case ChangesetId:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as Changeset).ChangesetId;
                    content = (openXmlElementDataContext.DataContext as Changeset).ChangesetId;
                    break;
                case ChangesetComment:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as Changeset).ChangesetComment;
                    content = (openXmlElementDataContext.DataContext as Changeset).ChangesetComment;
                    break;
                case ChangesetCommittedBy:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as Changeset).ChangesetCommittedBy;
                    content = (openXmlElementDataContext.DataContext as Changeset).ChangesetCommittedBy;
                    break;
                case ChangesetCommittedOn:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as Changeset).ChangesetCommittedOn;
                    content = (openXmlElementDataContext.DataContext as Changeset).ChangesetCommittedOn;
                    break;
                case ChangesetTotalCount:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).ChangesetTotalCount;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).ChangesetTotalCount;
                    break;
                case ChangesetChangeId:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as ChangesetChangeDetail).ChangesetChangeId;
                    content = (openXmlElementDataContext.DataContext as ChangesetChangeDetail).ChangesetChangeId;
                    break;
                case ChangesetChangeServerPath:
                    bubblePlaceHolder = false;
                    tagValue = openXmlElementDataContext.DataContext as string;
                    content = openXmlElementDataContext.DataContext as string;
                    break;
                case WorkItemId:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemId;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemId;
                    break;
                case WorkItemTitle:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemTitle;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemTitle;
                    break;
                case WorkItemType:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemType;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemType;
                    break;
                case WorkItemState:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemState;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemState;
                    break;
                case WorkItemAssignedTo:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemAssignedTo;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemAssignedTo;
                    break;
                case WorkItemIteration:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemIterationPath;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemIterationPath;
                    break;
                case WorkItemAreaPath:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as WorkItem).WorkItemAreaPath;
                    content = (openXmlElementDataContext.DataContext as WorkItem).WorkItemAreaPath;
                    break;
                case WorkItemIntroduction:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).WorkItemIntroduction;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).WorkItemIntroduction;
                    break;
                case TestResultId:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultId;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultId;
                    break;
                case TestResultTitle:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTitle;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTitle;
                    break;
                case TestResultStatus:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultStatus;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultStatus;
                    break;
                case TestResultTotalTest:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTest;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTest;
                    break;
                case TestResultTotalTestCompleted:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestCompleted;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestCompleted;
                    break;
                case TestResultTotalTestPassed:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestPassed;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestPassed;
                    break;
                case TestResultTotalTestFailed:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestFailed;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestFailed;
                    break;
                case TestResultTotalTestPassRate:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestPassRate;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestPassRate;
                    break;
                case TestResultTotalTestInconclusive:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestInconclusive;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultTotalTestInconclusive;
                    break;
                case TestResultIsAutomated:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultIsAutomated;
                    content = (openXmlElementDataContext.DataContext as BuildDetail).TestResult.TestResultIsAutomated;
                    break;
                case TestResultPassedListTitle:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as TestResultPassedItem).TestResultPassedListTitle;
                    content = (openXmlElementDataContext.DataContext as TestResultPassedItem).TestResultPassedListTitle;
                    break;
                case TestResultPassedListStatus:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as TestResultPassedItem).TestResultPassedListStatus;
                    content = (openXmlElementDataContext.DataContext as TestResultPassedItem).TestResultPassedListStatus;
                    break;
                case TestResultFailedListTitle:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as TestResultFailedItem).TestResultFailedListTitle;
                    content = (openXmlElementDataContext.DataContext as TestResultFailedItem).TestResultFailedListTitle;
                    break;
                case TestResultFailedListReason:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as TestResultFailedItem).TestResultFailedListReason;
                    content = (openXmlElementDataContext.DataContext as TestResultFailedItem).TestResultFailedListReason;
                    break;
                case TestResultInconclusiveListTitle:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as TestResultInconclusiveItem).TestResultInconclusiveListTitle;
                    content = (openXmlElementDataContext.DataContext as TestResultInconclusiveItem).TestResultInconclusiveListTitle;
                    break;
                case TestResultInconclusiveListStatus:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as TestResultInconclusiveItem).TestResultInconclusiveListStatus;
                    content = (openXmlElementDataContext.DataContext as TestResultInconclusiveItem).TestResultInconclusiveListStatus;
                    break;
            }

            if (bubblePlaceHolder)
            {
                // Use base class code as tags are already defined in base class.
                // base.
                this.NonRecursivePlaceholderFound(placeholderTag, openXmlElementDataContext);
            }
            else
            {
                // Set the tag for the content control
                if (!string.IsNullOrEmpty(tagValue))
                {
                    DocumentGenerator.SetTagValue(openXmlElementDataContext.Element as SdtElement, DocumentGenerator.GetFullTagValue(tagPlaceHolderValue, tagValue));
                }

                // Set the content for the content control
                DocumentGenerator.SetContentOfContentControl(openXmlElementDataContext.Element as SdtElement, content);
            }
        }

        /// <summary>
        /// Picture container placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The picture container placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected override void PictureContainerPlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (openXmlElementDataContext == null || openXmlElementDataContext.Element == null || openXmlElementDataContext.DataContext == null)
            {
                return;
            }

            string tagPlaceHolderValue;
            string tagGuidPart;
            DocumentGenerator.GetTagValue(openXmlElementDataContext.Element as SdtElement, out tagPlaceHolderValue, out tagGuidPart);

            string tagValue = string.Empty;
            string content = string.Empty;

            // Reuse base class code and handle only tags unavailable in base class
            bool bubblePlaceHolder = true;

            switch (tagPlaceHolderValue)
            {
                case BuildState:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildState;
                    switch (tagValue.ToUpper())
                    {
                        case "SUCCEEDED":
                            content = @"Graphics\Succeeded.JPG";
                            break;
                        case "PARTIALLYSUCCEEDED":
                            content = @"Graphics\PartiallySucceeded.PNG";
                            break;
                        case "FAILED":
                            content = @"Graphics\Failed.JPG";
                            break;
                        case "STOPPED":
                            content = @"Graphics\Stopped.PNG";
                            break;
                        default:
                            content = @"Graphics\NotStarted.JPG";
                            break;
                    }

                    break;
                case BuildRetainIndefinitely:
                    bubblePlaceHolder = false;
                    tagValue = (openXmlElementDataContext.DataContext as BuildDetail).Build.BuildRetainIndefinitely;
                    switch (tagValue.ToUpper())
                    {
                        case "TRUE":
                            content = @"Graphics\Lock.PNG";
                            break;
                        case "FALSE":
                            content = @"Graphics\Unlock.PNG";
                            break;
                    }

                    break;
                case CheckInInformationImg:
                    bubblePlaceHolder = false;
                    content = @"Graphics\Changeset.PNG";
                    break;
                case TestResultDetailsImage:
                    bubblePlaceHolder = false;
                    content = @"Graphics\TestSuite.Png";
                    break;
            }

            if (bubblePlaceHolder)
            {
                // Use base class code as tags are already defined in base class.
                // base.
                this.NonRecursivePlaceholderFound(placeholderTag, openXmlElementDataContext);
            }
            else
            {
                // Set the tag for the content control
                if (!string.IsNullOrEmpty(tagValue))
                {
                    DocumentGenerator.SetTagValue(openXmlElementDataContext.Element as SdtElement, DocumentGenerator.GetFullTagValue(tagPlaceHolderValue, tagValue));
                }

                // Set the content for the content control
                DocumentGenerator.SetContentOfContentControl(openXmlElementDataContext.Element as SdtElement, content);
            }
        }

        /// <summary>
        /// Recursive placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Don't have a fix for this just now")]
        protected override void RecursivePlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext)
        {
            if (openXmlElementDataContext == null || openXmlElementDataContext.Element == null || openXmlElementDataContext.DataContext == null)
            {
                return;
            }

            string tagPlaceHolderValue;
            string tagGuidPart;
           DocumentGenerator.GetTagValue(openXmlElementDataContext.Element as SdtElement, out tagPlaceHolderValue, out tagGuidPart);

            // Reuse base class code and handle only tags unavailable in base class
            bool bubblePlaceHolder = true;

            switch (tagPlaceHolderValue)
            {
                case BuildConfigurationSolutionContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (BuildConfigurationSolution bcs in (openXmlElementDataContext.DataContext as BuildDetail).BuildConfigurationSolutions)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = bcs });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case BuildConfigurationContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (BuildConfiguration bc in (openXmlElementDataContext.DataContext as BuildDetail).BuildConfigurations)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = bc });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case ChangesetContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (Changeset cs in (openXmlElementDataContext.DataContext as BuildDetail).Changesets)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = cs });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case ChangesetChangeContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (ChangesetChangeDetail ccd in (openXmlElementDataContext.DataContext as BuildDetail).ChangesetChangeDetails)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = ccd });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case ChangesetChangeServerPaths:
                    bubblePlaceHolder = false;

                    foreach (var ccsp in (openXmlElementDataContext.DataContext as ChangesetChangeDetail).ChangesetChangeServerPaths)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = ccsp });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case WorkItemContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (WorkItem wi in (openXmlElementDataContext.DataContext as BuildDetail).WorkItems)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = wi });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case TestResultPassedItemDetailContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (TestResultPassedItem trpi in (openXmlElementDataContext.DataContext as BuildDetail).TestResultPassedItems)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = trpi });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case TestResultFailedItemDetailContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (TestResultFailedItem trfi in (openXmlElementDataContext.DataContext as BuildDetail).TestResultFailedItems)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = trfi });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
                case TestResultInconclusiveItemDetailContentControlRow:
                    bubblePlaceHolder = false;

                    foreach (TestResultInconclusiveItem trii in (openXmlElementDataContext.DataContext as BuildDetail).TestResultInconclusiveItems)
                    {
                        // SdtElement clonedElement = 
                        this.CloneElementAndSetContentInPlaceholders(new OpenXmlElementDataContext { Element = openXmlElementDataContext.Element, DataContext = trii });
                    }

                    openXmlElementDataContext.Element.Remove();
                    break;
            }

            if (bubblePlaceHolder)
            {
                // Use base class code as tags are already defined in base class.
                // base.
                this.RecursivePlaceholderFound(placeholderTag, openXmlElementDataContext);
            }
        }

        protected override void ContainerPlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext)
        {
        }

        /// <summary>
        /// Ignore placeholder found.
        /// </summary>
        /// <param name="placeholderTag">The placeholder tag.</param>
        /// <param name="openXmlElementDataContext">The open XML element data context.</param>
        protected override void IgnorePlaceholderFound(string placeholderTag, OpenXmlElementDataContext openXmlElementDataContext)
        {
        }
    }
}