using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlsXL
{
    public enum SliderThumbStyle
    {
        Square,
        Round
    }

    [TemplatePart(Name = PART_THUMB, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_TRACK, Type = typeof(Border))]
    public class Slider : Control
    {
        

        private const string PART_THUMB = "PART_Thumb";
        private const string PART_TRACK = "PART_Track";

        #region Fields

        private Border _Track;
        private Thumb _Thumb;

        private double _Resolution = 0;

        #endregion

        #region Constructor

        static Slider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Slider), new FrameworkPropertyMetadata(typeof(Slider)));

            StylesXL.StyleManager.Initialize();
        }

        public Slider()
        {
            if (Orientation == Orientation.Horizontal)
            {
                Margin = new Thickness(TrackSize, 0, TrackSize, 0);
            }
            else if (Orientation == Orientation.Vertical)
            {
                Margin = new Thickness(0, TrackSize, 0, TrackSize);
            }

            Loaded += SliderLoaded;
        }

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration
        
        /// <summary>
        /// Registers the property to set the interval.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(nameof(Interval), typeof(double), typeof(Slider), new PropertyMetadata(1.0));

        /// <summary>
        /// Registers the property to set the maximum value.
        /// </summary>
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(Slider), new PropertyMetadata(100.0));

        /// <summary>
        /// Registers the property to set the minimum value.
        /// </summary>
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(Slider), new PropertyMetadata(0.0));

        /// <summary>
        /// Registers the property to set the orientation.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Slider), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Registers the property to set the position of the <see cref="Slider"/> thumb.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Registers the property to set the style of the <see cref="Slider"/> thumb.
        /// </summary>
        public static readonly DependencyProperty ThumbStyleProperty = DependencyProperty.Register(nameof(ThumbStyle), typeof(SliderThumbStyle), typeof(Slider), new PropertyMetadata(SliderThumbStyle.Square));

        /// <summary>
        /// Registers the property to set the value text.
        /// </summary>
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register(nameof(ValueText), typeof(string), typeof(Slider), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set the slider track size.
        /// </summary>
        public static readonly DependencyProperty TrackSizeProperty = DependencyProperty.Register(nameof(TrackSize), typeof(double), typeof(Slider), new PropertyMetadata(5.0));

        /// <summary>
        /// Registers the property to set the value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(Slider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValuePropertyChanged, CoerceValueProperty));

       

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        /// <summary>
        /// Get or sets the minimum value.
        /// </summary>
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set 
            { 
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the <see cref="Slider"/> thumb.
        /// </summary>
        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            private set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style of the <see cref="Slider"/> thumb.
        /// </summary>
        public SliderThumbStyle ThumbStyle
        {
            get { return (SliderThumbStyle)GetValue(ThumbStyleProperty); }
            set { SetValue(ThumbStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the track size.
        /// </summary>
        public double TrackSize
        {
            get { return (double)GetValue(TrackSizeProperty); }
            set { SetValue(TrackSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                // Coerce value bug
                value = Math.Max(value, Min);
                value = Math.Min(value, Max);

                SetValue(ValueProperty, value);
                SetPosition();
            }
        }

        /// <summary>
        /// Gets the value text.
        /// </summary>
        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            private set { SetValue(ValueTextProperty, value); }
        }

        #endregion

        #region Dependency Properties: Callbacks

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider slider = (Slider)d;
            double value = (double)e.NewValue;

            value = Math.Max(value, slider.Min);
            value = Math.Min(value, slider.Max);

            slider.ValueText = value.ToString("N" + slider._Resolution.ToString());

            slider.SetPosition();
        }

        /// <summary>
        /// Invalidates the <see cref="ValueProperty"/> to be in range.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceValueProperty(DependencyObject d, object baseValue)
        {
            Slider slider = (Slider)d;
            double value = (double)baseValue;

            value = Math.Max(value, slider.Min);
            value = Math.Min(value, slider.Max);

            return value;
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// Extends the base method to get and bind the template parts of the <see cref="Slider"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Track = GetTemplateChild(PART_TRACK) as Border;

            _Track.PreviewMouseDown += TrackPreviewMouseDown;
            _Track.PreviewMouseWheel += TrackPreviewMouseWheel;

            _Thumb = GetTemplateChild(PART_THUMB) as Thumb;

            // Prevent stopping mouse wheel action when mouse is over thumb
            _Thumb.PreviewMouseWheel += TrackPreviewMouseWheel;
            _Thumb.DragDelta += ThumbDragDelta;
            _Thumb.PreviewKeyDown += ThumbPreviewKeyDown;

        }

        #endregion

        #region Methods

        private void Initialize()
        {
            Value = Math.Min(Value, Max);
            Value = Math.Max(Value, Min);

            _Thumb.Width = TrackSize * 2;
            _Thumb.Height = TrackSize * 2;

            if (Orientation == Orientation.Horizontal)
            {

                MinWidth = 50;
                MinHeight = _Thumb.Height;

                _Thumb.Margin = new Thickness(-TrackSize, -(_Thumb.Height - TrackSize) / 2, 0, 0);

                Margin = new Thickness(TrackSize, 0, TrackSize, 0);
            }
            else
            {
                MinWidth = _Thumb.Width;
                MinHeight = 50;

                _Thumb.Margin = new Thickness(-(_Thumb.Width - TrackSize) / 2, -TrackSize, 0, -TrackSize);

                Margin = new Thickness(0, TrackSize, 0, TrackSize);
            }

            SetPosition();
        }

       
        private void SetPosition()
        {
            if (Orientation == Orientation.Horizontal)
            {
                Position = (ActualWidth / (Max - Min)) * (Value - Min);
            }
            else
            {
                Position = (ActualHeight / (Max - Min)) * (Value - Min);
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnRender(DrawingContext drawingContext)
        {
            SetPosition();

            base.OnRender(drawingContext);
        }

        private void SliderLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SliderLoaded;

            Initialize();
        }

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
            ValueText = Value.ToString("N" + _Resolution.ToString());
        }

        private void ThumbPreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isCTRLKeyDown = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    Value += isCTRLKeyDown ? Interval * 10 : Interval;
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    Value -= isCTRLKeyDown ? Interval * 10 : Interval;
                    e.Handled = true;
                    break;

                case Key.PageUp:

                    Value += Interval * 10;
                    e.Handled = true;
                    break;

                case Key.PageDown:

                    Value -= Interval * 10;
                    e.Handled = true;
                    break;

                case Key.Home:

                    Value = Min;
                    e.Handled = true;
                    break;

                case Key.End:

                    Value = Max;
                    e.Handled = true;
                    break;

                case Key.Tab:

                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }

        private void TrackPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (e.Delta > 0)
                {
                    Value += Interval;
                }
                else
                {
                    Value -= Interval;
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    Value += Interval;
                }
                else
                {
                    Value -= Interval;
                }
            }
        }

        private void TrackPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
            {
                Value = e.GetPosition(this).X / (ActualWidth / (Max - Min));
            }
            else
            {
                Value = (ActualHeight - e.GetPosition(this).Y) / (ActualHeight / (Max - Min));
            }
        }

      
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
            {
                Value += e.HorizontalChange / (ActualWidth / (Max - Min));
            }
            else
            {
                Value -= e.VerticalChange / (ActualHeight / (Max - Min));
            }
        }

        #endregion
    }
}
