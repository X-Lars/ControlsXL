using ControlsXL.Adorners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ControlsXL
{
    public class EnumeratedText : Control
    {
        #region Fields

        /// <summary>
        /// Dictionary to store the enumeration value and description.
        /// </summary>
        /// <remarks><i>
        /// - The key is the index from 0 to the size of the bound enumeration.<br/>
        /// </i></remarks>
        private readonly Dictionary<int, Descriptor> Enumeration = new();

        /// <summary>
        /// Stores the maximum index value.
        /// </summary>
        private int _Max = 0;

        /// <summary>
        /// Stores the index of the currently selected value.
        /// </summary>
        private int _Index = 0;

        /// <summary>
        /// Tracks wheter the adorner is attached.
        /// </summary>
        private bool _IsAdorned = false;

        /// <summary>
        /// Stores the adorner.
        /// </summary>
        private readonly ScrollAdorner _Adorner;

        #endregion

        #region Constructor

        static EnumeratedText()
        {
            StylesXL.StyleManager.Initialize();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumeratedText), new FrameworkPropertyMetadata(typeof(EnumeratedText)));
        }

        public EnumeratedText() : base()
        {
            _Adorner = new ScrollAdorner(this);

            Focusable = true;
            IsTabStop = true;
        }


        #endregion

        #region Structures

        /// <summary>
        /// Structure to hold an enumeration value and description.
        /// </summary>
        internal struct Descriptor
        {
            /// <summary>
            /// Stores the actual enumeration value.
            /// </summary>
            public object ID;

            /// <summary>
            /// Stores the enumeration value description.
            /// </summary>
            public string Description;
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

        /// <summary>
        /// The displayed text.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(EnumeratedText), new PropertyMetadata(string.Empty));

        #endregion

        #region Properties

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
                    if (value < 0)
                        value = 0;

                    if (value > _Max)
                        value = _Max;

                    if (value != _Index)
                    {
                        _Index = value;

                        Value = Enumeration[value];
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the bound enumeration property's value.
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, Enumeration[Index].ID);
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            private set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Properties: Callbacks

        /// <summary>
        /// Handles the <see cref="Value"/> property changed event.
        /// </summary>
        /// <param name="d">The dependency object that raised the event.</param>
        /// <param name="e">The event's associated data.</param>
        /// <remarks><i>
        /// - If the new value not is null, the old value is checked.<br/>
        /// - If the old value is null, the enumeration values and descriptions are stored.<br/>
        /// - If the old value not is null, the index and text properties are set.<br/>
        /// - If the new value is null, the properties are reset.<br/>
        /// </i></remarks>
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                EnumeratedText instance = (EnumeratedText)d;

                if (e.OldValue == null)
                {
                    // This should in normal circumstances only be excecuted once on binding.
                    if (e.NewValue.GetType().IsEnum)
                    {
                        Type  type   = e.NewValue.GetType();
                        Array values = Enum.GetValues(type);

                        instance.Enumeration.Clear();
                        instance._Max = values.Length - 1;

                        for (int i = 0; i <= instance._Max; i++)
                        {
                            Descriptor descriptor = new();

                            descriptor.ID = Convert.ChangeType(values.GetValue(i), type);
                            descriptor.Description = (string)TypeDescriptor.GetConverter(e.NewValue).ConvertTo(values.GetValue(i), typeof(string));

                            instance.Enumeration.Add(i, descriptor);
                        }

                        // Gets the dictionary key based on the actual enumeration value
                        // Backing field is used because the Index property also changes the Value property
                        instance._Index = instance.Enumeration.Where(x => x.Value.ID.ToString() == e.NewValue.ToString()).Select(x => x.Key).FirstOrDefault();
                        instance.Text = instance.Enumeration[instance.Index].Description;

                        ToolTipService.SetToolTip(instance, string.Format("{2} {0} ... {1} {3}", instance.Enumeration[0].Description, instance.Enumeration[instance._Max].Description, instance.Prefix, instance.Suffix));
                    }
                    else
                    {
                        throw new Exception($"[{nameof(EnumeratedText)}]\nThe {nameof(Value)} property can only be bound to an enumeration.");
                    }
                }
                else if (e.OldValue.ToString() != e.NewValue.ToString())
                {
                    // Gets the dictionary key based on the actual enumeration value
                    // Backing field is used because the Index property also changes the Value property
                    instance._Index = instance.Enumeration.Where(x => x.Value.ID.ToString() == e.NewValue.ToString()).Select(x => x.Key).FirstOrDefault();
                    instance.Text = instance.Enumeration[instance.Index].Description;
                }
            }
            else
            {
                // Reset the instance properties
                EnumeratedText instance = (EnumeratedText)d;

                instance.Enumeration.Clear();
                instance._Max = 0;
                instance.Text = string.Empty;

                ToolTipService.SetToolTip(instance, string.Empty);
            }
        }

        #endregion

        #region Override: Control

        /// <summary>
        /// Handles the mouse enter event to attach the adorner.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!_IsAdorned)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                layer.Add(_Adorner);

                _IsAdorned = true;
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

            if (_IsAdorned)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                if (layer != null)
                {
                    layer.Remove(_Adorner);
                }

                _IsAdorned = false;
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
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

                    Index += CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    Index -= CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.PageDown:
                    
                    Index -= 10;
                    break;

                case Key.End:

                    Index = _Max;
                    break;

                case Key.PageUp:

                    Index += 10;
                    break;

                case Key.Home:

                    Index = 0;
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
                if (Index < _Max)
                    Index++;
            }
            else
            {
                if (Index > 0)
                    Index--;
            }
        }

        #endregion
    }
}
