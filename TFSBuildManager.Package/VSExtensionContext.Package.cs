//-----------------------------------------------------------------------
// <copyright file="VSExtensionContext.Package.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager
{
    using System;
    using Microsoft.VisualStudio.TeamFoundation;
    using Microsoft.VisualStudio.TeamFoundation.Build;
    using TfsBuildManager.Repository;

    public class VSExtensionContext : ITfsContext
    {
        private readonly TeamFoundationServerExt ext;
        private readonly IVsTeamFoundationBuild buildExt;
        private string currentConnectionUri;

        public VSExtensionContext(TeamFoundationServerExt ext, IVsTeamFoundationBuild buildExt)
        {
            this.ext = ext;
            this.buildExt = buildExt;
            if (ext != null)
            {
                this.currentConnectionUri = ext.ActiveProjectContext.DomainUri;
                ext.ProjectContextChanged += this.OnSelectedProjectChanged;
            }
        }

        public event EventHandler ProjectChanged;

        public string SelectedProject
        {
            get { return this.ext.ActiveProjectContext.ProjectName; }
        }

        public string ActiveConnection
        {
            get { return this.ext.ActiveProjectContext.DomainUri; }
        }

        public void OnSelectedProjectChanged(object sender, EventArgs e)
        {
            if (this.ProjectChanged != null && this.ext.ActiveProjectContext.DomainUri != this.currentConnectionUri)
            {
                this.currentConnectionUri = this.ext.ActiveProjectContext.DomainUri;
                this.ProjectChanged(this, new EventArgs());
            }
        }

        public void ShowBuild(Uri buildUri)
        {
            this.buildExt.DetailsManager.OpenBuild(buildUri);
        }

        public void EditBuildDefinition(Uri buildDefinition)
        {
            this.buildExt.DefinitionManager.OpenDefinition(buildDefinition);
        }

        public void ShowControllerManager()
        {
            this.buildExt.ControllerManager.Show();
        }
    }
}