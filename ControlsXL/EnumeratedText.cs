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

        private int Min = 0;
        private int Max = 0;
        private int Index = 0;

        private readonly Dictionary<int, KeyValuePair<object, string>> EnumerationValues = new();

        private Type EnumerationType;
        private TextBlock _Text;
        private Border _Border;

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Text = GetTemplateChild("PART_Content") as TextBlock;
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(EnumeratedText), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueProviderChanged));
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(EnumeratedText), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(EnumeratedText), new UIPropertyMetadata(string.Empty));

        public string Text
        {
            get => _Text.Text;
            private set => _Text.Text = value;
        }

        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        public string Suffix
        {
            get { return (string)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }

        private int SelectedIndex
        {
            get => Index;

            set
            {
                if (value != Index)
                {
                    if (value < Min)
                        value = Min;

                    if (value > Max)
                        value = Max;

                    Index = value;
                    Text = EnumerationValues[value].Value;
                    Value = EnumerationValues[value];
                }
            }
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, EnumerationValues[SelectedIndex].Key);
        }


        private static void ValueProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                EnumeratedText instance = (EnumeratedText)d;

                if (e.NewValue.GetType() != instance.EnumerationType)
                {
                    if (e.NewValue.GetType().IsEnum)
                    {
                        instance.EnumerationValues.Clear();
                        instance.EnumerationType = e.NewValue.GetType();
                        instance.Max = Enum.GetValues(instance.EnumerationType).Length - 1;

                        for (int index = 0; index <= instance.Max; index++)
                        {
                            object value = Convert.ChangeType(Enum.GetValues(instance.EnumerationType).GetValue(index), instance.EnumerationType);
                            string description = (string)TypeDescriptor.GetConverter(e.NewValue).ConvertTo(Enum.GetValues(instance.EnumerationType).GetValue(index), typeof(string));

                            KeyValuePair<object, string> enumValue = new KeyValuePair<object, string>(value, description);
                            instance.EnumerationValues.Add(index, enumValue);
                        }

                        instance.Text = instance.EnumerationValues[instance.SelectedIndex].Value;
                    }
                }
            }
        }

        #region Event Handlers

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Keyboard.Focus(this);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            bool CTRL_KEY_DOWN = e.KeyboardDevice.IsKeyDown(Key.RightCtrl) || e.KeyboardDevice.IsKeyDown(Key.LeftCtrl);

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    SelectedIndex += CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    SelectedIndex -= CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.PageDown:
                case Key.End:

                    SelectedIndex = Max;
                    break;

                case Key.PageUp:
                case Key.Home:

                    SelectedIndex = Min;
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            int distance = e.Delta / 100;

            if (distance > 0)
            {
                if (SelectedIndex > Min)
                    SelectedIndex--;
            }
            else
            {
                if (SelectedIndex < Max)
                    SelectedIndex++;
            }
        }

        #endregion
    }
}
