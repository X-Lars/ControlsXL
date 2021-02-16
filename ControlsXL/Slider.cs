using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ControlsXL
{

    [TemplatePart(Name = PART_THUMB, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_TRACK, Type = typeof(Border))]
    public class Slider : Control
    {
        private const string PART_THUMB = "PART_Thumb";
        private const string PART_TRACK = "PART_Track";

        private Border _Track;
        private Thumb _Thumb;

        static Slider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Slider), new FrameworkPropertyMetadata(typeof(Slider)));
        }

        public Slider()
        {
            
        }

       
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(double), typeof(Slider), new PropertyMetadata(0.0));



        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(double), typeof(Slider), new PropertyMetadata(100.0));



        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }




        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Slider), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));




        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(Slider), new PropertyMetadata(0.0));


        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(double), typeof(Slider), new PropertyMetadata(0.0, PositionPropertyChanged, CoercePositionProperty));

        private static object CoercePositionProperty(DependencyObject d, object baseValue)
        {
            Slider slider = (Slider)d;
            double value = (double)baseValue;

            value = Math.Max(value, 0);
            value = Math.Min(value, slider.ActualWidth - slider._Thumb.ActualWidth / 2);

            return value;
        }

        private static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider slider = (Slider)d;

            slider.Value = (((slider.Max - slider.Min) / (slider.ActualWidth - slider._Thumb.ActualWidth / 2)) * slider.Position) + slider.Min;

            Console.WriteLine(slider.Value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Track = GetTemplateChild(PART_TRACK) as Border;
            _Track.PreviewMouseDown += TrackPreviewMouseDown;
            _Track.PreviewMouseWheel += TrackPreviewMouseWheel;

            _Thumb = GetTemplateChild(PART_THUMB) as Thumb;
            _Thumb.DragDelta += ThumbDragDelta;
            _Thumb.PreviewMouseWheel += TrackPreviewMouseWheel;
            _Thumb.PreviewKeyDown += ThumbPreviewKeyDown;
        }

        private void ThumbPreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isCTRLKeyDown = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            int interval = (int)((ActualWidth - _Thumb.ActualWidth) / (Max - Min));

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    Position += isCTRLKeyDown ? interval * 10 : interval;
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    Position -= isCTRLKeyDown ? interval * 10 : interval;
                    e.Handled = true;
                    break;

                case Key.PageUp:

                    Position += interval * 10;
                    e.Handled = true;
                    break;

                case Key.PageDown:

                    Position -= interval * 10;
                    e.Handled = true;
                    break;

                case Key.Home:

                    Position = 0;
                    e.Handled = true;
                    break;

                case Key.End:

                    Position = ActualWidth;
                    e.Handled = true;
                    break;

                case Key.Tab:

                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }

        private void TrackPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int speed = 2;

            if(e.Delta > 0)
            {
                Position += ((Max - Min) / 100) * speed;
            }
            else
            {
                Position -= ((Max - Min) / 100) * speed;
            }
        }

        private void TrackPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Position = e.GetPosition(this).X;
        }

      
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Position += e.HorizontalChange;
        }

    }
}
