//-----------------------------------------------------------------------
// <copyright file="DelegateCommand.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    /// <summary>
    /// This class allows delegating the commanding logic to methods passed as parameters,
    /// and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// The _can execute method.
        /// </summary>
        private readonly Func<bool> canExecuteMethod;

        /// <summary>
        /// The _execute method.
        /// </summary>
        private readonly Action executeMethod;

        /// <summary>
        /// The _can execute changed handlers.
        /// </summary>
        private List<WeakReference> canExecuteChangedHandlers;

        /// <summary>
        /// The _is automatic re-query disabled.
        /// </summary>
        private bool isAutomaticRequeryDisabled;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeMethod">
        /// ExecuteMethod
        /// </param>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeMethod">
        /// ExecuteMethod
        /// </param>
        /// <param name="canExecuteMethod">
        /// CanExecuteMethod
        /// </param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeMethod">
        /// ExecuteMethod
        /// </param>
        /// <param name="canExecuteMethod">
        /// CanExecuteMethod
        /// </param>
        /// <param name="isAutomaticRequeryDisabled">
        /// IsAutomaticRequeryDisabled
        /// </param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }

                CommandManagerHelper.AddWeakReferenceHandler(ref this.canExecuteChangedHandlers, value, 2);
            }

            remove
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }

                CommandManagerHelper.RemoveWeakReferenceHandler(this.canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
        /// Property to enable or disable CommandManager's automatic re-query on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return this.isAutomaticRequeryDisabled;
            }

            set
            {
                if (this.isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(this.canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(this.canExecuteChangedHandlers);
                    }

                    this.isAutomaticRequeryDisabled = value;
                }
            }
        }

        /// <summary>
        /// Method to determine if the command can be executed
        /// </summary>
        /// <returns>bool</returns>
        public bool CanExecute()
        {
            return this.canExecuteMethod == null || this.canExecuteMethod();
        }

        /// <summary>
        /// Execution of the command
        /// </summary>
        public void Execute()
        {
            if (this.executeMethod != null)
            {
                this.executeMethod();
            }
        }

        /// <summary>
        /// Raises the CanExecuteChanged event
        /// </summary>
        public void InvokeCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        /// <summary>
        /// Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(this.canExecuteChangedHandlers);
        }
    }

    /// <summary>
    /// This class allows delegating the commanding logic to methods passed as parameters,
    /// and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the parameter passed to the delegates
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DelegateCommand<T> : ICommand
    {
        /// <summary>
        /// The _can execute method.
        /// </summary>
        private readonly Func<T, bool> canExecuteMethod;

        /// <summary>
        /// The _execute method.
        /// </summary>
        private readonly Action<T> executeMethod;

        /// <summary>
        /// The _can execute changed handlers.
        /// </summary>
        private List<WeakReference> canExecuteChangedHandlers;

        /// <summary>
        /// The _is automatic requery disabled.
        /// </summary>
        private bool isAutomaticRequeryDisabled;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class. 
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeMethod">
        /// ExecuteMethod
        /// </param>
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class. 
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeMethod">
        /// ExecuteMethod
        /// </param>
        /// <param name="canExecuteMethod">
        /// CanExecuteMethod
        /// </param>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class. 
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeMethod">
        /// ExecuteMethod
        /// </param>
        /// <param name="canExecuteMethod">
        /// CanExecuteMethod
        /// </param>
        /// <param name="isAutomaticRequeryDisabled">
        /// IsAutomaticRequeryDisabled
        /// </param>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }

                CommandManagerHelper.AddWeakReferenceHandler(ref this.canExecuteChangedHandlers, value, 2);
            }

            remove
            {
                if (!this.isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }

                CommandManagerHelper.RemoveWeakReferenceHandler(this.canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
        ///     Property to enable or disable CommandManager's automatic requery on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return this.isAutomaticRequeryDisabled;
            }

            set
            {
                if (this.isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(this.canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(this.canExecuteChangedHandlers);
                    }

                    this.isAutomaticRequeryDisabled = value;
                }
            }
        }

        /// <summary>
        /// Method to determine if the command can be executed
        /// </summary>
        /// <param name="parameter">
        /// Parameter
        /// </param>
        /// <returns>
        /// bool
        /// </returns>
        public bool CanExecute(T parameter)
        {
            if (this.canExecuteMethod != null)
            {
                return this.canExecuteMethod(parameter);
            }

            return true;
        }

        /// <summary>
        /// Execution of the command
        /// </summary>
        /// <param name="parameter">
        /// Parameter
        /// </param>
        public void Execute(T parameter)
        {
            if (this.executeMethod != null)
            {
                this.executeMethod(parameter);
            }
        }

        /// <summary>
        /// Raises the CanExecuteChaged event
        /// </summary>
        public void InvokeCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            // if T is of value type and the parameter is not
            // set yet, then return false if CanExecute delegate
            // exists, else return true
            if (parameter == null && typeof(T).IsValueType)
            {
                return this.canExecuteMethod == null;
            }

            return this.CanExecute((T)parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        void ICommand.Execute(object parameter)
        {
            this.Execute((T)parameter);
        }

        /// <summary>
        /// Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(this.canExecuteChangedHandlers);
        }
    }

    /// <summary>
    /// This class contains methods for the CommandManager that help avoid memory leaks by
    /// using weak references.
    /// </summary>
    internal class CommandManagerHelper
    {
        private CommandManagerHelper()
        {
        }

        /// <summary>
        /// The call weak reference handlers.
        /// </summary>
        /// <param name="handlers">
        /// The handlers.
        /// </param>
        internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                // Take a snapshot of the handlers before we call out to them since the handlers
                // could cause the array to me modified while we are reading it.
                EventHandler[] callees = new EventHandler[handlers.Count];
                int count = 0;

                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler handler = reference.Target as EventHandler;
                    if (handler == null)
                    {
                        // Clean up old handlers that have been collected
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        callees[count] = handler;
                        count++;
                    }
                }

                // Call the handlers that we snapshotted
                for (int i = 0; i < count; i++)
                {
                    EventHandler handler = callees[i];
                    handler(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The add handlers to requery suggested.
        /// </summary>
        /// <param name="handlers">
        /// The handlers.
        /// </param>
        internal static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;
                    if (handler != null)
                    {
                        CommandManager.RequerySuggested += handler;
                    }
                }
            }
        }

        /// <summary>
        /// The remove handlers from requery suggested.
        /// </summary>
        /// <param name="handlers">
        /// The handlers.
        /// </param>
        internal static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;
                    if (handler != null)
                    {
                        CommandManager.RequerySuggested -= handler;
                    }
                }
            }
        }

        /// <summary>
        /// The add weak reference handler.
        /// </summary>
        /// <param name="handlers">
        /// The handlers.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <param name="defaultListSize">
        /// The default list size.
        /// </param>
        internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (handlers == null)
            {
                handlers = defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>();
            }

            handlers.Add(new WeakReference(handler));
        }

        /// <summary>
        /// The remove weak reference handler.
        /// </summary>
        /// <param name="handlers">
        /// The handlers.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler existingHandler = reference.Target as EventHandler;
                    if ((existingHandler == null) || (existingHandler == handler))
                    {
                        // Clean up old handlers that have been collected
                        // in addition to the handler that is to be removed.
                        handlers.RemoveAt(i);
                    }
                }
            }
        }
    }
}