//-----------------------------------------------------------------------
// <copyright file="Program.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
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
    using Microsoft.TeamFoundation.Client;
    using TfsBuildManager.Views;
    using TfsBuildManager.Views.ViewModels;

    public class Program
    {
        private static readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();
        private static ReturnCode rc = ReturnCode.NoErrors;

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
            /// InvalidArgumentsSupplied
            /// </summary>
            InvalidArgumentsSupplied = -1500,

            /// <summary>
            /// UnhandledException
            /// </summary>
            UnhandledException = -1999,

            /// <summary>
            /// UsageRequested
            /// </summary>
            UsageRequested = -9000
        }

        internal static string Action
        {
            get
            {
                string action;
                if (Arguments.TryGetValue("Action", out action))
                {
                    return action;
                }

                return "Export";
            }
        }

        internal static string TeamProject
        {
            get
            {
                string project;
                if (Arguments.TryGetValue("TeamProject", out project))
                {
                    return project;
                }

                throw new ArgumentNullException("TeamProject");
            }
        }

        internal static Uri ProjectCollection
        {
            get
            {
                string collection;
                if (Arguments.TryGetValue("ProjectCollection", out collection))
                {
                    return new Uri(collection);
                }

                throw new ArgumentNullException("ProjectCollection");
            }
        }

        internal static string ExportPath
        {
            get
            {
                string path;
                if (Arguments.TryGetValue("ExportPath", out path))
                {
                    return path;
                }

                throw new ArgumentNullException("ExportPath");
            }
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

                TfsTeamProjectCollection collection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(ProjectCollection);

                IBuildServer buildServer = (IBuildServer)collection.GetService(typeof(IBuildServer));

                switch (Action.ToUpper())
                {
                    case "EXPORT":
                        // ---------------------------------------------------
                        // Export the specified builds
                        // ---------------------------------------------------
                        retval = ExportBuilds(buildServer);
                        if (retval != 0)
                        {
                            return retval;
                        }

                        break;
                    case "IMPORT":
                        Console.WriteLine("ImportBuildDefinitions is not yet implemented");
                        break;
                    default:
                        rc = ReturnCode.InvalidArgumentsSupplied;
                        return (int)rc;
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

        private static int ExportBuilds(IBuildServer buildServer)
        {
            IBuildDefinition[] defs = buildServer.QueryBuildDefinitions(TeamProject);

            if (!Directory.Exists(ExportPath))
            {
                Console.WriteLine("ExportPath not found, creating: {0}", ExportPath);
                Directory.CreateDirectory(ExportPath);
            }

            Console.WriteLine("Exporting {0} definitions to: {1}", defs.Length, ExportPath);
            Console.WriteLine(string.Empty);

            foreach (var b in defs)
            {
                Console.WriteLine(b.Name);
                BuildManagerViewModel.ExportDefinition(new BuildDefinitionViewModel(b), ExportPath);
            }
            
            Console.WriteLine(string.Empty);
            Console.WriteLine("{0} definitions exported to: {1}", defs.Length, ExportPath);

            return 0;
        }

        private static int ProcessArguments(string[] args)
        {
            if (args.Contains("/?") || args.Contains("/help"))
            {
                Console.WriteLine(@"Syntax: ctfsbm.exe /ProjectCollection:<ProjectCollection> /TeamProject:<TeamProject> /ExportPath:<ExportPath>");
                Console.WriteLine("Argument names are case sensitive.\n");
                Console.WriteLine(@"Sample: ctfsbm.exe /ProjectCollection:http://yourcollection:8080/tfs /TeamProject:""Your Team Project"" /ExportPath:""c:\myexporteddefs""");
                return (int)ReturnCode.UsageRequested;
            }

            Console.Write("Processing Arguments");
            if (args.Length == 0)
            {
                rc = ReturnCode.ArgumentsNotSupplied;
                LogMessage();
                return (int)rc;
            }

            Regex searchTerm = new Regex(@"/ProjectCollection:.*");
            bool propertiesargumentfound = args.Select(arg => searchTerm.Match(arg)).Any(m => m.Success);
            if (propertiesargumentfound)
            {
                Arguments.Add("ProjectCollection", args.First(item => item.Contains("/ProjectCollection:")).Replace("/ProjectCollection:", string.Empty));
            }

            searchTerm = new Regex(@"/TeamProject:.*");
            propertiesargumentfound = args.Select(arg => searchTerm.Match(arg)).Any(m => m.Success);
            if (propertiesargumentfound)
            {
                Arguments.Add("TeamProject", args.First(item => item.Contains("/TeamProject:")).Replace("/TeamProject:", string.Empty));
            }

            searchTerm = new Regex(@"/ExportPath:.*");
            propertiesargumentfound = args.Select(arg => searchTerm.Match(arg)).Any(m => m.Success);
            if (propertiesargumentfound)
            {
                Arguments.Add("ExportPath", args.First(item => item.Contains("/ExportPath:")).Replace("/ExportPath:", string.Empty));
            }

            Console.Write("...Success\n");
            return 0;
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
