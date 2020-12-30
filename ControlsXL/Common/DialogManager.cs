using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ControlsXL
{
    /// <summary>
    /// Manages creation of dialogs.
    /// </summary>
    public static class DialogManager
    {
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
        private static readonly Dialog _Placeholder;

        #endregion


        public delegate void DialogCloseEventHandler(object sender, DialogEventArgs e);

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

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ProgressDialog"/>.
        /// </summary>
        /// <returns>A reference to the created <see cref="ProgressDialog"/>.</returns>
        public static ProgressDialog ProgressDialog()
        {
            if(!_Owner.IsLoaded)
            {
                throw new ApplicationException("Can't create dialog before the main window is loaded.");
            }

            ProgressDialog dialog = null;

            _Dispatcher.Invoke(() =>
            {
                dialog = new ProgressDialog();
                dialog.Closed += DialogClosed;

                _Placeholder.Content = dialog;
            });

            return dialog;
        }


        private static void DialogClosed(object sender, EventArgs e)
        {
            _Dispatcher.Invoke(() =>
            {
                _Placeholder.Content = null;
                sender = null;
            });
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

    public class DialogEventArgs : EventArgs
    {
        DialogResult _DialogResult;

        public DialogEventArgs() { }
    }

    public class DialogResult
    {

    }
}
