using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlsXL
{
    [TemplatePart(Name = "PART_Content", Type = typeof(TextBlock))]
    public class EnumeratedText : Control
    {
        #region Fields

        /// <summary>
        /// Dictionary to store the enumeration value and description to the associated <see cref="Index"/>.
        /// </summary>
        /// <remarks><i>
        /// - The key is the index from 0 to the size of the bound enumeration.<br/>
        /// - The key value pair key references the selected enumeration value.<br/>
        /// - The key value pair value references an enumeration to string type converter value if present to override the displayed value.<br/>
        /// </i></remarks>
        private readonly Dictionary<int, KeyValuePair<object, string>> EnumerationValues = new();

        /// <summary>
        /// Stores the minimum index value.
        /// </summary>
        private int _Min = 0;

        /// <summary>
        /// Stores the maximum index value.
        /// </summary>
        private int _Max = 0;

        /// <summary>
        /// Stores the index of the currently selected value.
        /// </summary>
        private int _Index = 0;

        /// <summary>
        /// Stores the type of the bound enumeration.
        /// </summary>
        private Type _Type;

        #endregion

        #region Fields: WPF

        /// <summary>
        /// Stores the template part.
        /// </summary>
        private TextBlock _Text;

        #endregion

        #region Constructor

        static EnumeratedText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumeratedText), new FrameworkPropertyMetadata(typeof(EnumeratedText)));
            StylesXL.StyleManager.Initialize();
        }

        public EnumeratedText() : base()
        {
            Focusable = true;
            IsTabStop = true;
        }

        #endregion

        #region Properties: WPF

        /// <summary>
        /// The actual value of the bound enumaration property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(EnumeratedText), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChanged));

        /// <summary>
        /// The displayed prefix.
        /// </summary>
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(EnumeratedText), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// The displayed suffix.
        /// </summary>
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(EnumeratedText), new UIPropertyMetadata(string.Empty));

        #endregion

        #region Properties

        /// <summary>
        /// Gets the control's displayed text.
        /// </summary>
        public string Text
        {
            get => _Text.Text;
            private set => _Text.Text = value;
        }

        /// <summary>
        /// Gets or sets the displayed prefix.
        /// </summary>
        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        /// <summary>
        /// Gets or sets the displayed suffix.
        /// </summary>
        public string Suffix
        {
            get { return (string)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }

        /// <summary>
        /// Gets or sets the currently selected index.
        /// </summary>
        private int Index
        {
            get => _Index;

            set
            {
                if (value != _Index)
                {
                    if (value < _Min)
                        value = _Min;

                    if (value > _Max)
                        value = _Max;

                    _Index = value;

                    Text  = EnumerationValues[value].Value;
                    Value = EnumerationValues[value];
                }
            }
        }

        /// <summary>
        /// Gets or sets the bound enumeration property's value.
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, EnumerationValues[Index].Key);
        }

        #endregion

        #region Properties: Callbacks

        /// <summary>
        /// Handles the <see cref="Value"/> property changed event.
        /// </summary>
        /// <param name="d">The dependency object that raised the event.</param>
        /// <param name="e">The event's associated data.</param>
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                EnumeratedText instance = (EnumeratedText)d;

                if (e.NewValue.GetType() != instance._Type)
                {
                    // This should in normal circumstances only be excecuted once on binding.

                    if (e.NewValue.GetType().IsEnum)
                    {
                        instance.EnumerationValues.Clear();
                        instance._Type = e.NewValue.GetType();
                        instance._Max = Enum.GetValues(instance._Type).Length - 1;

                        for (int index = 0; index <= instance._Max; index++)
                        {
                            object value = Convert.ChangeType(Enum.GetValues(instance._Type).GetValue(index), instance._Type);
                            string description = (string)TypeDescriptor.GetConverter(e.NewValue).ConvertTo(Enum.GetValues(instance._Type).GetValue(index), typeof(string));

                            KeyValuePair<object, string> enumValue = new KeyValuePair<object, string>(value, description);
                            instance.EnumerationValues.Add(index, enumValue);
                        }

                        instance.Text = instance.EnumerationValues[instance.Index].Value;

                        ToolTipService.SetToolTip(instance, string.Format("{2} {0} ... {1} {3}", instance.EnumerationValues[instance._Min].Value, instance.EnumerationValues[instance._Max].Value, instance.Prefix, instance.Suffix));
                    }
                }
            }
            else
            {
                EnumeratedText instance = (EnumeratedText)d;

                instance.EnumerationValues.Clear();
                instance._Type = null;
                instance._Max = 0;
                instance.Text = string.Empty;

                ToolTipService.SetToolTip(instance, string.Empty);
            }
        }

        #endregion

        #region Override: Control

        /// <summary>
        /// Handles the apply template event to get the template parts of the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Text = GetTemplateChild("PART_Content") as TextBlock;
        }

        /// <summary>
        /// Handles the mouse up event to set keyboard focus.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
        }

        /// <summary>
        /// Handles the preview key down event to change the control's value.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            bool CTRL_KEY_DOWN = e.KeyboardDevice.IsKeyDown(Key.RightCtrl) || e.KeyboardDevice.IsKeyDown(Key.LeftCtrl);

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    Index -= CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    Index += CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.PageDown:
                    
                    Index += 10;
                    break;

                case Key.End:

                    Index = _Max;
                    break;

                case Key.PageUp:

                    Index -= 10;
                    break;

                case Key.Home:

                    Index = _Min;
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
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the mouse wheel event to scroll through the control's values.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            int distance = e.Delta / 100;

            if (distance > 0)
            {
                if (Index > _Min)
                    Index--;
            }
            else
            {
                if (Index < _Max)
                    Index++;
            }

            e.Handled = true;
        }

        #endregion
    }
}
