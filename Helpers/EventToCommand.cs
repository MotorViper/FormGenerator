using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Helpers
{
    public class EventToCommand : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand),
            new PropertyMetadata(null, (s, e) => OnCommandChanged(s as EventToCommand, e)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }

        protected override void Invoke(object parameter)
        {
            if (!AssociatedElementIsDisabled() && CanExecute())
                Execute(parameter);
        }

        private static void OnCommandChanged(EventToCommand element, DependencyPropertyChangedEventArgs e)
        {
            if (element != null)
            {
                if (e.OldValue != null)
                    ((ICommand)e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;

                ICommand command = (ICommand)e.NewValue;

                if (command != null)
                    command.CanExecuteChanged += element.OnCommandCanExecuteChanged;

                element.EnableDisableElement();
            }
        }

        private bool AssociatedElementIsDisabled()
        {
            return AssociatedObject != null && !AssociatedObject.IsEnabled;
        }

        private void EnableDisableElement()
        {
            if (AssociatedObject != null)
                AssociatedObject.IsEnabled = CanExecute();
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }

        private void Execute(object parameter)
        {
            if (Command != null)
            {
                RoutedCommand routedCommand = Command as RoutedCommand;
                if (routedCommand != null)
                    routedCommand.Execute(parameter, AssociatedObject);
                else
                    Command.Execute(parameter);
            }
        }

        private bool CanExecute()
        {
            if (Command != null)
            {
                RoutedCommand routedCommand = Command as RoutedCommand;
                return routedCommand?.CanExecute(null, AssociatedObject) ?? Command.CanExecute(null);
            }

            return false;
        }
    }
}
