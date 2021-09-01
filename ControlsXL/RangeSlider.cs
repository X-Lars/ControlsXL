using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlsXL
{
    [TemplatePart(Name = PART_MIN_THUMB, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_MAX_THUMB, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_TRACK, Type = typeof(Border))]
    public class RangeSlider : Control
    {
        #region Fields

        private const string PART_MIN_THUMB = "PART_MinThumb";
        private const string PART_MAX_THUMB = "PART_MaxThumb";
        private const string PART_TRACK = "PART_Track";

        private Border _Track;
        private Thumb _MinThumb;
        private Thumb _MaxThumb;

        #endregion

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
            StylesXL.StyleManager.Initialize();
        }

        public RangeSlider()
        {
            Loaded += SliderLoaded;
        }

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the maximum value property.
        /// </summary>
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(RangeSlider), new PropertyMetadata(100.0));

        /// <summary>
        /// Registers the minimum value property.
        /// </summary>
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(RangeSlider), new PropertyMetadata(0.0));

        /// <summary>
        /// Registers the min position property.
        /// </summary>
        public static readonly DependencyProperty MinPositionProperty = DependencyProperty.Register(nameof(MinPosition), typeof(double), typeof(RangeSlider), new PropertyMetadata(0.0, MinPositionPropertyChanged, CoerceMinPositionProperty));

        /// <summary>
        /// Registers the min position property.
        /// </summary>
        public static readonly DependencyProperty MaxPositionProperty = DependencyProperty.Register(nameof(MaxPosition), typeof(double), typeof(RangeSlider), new PropertyMetadata(100.0, MaxPositionPropertyChanged, CoerceMaxPositionProperty));

        /// <summary>
        /// Registers the value property.
        /// </summary>
        public static readonly DependencyProperty RangeProperty = DependencyProperty.Register(nameof(Range), typeof(double), typeof(RangeSlider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));





        public double TrackSize
        {
            get { return (double)GetValue(TrackSizeProperty); }
            set { SetValue(TrackSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackSizeProperty =
            DependencyProperty.Register(nameof(TrackSize), typeof(double), typeof(RangeSlider), new PropertyMetadata(5.0));



        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(RangeSlider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MinValuePropertyChanged, CoerceMinValueProperty, false));

        

        private static void MinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(RangeSlider), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MaxValuePropertyChanged, CoerceMaxValueProperty, false));

        

        private static void MaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }




        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        /// <summary>
        /// Gets the slider min thumb position.
        /// </summary>
        public double MinPosition
        {
            get { return (double)GetValue(MinPositionProperty); }
            private set { SetValue(MinPositionProperty, value); }
        }

        /// <summary>
        /// Gets the slider max thumb position.
        /// </summary>
        public double MaxPosition
        {
            get { return (double)GetValue(MaxPositionProperty); }
            private set { SetValue(MaxPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the slider value.
        /// </summary>
        public double Range
        {
            get { return (double)GetValue(RangeProperty); }
            set { SetValue(RangeProperty, value); }
        }


        #endregion

        #region Dependency Properties: Validation

        /// <summary>
        /// Invalidates the thumb position limits.
        /// </summary>
        /// <param name="d">The dependency object providing the property to invalidate.</param>
        /// <param name="baseValue">The value to invalidate.</param>
        /// <returns>The invalidated value.</returns>
        private static object CoerceMinPositionProperty(DependencyObject d, object baseValue)
        {
            RangeSlider slider = (RangeSlider)d;
            double value = (double)baseValue;

            value = Math.Max(value, 0);

            if (slider.Orientation == Orientation.Horizontal)
            {
                value = Math.Min(value, slider.MaxPosition - (slider._MaxThumb.ActualWidth / 2) - (slider._MinThumb.ActualWidth / 2) - slider._MinThumb.Margin.Left);
            }
            else
            {
                value = Math.Min(value, slider.MaxPosition - (slider._MaxThumb.ActualHeight / 2) - (slider._MinThumb.ActualHeight / 2));
            }

            return value;
        }

        /// <summary>
        /// Invalidates the thumb position limits.
        /// </summary>
        /// <param name="d">The dependency object providing the property to invalidate.</param>
        /// <param name="baseValue">The value to invalidate.</param>
        /// <returns>The invalidated value.</returns>
        private static object CoerceMaxPositionProperty(DependencyObject d, object baseValue)
        {
            RangeSlider slider = (RangeSlider)d;
            double value = (double)baseValue;

            if (slider.Orientation == Orientation.Horizontal)
            {
                value = Math.Min(value, slider.ActualWidth - (slider._MaxThumb.ActualWidth / 2) - slider._MaxThumb.Margin.Right);
                value = Math.Max(value, slider.MinPosition + (slider._MinThumb.ActualWidth / 2) + (slider._MaxThumb.ActualWidth / 2) + slider._MinThumb.Margin.Left);
            }
            else
            {
                value = Math.Min(value, slider.ActualWidth - (slider._MaxThumb.ActualWidth));
                value = Math.Max(value, slider.MinPosition + (slider._MinThumb.ActualHeight / 2) + (slider._MaxThumb.ActualHeight / 2));
            }

            return value;
        }

        private static object CoerceMinValueProperty(DependencyObject d, object baseValue)
        {
            RangeSlider slider = (RangeSlider)d;
            double value = (double)baseValue;

            value = Math.Max(value, slider.Min);
            value = Math.Min(value, slider.MaxValue);

            return value;
        }

        private static object CoerceMaxValueProperty(DependencyObject d, object baseValue)
        {
            RangeSlider slider = (RangeSlider)d;
            double value = (double)baseValue;

            value = Math.Min(value, slider.Max);
            value = Math.Max(value, slider.MinValue);

            return value;
        }

        #endregion

        #region Dependency Properties: Callbacks

        /// <summary>
        /// Updates the value when the thumb position is changed.
        /// </summary>
        /// <param name="d">The dependency object providing the property that is changed.</param>
        /// <param name="e"></param>
        private static void MinPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;

            if (slider.Orientation == Orientation.Horizontal)
            {
                //double o = slider._MinThumb.ActualWidth + slider._MaxThumb.ActualWidth;
                //double w = slider.ActualWidth - o;

                //double r = slider.Max - slider.Min;

                //double f = r / w;
                //// min pos 0 - 85
                //Console.WriteLine($"ACT = {w}");
                //Console.WriteLine($"FAC = {f}");
                //Console.WriteLine($"VAL = {(slider.MinPosition) * f}");

                slider.MinValue = ((slider.Max - slider.Min) / (slider.ActualWidth - slider._MinThumb.ActualWidth - slider._MaxThumb.ActualWidth) * slider.MinPosition) + slider.Min;
                slider.Range = (slider.MaxValue - slider.MinValue);
            }
            else
            {
                slider.MinValue = (((slider.Max - slider.Min) / (slider.ActualHeight - slider.Margin.Bottom - slider._MinThumb.ActualHeight / 2 - slider._MaxThumb.ActualHeight / 2)) * slider.MinPosition) + slider.Min;
                slider.Range = (slider.MaxValue - slider.MinValue);
            }

            Console.WriteLine($"MinValue: {slider.MinValue}");
            Console.WriteLine($"Range: {Math.Floor(slider.Range)}");
        }

        /// <summary>
        /// Updates the value when the thumb position is changed.
        /// </summary>
        /// <param name="d">The dependency object providing the property that is changed.</param>
        /// <param name="e"></param>
        private static void MaxPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;

            if (slider.Orientation == Orientation.Horizontal)
            {
                //double o = slider._MaxThumb.ActualWidth + slider._MinThumb.ActualWidth;
                //double w = slider.ActualWidth - o;

                //double r = slider.Max - slider.Min;

                //double f = r / w;
                //// min pos 0 - 85
                //Console.WriteLine($"ACT = {w}");
                //Console.WriteLine($"FAC = {f}");
                //Console.WriteLine($"VAL = {((slider.MaxPosition - slider._MaxThumb.ActualWidth) * f)}");

                slider.MaxValue = ((slider.Max - slider.Min) / (slider.ActualWidth  - slider._MinThumb.ActualWidth - slider._MaxThumb.ActualWidth) * (slider.MaxPosition - slider._MaxThumb.ActualWidth)) + slider.Min;
                slider.Range = (slider.MaxValue - slider.MinValue);
            }
            else
            {
                slider.MaxValue= (((slider.Max - slider.Min) / (slider.ActualHeight - slider._MinThumb.ActualWidth / 2 - slider._MaxThumb.ActualHeight / 2)) * slider.MaxPosition) + slider.Min;
                slider.Range = (slider.MaxValue - slider.MinValue);
            }


            Console.WriteLine($"MaxValue: {slider.MaxValue}");
            Console.WriteLine($"Range: {Math.Floor(slider.Range)}");
        }

        #endregion

        #endregion






        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RangeSlider), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));



        #region Overrides

        /// <summary>
        /// Extends the base method to get the slider template parts.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Track = GetTemplateChild(PART_TRACK) as Border;
            //_Track.PreviewMouseDown += TrackPreviewMouseDown;

            _MinThumb = GetTemplateChild(PART_MIN_THUMB) as Thumb;

            _MinThumb.DragDelta += ThumbDragDelta;
            _MinThumb.PreviewKeyDown += ThumbPreviewKeyDown;

            _MaxThumb = GetTemplateChild(PART_MAX_THUMB) as Thumb;
            _MaxThumb.DragDelta += ThumbDragDelta;
            _MaxThumb.PreviewKeyDown += ThumbPreviewKeyDown;


            // Initialize height and width
            if (Orientation == Orientation.Horizontal)
            {
                _MinThumb.Height = TrackSize * 3;
                _MaxThumb.Height = TrackSize * 3;

                _MinThumb.Margin = new Thickness(-(_MinThumb.Width / 2), -(_MinThumb.Height / 3), 0, 0);
                _MaxThumb.Margin = new Thickness(0, -(_MaxThumb.Height / 3), -(_MaxThumb.Width / 2), 0);

                Margin = new Thickness(_MinThumb.Width / 2, 0, _MaxThumb.Width / 2, 0);

                MinHeight = 21;
                MinWidth = 50;
            }
            else
            {
                _MinThumb.Width = TrackSize * 3;
                _MaxThumb.Width = TrackSize * 3;

                Margin = new Thickness(0, _MaxThumb.Height / 2, 0, _MinThumb.Height / 2);

                MinHeight = 50;
                MinWidth = 21;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Initializes the slide thumb position on load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderLoaded(object sender, RoutedEventArgs e)
        {
            //if (Orientation == Orientation.Horizontal)
            //{
            //    MinPosition = ((MinValue / (Max - Min)) * (ActualWidth - (_MinThumb.ActualWidth / 2))) - Min;
            //    MaxPosition = ((MaxValue / (Max - Min)) * (ActualWidth - (_MaxThumb.ActualWidth / 2))) - Min;
            //}
            //else
            //{
            //    MinPosition = ((MinValue / (Max - Min)) * (ActualHeight - (_MinThumb.ActualHeight / 2))) - Min;
            //    MaxPosition = ((MaxValue / (Max - Min)) * (ActualHeight - (_MaxThumb.ActualHeight / 2))) - Min;
            //}
        }

        /// <summary>
        /// Sets the thumb position based on the pressed key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbPreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isCTRLKeyDown = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            double interval = 0;

            
            //if (Orientation == Orientation.Horizontal)
            //{
            //    interval = ((ActualWidth - _Thumb.ActualWidth) / (Max - Min));
            //}
            //else
            //{
            //    interval = ((ActualHeight - _Thumb.ActualHeight) / (Max - Min));
            //}

            switch (e.Key)
            {
                case Key.Add:
                case Key.Up:
                case Key.OemPlus:

                    MinPosition += isCTRLKeyDown ? interval * 10 : interval;
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.Down:
                case Key.OemMinus:

                    MinPosition -= isCTRLKeyDown ? interval * 10 : interval;
                    e.Handled = true;
                    break;

                case Key.PageUp:

                    MinPosition += interval * 10;
                    e.Handled = true;
                    break;

                case Key.PageDown:

                    MinPosition -= interval * 10;
                    e.Handled = true;
                    break;

                case Key.Home:

                    MinPosition = 0;
                    e.Handled = true;
                    break;

                case Key.End:

                    if (Orientation == Orientation.Horizontal)
                        MinPosition = ActualWidth;
                    else
                        MinPosition = ActualHeight;
                    e.Handled = true;
                    break;

                case Key.Tab:

                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }

        ///// <summary>
        ///// Increments or decrements the thumb position based on the mouse wheel.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TrackPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    int speed = 2;

        //    if (e.Delta > 0)
        //    {
        //        MinPosition += ((Max - Min) / 100) * speed;
        //    }
        //    else
        //    {
        //        MinPosition -= ((Max - Min) / 100) * speed;
        //    }
        //}

        ///// <summary>
        ///// Sets the thumb position based on the mouse down position.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TrackPreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
            
        //    if (Orientation == Orientation.Horizontal)
        //        MinPosition = e.GetPosition(this).X;
        //    else
        //        MinPosition = ActualHeight - e.GetPosition(this).Y;
        //}

        /// <summary>
        /// Handles dragging of the thumb.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender == _MinThumb)
            {
                if (Orientation == Orientation.Horizontal)
                    MinPosition += e.HorizontalChange;
                else
                    MinPosition -= e.VerticalChange;
            }
            else if (sender == _MaxThumb)
            {
                if (Orientation == Orientation.Horizontal)
                    MaxPosition += e.HorizontalChange;
                else
                    MaxPosition -= e.VerticalChange;
            }
        }

        #endregion
    }
}
