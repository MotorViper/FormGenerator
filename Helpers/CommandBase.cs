using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Helpers
{
    /// <summary>
    ///     Base class for ICommand implementations.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        private readonly Predicate<object> _canExecute;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="canExecute">The method to use to check if the command can be activated. If null this is always true.</param>
        protected CommandBase(Predicate<object> canExecute)
        {
            _canExecute = canExecute ?? (x => true);
        }

        #region ICommand Members

        /// <summary>
        ///     Called to find out if the command should be enabled.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed, this object can be
        ///     set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        [DebuggerStepThrough]
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require
        ///     data to be passed, this object can be set to null.
        /// </param>
        public abstract void Execute(object parameter);

        #endregion
    }
}
