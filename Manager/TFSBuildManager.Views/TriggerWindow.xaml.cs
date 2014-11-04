//-----------------------------------------------------------------------
// <copyright file="TriggerWindow.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Text.RegularExpressions;
    using System.Windows;
    using Microsoft.TeamFoundation.Build.Client;
    using TfsBuildManager.Views.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Text;


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
            this.SetTimeZoneInfo();
        }

        private TimeZoneInfo _timeZoneInfo;

        private void SetTimeZoneInfo()
        {
            _timeZoneInfo = TimeZoneInfo.Local;


            lblTimeZone.Content = GetTimeZoneLabel();

            ScheduleTimes().ForEach(x => cboScheduleTime.Items.Add(x));
        }

        private string GetTimeZoneLabel()
        {
            StringBuilder content = new StringBuilder();

            if (_timeZoneInfo.IsDaylightSavingTime(DateTime.Now))
                content.Append(_timeZoneInfo.DaylightName);
            else
                content.Append(_timeZoneInfo.StandardName);

            content.Append(" (UTC ");

            if (_timeZoneInfo.GetUtcOffset(DateTime.Now).Hours < 0)
            {
                content.Append("-");
            }
            else if (_timeZoneInfo.GetUtcOffset(DateTime.Now).Hours > 0)
            {
                content.Append("+");
            }

            content.Append(_timeZoneInfo.GetUtcOffset(DateTime.Now).ToString("hh':'mm")).Append(")");

            return content.ToString();
        }

        private List<string> ScheduleTimes()
        {
            var items = new List<string>();
            
            TimeSpan time = new TimeSpan(0, 0, 0);
            TimeSpan interval = new TimeSpan(0,30,0);
            do
            {
                items.Add(string.Format("{0}:{1}", time.Hours, time.Minutes.ToString().PadRight(2,'0')));
                time += interval;
            } 
            while (time.TotalHours <= 23);

            return items;
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

            if (this.rdoTriggerSchedule.IsChecked.HasValue && this.rdoTriggerSchedule.IsChecked.Value)
            {
                if (this.checkboxForceBuild.IsChecked.HasValue && checkboxForceBuild.IsChecked.Value)
                {
                    this.Trigger.TriggerType = DefinitionTriggerType.ScheduleForced;
                }
                else
                {
                    this.Trigger.TriggerType = DefinitionTriggerType.Schedule;
                }

                this.Trigger.ScheduleDays = GetSelectedDays();
                this.Trigger.ScheduleTime = DateTime.Parse(cboScheduleTime.SelectedValue.ToString());
                this.Trigger.TimeZoneInfo = _timeZoneInfo;
            }

            this.DialogResult = true;
            this.Close();
        }

        private ScheduleDays GetSelectedDays()
        {
            ScheduleDays scheduleDays = ScheduleDays.None;
            if (IsChecked(checkboxMonday))
                scheduleDays |= ScheduleDays.Monday;
            if (IsChecked(checkboxTuesday))
                scheduleDays |= ScheduleDays.Tuesday;
            if (IsChecked(checkboxWednesday))
                scheduleDays |= ScheduleDays.Wednesday;
            if (IsChecked(checkboxThursday))
                scheduleDays |= ScheduleDays.Thursday;
            if (IsChecked(checkboxFriday))      
                scheduleDays |= ScheduleDays.Friday;
            if (IsChecked(checkboxSaturday))
                scheduleDays |= ScheduleDays.Saturday;
            if (IsChecked(checkboxSunday))
                scheduleDays |= ScheduleDays.Sunday;
            return scheduleDays;
        }

        private bool IsChecked(System.Windows.Controls.CheckBox checkbox)
        {
            return checkbox.IsChecked.HasValue && checkbox.IsChecked.Value;
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

        private void rdoTriggerSchedule_Checked(object sender, RoutedEventArgs e)
        {
            TriggerScheduleEnabledState(true);
        }

        private void rdoTriggerSchedule_Unchecked(object sender, RoutedEventArgs e)
        {
            TriggerScheduleEnabledState(false);
        }

        private void TriggerScheduleEnabledState(bool enabled)
        {
            checkboxMonday.IsEnabled     = enabled;
            checkboxTuesday.IsEnabled    = enabled;
            checkboxWednesday.IsEnabled  = enabled;
            checkboxThursday.IsEnabled   = enabled;
            checkboxFriday.IsEnabled     = enabled;
            checkboxSaturday.IsEnabled   = enabled;
            checkboxSunday.IsEnabled     = enabled;
            cboScheduleTime.IsEnabled    = enabled;
            checkboxForceBuild.IsEnabled = enabled;
        }
    }
}
