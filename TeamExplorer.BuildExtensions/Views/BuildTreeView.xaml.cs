using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BuildTree.Sections;

namespace BuildTree.Views
{
    /// <summary>
    /// Interaction logic for BuildTreeView.xaml
    /// </summary>
    public partial class BuildTreeView : UserControl
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(BuildTreeSection), typeof(BuildTreeView));

        public BuildTreeSection ParentSection
        {
            get
            {
                return (BuildTreeSection)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
            }
        }

        public BuildTreeView()
        {
            InitializeComponent();
        }

        private void ViewBuilds_Click(object sender, RoutedEventArgs e)
        {
            ParentSection.ViewBuilds();
        }

        private void QueueNewBuild_Click(object sender, RoutedEventArgs e)
        {
            ParentSection.QueueNewBuild();
        }

        private void EditBuildDefinition_Click(object sender, RoutedEventArgs e)
        {
            ParentSection.EditBuildDefinition();
        }

        private void TreeViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                var viewModel = item.DataContext as BuildDefinitionViewModel;
                if (viewModel != null)
                {
                    ParentSection.SelectedBuildDefinition = viewModel;
                }

                item.Focus();

                if (item.ContextMenu != null)
                {
                    item.ContextMenu.PlacementTarget = item;
                    item.ContextMenu.IsOpen = true;
                }

                e.Handled = true;
            }
        }
    }
}
