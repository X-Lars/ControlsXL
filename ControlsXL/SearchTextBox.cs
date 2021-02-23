using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StylesXL;
namespace ControlsXL
{

    public class SearchTextBox : TextBox
    {
        #region Constructor

        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
            StylesXL.StyleManager.Initialize();

        }

        public SearchTextBox() : base()
        {
            CommandBindings.Add(new CommandBinding(ClearCommand, OnClearCommand));
        }

       
        #endregion

        #region Commands

        #region Commands: Registration

        /// <summary>
        /// Registers the command to clear the search text.
        /// </summary>
        private static RoutedUICommand _ClearCommand = new RoutedUICommand(nameof(ClearCommand), nameof(ClearCommand), typeof(SearchTextBox));

        #endregion

        #region Commands: Properties

        /// <summary>
        /// Gets the command to clear the search text.
        /// </summary>
        public static ICommand ClearCommand
        {
            get { return _ClearCommand; }
        }

        #endregion

        #region Commands: Handlers

        /// <summary>
        /// Executes the <see cref="ClearCommand"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Text = string.Empty;
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the <see cref="SearchTextBox"/> icon scale.
        /// </summary>
        public static readonly DependencyProperty IconScaleProperty = DependencyProperty.Register("IconScale", typeof(double), typeof(SearchTextBox), new PropertyMetadata(0.8));

        /// <summary>
        /// Registers the property to set the <see cref="SearchTextBox"/> icon visibility.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(SearchTextBox), new PropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the <see cref="SearchTextBox"/> text hint.
        /// </summary>
        public static readonly DependencyProperty TextHintProperty = DependencyProperty.Register(nameof(TextHint), typeof(string), typeof(SearchTextBox), new UIPropertyMetadata("Search..."));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the <see cref="SearchTextBox"/> icon scale.
        /// </summary>
        public double IconScale
        {
            get { return (double)GetValue(IconScaleProperty); }
            set { SetValue(IconScaleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of the <see cref="SearchTextBox"/> icon.
        /// </summary>
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

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
