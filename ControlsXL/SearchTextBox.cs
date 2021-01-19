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
   
    public class SearchTextBox : TextBox
    {
        #region Constructor

        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        public SearchTextBox() : base()
        {
            CommandBindings.Add(new CommandBinding(ClearCommand, OnClearCommand));
        }

       
        #endregion

        #region Commands
        #region Commands: Registration

        /// <summary>
        /// 
        /// </summary>
        private static RoutedUICommand _ClearCommand = new RoutedUICommand(nameof(ClearCommand), nameof(ClearCommand), typeof(SearchTextBox));

        #endregion
        #region Commands: Properties

        /// <summary>
        /// Gets the command 
        /// </summary>
        public static ICommand ClearCommand
        {
            get { return _ClearCommand; }
        }

        #endregion
        #region Commands: Handlers

        private void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Text = string.Empty;
        }

        #endregion
        #endregion

        #region Dependency Properties
        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the <see cref="SearchTextBox"/> text hint.
        /// </summary>
        public static readonly DependencyProperty TextHintProperty = DependencyProperty.Register(nameof(TextHint), typeof(string), typeof(SearchTextBox), new UIPropertyMetadata("Search..."));

        #endregion
        #region Dependency Properties: Implementation


        /// <summary>
        /// Gets or sets the text hint of the <see cref="SearchTextBox"/>.
        /// </summary>
        public string TextHint
        {
            get { return (string)GetValue(TextHintProperty); }
            set { SetValue(TextHintProperty, value); }
        }

        #endregion
        #endregion

        
    }
}
