using ControlsXL.Adorners;
using ControlsXL.Extensions;
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

    public class ListText : Control
    {
        #region Fields

        /// <summary>
        /// Dictionary to store the enumeration value and description.
        /// </summary>
        /// <remarks><i>
        /// - The key is the index from 0 to the size of the bound enumeration.<br/>
        /// </i></remarks>
        private readonly List<string> List = new();

        /// <summary>
        /// Stores the maximum index value.
        /// </summary>
        private int _Max = 0;

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

        #region Constructor

        static ListText()
        {
            StylesXL.StyleManager.Initialize();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListText), new FrameworkPropertyMetadata(typeof(ListText)));
        }

        public ListText() : base()
        {
            _MouseAdorner = new ScrollAdorner(this);
            _KeyboardAdorner = new SpinAdorner(this);

            FocusVisualStyle = null;

            Focusable = true;
            IsTabStop = true;
        }

        #endregion

        #region Properties: WPF

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ListText), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(List<string>), typeof(ListText), new PropertyMetadata(null, ItemsSourceChanged));

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(ListText), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedIndexChanged));
        /// <summary>
        /// Registers the property to set the prefix of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(ListText), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>
        /// Registers the property to set the suffix of the <see cref="SpinText"/>.
        /// </summary>
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(ListText), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        private static void SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListText instance = (ListText)d;

            if(instance.List.Count != 0)
                instance.Text = instance.List[(int)e.NewValue];
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListText instance = (ListText)d;

            if (e.NewValue != null)
            {
                if (e.OldValue == null)
                {
                    TextBlock textBlock = new();
                    double minWidth = 0;
                    
                    var items = (List<string>)e.NewValue;

                    for (int i = 0; i < items.Count; i++)
                    {
                        instance.List.Add(items[i]);

                        textBlock.Text = items[i] + instance.Prefix + instance.Suffix;

                        minWidth = Math.Max(textBlock.GetTextWidth(20), minWidth);
                    }

                    instance.MinWidth = minWidth;
                    instance._Max = items.Count - 1;
                    instance.Text = instance.List[instance.SelectedIndex];

                    ToolTipService.SetToolTip(instance, string.Format("{0} ... {1}", instance.List[0], instance.List[instance._Max]));
                }
            }
            else
            {
                instance.List.Clear();
                instance._Max = 0;
                instance.Text = string.Empty;
                ToolTipService.SetToolTip(instance, string.Empty);
            }
        }
        #endregion

        #region Properties

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set
            {
                value = value < 0 ? 0 : value > _Max ? _Max : value;

                if(SelectedIndex != value)
                { 
                    SetValue(SelectedIndexProperty, value);
                }
            }
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
        /// Gets or sets the suffix of the <see cref="SpinText"/>.
        /// </summary>
        public string Suffix
        {
            get => (string)GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }
        /// <summary>
        /// Gets or sets the list of predefined values of the <see cref="NumericTextBox"/>.
        /// </summary>
        public List<string> ItemsSource
        {
            get { return (List<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
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

        #endregion

        #region Override: Control

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

                    SelectedIndex += CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    SelectedIndex -= CTRL_KEY_DOWN ? 10 : 1;
                    break;

                case Key.PageDown:

                    SelectedIndex -= 10;
                    break;

                case Key.End:

                    SelectedIndex = _Max;
                    break;

                case Key.PageUp:

                    SelectedIndex += 10;
                    break;

                case Key.Home:

                    SelectedIndex = 0;
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
        /// Handles the mouse wheel event to scroll through the control's values.
        /// </summary>
        /// <param name="e">The event's associated data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            int distance = e.Delta / 100;

            if (distance > 0)
            {
                if (SelectedIndex < _Max)
                    SelectedIndex++;
            }
            else
            {
                if (SelectedIndex > 0)
                    SelectedIndex--;
            }
        }

        #endregion
    }
}
