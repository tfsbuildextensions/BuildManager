using System.Collections.ObjectModel;
using BuildTree.Views;

namespace BuildTree.Sections
{
    internal class BuildsSectionContext
    {
        public ObservableCollection<BuildDefinitionViewModel> Builds { get; set; }
    }
}
