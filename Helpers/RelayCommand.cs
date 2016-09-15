using System;

namespace Helpers
{
    /// <summary>
    ///     MVVM class for handling commands.
    /// </summary>
    public class RelayCommand : CommandBase
    {
        private readonly Action<object> _execute;

        /// <summary>
        /// Static constructor to set up logging.
        /// </summary>
        static RelayCommand()
        {
            Logging = new MessageBoxLogger();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="execute">The action to execute when the command is activated.</param>
        /// <param name="canExecute">The method to use to check if the command can be activated. If null this is always true.</param>
        /// <exception cref="ArgumentNullException">Thrown if execute is null.</exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null) : base(canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
        }

        /// <summary>
        /// The class to use for WPF logging.
        /// </summary>
        public static ILogging Logging { get; set; }

        /// <summary>
        ///     Method called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be passed, this object can be
        ///     set to null.
        /// </param>
        public override void Execute(object parameter)
        {
            try
            {
                _execute(parameter);
            }
            catch (NotImplementedException)
            {
                Logging.LogError("Command not implemented", "Not Implemented");
            }
        }
    }
}
