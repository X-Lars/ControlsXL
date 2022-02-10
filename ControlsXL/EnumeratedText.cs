using ControlsXL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ControlsXL"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ControlsXL;assembly=ControlsXL"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:PlainTextSpinner/>
    ///
    /// </summary>
    public class EnumeratedText : TextBox
    {

        private int _Min = 0;
        private int _Max = 0;
        private readonly List<string> _Values = new();

        static EnumeratedText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumeratedText), new FrameworkPropertyMetadata(typeof(EnumeratedText)));
            StylesXL.StyleManager.Initialize();
        }

        public EnumeratedText() : base()
        {
            AcceptsReturn = false;
            AcceptsTab = false;
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(EnumeratedText), new PropertyMetadata(0, SelectedIndexChanged));
        public static readonly DependencyProperty ValueProviderProperty = DependencyProperty.Register(nameof(ValueProvider), typeof(Enum), typeof(EnumeratedText), new PropertyMetadata(null, ValueProviderChanged));
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(EnumeratedText), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(EnumeratedText), new UIPropertyMetadata(string.Empty));

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

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set 
            {
                if (value < _Min)
                    value = _Min;

                if(value > _Max)
                    value = _Max;

                SetValue(SelectedIndexProperty, value); 
            }
        }


        private static void SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnumeratedText text = (EnumeratedText)d;

            if(text.ValueProvider.GetType().IsEnum)
            {

                text.Text = text._Values[(int)e.NewValue];
            }
        }

        private static void ValueProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnumeratedText text = (EnumeratedText)d;

            if (e.NewValue != null)
            {
                if(e.NewValue.GetType().IsEnum)
                {
                    text._Values.Clear();

                    for(int i = 0; i < Enum.GetValues(text.ValueProvider.GetType()).Length; i++)
                    {
                        text._Values.Add((string)TypeDescriptor.GetConverter(e.NewValue).ConvertTo(Enum.GetValues(e.NewValue.GetType()).GetValue(i), typeof(string)));
                    }
                    
                    text._Min = 0;
                    text._Max = Enum.GetValues(text.ValueProvider.GetType()).Length - 1;

                    text.Text = text._Values[text.SelectedIndex];
                }
            }
        }

        public Enum ValueProvider
        {
            get { return (Enum)GetValue(ValueProviderProperty); }
            set { SetValue(ValueProviderProperty, value); }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            bool isCTRLKeyDown = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) ||
                                 e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    SelectedIndex += isCTRLKeyDown ? 10 : 1;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                        SelectedIndex -= isCTRLKeyDown ? 10 : 1;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.PageUp:

                    SelectedIndex = _Max;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.PageDown:

                    SelectedIndex = _Min;

                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.Tab:

                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;

                case Key.Enter:

                    SelectAll();
                    break;
            }
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            int distance = e.Delta / 100;

            if(distance > 0)
            {
                SelectedIndex--;
            }
            else
            {
                SelectedIndex++;
            }
        }
    }
}
