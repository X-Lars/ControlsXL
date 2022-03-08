using ControlsXL.Adorners;
using ControlsXL.Common;
using ControlsXL.Extensions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ControlsXL
{

    public class SpinText : Control
    {
        #region Fields

        /// <summary>
        /// Tracks the number of decimal places of the <see cref="Interval"/>.
        /// </summary>
        private int _Resolution = 0;

        /// <summary>
        /// Tracks wheter the mouse adorner is attached.
        /// </summary>
        private bool _HasMouseAdorner = false;

        /// <summary>
        /// Tracks wheter the keyboard adorner is attached.
        /// </summary>
        private bool _HasKeyboardAdorner = false;

        /// <summary>
        /// Stores the mouse adorner.
        /// </summary>
        private readonly ScrollAdorner _MouseAdorner;

        /// <summary>
        /// Stores the keyboard adorner
        /// </summary>
        private readonly SpinAdorner _KeyboardAdorner;

        private TextBlock _Prefix;
        private TextBlock _Suffix;

        #endregion


        static SpinText()
        {
            StylesXL.StyleManager.Initialize();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpinText), new FrameworkPropertyMetadata(typeof(SpinText)));
        }

        public SpinText()
        {
            _MouseAdorner = new ScrollAdorner(this);
            _KeyboardAdorner = new SpinAdorner(this);

            FocusVisualStyle = null;

            Focusable = true;
            IsTabStop = true;
        }

        #region Properties: Registration

        /// <summary>
        /// Registers the property to set the interval of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(nameof(Interval), typeof(double), typeof(SpinText), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>
        /// Registers the property to set the maximum value of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(SpinText), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.NotDataBindable, null, CoerceMax));

        /// <summary>
        /// Registers the property to set the minimum value of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(SpinText), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.NotDataBindable, null, CoerceMin));

        /// <summary>
        /// Registers the property to set the prefix of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(SpinText), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>
        /// Registers the property to set the suffix of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(SpinText), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>
        /// Registers the property to set the value of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(SpinText), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChanged));

        /// <summary>
        /// Registers the property to set the displayed text of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(SpinText), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>
        /// Registers the property to display or hide the positive sign of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty ShowPositiveSignProperty = DependencyProperty.Register(nameof(ShowPositiveSign), typeof(bool), typeof(SpinText), new PropertyMetadata(false));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interval of the <see cref="SpinText"/> which also defines the number of decimal places.
        /// </summary>
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum value of the <see cref="SpinText"/>.
        /// </summary>
        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum value of the <see cref="SpinText"/>.
        /// </summary>
        public double Min
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        /// <summary>
        /// Gets or sets the prefix of the <see cref="SpinText"/>.
        /// </summary>
        public string Prefix
        {
            get => (string)GetValue(PrefixProperty);
            set => SetValue(PrefixProperty, value);
        }

        /// <summary>
        /// Gets or sets wheter the positive sign is shown.
        /// </summary>
        public bool ShowPositiveSign
        {
            get { return (bool)GetValue(ShowPositiveSignProperty); }
            set { SetValue(ShowPositiveSignProperty, value); }
        }

        /// <summary>
        /// Gets or sets the suffix of the <see cref="SpinText"/>.
        /// </summary>
        public string Suffix
        {
            get => (string)GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        /// <summary>
        /// Gets the displayed text value of the <see cref="SpinText"/>.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            private set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="SpinText"/>.
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set 
            {
                value = value < Min ? Min : value > Max ? Max : value;

                if (value != Value)
                {
                    SetValue(ValueProperty, value);
                }
            }
        }

        #endregion

        #region Properties: Callbacks

        /// <summary>
        /// Invalidates the <see cref="Min"/> value.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceMin(DependencyObject d, object baseValue)
        {
            SpinText instance = (SpinText)d;

            double value = (double)baseValue;
            double max = instance.Max;

            if (value > max)
            {
                instance.Max = value;
                return max;
            }

            return value;
        }

        /// <summary>
        /// Invalidates the <see cref="Max"/> value.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceMax(DependencyObject d, object baseValue)
        {
            SpinText instance = (SpinText)d;

            double value = (double)baseValue;
            double min = instance.Min;

            if (value < min)
            {
                instance.Min = value;
                return min;
            }

            return value;
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="Value"/> property.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SpinText instance = (SpinText)d;

            double value = (double)e.NewValue;

            instance.Text = value.ToString($"N{instance._Resolution}");

            if (instance.ShowPositiveSign && value > 0)
                instance.Text = $"+{instance.Text}";
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Prefix = GetTemplateChild("Prefix") as TextBlock;
            _Suffix = GetTemplateChild("Suffix") as TextBlock;

            if (string.IsNullOrEmpty(_Prefix.Text))
                _Prefix.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(_Suffix.Text))
                _Suffix.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Initializes the <see cref="SpinText"/> properties.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double interval = Interval;
            int padding = 0;

            _Resolution = Interval.GetDecimalPlaces();

            // Invalidate the value after the min and max properties are set
            if (Value < Min) Value = Min;
            if (Value > Max) Value = Max;


            TextBlock textBlock = new();

            textBlock.Text = Min.ToString($"-N{_Resolution}") + Prefix + Suffix;
            
            padding += string.IsNullOrEmpty(Prefix) ? 0 : 5;
            padding += string.IsNullOrEmpty(Suffix) ? 0 : 5;

            double minWidth = textBlock.GetTextWidth(padding);

            textBlock.Text = Max.ToString($"-N{_Resolution}") + Prefix + Suffix;
            MinWidth = Math.Max(minWidth, textBlock.GetTextWidth(padding));


            // Apply the number of decimal places to the text
            Text = Value.ToString($"N{_Resolution}");

            // Sets the tooltip of the text box "PF 0 ... 100 SF"
            ToolTipService.SetToolTip(this, string.Format("{2} {0} ... {1} {3}", Min, Max, Prefix, Suffix));
        }

        /// <summary>
        /// Handles specialized key presses of the <see cref="NumericTextBox"/>.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> containing event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            bool CTRL_KEY_DOWN = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    Value += CTRL_KEY_DOWN ? Interval * 10 : Interval;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    Value -= CTRL_KEY_DOWN ? Interval * 10 : Interval;
                    break;

                case Key.PageUp:

                        Value += Interval * 10;
                    break;

                case Key.PageDown:

                        Value -= Interval * 10;
                    break;

                case Key.Home:
                    Value = Min;
                    break;

                case Key.End:
                    Value = Max;
                    break;

                case Key.Tab:

                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift) || e.KeyboardDevice.IsKeyToggled(Key.CapsLock))
                    {
                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    }
                    else
                    {
                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }

                    break;

                case Key.Enter:
                case Key.Escape:
                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
                    Keyboard.ClearFocus();
                    break;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the mouse enter event to attach the adorner.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!_HasMouseAdorner)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                layer.Add(_MouseAdorner);

                _HasMouseAdorner = true;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the mouse leave event to release the adorner.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_HasMouseAdorner)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                if (layer != null)
                {
                    layer.Remove(_MouseAdorner);
                }

                _HasMouseAdorner = false;
            }

            e.Handled = true;
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (!_HasKeyboardAdorner)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                layer.Add(_KeyboardAdorner);

                _HasKeyboardAdorner = true;
            }

            e.Handled = true;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (_HasKeyboardAdorner)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                if (layer != null)
                {
                    layer.Remove(_KeyboardAdorner);
                }

                _HasKeyboardAdorner = false;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the mouse up event to set keyboard focus.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Keyboard.Focus(this);
        }

        /// <summary>
        /// Overrides the mouse wheel to increment or decrement the <see cref="Value"/>.
        /// </summary>
        /// <param name="e">A <see cref="MouseWheelEventArgs"/> containing event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            int distance = e.Delta / 100;

            if (distance > 0)
            {
                    Value += Interval;
            }
            else
            {
                    Value -= Interval;
            }
        }

    }
}
