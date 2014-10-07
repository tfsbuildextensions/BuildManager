//-----------------------------------------------------------------------
// <copyright file="RetentionPolicyWnd.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.TeamFoundation.Build.Client;
    using TfsBuildManager.Repository;

    /// <summary>
    /// Interaction logic for DeleteOptionsWnd
    /// </summary>
    public partial class RetentionPolicyWindow
    {
        public RetentionPolicyWindow()
        {
            this.InitializeComponent();
        }

        public BuildRetentionPolicy BuildRetentionPolicy { get; set; }

        private static void SetPolicies(ComboBox item, string keepValue, ref DeleteOptions options, ref int keep)
        {
            bool result = int.TryParse(keepValue, out keep);
            if (!result) 
            {
                // 2147483647 == keep all
                keep = 2147483647;
            }

            string tag = ((ComboBoxItem)item.SelectedItem).Tag.ToString();
            options = SetDeleteOptions(tag);
        }

        private static DeleteOptions SetDeleteOptions(string tag)
        {
            DeleteOptions options = DeleteOptions.None;
            switch (tag)
            {
                case "0":
                    options = DeleteOptions.Details | DeleteOptions.DropLocation | DeleteOptions.Label | DeleteOptions.Symbols;
                    break;
                case "1":
                    options = DeleteOptions.All;
                    break;
                case "2":
                    options = DeleteOptions.Details | DeleteOptions.DropLocation | DeleteOptions.TestResults;
                    break;
                case "3":
                    options = DeleteOptions.Details | DeleteOptions.DropLocation | DeleteOptions.Label;
                    break;
                case "4":
                    options = DeleteOptions.Details | DeleteOptions.DropLocation;
                    break;
                case "5":
                    options = DeleteOptions.Details;
                    break;
            }

            return options;
        }

        private static bool AreAllValidNumericChars(string str)
        {
            return str.All(char.IsNumber);
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            var p = new BuildRetentionPolicy();

            SetPolicies(this.StoppedWhatToDelete, this.StoppedKeep.Text, ref p.StoppedDeleteOptions, ref p.StoppedKeep);
            SetPolicies(this.FailedWhatToDelete, this.FailedKeep.Text, ref p.FailedDeleteOptions, ref p.FailedKeep);
            SetPolicies(this.PartiallySucceededWhatToDelete, this.PartiallySucceededKeep.Text, ref p.PartiallySucceededDeleteOptions, ref p.PartiallySucceededKeep);
            SetPolicies(this.SuceededWhatToDelete, this.SucceededKeep.Text, ref p.SucceededDeleteOptions, ref p.SucceededKeep);

            this.BuildRetentionPolicy = p;
            DialogResult = true;
            this.Close();
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
        } 
    }
}
