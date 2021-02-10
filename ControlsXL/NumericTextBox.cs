using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlsXL
{
    public class NumericTextBox : TextBox
    {
        #region Fields

        /// <summary>
        /// An <see cref="int"/> to store the number of decimal places from the interval.
        /// </summary>
        private int _Resolution = 0;
        private bool _HasList = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="NumericTextBox"/>.
        /// </summary>
        static NumericTextBox()
        {
            // Overrides the default style of the inherited Button to use the NumericTextBox style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(typeof(NumericTextBox)));
        }

        /// <summary>
        /// Creates and initializes a new <see cref="NumericTextBox"/> instance.
        /// </summary>
        public NumericTextBox() : base()
        {
            HorizontalContentAlignment = HorizontalAlignment.Right;

            CommandBindings.Add(new CommandBinding(_DecrementCommand, OnDecrementCommand, CanExecuteDecrementCommand));
            CommandBindings.Add(new CommandBinding(_IncrementCommand, OnIncrementCommand, CanExecuteIncrementCommand));

            AcceptsReturn = false;
            AcceptsTab = false;

            //SelectionBrush = (SolidColorBrush)StyleManager.GetStyleValue("TestBrush");
        }

        #endregion

        #region Commands

        #region Commands: Registrations

        /// <summary>
        /// Defines the command to decrement the <see cref="Value"/>.
        /// </summary>
        private static RoutedUICommand _DecrementCommand = new RoutedUICommand(nameof(DecrementCommand), nameof(DecrementCommand), typeof(NumericTextBox));

        /// <summary>
        /// Defines the command to increment the <see cref="Value"/>.
        /// </summary>
        private static RoutedUICommand _IncrementCommand = new RoutedUICommand(nameof(IncrementCommand), nameof(IncrementCommand), typeof(NumericTextBox));
        
        #endregion

        #region Command: Properties

        /// <summary>
        /// Gets the command to decrement the <see cref="Value"/>.
        /// </summary>
        public static ICommand DecrementCommand
        {
            get { return _DecrementCommand; }
        }

        /// <summary>
        /// Gets the command to increment the <see cref="Value"/>.
        /// </summary>
        public static ICommand IncrementCommand
        {
            get { return _IncrementCommand; }
        }

        #endregion

        #region Command: Event Handlers

        /// <summary>
        /// Implements the <see cref="DecrementCommand"/> to decrement the <see cref="Value"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnDecrementCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_HasList)
                Index--;
            else
                Value -= Interval;
        }

        /// <summary>
        /// Implements the <see cref="IncrementCommand"/> to increment the <see cref="Value"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnIncrementCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_HasList)
                Index++;
            else
                Value += Interval;
        }

        /// <summary>
        /// Determines if the <see cref="CloseCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteDecrementCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_HasList)
            {
                if (Index > 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            else
            {
                if (Value > MinValue)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        /// <summary>
        /// Determines if the <see cref="MaximizeCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteIncrementCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_HasList)
            {
                if (Index < Values.Count - 1)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            else
            {
                if (Value < MaxValue)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the interval of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(nameof(Interval), typeof(double), typeof(NumericTextBox), new UIPropertyMetadata(1d));

        /// <summary>
        /// Registers the property to set the maximum value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(NumericTextBox), new UIPropertyMetadata(100d));

        /// <summary>
        /// Registers the property to set the minimum value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(NumericTextBox), new UIPropertyMetadata(0d));

        /// <summary>
        /// Registers the property to set the orientation of the <see cref="NumericTextBox"/> buttons.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(NumericTextBox), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Registers the property to set the prefix of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(NumericTextBox), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to determin whether the <see cref="NumericTextBox"/> buttons are visible
        /// </summary>
        public static readonly DependencyProperty ShowButtonsProperty = DependencyProperty.Register(nameof(ShowButtons), typeof(bool), typeof(NumericTextBox), new PropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the suffix of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(NumericTextBox), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set the value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(NumericTextBox), new FrameworkPropertyMetadata(new double(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValuePropertyChanged)));

        /// <summary>
        /// Registers the property to set the predefined values of the <see cref="NumericTextBox"/>.
        /// </summary>
        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(nameof(Values), typeof(List<string>), typeof(NumericTextBox), new PropertyMetadata(null, ValuesPropertyChanged));

        /// <summary>
        /// Registers the property to set the index of the <see cref="Values"/>.
        /// </summary>
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int), typeof(NumericTextBox), new FrameworkPropertyMetadata(new int(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(IndexPropertyChanged)));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the interval of the <see cref="NumericTextBox"/> which also defines the number of decimal places.
        /// </summary>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation of the <see cref="NumericTextBox"/> buttons.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the prefix of the <see cref="NumericTextBox"/>.
        /// </summary>
        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        /// <summary>
        /// Gets or sets wheter the <see cref="NumericTextBox"/> buttons are visible.
        /// </summary>
        public bool ShowButtons
        {
            get { return (bool)GetValue(ShowButtonsProperty); }
            set { SetValue(ShowButtonsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the suffix of the <see cref="NumericTextBox"/>.
        /// </summary>
        public string Suffix
        {
            get { return (string)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                value = Math.Min(value, MaxValue);
                value = Math.Max(value, MinValue);

                SetValue(ValueProperty, value);

                // "N" Specifies the number of fractional digits
                Text = value.ToString("N" + _Resolution.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the list of predefined values of the <see cref="NumericTextBox"/>.
        /// </summary>
        public List<string> Values
        {
            get { return (List<string>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the <see cref="Values"/>.
        /// </summary>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set 
            {
                value = Math.Min(value, Values.Count - 1);
                value = Math.Max(value, 0);

                SetValue(IndexProperty, value); 
            }
        }

        #endregion

        #region Dependency Properties: Callbacks

        /// <summary>
        /// Handles changes of the <see cref="ValueProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBox numericTextBox = (NumericTextBox)sender;

            double value = double.TryParse(e.NewValue.ToString(), out value) ? value : 0d;

            value = Math.Min(value, numericTextBox.MaxValue);
            value = Math.Max(value, numericTextBox.MinValue);

            numericTextBox.Value = value;
        }

        private static void IndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBox numericTextBox = (NumericTextBox)d;

            numericTextBox.Text = numericTextBox.Values[(int)e.NewValue];
        }


        private static void ValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBox numericTextBox = (NumericTextBox)d;

            if (e.NewValue != null)
            {
                numericTextBox._HasList = true;
                numericTextBox.Text = ((List<string>)e.NewValue)[numericTextBox.Index];
            }
            else
            {
                numericTextBox._HasList = false;
                numericTextBox.Text = numericTextBox.Value.ToString("N" + numericTextBox._Resolution.ToString());
            }
        }


        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Check if the provided string value is numeric.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to check.</param>
        /// <returns>A <see cref="bool"/> set to true if the value is numeric, false otherwise.</returns>
        private bool IsNumeric(string value)
        {
            Regex regex = new Regex("[^0-9.-]+");

            return !regex.IsMatch(value);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Initializes the <see cref="NumericTextbox"/> properties.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double value = Interval;

            // Get the number of decimal places from the interval if smaller than 1
            while ((int)value % 10 == 0)
            {
                value *= 10;
                _Resolution++;
            }

            // Apply the number of decimal places to the text
            Text = Value.ToString("N" + _Resolution.ToString());

            // Sets the tooltip of the text box "PF 0 ... 100 SF"
            ToolTipService.SetToolTip(this, string.Format("{2} {0} ... {1} {3}", MinValue, MaxValue, Prefix, Suffix));
        }

        /// <summary>
        /// Handles specialized key presses of the <see cref="NumericTextBox"/>.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> containing event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            bool isCTRLKeyDown = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);
            
            switch(e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    if (_HasList)
                        Index += isCTRLKeyDown ? 10 : 1;
                    else
                        Value += isCTRLKeyDown ? Interval * 10 : Interval;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    if (_HasList)
                        Index -= isCTRLKeyDown ? 10 : 1;
                    else
                        Value -= isCTRLKeyDown ? Interval * 10 : Interval;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.PageUp:

                    if (_HasList)
                        Index = Values.Count - 1;
                    else
                        Value = MaxValue;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.PageDown:

                    if (_HasList)
                        Index = 0;
                    else
                        Value = MinValue;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.Tab:
                    {
                        if (!_HasList)
                        {
                            double value;

                            Value = double.TryParse(Text, out value) ? value : Value;
                        }

                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    }
                    break;

                case Key.Enter:
                    {
                        if (!_HasList)
                        {
                            double value;

                            Value = double.TryParse(Text, out value) ? value : Value;
                        }

                        SelectAll();
                    }
                    break;
            }
        }

        /// <summary>
        /// Overrides the mouse wheel to increment or decrement the <see cref="Value"/>.
        /// </summary>
        /// <param name="e">A <see cref="MouseWheelEventArgs"/> containing event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            int distance = e.Delta / 100;

            if(distance > 0)
            {
                if (_HasList)

                    Index += 1;
                else
                    Value += Interval;
            }
            else
            {
                if (_HasList)
                    Index -= 1;
                else
                    Value -= Interval;
            }

            CommandManager.InvalidateRequerySuggested();
            
            SelectAll();
        }

        #endregion
    }
}
