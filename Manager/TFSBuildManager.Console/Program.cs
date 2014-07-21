//-----------------------------------------------------------------------
// <copyright file="Program.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TFSBuildManager.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;
    using Microsoft.TeamFoundation.Build.Workflow;
    using Microsoft.TeamFoundation.Build.Workflow.Activities;

    using Newtonsoft.Json;
    using TfsBuildManager.Views;

    public class Program
    {
        private static ReturnCode rc = ReturnCode.NoErrors;
        private static ConsoleAction action = ConsoleAction.ExportBuildDefinitions;
        
        private enum ConsoleAction
        {
            /// <summary>
            /// ExportBuildDefinitions
            /// </summary>
            ExportBuildDefinitions = 1,
  
            /// <summary>
            /// ImportBuildDefinitions
            /// </summary>
            ImportBuildDefinitions = 2
        }

        private enum ReturnCode
        {
            /// <summary>
            /// NoErrors
            /// </summary>
            NoErrors = 0,

            /// <summary>
            /// ArgumentsNotSupplied
            /// </summary>
            ArgumentsNotSupplied = -1000,

            /// <summary>
            /// TfsUrlNotProvided
            /// </summary>
            TfsUrlNotProvided = -1050,

            /// <summary>
            /// UnhandledException
            /// </summary>
            UnhandledException = -1999,

            /// <summary>
            /// UsageRequested
            /// </summary>
            UsageRequested = -9000
        }

        private static int Main(string[] args)
        {
            Console.WriteLine("Community TFS Build Manager Console - {0}\n", GetFileVersion(Assembly.GetExecutingAssembly()));

            try
            {
                // ---------------------------------------------------
                // Process the arguments
                // ---------------------------------------------------
                int retval = ProcessArguments(args);
                if (retval != 0)
                {
                    return retval;
                }

                switch (action)
                {
                    case ConsoleAction.ExportBuildDefinitions:
                        // ---------------------------------------------------
                        // Export the specified builds
                        // ---------------------------------------------------
                        retval = ExportBuilds();
                        if (retval != 0)
                        {
                            return retval;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += string.Format("Inner Exception: {0}", ex.InnerException.Message);
                }

                rc = ReturnCode.UnhandledException;
                LogMessage(message);
                return (int)rc;
            }

            return (int)rc;
        }

        private static int ExportBuilds()
        {
            throw new NotImplementedException();
        }

        private static int ProcessArguments(string[] args)
        {
            if (args.Contains("/?") || args.Contains("/help"))
            {
                Console.WriteLine(@"Syntax:\t\ctfsbm.exe /f:<files> | /auto [switches]\n");
                Console.WriteLine("Optional Switches:\t\t\n");
                Console.WriteLine("Samples:\t\t\n");

                return (int)ReturnCode.UsageRequested;
            }

            Console.Write("Processing Arguments");
            if (args.Length == 0)
            {
                rc = ReturnCode.ArgumentsNotSupplied;
                LogMessage();
                return (int)rc;
            }

            Regex searchTerm = new Regex(@"/p:.*", RegexOptions.IgnoreCase);
            bool propertiesargumentfound = args.Select(arg => searchTerm.Match(arg)).Any(m => m.Success);
            if (propertiesargumentfound)
            {
                // properties = args.First(item => item.Contains("/p:")).Replace("/p:", string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            Console.Write("...Success\n");
            return 0;
        }

        private static void ExportDefinition(IBuildDefinition b, string filePath)
        {
            ExportedBuildDefinition buildToExport = new ExportedBuildDefinition { Name = b.Name, Description = b.Description };
            if (b.BuildController != null)
            {
                buildToExport.BuildController = b.BuildController.Name;
            }

            buildToExport.ContinuousIntegrationType = b.ContinuousIntegrationType;
            buildToExport.DefaultDropLocation = b.DefaultDropLocation;
            buildToExport.Schedules = new List<ExportedISchedule>();

            foreach (var schedule in b.Schedules)
            {
                buildToExport.Schedules.Add(new ExportedISchedule { StartTime = schedule.StartTime, DaysToBuild = schedule.DaysToBuild, TimeZone = schedule.TimeZone });
            }

            buildToExport.SourceProviders = new List<ExportedIBuildDefinitionSourceProvider>();
            foreach (var provider in b.SourceProviders)
            {
                buildToExport.SourceProviders.Add(new ExportedIBuildDefinitionSourceProvider { Fields = provider.Fields, Name = provider.Name, SupportedTriggerTypes = provider.SupportedTriggerTypes });
            }

            buildToExport.QueueStatus = b.QueueStatus;
            buildToExport.ContinuousIntegrationQuietPeriod = b.ContinuousIntegrationQuietPeriod;

            if (b.SourceProviders.All(s => s.Name != "TFGIT"))
            {
                buildToExport.Mappings = new List<ExportedIWorkspaceMapping>();
                foreach (var map in b.Workspace.Mappings)
                {
                    buildToExport.Mappings.Add(new ExportedIWorkspaceMapping { Depth = map.Depth, LocalItem = map.LocalItem, MappingType = map.MappingType, ServerItem = map.ServerItem });
                }
            }

            buildToExport.RetentionPolicyList = new List<ExportedIRetentionPolicy>();
            foreach (var rp in b.RetentionPolicyList)
            {
                buildToExport.RetentionPolicyList.Add(new ExportedIRetentionPolicy { BuildDefinition = rp.BuildDefinition, BuildReason = rp.BuildReason, BuildStatus = rp.BuildStatus, NumberToKeep = rp.NumberToKeep, DeleteOptions = rp.DeleteOptions });
            }

            if (b.Process != null)
            {
                buildToExport.ProcessTemplate = b.Process.ServerPath;
            }

            var processParameters = WorkflowHelpers.DeserializeProcessParameters(b.ProcessParameters);
            if (processParameters.ContainsKey("AgentSettings"))
            {
                if (processParameters["AgentSettings"].GetType() == typeof(AgentSettings))
                {
                    AgentSettings ags = (AgentSettings)processParameters["AgentSettings"];
                    AgentSettingsBuildParameter agentSet = new AgentSettingsBuildParameter();
                    agentSet.MaxExecutionTime = ags.MaxExecutionTime;
                    agentSet.MaxWaitTime = ags.MaxWaitTime;
                    agentSet.Name = ags.Name;
                    agentSet.Comparison = ags.TagComparison;
                    agentSet.Tags = ags.Tags;
                    buildToExport.TfvcAgentSettings = agentSet;
                }
                else if (processParameters["AgentSettings"].GetType() == typeof(BuildParameter))
                {
                    BuildParameter ags = (BuildParameter)processParameters["AgentSettings"];
                    {
                        buildToExport.GitAgentSettings = ags;
                    }
                }
            }

            buildToExport.ProcessParameters = WorkflowHelpers.DeserializeProcessParameters(b.ProcessParameters);

            File.WriteAllText(Path.Combine(filePath, b.Name + ".json"), JsonConvert.SerializeObject(buildToExport, Formatting.Indented));
        }

        private static void LogMessage(string message = null)
        {
            const string MessageBlockStart = "\n-------------------------------------------------------------------";
            const string MessageBlockEnd = "-------------------------------------------------------------------";
            Console.WriteLine(MessageBlockStart);
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }

            Console.WriteLine("Return Code: {0} ({1})", (int)rc, rc);
            Console.WriteLine(MessageBlockEnd);
        }

        private static Version GetFileVersion(Assembly asm)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
            return new Version(versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
        }
    }
}
