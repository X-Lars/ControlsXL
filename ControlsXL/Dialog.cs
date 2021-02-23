using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ControlsXL
{
    public enum ProgressIndicatorStyles
    {
        Linear,
        Circular
    }

    /// <summary>
    /// Base class for all dialogs, functions as placeholder element for the <see cref="DialogManager"/> to show it's dialogs.<br/>
    /// Can be used to create a customized dialog before attaching it to the UI, use <see cref="DialogManager.Show(Dialog)"/> to attach the dialog to the UI.
    /// </summary>
    /// <remarks><i>A single Dialog element must be added to the application main window.</i></remarks>
    /// <example>
    /// <code>
    /// Dialog dialog = new Dialog();
    /// dialog.Title = "Title";
    /// dialog.Message = "Message";
    /// dialog.ShowConfirmation = true;
    /// DialogManager.Show(dialog);
    /// </code>
    /// </example>
    public class Dialog : ContentControl//, ICloneable
    {
        #region Constants

        /// <summary>
        /// Defines the task delay for the <see cref="Result"/> method.
        /// </summary>
        protected const int DIALOG_TASK_DELAY = 50;

        /// <summary>
        /// Defines the timer interval for indeterminate <see cref="ProgressDialog"/>s.
        /// </summary>
        protected const int INDETERMINATE_TIMER_INTERVAL = 100;

        #endregion

        #region Fields

        /// <summary>
        /// Stores the result of the <see cref="Dialog"/>.
        /// </summary>
        protected DialogResults _Result = DialogResults.DialogNone;

        /// <summary>
        /// Timer for indeterminate progress.
        /// </summary>
        protected DispatcherTimer _Timer;

        #endregion

        #region Events

        /// <summary>
        /// Defines the signature for the <see cref="CloseRequested"/> event.
        /// </summary>
        /// <param name="sender">An <see cref="object"/> representing the <see cref="Dialog"/> to close.</param>
        /// <param name="e">A <see cref="DialogEventArgs"/> containing event data.</param>
        internal delegate void DialogCloseEventHandler(object sender, DialogEventArgs e);
        
        /// <summary>
        /// Event to raise when a <see cref="Dialog"/> is requested to close.
        /// </summary>
        /// <remarks><i>The event is catched by the <see cref="DialogManager"/> to actually close the <see cref="Dialog"/>.</i></remarks>
        internal event DialogCloseEventHandler CloseRequested;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MessageDialog"/>.
        /// </summary>
        static Dialog() 
        {
            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initializes a new blank <see cref="Dialog"/> instance without any predefined options.<br/>
        /// Options have to be set by the user.
        /// </summary>
        /// <remarks><i>Use <see cref="DialogManager.Show(Dialog)"/> method to attach the <see cref="Dialog"/> to the UI.</i></remarks>
        public Dialog()
        {
            Panel.SetZIndex(this, int.MaxValue);

            CommandBindings.Add(new CommandBinding(_AcceptCommand, OnAcceptCommand, CanExecuteOptionCommand));
            CommandBindings.Add(new CommandBinding(_CancelCommand, OnCancelCommand, CanExecuteCancelCommand));
            CommandBindings.Add(new CommandBinding(_ConfirmCommand, OnConfirmCommand, CanExecuteConfirmCommand));
            CommandBindings.Add(new CommandBinding(_DeclineCommand, OnDeclineCommand, CanExecuteOptionCommand));
        }

        #endregion

        #region Commands

        #region Commands: Registration

    /// <summary>
    /// Registers the command to cancel the <see cref="Dialog"/>.
    /// </summary>
    private static RoutedUICommand _CancelCommand = new RoutedUICommand(nameof(CancelCommand), nameof(CancelCommand), typeof(Dialog));

        /// <summary>
        /// Registers the command to confirm the <see cref="Dialog"/>.
        /// </summary>
        private static RoutedUICommand _ConfirmCommand = new RoutedUICommand(nameof(ConfirmCommand), nameof(ConfirmCommand), typeof(Dialog));

        /// <summary>
        /// Registers the command to accept the <see cref="Dialog"/>.
        /// </summary>
        private static RoutedUICommand _AcceptCommand = new RoutedUICommand(nameof(AcceptCommand), nameof(AcceptCommand), typeof(Dialog));

        /// <summary>
        /// Registers the command to decline the <see cref="Dialog"/>.
        /// </summary>
        private static RoutedUICommand _DeclineCommand = new RoutedUICommand(nameof(DeclineCommand), nameof(DeclineCommand), typeof(Dialog));

        #endregion

        #region Commands: Properties

        /// <summary>
        /// Gets the command to cancel the <see cref="Dialog"/>.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return _CancelCommand; }
        }

        /// <summary>
        /// Gets the command to close the <see cref="Dialog"/>.
        /// </summary>
        public ICommand ConfirmCommand
        {
            get { return _ConfirmCommand; }
        }

        /// <summary>
        /// Gets the command to accept the <see cref="Dialog"/>.
        /// </summary>
        public ICommand AcceptCommand
        {
            get { return _AcceptCommand; }
        }

        /// <summary>
        /// Gets the command to decline the <see cref="Dialog"/>.
        /// </summary>
        public ICommand DeclineCommand
        {
            get { return _DeclineCommand; }
        }

        #endregion

        #region Commands: Event Handlers

        /// <summary>
        /// Implements the <see cref="AcceptCommand"/> to set the result to <see cref="DialogResults.DialogYes"/> and request closing of the <see cref="Dialog"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnAcceptCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _Result = DialogResults.DialogYes;
            Close();
        }

        /// <summary>
        /// Implements the <see cref="CancelCommand"/> to set the result to <see cref="DialogResults.DialogCancelled"/> and request closing of the <see cref="Dialog"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnCancelCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _Result = DialogResults.DialogCancelled;
            Close();
        }

        /// <summary>
        /// Implements the <see cref="ConfirmCommand"/> to set the result to <see cref="DialogResults.DialogOK"/> and request closing of the <see cref="Dialog"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnConfirmCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _Result = DialogResults.DialogOK;
            Close();
        }

        /// <summary>
        /// Implements the <see cref="DeclineCommand"/> to set the result to <see cref="DialogResults.DialogNo"/> and request closing of the <see cref="Dialog"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnDeclineCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _Result = DialogResults.DialogNo;
            Close();
        }

        /// <summary>
        /// Determines if the <see cref="CancelCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteCancelCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCancel;
        }

        /// <summary>
        /// Determines if the <see cref="ConfirmCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteConfirmCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ShowConfirmation;
        }

        /// <summary>
        /// Determines if the <see cref="AcceptCommand"/> and <see cref="DeclineCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteOptionCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = HasOptions;
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set whether the <see cref="Dialog"/> can be cancelled.
        /// </summary>
        public static readonly DependencyProperty CanCancelProperty = DependencyProperty.Register(nameof(CanCancel), typeof(bool), typeof(Dialog), new PropertyMetadata(false));
        
        /// <summary>
        /// Registers the property to set whether the <see cref="Dialog"/> has option buttons.
        /// </summary>
        public static readonly DependencyProperty HasOptionsProperty = DependencyProperty.Register(nameof(HasOptions), typeof(bool), typeof(Dialog), new PropertyMetadata(false));

        /// <summary>
        /// Registers the property to set the <see cref="Dialog"/> message.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(Dialog), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set whether the <see cref="Dialog"/> confirmation button is shown.
        /// </summary>
        public static readonly DependencyProperty ShowConfirmationProperty = DependencyProperty.Register(nameof(ShowConfirmation), typeof(bool), typeof(Dialog), new PropertyMetadata(false));

        /// <summary>
        /// Registers the property to set whether the <see cref="Dialog"/> status text is shown.
        /// </summary>
        public static readonly DependencyProperty ShowStatusProperty =DependencyProperty.Register(nameof(ShowStatus), typeof(bool), typeof(Dialog), new PropertyMetadata(false));

        /// <summary>
        /// Registers the property to set the <see cref="Dialog"/> status text.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(string), typeof(Dialog), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set the <see cref="Dialog"/> title.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Dialog), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set whether the <see cref="ProgressDialog"/> is indeterminate.
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(Dialog), new PropertyMetadata(false, IsIndeterminatePropertyChanged));

        /// <summary>
        /// Registers the property to set the <see cref="Dialog"/> progress.
        /// </summary>
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(nameof(Progress), typeof(double), typeof(Dialog), new PropertyMetadata(0.0));

        /// <summary>
        /// Registers the property to set the style of the <see cref="Dialog"/> progress indicator
        /// </summary>
        public static readonly DependencyProperty ProgressStyleProperty = DependencyProperty.Register(nameof(ProgressStyle), typeof(ProgressIndicatorStyles), typeof(Dialog), new PropertyMetadata(ProgressIndicatorStyles.Linear));

        /// <summary>
        /// Registers the property to set whether the <see cref="Dialog"/> progress bar is shown.
        /// </summary>
        public static readonly DependencyProperty ShowProgressProperty = DependencyProperty.Register(nameof(ShowProgress), typeof(bool), typeof(Dialog), new PropertyMetadata(false));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> can be cancelled.
        /// </summary>
        public bool CanCancel
        {
            get { return (bool)GetValue(CanCancelProperty); }
            set { SetValue(CanCancelProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> has options.
        /// </summary>
        public bool HasOptions
        {
            get { return (bool)GetValue(HasOptionsProperty); }
            set { SetValue(HasOptionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Dialog"/> message.
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> confirmation button is shown.
        /// </summary>
        public bool ShowConfirmation
        {
            get { return (bool)GetValue(ShowConfirmationProperty); }
            set { SetValue(ShowConfirmationProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> status text is shown.
        /// </summary>
        public bool ShowStatus
        {
            get { return (bool)GetValue(ShowStatusProperty); }
            set { SetValue(ShowStatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Dialog"/> status text.
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Dialog"/> title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> is indeterminate.
        /// </summary>
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Dialog"/> progress.
        /// </summary>
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style of the <see cref="Dialog"/> progress indicator.
        /// </summary>
        public ProgressIndicatorStyles ProgressStyle
        {
            get { return (ProgressIndicatorStyles)GetValue(ProgressStyleProperty); }
            set { SetValue(ProgressStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> progress bar is shown.
        /// </summary>
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        #endregion

        #region Dependency Properties: Callbacks

        private static void IsIndeterminatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dialog dialog = (Dialog)d;

            if((bool)e.NewValue == true)
            {
                if (dialog._Timer == null)
                {
                    dialog._Timer = new DispatcherTimer();
                    dialog._Timer.Tick += dialog.TimerTick;
                    dialog._Timer.Interval = TimeSpan.FromMilliseconds(INDETERMINATE_TIMER_INTERVAL);
                    dialog._Timer.Start();
                }
                else
                {
                    dialog._Timer.Tick += dialog.TimerTick;
                    dialog._Timer.Start();
                }
            }
            else
            {
                if(dialog._Timer != null)
                {
                    dialog._Timer.Stop();
                    dialog._Timer.Tick -= dialog.TimerTick;
                    dialog._Timer = null;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the result of the <see cref="Dialog"/> async.
        /// </summary>
        /// <returns>A <see cref="Task{DialogResults}"/> containing the result of the <see cref="Dialog"/>.</returns>
        public virtual async Task<DialogResults> Result()
        {
            _Result = DialogResults.DialogNone;

            while (_Result == DialogResults.DialogNone)
            {
                await Task.Delay(DIALOG_TASK_DELAY);
            }

            return _Result;
        }

        /// <summary>
        /// Raises the <see cref="CloseRequested"/> event for the <see cref="Dialog"/>.
        /// </summary>
        /// <remarks><i>The <see cref="CloseRequested"/> event is catched by the <see cref="DialogManager"/> to actualy close the <see cref="Dialog"/>.</i></remarks>
        public virtual void Close()
        {
            if(_Timer != null)
            {
                _Timer.Stop();
                _Timer.Tick -= TimerTick;
                _Timer = null;
            }

            if(CloseRequested != null)
                CloseRequested?.Invoke(this, new DialogEventArgs(_Result));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="DispatcherTimer.Tick"/> event for indeterminate progress dialogs.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            int threshold = INDETERMINATE_TIMER_INTERVAL + 5;

            Progress = ((Progress + 5) % threshold);
        }

        #endregion

        #region Overrides

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Overloads the assignment operator to convert the RHS <see cref="Dialog"/> into a LHS async <see cref="Task{DialogResults}"/>.
        /// </summary>
        /// <param name="dialog">The <see cref="Dialog"/> to convert to an async <see cref="Task{DialogResults}"/>.</param>
        public static implicit operator Task<DialogResults>(Dialog dialog)
        {
            return dialog.Result();
        }

        #endregion
    }

    internal sealed class DialogBase : Dialog
    {
        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="BaseDialog"/>.
        /// </summary>
        static DialogBase()
        {
            // Overrides the default style to use the Dialog style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogBase), new FrameworkPropertyMetadata(typeof(Dialog)));
            StylesXL.StyleManager.Initialize();
        }

        internal DialogBase(Dialog dialog)
        {
            CanCancel = dialog.CanCancel;
            HasOptions = dialog.HasOptions;
            Message = dialog.Message;
            ShowConfirmation = dialog.ShowConfirmation;
            ShowStatus = dialog.ShowStatus;
            Status = dialog.Status;
            Title = dialog.Title;
            IsIndeterminate = dialog.IsIndeterminate;
            Progress = dialog.Progress;
            ProgressStyle = dialog.ProgressStyle;
            ShowProgress = dialog.ShowProgress;
        }
    }


    /// <summary>
    /// Defines a dialog to show a message.
    /// </summary>
    public sealed class MessageDialog : Dialog
    {
        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MessageDialog"/>.
        /// </summary>
        static MessageDialog()
        {
            // Overrides the default style to use the Dialog style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(Dialog)));
            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initializes a new <see cref="MessageDialog"/> instance with confirmation button.
        /// </summary>
        /// <param name="title">A <see cref="string"/> defining the title of the <see cref="MessageDialog"/>.</param>
        /// <param name="message">A <see cref="string"/> defining the message of the <see cref="MessageDialog"/>.</param>
        /// <remarks><i>Use the <see cref="DialogManager.Show(Dialog)"/> method to attach the <see cref="MessageDialog"/> to the UI.</i></remarks>
        public MessageDialog(string title, string message) : base()
        {
            Title = title;
            Message = message;

            ShowConfirmation = true;
        }
    }

    /// <summary>
    /// Defines a dialog to request user confirmation.
    /// </summary>
    public sealed class QuestionDialog : Dialog
    {
        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="QuestionDialog"/>.
        /// </summary>
        static QuestionDialog()
        {
            // Overrides the default style to use the Dialog style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuestionDialog), new FrameworkPropertyMetadata(typeof(Dialog)));

            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initializes a new <see cref="QuestionDialog"/> instance with option buttons.
        /// </summary>
        /// <param name="title">A <see cref="string"/> defining the title of the <see cref="QuestionDialog"/>.</param>
        /// <param name="message">A <see cref="string"/> defining the message of the <see cref="QuestionDialog"/>.</param>
        /// <remarks><i>Use the <see cref="DialogManager.Show(Dialog)"/> method to attach the <see cref="QuestionDialog"/> to the UI.</i></remarks>
        public QuestionDialog(string title, string message) : base()
        {
            Title = title;
            Message = message;

            HasOptions = true;
        }

        #endregion
    }

    /// <summary>
    /// Define a dialog to show operation progress.
    /// </summary>
    public sealed class ProgressDialog : Dialog
    {
        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="ProgressDialog"/>.
        /// </summary>
        static ProgressDialog()
        {
            // Overrides the default style to use the Dialog style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressDialog), new FrameworkPropertyMetadata(typeof(Dialog)));
            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initializes a new <see cref="ProgressDialog"/> instance with a progress bar and status text.
        /// </summary>
        /// <param name="title">A <see cref="string"/> defining the title of the <see cref="ProgressDialog"/>.</param>
        /// <param name="message">A <see cref="string"/> defining the message of the <see cref="ProgressDialog"/>.</param>
        /// <param name="status">A <see cref="string"/> defining the status text of the <see cref="ProgressDialog"/>.</param>
        /// <param name="showProgress">A <see cref="bool"/> to determine if the progress bar is shown.</param>
        /// <param name="isIndeterminate">A <see cref="bool"/> to determine if the <see cref="ProgressDialog"/> is indeterminate.</param>
        /// <param name="canCancel">A <see cref="bool"/> to determine if the <see cref="ProgressDialog"/> operation can be cancelled.</param>
        /// <param name="style">The style of the progress indicator.</param>
        /// <remarks><i>Use the <see cref="DialogManager.Show(Dialog)"/> method to attach the <see cref="ProgressDialog"/> to the UI.</i></remarks>
        public ProgressDialog(string title, string message, string status, bool isIndeterminate = false, bool canCancel = false, ProgressIndicatorStyles style = ProgressIndicatorStyles.Linear) : base()
        {
            Title = title;
            Message = message;
            Status = status;
            IsIndeterminate = isIndeterminate;
            CanCancel = canCancel;

            ShowProgress = true;
            ProgressStyle = style;
            ShowStatus = true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the result of the <see cref="ProgressDialog"/> async.
        /// </summary>
        /// <returns>A <see cref="Task{DialogResults}"/> containing the result of the <see cref="ProgressDialog"/>.</returns>
        public override async Task<DialogResults> Result()
        {
            _Result = DialogResults.DialogOK;

            int threshold = INDETERMINATE_TIMER_INTERVAL + 5;

            if (IsIndeterminate)
            {
                while (_Result == DialogResults.DialogOK)
                {
                    await Task.Delay(DIALOG_TASK_DELAY);
                    Progress = (Progress + 5) % threshold;
                }
            }
            else
            {
                while (_Result == DialogResults.DialogOK)
                {
                    await Task.Delay(DIALOG_TASK_DELAY);
                }
            }

            return _Result;
        }

        #endregion
    }
}
