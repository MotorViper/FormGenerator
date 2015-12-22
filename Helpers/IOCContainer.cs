using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Helpers
{
    /// <summary>
    /// Simple IOC container.
    /// 
    /// Usage:
#pragma warning disable 1570
    /// <example>
    ///     IOCContainer.Instance.Register<IInterface1, Type1>();
    ///     IOCContainer.Instance.Register<IInterface2, Type2>().AsSingleton();
    ///     IOCContainer.Instance.Register<IInterface3, Type3a>("a");
    ///     IOCContainer.Instance.Register<IInterface3, Type3b>("b");
    ///     IOCContainer.Instance.Register<IInterface4, Type4>().WithDependancy<IInterface3>("a");
    ///     
    ///
    ///     IType1 t1 = IOCContainer.Instance.Resolve<IInterface1>();
    ///     IType2 t2a = IOCContainer.Instance.Resolve<IInterface2>();
    ///     IType2 t2b = IOCContainer.Instance.Resolve<IInterface2>();
    ///     // t2a will be the same instance as t2b
    ///     IType3 t3a = IOCContainer.Instance.Resolve<IInterface3>("a");
    ///     // t3a will be of type Type3a
    ///     IType3 t3b = IOCContainer.Instance.Resolve<IInterface3>("b");
    ///     // t3b will be of type Type3b
    ///     IType4 t4 = IOCContainer.Instance.Resolve<IInterface4>();
    ///     // The first constructor's parameters of type IInterface3 will have values of type Type3a
    /// </example>
#pragma warning restore 1570
    /// </summary>
    public class IOCContainer
    {
        private static readonly Lazy<IOCContainer> s_instance = new Lazy<IOCContainer>(() => new IOCContainer());
        private readonly Dictionary<Key, Func<object>> _components = new Dictionary<Key, Func<object>>();
        private readonly object _lock = new object();

        /// <summary>
        ///     Private constructor to force use only through Instance.
        /// </summary>
        private IOCContainer()
        {
        }

        /// <summary>
        ///     The number of registered components.
        /// </summary>
        public int ComponentCount => _components.Count;

        /// <summary>
        ///     Single instance of the IOC container.
        /// </summary>
        public static IOCContainer Instance => s_instance.Value;

        /// <summary>
        ///     Resets the container - mostly used for testing purposes.
        /// </summary>
        public void Reset()
        {
            lock (_lock)
                _components.Clear();
        }

        /// <summary>
        ///     Registers a class that implements an interface.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <returns>The dependency manager so that attributes can be added.</returns>
        public DependencyManager Register<TInterface, TClass>() where TClass : TInterface
        {
            return Register<TInterface, TClass>("");
        }

        /// <summary>
        ///     Registers a named instance of a class that implements an interface.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <param name="name">The named instance.</param>
        /// <returns>The dependency manager so that attributes can be added.</returns>
        public DependencyManager Register<TInterface, TClass>(string name) where TClass : TInterface
        {
            return new DependencyManager(new Key<TInterface>(name), typeof(TClass));
        }

        /// <summary>
        ///     Registers a named instance of a class that implements an interface.
        /// </summary>
        /// <param name="name">The object name.</param>
        /// <param name="type">The type of the object.</param>
        public DependencyManager Register<TInterface>(string name, Type type)
        {
            return new DependencyManager(new Key<TInterface>(name), type);
        }

        /// <summary>
        ///     Registers a named object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="name">The object name.</param>
        /// <param name="value">The object value.</param>
        public void Register<T>(string name, T value)
        {
            lock (_lock)
                _components[new Key<T>(name)] = () => value;
        }

        /// <summary>
        ///     Registers a default object.
        ///     N.B. Any registered component whose constructor takes an object of the given type will use
        ///     this value unless specifically set, so take care.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="value">The object value.</param>
        public void Register<T>(T value)
        {
            Register("", value);
        }

        /// <summary>
        ///     Finds the class that implements an interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <returns>An instance of a class that implements the interface.</returns>
        public T Resolve<T>()
        {
            return Resolve<T>("");
        }

        /// <summary>
        ///     Finds the class that implements a named instance of an interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="name">The name of the instance.</param>
        /// <param name="throwIfNotFound">If true and the item has not been registered then throw an exception.</param>
        /// <returns>The class that implements the interface that was associated with the name.</returns>
        public T Resolve<T>(string name, bool throwIfNotFound = false)
        {
            Func<object> obj;
            bool found;
            lock (_lock)
                found = _components.TryGetValue(new Key<T>(name), out obj);
            if (found)
                return (T)obj();
            if (throwIfNotFound)
                throw string.IsNullOrEmpty(name)
                    ? new InvalidDataException($"No unnamed data of type {typeof(T).Name} found.")
                    : new InvalidDataException($"No data of type {typeof(T).Name} with name {name} found.");
            return default(T);
        }

        /// <summary>
        ///     Finds the class that implements an interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <returns>An lazy instance of a class that implements the interface.</returns>
        public Lazy<T> LazyResolve<T>() where T : class
        {
            return LazyResolve<T>("");
        }

        /// <summary>
        ///     Finds the class that implements a named instance of an interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="name">The name of the instance.</param>
        /// <returns>A lazy instance of the class that implements the interface that was associated with the name.</returns>
        public Lazy<T> LazyResolve<T>(string name) where T : class
        {
            return new Lazy<T>(() => Resolve<T>(name));
        }

        #region Nested type: DependencyManager

        /// <summary>
        ///     Class that handles the registration and resolution of IOC objects.
        /// </summary>
        public class DependencyManager
        {
            private readonly Dictionary<Key, Func<object>> _args;
            private readonly Key _key;

            /// <summary>
            ///     Constructor.
            /// </summary>
            /// <param name="key">The key for storing the object.</param>
            /// <param name="type">The type of the class being registered.</param>
            internal DependencyManager(Key key, Type type)
            {
                _key = key;

                IOCContainer container = Instance;
                ConstructorInfo c = type.GetConstructors().First();
                _args = c.GetParameters().ToDictionary<ParameterInfo, Key, Func<object>>
                    (
                        x => new Key(x.Name, x.ParameterType),
                        x => (() =>
                        {
                            Key k = new Key(x.Name, x.ParameterType);
                            lock (container._lock)
                            {
                                Key componentKey = container._components.ContainsKey(k) ? k : new Key("", x.ParameterType);
                                try
                                {
                                    return container._components[componentKey]();
                                }
                                catch (KeyNotFoundException)
                                {
                                    throw new MissingIOCKeyException(componentKey);
                                }
                            }
                        })
                    );

                lock (container._lock)
                    container._components[key] = () => c.Invoke(_args.Values.Select(x => x()).ToArray());
            }

            /// <summary>
            ///     Causes all instances to resolve to the same object.
            /// </summary>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager AsSingleton()
            {
                object value = null;
                IOCContainer container = Instance;
                lock (container._lock)
                {
                    try
                    {
                        Func<object> service = container._components[_key];
                        container._components[_key] = () => value ?? (value = service());
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new MissingIOCKeyException(_key);
                    }
                }
                return this;
            }

            /// <summary>
            ///     Finds the key for the parameter that best matches the input.
            /// </summary>
            /// <typeparam name="T">The parameter type.</typeparam>
            /// <returns>The first key of the correct type.</returns>
            private Key FindKey<T>()
            {
                Type type = typeof(T);
                return _args.Keys.FirstOrDefault(k => k.Type == type);
            }

            /// <summary>
            ///     Finds the key for the parameter that best matches the input.
            /// </summary>
            /// <param name="parameter">The parameter name.</param>
            /// <returns>The first key with the correct name.</returns>
            private Key FindKey(string parameter)
            {
                return _args.Keys.FirstOrDefault(k => k.Name == parameter);
            }

            /// <summary>
            ///     Links a parameter of the resolved objects constructor to another IOC object.
            ///     This version is used where the parameter type may change.
            /// </summary>
            /// <param name="parameter">The name of the constructor parameter.</param>
            /// <param name="component">The name of the IOC object.</param>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager WithDependency(string parameter, string component)
            {
                Key key = FindKey(parameter);
                _args[key] = () =>
                {
                    Key componentKey = new Key(component, key.Type);
                    return FindComponent(componentKey);
                };
                return this;
            }

            private static object FindComponent(Key componentKey)
            {
                IOCContainer container = Instance;
                lock (container._lock)
                {
                    try
                    {
                        return container._components[componentKey]();
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new MissingIOCKeyException(componentKey);
                    }
                }
            }

            /// <summary>
            ///     Links a parameter of the resolved objects constructor to another IOC object.
            ///     This is the most restrictive and therefore safest version.
            /// </summary>
            /// <typeparam name="TInterface">The parameter type.</typeparam>
            /// <param name="parameter">The name of the constructor parameter.</param>
            /// <param name="component">The name of the IOC object.</param>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager WithDependency<TInterface>(string parameter, string component)
            {
                _args[new Key<TInterface>(parameter)] = () =>
                {
                    Key key = new Key<TInterface>(component);
                    return FindComponent(key);
                };
                return this;
            }

            /// <summary>
            ///     Links a parameter of the resolved objects constructor to another IOC object.
            ///     This is used where all parameters of the given type have the same value and is useful if the parameter name is unknown.
            /// </summary>
            /// <typeparam name="TInterface">The parameter type.</typeparam>
            /// <param name="component">The name of the IOC object.</param>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager WithDependency<TInterface>(string component)
            {
                _args[FindKey<TInterface>()] = () =>
                {
                    Key key = new Key<TInterface>(component);
                    return FindComponent(key);
                };
                return this;
            }

            /// <summary>
            ///     Links a parameter of the resolved objects constructor to another IOC object.
            ///     This version is used where the parameter type may change.
            /// </summary>
            /// <param name="parameter">The name of the constructor parameter.</param>
            /// <param name="value">The value to set the parameter to.</param>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager WithValue(string parameter, object value)
            {
                _args[FindKey(parameter)] = () => value;
                return this;
            }

            /// <summary>
            ///     Links a parameter of the resolved objects constructor to another IOC object.
            ///     This is the most restrictive and therefore safest version.
            /// </summary>
            /// <typeparam name="TValue">The parameter type.</typeparam>
            /// <param name="parameter">The name of the constructor parameter.</param>
            /// <param name="value">The value to set the parameter to.</param>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager WithValue<TValue>(string parameter, TValue value)
            {
                _args[new Key<TValue>(parameter)] = () => value;
                return this;
            }

            /// <summary>
            ///     Links a parameter of the resolved objects constructor to another IOC object.
            ///     This is used where all parameters of the given type have the same value and is useful if the parameter name is unknown.
            /// </summary>
            /// <typeparam name="TValue">The parameter type.</typeparam>
            /// <param name="value">The value to set the parameter to.</param>
            /// <returns>This dependency manager so that more attributes can be added.</returns>
            public DependencyManager WithValue<TValue>(TValue value)
            {
                _args[FindKey<TValue>()] = () => value;
                return this;
            }
        }

        #endregion

        #region Nested type: Key

        /// <summary>
        ///     Key used to store objects that are to be retrieved.
        /// </summary>
        public class Key
        {
            /// <summary>
            ///     Constructor.
            /// </summary>
            /// <param name="name">The instance name for named objects.</param>
            /// <param name="type">The object type.</param>
            public Key(string name, Type type)
            {
                Name = name;
                Type = type;
            }

            /// <summary>
            ///     The instance name for named objects.
            /// </summary>
            public string Name { get; }

            /// <summary>
            ///     The object type.
            /// </summary>
            public Type Type { get; }

            /// <summary>
            ///     String version of object.
            /// </summary>
            /// <returns>A string representing the object.</returns>
            public override string ToString()
            {
                return Name + "." + Type.Name;
            }

            /// <summary>
            ///     The hash code - needed for use with Dictionary.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            /// <summary>
            ///     Comparison method.
            /// </summary>
            /// <param name="obj">The object to compare this to.</param>
            /// <returns>True if the input object is functionally equivalent to this one.</returns>
            public override bool Equals(object obj)
            {
                Key key = obj as Key;
                return key != null && ToString().Equals(key.ToString());
            }
        }

        /// <summary>
        ///     Key used to store objects that are to be retrieved.
        /// </summary>
        /// <typeparam name="T">The type the key is for.</typeparam>
        public class Key<T> : Key
        {
            /// <summary>
            ///     Constructor.
            /// </summary>
            /// <param name="name">The instance name for named objects.</param>
            public Key(string name)
                : base(name, typeof(T))
            {
            }
        }

        #endregion
    }

    /// <summary>
    ///     Exception thrown when an item to be resolved has not been registered.
    /// </summary>
    public class MissingIOCKeyException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="key">The key being used to look up the registered item.</param>
        public MissingIOCKeyException(IOCContainer.Key key) :
            base($"The class {key.Type.Name}{(string.IsNullOrWhiteSpace(key.Name) ? "" : "(" + key.Name + ")")} is missing from the IOC container.")
        {
        }
    }
}
