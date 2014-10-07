//-----------------------------------------------------------------------
// <copyright file="BuildResourceViewModel.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TfsBuildManager.Repository;

    public abstract class BuildResourceViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public int Id { get; set; }
        
        public string Status { get; set; }

        public bool Enabled { get; set; }
        
        public string IsReserved { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string QueueCount { get; set; }

        public string MaxConcurrentBuilds { get; set; }

        public string CustomAssemblyPath { get; set; }

        public string EnabledText
        {
            get { return this.Enabled ? "Enabled" : "Disabled"; }
        }

        public string StatusMessage { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string TagsAsList        
        {
            get
            {
                if (this.Tags != null)
                {
                    string tags = this.Tags.Aggregate(string.Empty, (current, tag) => current + tag + ",");
                    return tags.TrimEnd(',');
                }

                return string.Empty;
            }
        }

        public string BuildDirectory { get; set; }

        public abstract void OnRemove(ITfsClientRepository repository);

        public abstract void OnEnable(ITfsClientRepository repository);

        public abstract void OnDisable(ITfsClientRepository repository);
    }
}
