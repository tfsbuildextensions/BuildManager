//-----------------------------------------------------------------------
// <copyright file="TriggerWindow.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Text.RegularExpressions;
    using System.Windows;
    using Microsoft.TeamFoundation.Build.Client;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for DropLocationWindow
    /// </summary>
    public partial class TriggerWindow
    {
        public TriggerWindow(TriggerViewModel trigger)
        {
            this.Trigger = trigger;
            this.InitializeComponent();
            this.DataContext = this.Trigger;
        }

        public TriggerViewModel Trigger { get; private set; }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.Trigger.Minutes = 0;
            this.Trigger.Submissions = 0;

            if (this.rdoTriggerManual.IsChecked.HasValue && this.rdoTriggerManual.IsChecked.Value)
            {
                this.Trigger.TriggerType = DefinitionTriggerType.None;
            }

            if (this.rdoTriggerContinuousIntegration.IsChecked.HasValue && this.rdoTriggerContinuousIntegration.IsChecked.Value)
            {
                this.Trigger.TriggerType = DefinitionTriggerType.ContinuousIntegration;
            }

            if (this.rdoTriggerRolling.IsChecked.HasValue && this.rdoTriggerRolling.IsChecked.Value)
            {
                this.Trigger.TriggerType = DefinitionTriggerType.BatchedContinuousIntegration;
                if (this.checkboxRolling.IsChecked.HasValue && this.checkboxRolling.IsChecked.Value)
                {
                    this.Trigger.Minutes = int.Parse(this.textboxMinutes.Text);
                }
            }

            if (this.rdoTriggerGated.IsChecked.HasValue && this.rdoTriggerGated.IsChecked.Value)
            {
                if (this.checkboxGated.IsChecked.HasValue && this.checkboxGated.IsChecked.Value)
                {
                    this.Trigger.TriggerType = DefinitionTriggerType.BatchedGatedCheckIn;
                    this.Trigger.Submissions = int.Parse(this.textboxSubmissions.Text);
                }
                else
                {
                    this.Trigger.TriggerType = DefinitionTriggerType.GatedCheckIn;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void rdoTriggerRolling_Checked(object sender, RoutedEventArgs e)
        {
            this.checkboxRolling.IsEnabled = true;
            this.labelMinutes.IsEnabled = true;
            this.textboxMinutes.IsEnabled = true;
        }

        private void rdoTriggerRolling_Unchecked(object sender, RoutedEventArgs e)
        {
            this.checkboxRolling.IsEnabled = false;
            this.labelMinutes.IsEnabled = false;
            this.textboxMinutes.IsEnabled = false;
        }

        private void rdoTriggerGated_Checked(object sender, RoutedEventArgs e)
        {
            this.checkboxGated.IsEnabled = true;
            this.labelSubmissions.IsEnabled = true;
            this.textboxSubmissions.IsEnabled = true;
        }

        private void rdoTriggerGated_Unchecked(object sender, RoutedEventArgs e)
        {
            this.checkboxGated.IsEnabled = false;
            this.labelSubmissions.IsEnabled = false;
            this.textboxSubmissions.IsEnabled = false;
        }

        private void textboxMinutes_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void textboxSubmissions_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
