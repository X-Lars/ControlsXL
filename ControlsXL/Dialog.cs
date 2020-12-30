using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlsXL
{
    /// <summary>
    /// 
    /// </summary>
    public class Dialog : ContentControl
    {
        public event DialogManager.DialogCloseEventHandler Closed;


        public DialogResult _DialogResult = new DialogResult();


        static Dialog()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(typeof(Dialog)));
        }

        public Dialog()
        {
            CommandBindings.Add(new CommandBinding(_CancelCommand, OnCancelCommand, CanExecuteCancelCommand));
        }

        
        #region Commands

        #region Commands: Registration
        private static RoutedUICommand _CancelCommand = new RoutedUICommand(nameof(CancelCommand), nameof(CancelCommand), typeof(Dialog));
        #endregion

        #region Commands: Properties

        public ICommand CancelCommand
        {
            get { return _CancelCommand; }
        }
        #endregion

        #region Commands: Event Handlers

        private void OnCancelCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void CanExecuteCancelCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #endregion

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Dialog), new PropertyMetadata("Title"));



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(Dialog), new PropertyMetadata("Message"));



        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInderminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(Dialog), new PropertyMetadata(false));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(string), typeof(Dialog), new PropertyMetadata("Status"));

        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register(nameof(ShowProgress), typeof(bool), typeof(Dialog), new PropertyMetadata(false));

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(nameof(Progress), typeof(int), typeof(Dialog), new PropertyMetadata(0));




        public void Close()
        {
            Closed?.Invoke(this, new DialogEventArgs());
        }
    }

    public class ProgressDialog : Dialog
    {

        #region Constructor

        static ProgressDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressDialog), new FrameworkPropertyMetadata(typeof(Dialog)));
        }

        public ProgressDialog()
        {
            ShowProgress = true;
        }

        #endregion

        #region Methods

       
        #endregion
    }
}
