//-----------------------------------------------------------------------
// <copyright file="BuildAgentViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using Microsoft.TeamFoundation.Build.Client;
    using TfsBuildManager.Repository;

    public class BuildAgentViewModel : BuildResourceViewModel
    {
        public BuildAgentViewModel(IBuildAgent agent)
        {
            this.Name = "\t" + agent.Name;
            this.BuildDirectory = agent.BuildDirectory;
            this.Status = agent.Status.ToString();
            this.Enabled = agent.Enabled;
            this.Tags = agent.Tags;
            this.StatusMessage = agent.StatusMessage;
            this.Agent = agent;
            string url = agent.Url.ToString();
            url = url.Substring(url.LastIndexOf(@"/", StringComparison.OrdinalIgnoreCase) + 1, url.Length - url.LastIndexOf(@"/", StringComparison.OrdinalIgnoreCase) - 1);
            this.Id = Convert.ToInt32(url);
        }

        protected IBuildAgent Agent { get; set; }

        public override void OnRemove(ITfsClientRepository repository)
        {
            repository.RemoveBuildAgent(this.Agent);
        }

        public override void OnEnable(ITfsClientRepository repository)
        {
            repository.EnableAgent(this.Agent);
        }

        public override void OnDisable(ITfsClientRepository repository)
        {
            repository.DisableAgent(this.Agent);
        }
    }

    public class BuildControllerResourceViewModel : BuildResourceViewModel
    {
        public BuildControllerResourceViewModel(IBuildController controller)
        {
            this.Name = controller.Name;
            this.Status = controller.Status.ToString();
            this.Enabled = controller.Enabled;
            this.Tags = controller.Tags;
            this.StatusMessage = controller.StatusMessage;
            this.Controller = controller;
            string url = controller.Url.ToString();
            url = url.Substring(url.LastIndexOf(@"/", StringComparison.OrdinalIgnoreCase) + 1, url.Length - url.LastIndexOf(@"/", StringComparison.OrdinalIgnoreCase) - 1);
            this.Id = Convert.ToInt32(url);
        }

        private IBuildController Controller { get; set; }

        public override void OnRemove(ITfsClientRepository repository)
        {
            repository.RemoveBuildController(this.Controller);
        }

        public override void OnEnable(ITfsClientRepository repository)
        {
            repository.EnableController(this.Controller);
        }

        public override void OnDisable(ITfsClientRepository repository)
        {
            repository.DisableController(this.Controller);
        }
    }
}
