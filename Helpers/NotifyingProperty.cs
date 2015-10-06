using System;
using System.Runtime.CompilerServices;

namespace Helpers
{
    /// <summary>
    ///     Class to simplify use of INotifyPropertyChanged on properties.
    /// </summary>
    public class NotifyingProperty<TValueType, TViewModel> where TViewModel : class, IViewModel
    {
        // ReSharper disable MemberCanBeProtected.Global
        private readonly Action<TViewModel, TValueType> _updateAction;
        private Action<TViewModel> _initialAction;
        private TValueType _value;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="initialValue">The initial value if it is not the default.</param>
        /// <param name="updateAction">Action to be performed when the value changes.</param>
        /// <param name="initialAction">Action to be performed when the value is first set.</param>
        public NotifyingProperty(TValueType initialValue = default(TValueType),
            Action<TViewModel, TValueType> updateAction = null, Action<TViewModel> initialAction = null)
        {
            _value = initialValue;
            _updateAction = updateAction;
            _initialAction = initialAction;
        }

        /// <summary>
        ///     Set the value of the property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parent">The view model parent.</param>
        /// <param name="name">The property name.</param>
        public virtual void SetValue(TValueType value, TViewModel parent, [CallerMemberName] string name = null)
        {
            if (_initialAction != null)
            {
                _initialAction(parent);
                _initialAction = null;
            }
            _value = value;
            parent?.OnPropertyChanged(name);
            _updateAction?.Invoke(parent, value);
        }

        /// <summary>
        ///     Get the value of the property.
        /// </summary>
        public TValueType GetValue()
        {
            return _value;
        }

        // ReSharper restore MemberCanBeProtected.Global
    }

    /// <summary>
    ///     Specialised version of NotifyingProperty.
    ///     This makes instances where the actual type of the view model is not important tidier.
    /// </summary>
    public class NotifyingProperty<TValueType> : NotifyingProperty<TValueType, IViewModel>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="initialValue">The initial value if it is not the default.</param>
        /// <param name="updateAction">Action to be performed when the value changes.</param>
        /// <param name="initialAction">Action to be performed when the value is first set.</param>
        public NotifyingProperty(TValueType initialValue = default(TValueType), Action<IViewModel, TValueType> updateAction = null,
            Action<IViewModel> initialAction = null)
            : base(initialValue, updateAction, initialAction)
        {
        }
    }
}
