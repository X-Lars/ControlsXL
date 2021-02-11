using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ControlsXL
{

    /// <summary>
    /// Enumeration specifying all possible <see cref="Dialog"/> result values.
    /// </summary>
    public enum DialogResults
    {
        DialogNone,
        DialogOK,
        DialogCancelled,
        DialogYes,
        DialogNo
    }

    /// <summary>
    /// Manages creation and deletion of dialogs.
    /// </summary>
    /// <example>
    /// <code>
    /// MessageDialog dialog = DialogManager.MessageDialog("Title", "Message");
    /// await dialog.Result()
    /// 
    /// DialogResult result = await DialogManager.QuestionDialog("Title", "Message");
    /// 
    /// Dialog dialog = new Dialog();
    /// dialog.Title = "Title";
    /// dialog.Message = "Message";
    /// await DialogManager.Show(dialog);
    /// </code>
    /// </example>
    /// <remarks>Ensure the main window contains a Dialog element.</remarks>
    public static class DialogManager
    {
        /// <summary>
        /// Defines the time in milliseconds to wait before actually closing the <see cref="Dialog"/>.
        /// </summary>
        private const int DIALOG_CLOSE_DELAY = 1000;

        #region Fields

        /// <summary>
        /// Stores a reference to the application main window.
        /// </summary>
        private static readonly Window _Owner;

        /// <summary>
        /// Stores a reference to the main window <see cref="Dispatcher"/>.
        /// </summary>
        private static readonly Dispatcher _Dispatcher;

        /// <summary>
        /// Stores a reference to the <see cref="Dialog"/> placeholder element.
        /// </summary>
        private static Dialog _Placeholder;

        /// <summary>
        /// Stores a stack of all open <see cref="Dialog"/>s.
        /// </summary>
        private static Stack<Dialog> _Dialogs = new Stack<Dialog>();

        /// <summary>
        /// Lock object for thread safety.
        /// </summary>
        private static readonly object _Lock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates and initializes the <see cref="DialogManager"/> instance.
        /// </summary>
        static DialogManager()
        {
            _Owner = Application.Current.MainWindow;
            _Dispatcher = _Owner.Dispatcher;

            if (!_Owner.IsLoaded)
                throw new ApplicationException("Access to the dialog manager is restricted until the main window is loaded.");

            _Placeholder = FindDialogPlaceholder(_Owner);

            if (_Placeholder == null)
                throw new ApplicationException("No <Dialog/> placeholder element found!");


        }

        #endregion

        #region Properties

        //public static bool IsLoaded { get; private set; }
        //public static bool IsInitialized { get; private set; }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="Dialog.CloseRequested"/> event to delete the <see cref="Dialog"/> and remove it from the UI.
        /// </summary>
        /// <param name="sender">An <see cref="object"/> representing the <see cref="Dialog"/> to close.</param>
        /// <param name="e">A <see cref="DialogEventArgs"/> containing event data.</param>
        private static void DialogCloseRequested(object sender, DialogEventArgs e)
        {
            //if(sender is ProgressDialog)
            //    Thread.Sleep(DIALOG_CLOSE_DELAY);

            lock (_Dialogs)
            {
                if (_Dialogs.Count == 0)
                    return;

                _Dialogs.Pop();

                if (_Dialogs.Count == 0)
                {
                    _Dispatcher.Invoke(() =>
                    {
                        _Placeholder.Content = null;
                        sender = null;
                    });
                }
                else
                {
                    _Dispatcher.Invoke(() =>
                    {
                        _Placeholder.Content = _Dialogs.Peek();
                    });
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the provided <see cref="Dialog"/> to the UI to be rendered.
        /// </summary>
        /// <param name="dialog">A <see cref="Dialog"/> inherited class.</param>
        /// <returns>A <see cref="Task{DialogResults}"/> containing the result of the <see cref="Dialog"/>.</returns>
        public static async Task<DialogResults> Show(Dialog dialog)
        {
            _Dispatcher.Invoke(() =>
            {
                // Dialog has to be cast to DialogBase because Dialog has not default style key
                dialog = new DialogBase(dialog);
                dialog.CloseRequested += DialogCloseRequested;

                _Placeholder.Content = dialog;

                lock (_Dialogs)
                {
                    _Dialogs.Push(dialog);
                }
            });

            return await dialog.Result();
        }

        /// <summary>
        /// Creates a new <see cref="MessageDialog"/> instance and attaches it to the UI.
        /// </summary>
        /// <param name="title">A <see cref="string"/> specifying the name.</param>
        /// <param name="message">A <see cref="string"/> specifying the message.</param>
        /// <returns>A reference to the created <see cref="MessageDialog"/>.</returns>
        public static MessageDialog MessageDialog(string title, string message)
        {
            
            MessageDialog dialog = null;

            _Dispatcher.Invoke(() =>
            {
                dialog = new MessageDialog(title, message);
                dialog.CloseRequested += DialogCloseRequested;

                _Placeholder.Content = dialog;

                lock (_Dialogs)
                {
                    _Dialogs.Push(dialog);
                }
            });

            return dialog;
        }

        /// <summary>
        /// Creates a new <see cref="QuestionDialog"/> instance and attaches it to the UI.
        /// </summary>
        /// <param name="title">A <see cref="string"/> specifying the name.</param>
        /// <param name="message">A <see cref="string"/> specifying the message.</param>
        /// <returns>A reference to the created <see cref="QuestionDialog"/>.</returns>
        public static QuestionDialog QuestionDialog(string title, string message)
        {
            QuestionDialog dialog = null;

            _Dispatcher.Invoke(() =>
            {
                dialog = new QuestionDialog(title, message);
                dialog.CloseRequested += DialogCloseRequested;

                _Placeholder.Content = dialog;

                lock (_Dialogs)
                {
                    _Dialogs.Push(dialog);
                }
            });

            return dialog;
        }

        /// <summary>
        /// Creates a new <see cref="ProgressDialog"/> and attaches it to the UI.
        /// </summary>
        /// <param name="title">A <see cref="string"/> specifying the title.</param>
        /// <param name="message">A <see cref="string"/> specifying the message.</param>
        /// <param name="status">A <see cref="string"/> specifying the status text.</param>
        /// <param name="isIndeterminate">A <see cref="bool"/> to determine whether the dialog is indeterminate.</param>
        /// <param name="canCancel">A <see cref="bool"/> to determine whether the dialog supports cancellation.</param>
        /// <returns>A reference to the created <see cref="ProgressDialog"/>.</returns>
        public static ProgressDialog ProgressDialog(string title, string message, string status, bool isIndeterminate = false, bool canCancel = false, ProgressIndicatorStyles style = ProgressIndicatorStyles.Linear)
        {
            ProgressDialog dialog = null;

            _Dispatcher.Invoke(() =>
            {
                dialog = new ProgressDialog(title, message, status, isIndeterminate, canCancel, style);
                dialog.CloseRequested += DialogCloseRequested;

                _Placeholder.Content = dialog;

                lock (_Dialogs)
                {
                    _Dialogs.Push(dialog);
                }
            });

            return dialog;
        }

        /// <summary>
        /// Searches the visual tree of the provided <see cref="DependencyObject"/> recursively for a <see cref="Dialog"/> placeholder element.
        /// </summary>
        /// <param name="parent">The <see cref="DependencyObject"/> to start searching for the <see cref="Dialog"/> placeholder element.</param>
        /// <returns>A reference to the <see cref="Dialog"/> placeholder if found or <see cref="null"/>.</returns>
        private static Dialog FindDialogPlaceholder(DependencyObject parent)
        {
            if (parent == null) return null;

            Dialog dialog = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if(child.GetType() == typeof(Dialog))
                {
                    dialog = (Dialog)child;
                    break;
                }
                else
                {
                    dialog = FindDialogPlaceholder(child);

                    if (dialog != null)
                        break;
                }
            }

            return dialog;
        }

        #endregion
    }

    /// <summary>
    /// Extends the <see cref="EventArgs"/> with a <see cref="Result"/> property.
    /// </summary>
    public class DialogEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a new <see cref="DialogEventArgs"/> instance.
        /// </summary>
        /// <param name="result">A <see cref="DialogResults"/> to associate with the event.</param>
        public DialogEventArgs(DialogResults result = DialogResults.DialogOK) 
        {
            Result = result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the result of associated event.
        /// </summary>
        public DialogResults Result { get; }

        #endregion
    }
}
