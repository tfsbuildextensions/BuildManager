//-----------------------------------------------------------------------
// <copyright file="ProcessParameterWindow.xaml.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ProcessParameterWindow
    /// </summary>
    public partial class ProcessParameterWindow
    {
        public string[] ProcessParameter;

        public bool BooleanType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessParameterWindow"/> class. 
        /// </summary>
        public ProcessParameterWindow()
        {
            InitializeComponent();
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TextBoxParameterName.Text) || string.IsNullOrWhiteSpace(this.TextBoxParameterValue.Text))
            {
                MessageBox.Show("Parameter Name and Required Value must be provided", "Community TFS Build Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!(this.RadioButtonBoolean.IsChecked.HasValue && this.RadioButtonBoolean.IsChecked.Value) && !(this.RadioButtonString.IsChecked.HasValue && this.RadioButtonString.IsChecked.Value))
            {
                MessageBox.Show("Please set whether this is a string or boolean parameter", "Community TFS Build Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.RadioButtonBoolean.IsChecked.HasValue && this.RadioButtonBoolean.IsChecked.Value)
            {
                this.BooleanType = true;
            }
            else
            {
                this.BooleanType = false;
            }

            this.ProcessParameter = new string[2];
            this.ProcessParameter[0] = this.TextBoxParameterName.Text;
            this.ProcessParameter[1] = this.TextBoxParameterValue.Text;
            DialogResult = true;
            this.Close();
        }
    }
}
