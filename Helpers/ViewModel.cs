using System.ComponentModel;

namespace Helpers
{
    /// <summary>
    ///     Base class for view models.
    /// </summary>
    public abstract class ViewModel : IViewModel
    {
        #region IViewModel Members

        /// <summary>
        ///     Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Method to send a property changed event.
        /// </summary>
        /// <param name="name">The name of the property that has been changed.</param>
        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
