//-----------------------------------------------------------------------
// <copyright file="BuildsGrid.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    /// <summary>
    /// Interaction logic for BuildsGrid
    /// </summary>
    public partial class BuildsGrid
    {
        public BuildsGrid()
        {
            this.InitializeComponent();
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = this.DataContext as BuildManagerViewModel;
            if (vm.SelectedBuildFilter == BuildFilter.Completed)
            {
                vm.OnShowDetails();
            }
            else if (vm.SelectedBuildFilter == BuildFilter.Queued)
            {
                vm.OnShowQueuedDetails();
            }
        }
    }
}
