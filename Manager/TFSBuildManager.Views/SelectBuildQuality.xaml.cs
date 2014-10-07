//-----------------------------------------------------------------------
// <copyright file="SelectBuildQuality.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Windows;

    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for SelectTeamProject
    /// </summary>
    public partial class SelectBuildQuality
    {
        private readonly BuildQualityListViewModel model;

        public SelectBuildQuality(BuildQualityListViewModel model)
        {
            this.model = model;
            this.InitializeComponent();

            this.Grid1.DataContext = this.model;
        }

        public BuildQualityViewModel SelectedBuildQuality { get; set; }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.SetBuildQuality();
        }

        private void SetBuildQuality()
        {
            if (this.BuildQualityList.SelectedItem != null)
            {
                this.SelectedBuildQuality = this.BuildQualityList.SelectedItem as BuildQualityViewModel;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetBuildQuality();
        }
    }
}
