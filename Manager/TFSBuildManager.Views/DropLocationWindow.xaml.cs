//-----------------------------------------------------------------------
// <copyright file="DropLocationWindow.xaml.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System.Collections.Generic;
    using System.Windows;
    using TfsBuildManager.Views.ViewModels;

    /// <summary>
    /// Interaction logic for DropLocationWindow
    /// </summary>
    public partial class DropLocationWindow
    {
        private readonly DropLocationViewModel model;

        public DropLocationWindow(DropLocationViewModel model)
        {
            this.model = model;
            this.InitializeComponent();
            this.DataContext = this.model;
        }
   
        private void OnOK(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void lstMacros_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string s = ((KeyValuePair<string, string>)lstMacros.SelectedItem).Key;
            IDataObject old = Clipboard.GetDataObject();
            Clipboard.SetText(s);
            txtSetDropLocation.BeginChange();
            txtSetDropLocation.Paste();
            txtSetDropLocation.EndChange();
            if (old != null)
            {
                Clipboard.SetDataObject(old);
                txtSetDropLocation.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
