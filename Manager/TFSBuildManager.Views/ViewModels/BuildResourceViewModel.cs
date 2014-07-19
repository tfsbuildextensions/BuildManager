//-----------------------------------------------------------------------
// <copyright file="BuildResourceViewModel.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using TfsBuildManager.Repository;

    public abstract class BuildResourceViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public int Id { get; set; }
        
        public string Status { get; set; }

        public bool Enabled { get; set; }

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
                string tags = this.Tags.Aggregate(string.Empty, (current, tag) => current + tag + ",");
                return tags.TrimEnd(',');
            }
        }

        public string BuildDirectory { get; set; }

        public abstract void OnRemove(ITfsClientRepository repository);

        public abstract void OnEnable(ITfsClientRepository repository);

        public abstract void OnDisable(ITfsClientRepository repository);
    }
}
