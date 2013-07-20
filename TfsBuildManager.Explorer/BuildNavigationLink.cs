//-----------------------------------------------------------------------
// <copyright file="BuildNavigationLink.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Explorer
{
    using System;
    using System.Windows.Forms;
    using EnvDTE;
    using Microsoft.TeamFoundation.Controls;

    [TeamExplorerNavigationLink("{5514499B-D1B9-4194-8D50-999F70DAC731}", TeamExplorerNavigationItemIds.Builds, 0)]
    public class BuildNavigationLink : ITeamExplorerNavigationLink
    {        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        public bool IsEnabled
        {
            get { return true; }
        }

        public bool IsVisible
        {
            get { return true; }
        }

        public string Text
        {
            get { return "Community TFS Build Manager"; }
        }

        public void Execute()
        {
            try
            {
                EnvDTE80.DTE2 dte2 = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
                if (dte2 != null)
                {
                    dte2.ExecuteCommand("Tools.CommunityTFSBuildManager");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Invalidate()
        {
        }

        public void Dispose()
        {
        }
    }
}
