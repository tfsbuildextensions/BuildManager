//-----------------------------------------------------------------------
// <copyright file="SortableListView.cs">(c) http://TfsBuildExtensions.codeplex.com/. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class DataBoundRadioButton : RadioButton
    {
        protected override void OnChecked(RoutedEventArgs e)
        {
            // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
        }

        protected override void OnToggle()
        {
            // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
        }
    }

    public class SortableListView : ListView
    {
        // Using a DependencyProperty as the backing store for ColumnHeaderSortedAscendingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderSortedAscendingTemplateProperty = DependencyProperty.Register("ColumnHeaderSortedAscendingTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for ColumnHeaderNotSortedTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderNotSortedTemplateProperty = DependencyProperty.Register("ColumnHeaderNotSortedTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for ColumnHeaderSortedDescendingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderSortedDescendingTemplateProperty = DependencyProperty.Register("ColumnHeaderSortedDescendingTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(string.Empty));
        private SortableGridViewColumn lastSortedOnColumn;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;

        static SortableListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SortableListView), new FrameworkPropertyMetadata(typeof(SortableListView)));
        }

        public string ColumnHeaderSortedDescendingTemplate
        {
            get { return (string)GetValue(ColumnHeaderSortedDescendingTemplateProperty); }
            set { this.SetValue(ColumnHeaderSortedDescendingTemplateProperty, value); }
        }

        public string ColumnHeaderSortedAscendingTemplate
        {
            get { return (string)GetValue(ColumnHeaderSortedAscendingTemplateProperty); }
            set { this.SetValue(ColumnHeaderSortedAscendingTemplateProperty, value); }
        }

        public string ColumnHeaderNotSortedTemplate
        {
            get { return (string)GetValue(ColumnHeaderNotSortedTemplateProperty); }
            set { this.SetValue(ColumnHeaderNotSortedTemplateProperty, value); }
        }

        internal static void ShowColumn(SortableGridViewColumn column, bool visible)
        {
            if (visible)
            {
                column.ClearValue(GridViewColumn.WidthProperty);
            }
            else
            {
                column.Width = 0.0;
            }
        }

        /// <summary>
        /// Executes when the control is initialized completely the first time through. Runs only once.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnInitialized(EventArgs e)
        {
            this.Loaded += this.OnLoaded;

            // add the event handler to the GridViewColumnHeader. This strongly ties this ListView to a GridView.
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(this.GridViewColumnHeaderClickedHandler));
            var defaultDirection = ListSortDirection.Ascending;

            // cast the ListView's View to a GridView
            GridView gridView = this.View as GridView;
            if (gridView != null)
            {
                // determine which column is marked as IsDefaultSortColumn. Stops on the first column marked this way.
                SortableGridViewColumn sortableGridViewColumn = null;
                foreach (GridViewColumn gridViewColumn in gridView.Columns)
                {
                    sortableGridViewColumn = gridViewColumn as SortableGridViewColumn;
                    if (sortableGridViewColumn != null)
                    {
                        if (sortableGridViewColumn.IsDefaultSortColumn)
                        {
                            defaultDirection = sortableGridViewColumn.DefaultSortDirection;
                            break;
                        }

                        sortableGridViewColumn = null;
                    }
                }

                // if the default sort column is defined, sort the data and then update the templates as necessary.
                if (sortableGridViewColumn != null)
                {
                    this.lastSortedOnColumn = sortableGridViewColumn;
                    this.Sort(sortableGridViewColumn, defaultDirection);

                    if (!string.IsNullOrEmpty(this.ColumnHeaderSortedAscendingTemplate))
                    {
                        sortableGridViewColumn.HeaderTemplate = this.TryFindResource(this.ColumnHeaderSortedAscendingTemplate) as DataTemplate;
                    }
                }
            }

            base.OnInitialized(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.lastSortedOnColumn == null)
            {
                return;
            }

            this.Sort(this.lastSortedOnColumn, this.lastDirection);

            if (!string.IsNullOrEmpty(this.ColumnHeaderSortedAscendingTemplate))
            {
                this.lastSortedOnColumn.HeaderTemplate = this.TryFindResource(this.ColumnHeaderSortedAscendingTemplate) as DataTemplate;
            }
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
           
            // ensure that we clicked on the column header and not the padding that's added to fill the space.
            if (headerClicked == null || headerClicked.Role == GridViewColumnHeaderRole.Padding)
            {
                return;
            }

            // attempt to cast to the sortableGridViewColumn object.
            SortableGridViewColumn sortableGridViewColumn = headerClicked.Column as SortableGridViewColumn;
            
            // ensure that the column header is the correct type and a sort property has been set.
            if (sortableGridViewColumn == null || string.IsNullOrEmpty(sortableGridViewColumn.SortPropertyName))
            {
                return;
            }

            ListSortDirection direction = ListSortDirection.Ascending;

            // determine if this is a new sort, 
            bool newSortColumn = this.lastSortedOnColumn == null || string.IsNullOrEmpty(this.lastSortedOnColumn.SortPropertyName) || !string.Equals(sortableGridViewColumn.SortPropertyName, this.lastSortedOnColumn.SortPropertyName, StringComparison.OrdinalIgnoreCase);

            // if not switch sort direction.
            if (!newSortColumn)
            {
                direction = (this.lastDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            // Sort the data.
            this.Sort(sortableGridViewColumn, direction);

            string template = (direction == ListSortDirection.Ascending) ? this.ColumnHeaderSortedAscendingTemplate : this.ColumnHeaderSortedDescendingTemplate;
            sortableGridViewColumn.HeaderTemplate = !string.IsNullOrEmpty(template) ? this.TryFindResource(template) as DataTemplate : null;

            // Remove arrow from previously sorted header
            if (newSortColumn && this.lastSortedOnColumn != null)
            {
                this.lastSortedOnColumn.HeaderTemplate = !string.IsNullOrEmpty(this.ColumnHeaderNotSortedTemplate) ? this.TryFindResource(this.ColumnHeaderNotSortedTemplate) as DataTemplate : null;
            }

            this.lastSortedOnColumn = sortableGridViewColumn;
        }

        private void Sort(SortableGridViewColumn sortableGridViewColumn, ListSortDirection direction)
        {
            this.lastDirection = direction;

            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            if (dataView == null)
            {
                return;
            }

            dataView.SortDescriptions.Clear();

            // get the sort property name from the column's information, add it as the first parameter 
            SortDescription sd = new SortDescription(sortableGridViewColumn.SortPropertyName, direction);

            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }

    public class SortableGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty SortPropertyNameProperty = DependencyProperty.Register("SortPropertyName", typeof(string), typeof(SortableGridViewColumn), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsDefaultSortColumnProperty = DependencyProperty.Register("IsDefaultSortColumn", typeof(bool), typeof(SortableGridViewColumn), new UIPropertyMetadata(false));

        public static readonly DependencyProperty DefaultSortDirectionProperty = DependencyProperty.Register("DefaultSortDirection", typeof(ListSortDirection), typeof(SortableGridViewColumn), new UIPropertyMetadata(ListSortDirection.Ascending));

        public static readonly DependencyProperty ColumnIdProperty = DependencyProperty.Register("ColumnId", typeof(string), typeof(SortableGridViewColumn));

        public static readonly DependencyProperty FilterEnabledProperty = DependencyProperty.Register("FilterEnabled", typeof(bool), typeof(SortableGridViewColumn), new UIPropertyMetadata(false));

        public static readonly DependencyProperty HasNoMarginProperty = DependencyProperty.Register("HasNoMargin", typeof(bool), typeof(SortableGridViewColumn), new UIPropertyMetadata(false));

        public string SortPropertyName
        {
            get { return (string)GetValue(SortPropertyNameProperty); }
            set { this.SetValue(SortPropertyNameProperty, value); }
        }

        public bool IsDefaultSortColumn
        {
            get { return (bool)GetValue(IsDefaultSortColumnProperty); }
            set { this.SetValue(IsDefaultSortColumnProperty, value); }
        }

        public ListSortDirection DefaultSortDirection
        {
            get { return (ListSortDirection)GetValue(DefaultSortDirectionProperty); }
            set { this.SetValue(DefaultSortDirectionProperty, value); }
        }

        public string ColumnId
        {
            get { return (string)GetValue(ColumnIdProperty); }
            set { this.SetValue(ColumnIdProperty, value); }
        }

        public bool FilterEnabled
        {
            get { return (bool)GetValue(FilterEnabledProperty); }
            set { this.SetValue(FilterEnabledProperty, value); }
        }

        public bool HasNoMargins
        {
            get { return (bool)GetValue(HasNoMarginProperty); }
            set { this.SetValue(HasNoMarginProperty, value); }
        }

        /// <summary>
        /// Column visibility.
        /// Note: Using Width = 0,0 to when visible is set to false.
        /// When set to visible width is set to double.NaN which sets the
        /// width of the column to auto adjust width to content.
        /// ToDo: Use standard Visibility-property of UIElements instead of
        /// this column-width hack.
        /// </summary>
        public bool Visible
        {
            get
            {
                var w = this.Width;
                return double.IsNaN(w) || (w > 0.0);
            }

            set
            {
                if (value)
                {
                    this.ClearValue(GridViewColumn.WidthProperty);
                }
                else
                {
                    this.Width = 0.0;
                }
            }
        }
    }
}
