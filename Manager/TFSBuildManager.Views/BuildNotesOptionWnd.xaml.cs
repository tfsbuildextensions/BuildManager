//-----------------------------------------------------------------------
// <copyright file="BuildNotesOptionWnd.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using TfsBuildManager.Repository;

    /// <summary>
    /// Interaction logic for BuildNotesOptionWnd
    /// </summary>
    public partial class BuildNotesOptionWnd
    {
        public BuildNotesOptionWnd()
        {
            this.InitializeComponent();
        }
        
        public IEnumerable<string> Option { get; set; }

        private static void SetOption(ToggleButton cb, ref List<string> currentOption, string option)
        {
            if (cb.IsChecked.HasValue && cb.IsChecked.Value)
            {
                currentOption.Add(option);
            }
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            var options = new List<string>(); 
            SetOption(this.cbWorkItems, ref options, BuildNoteOptions.WorkItemDetails.ToString());
            SetOption(this.cbTestResults, ref options, BuildNoteOptions.TestResults.ToString());
            SetOption(this.cbChangesets, ref options, BuildNoteOptions.ChangesetDetails.ToString());
            SetOption(this.cbBuildConfiguration, ref options, BuildNoteOptions.BuildConfigurationSummary.ToString());
 
            this.Option = options;
            DialogResult = true;
            Close();
        }
    }
}