//-----------------------------------------------------------------------
// <copyright file="DeleteOptionsWnd.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.TeamFoundation.Build.Client;

    /// <summary>
    /// Interaction logic for DeleteOptionsWnd
    /// </summary>
    public partial class DeleteOptionsWindow
    {
        public DeleteOptionsWindow()
        {
            this.InitializeComponent();
        }

        public DeleteOptions Option { get; private set; }

        private static void SetOption(CheckBox cb, ref DeleteOptions currentOption, DeleteOptions option)
        {
            if (cb.IsChecked != null && cb.IsChecked.Value)
            {
                currentOption |= option;
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteOptions options = new DeleteOptions();
            SetOption(this.cbDetails, ref options, DeleteOptions.Details);
            SetOption(this.cbDrop, ref options, DeleteOptions.DropLocation);
            SetOption(this.cbTestResults, ref options, DeleteOptions.TestResults);
            SetOption(this.cbLabel, ref options, DeleteOptions.Label);
            SetOption(this.cbSymbols, ref options, DeleteOptions.Symbols);

            this.Option = options;
            this.DialogResult = true;
            this.Close();
        }
    }
}
