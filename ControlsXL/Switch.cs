using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ControlsXL
{

    public enum SwitchStyles
    {
        Round,
        Square
    }

    [TemplatePart(Name = PART_ON_LABEL)]
    [TemplatePart(Name = PART_OFF_LABEL)]
    public class Switch : ToggleButton
    {
        private const string PART_ON_LABEL = "PART_OnLabel";
        private const string PART_OFF_LABEL = "PART_OffLabel";

        static Switch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch), new FrameworkPropertyMetadata(typeof(Switch)));
            StylesXL.StyleManager.Initialize();
        }



        public double LabelOffset
        {
            get { return (double)GetValue(LabelOffsetProperty); }
            private set { SetValue(LabelOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelOffsetProperty =
            DependencyProperty.Register(nameof(LabelOffset), typeof(double), typeof(Switch), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));




        public string OnLabel
        {
            get { return (string)GetValue(OnLabelProperty); }
            set { SetValue(OnLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnLabelProperty =
            DependencyProperty.Register(nameof(OnLabel), typeof(string), typeof(Switch), new PropertyMetadata("On"));



        public string OffLabel
        {
            get { return (string)GetValue(OffLabelProperty); }
            set { SetValue(OffLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffLabelProperty =
            DependencyProperty.Register(nameof(OffLabel), typeof(string), typeof(Switch), new PropertyMetadata("Off"));



        public bool InlineLabels
        {
            get { return (bool)GetValue(InlineLabelsProperty); }
            set { SetValue(InlineLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InlineLables.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlineLabelsProperty =
            DependencyProperty.Register(nameof(InlineLabels), typeof(bool), typeof(Switch), new PropertyMetadata(true));




        public SwitchStyles SwitchStyle
        {
            get { return (SwitchStyles)GetValue(SwitchStyleProperty); }
            set { SetValue(SwitchStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SwitchStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchStyleProperty =
            DependencyProperty.Register(nameof(SwitchStyle), typeof(SwitchStyles), typeof(Switch), new PropertyMetadata(SwitchStyles.Round));



        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(double), typeof(Switch), new PropertyMetadata(23.0));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //_OnLabel = GetTemplateChild(PART_ON_LABEL) as Label;
            //_OffLabel = GetTemplateChild(PART_OFF_LABEL) as Label;

            //_OnLabel.SizeChanged += _OnLabel_SizeChanged;
            //_OffLabel.SizeChanged += _OffLabel_SizeChanged;
            Loaded += SwitchLoaded;
            
        }

        private void _OffLabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Console.WriteLine(_OffLabel.ActualWidth);
        }

        private void _OnLabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Console.WriteLine(_OnLabel.ActualWidth);
        }

        private void SwitchLoaded(object sender, RoutedEventArgs e)
        {
            LabelOffset = ActualHeight / 3;
            InvalidateVisual();
            Console.WriteLine("label offset" + LabelOffset);
            //_OffLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //_OnLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //Console.WriteLine(_OffLabel.DesiredSize.Width);
            //Console.WriteLine(_OnLabel.DesiredSize.Width);
        }

        //private Label _OffLabel;
        //private Label _OnLabel;

        public double LabelWidth
        {
            get { return (double)GetValue(LabelWidthProperty); }
            private set { SetValue(LabelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register(nameof(LabelWidth), typeof(double), typeof(Switch), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                IsChecked = true;
            }
            else
            {
                IsChecked = false;
            }
        }

    }
}
