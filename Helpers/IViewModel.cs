using System.ComponentModel;

namespace Helpers
{
    /// <summary>
    ///     Interface for view models.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     Method to send a property changed event.
        /// </summary>
        /// <param name="name">The name of the property that has been changed.</param>
        void OnPropertyChanged(string name);
    }
}
