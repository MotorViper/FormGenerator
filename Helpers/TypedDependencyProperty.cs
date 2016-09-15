using System;
using System.Windows;

namespace Helpers
{
    // ReSharper disable UnusedMember.Global

    /// <summary>
    ///     Extends DependencyProperty to make it type safe.
    /// </summary>
    /// <typeparam name="TDataType">The property type.</typeparam>
    public class TypedDependencyProperty<TDataType>
    {
        private readonly DependencyProperty _dependencyProperty;

        public TypedDependencyProperty(DependencyProperty dp)
        {
            _dependencyProperty = dp;
        }

        public static TypedDependencyProperty<TDataType> Register(string name, Type parentType, TDataType defaultValue = default(TDataType))
        {
            DependencyProperty dp = DependencyProperty.Register(name, typeof(TDataType), parentType,
                new PropertyMetadata(defaultValue));
            return new TypedDependencyProperty<TDataType>(dp);
        }

        public static TypedDependencyProperty<TDataType> RegisterAttached(string name, Type parentType, TDataType defaultValue,
            PropertyChangedCallback callback)
        {
            DependencyProperty dp = DependencyProperty.RegisterAttached(name, typeof(TDataType), parentType,
                new PropertyMetadata(defaultValue, callback));
            return new TypedDependencyProperty<TDataType>(dp);
        }

        public static TypedDependencyProperty<TDataType> RegisterAttached(string name, Type parentType, TDataType defaultValue = default(TDataType))
        {
            DependencyProperty dp = DependencyProperty.RegisterAttached(name, typeof(TDataType), parentType, new PropertyMetadata(defaultValue));
            return new TypedDependencyProperty<TDataType>(dp);
        }

        public static TypedDependencyProperty<TDataType> RegisterAttached<TParentType>(string name, TDataType defaultValue,
            PropertyChangedCallback callback)
        {
            return RegisterAttached(name, typeof(TParentType), defaultValue, callback);
        }

        public static TypedDependencyProperty<TDataType> RegisterAttached<TParentType>(string name, TDataType defaultValue = default(TDataType))
        {
            return RegisterAttached(name, typeof(TParentType), defaultValue);
        }

        public TDataType GetValue(DependencyObject dp)
        {
            return (TDataType)dp.GetValue(_dependencyProperty);
        }

        public void SetValue(DependencyObject dp, TDataType value)
        {
            dp.SetValue(_dependencyProperty, value);
        }
    }

    // ReSharper restore UnusedMember.Global
}
