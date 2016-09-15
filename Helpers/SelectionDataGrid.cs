using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Helpers
{
    /// <summary>
    /// </summary>
    public class SelectionDataGrid : DataGrid
    {
        static SelectionDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionDataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
        }

        #region DependencyProperty Selection

        /// <summary>
        ///     Registers a dependency property as backing store for the Selection property
        /// </summary>
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register("Selection", typeof(IEnumerable), typeof(SelectionDataGrid), new PropertyMetadata(new List<object>()));

        /// <summary>
        ///     Gets or sets the Selection.
        /// </summary>
        /// <value>The Selection.</value>
        public IList Selection
        {
            get { return (IList)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }

        #endregion

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            foreach (object removedItem in e.RemovedItems)
                Selection.Remove(removedItem);
            foreach (object addedItem in e.AddedItems)
                Selection.Add(addedItem);
        }
    }
}
