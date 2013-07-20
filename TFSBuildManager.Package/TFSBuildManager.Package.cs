//-----------------------------------------------------------------------
// <copyright file="TFSBuildManager.Package.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TeamFoundation;
    using Microsoft.VisualStudio.TeamFoundation.Build;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)] // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is a package.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // This attribute is used to register the informations needed to show the this package in the Help/About dialog of Visual Studio.
    [ProvideMenuResource("Menus.ctmenu", 1)] // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideToolWindow(typeof(BuildManagerToolWindow), Transient = true, Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom)] // This attribute registers a tool window exposed by this package.
    [Guid(GuidList.GuidTfsBuildManagerPackageString)]
    public sealed class TfsBuildManagerPackage : Package
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="TfsBuildManagerPackage"/> class.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public TfsBuildManagerPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
        }

        public static TfsTeamProjectCollection Tfs
        {
            get
            {
                TeamFoundationServerExt ext = GetTeamFoundationServerExt();
                TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(ext.ActiveProjectContext.DomainUri));
                return tfs;
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();

            try
            {
                // Add our command handlers for menu (commands must exist in the .vsct file)
                OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
                if (null != mcs)
                {
                    // Create the command for the menu item.
                    CommandID menuCommandID = new CommandID(GuidList.GuidTfsBuildManagerPackageCmdSet, (int)PkgCmdIDList.CmdidTestCommand);
                    MenuCommand menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                    mcs.AddCommand(menuItem);

                    // Create the command for the tool window
                    CommandID toolwndCommandID = new CommandID(GuidList.GuidTfsBuildManagerPackageCmdSet, (int)PkgCmdIDList.CmdidTestTool);
                    MenuCommand menuToolWin = new MenuCommand(this.ShowToolWindow, toolwndCommandID);
                    mcs.AddCommand(menuToolWin);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private static TeamFoundationServerExt GetTeamFoundationServerExt()
        {
            var dte = GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            return dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
        }

        private static void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Community TFS Build Manager", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            try
            {
                var ext = GetTeamFoundationServerExt();
                IVsTeamFoundationBuild vstfBuild = (IVsTeamFoundationBuild)GetService(typeof(IVsTeamFoundationBuild));

                // Get the instance number 0 of this tool window. This window is single instance so this instance is actually the only one.
                // The last flag is set to true so that if the tool window does not exists it will be created.
                ToolWindowPane window = this.FindToolWindow(typeof(BuildManagerToolWindow), 0, true);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException(Resources.CanNotCreateWindow);
                }

                var wnd = window as BuildManagerToolWindow;
                wnd.InitializeExtension(ext, vstfBuild);
                IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            this.ShowToolWindow(this, new EventArgs());
        }
    }
}
