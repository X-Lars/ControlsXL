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

    [TemplatePart(Name = PART_THUMB, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_TRACK, Type = typeof(Border))]
    public class MultiRangeSlider : Control
    {
        #region Fields

        private const string PART_THUMB = "PART_Thumb";
        private const string PART_TRACK = "PART_Track";

        private Border _Track;
        private Thumb _Thumb;

        #endregion
        static MultiRangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiRangeSlider), new FrameworkPropertyMetadata(typeof(MultiRangeSlider)));
            StylesXL.StyleManager.Initialize();
        }

        public MultiRangeSlider()
        {
            Loaded += SliderLoaded;
        }

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the maximum value property.
        /// </summary>
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(MultiRangeSlider), new PropertyMetadata(100.0));

        /// <summary>
        /// Registers the minimum value property.
        /// </summary>
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(MultiRangeSlider), new PropertyMetadata(0.0));

        /// <summary>
        /// Registers the position property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(double), typeof(MultiRangeSlider), new PropertyMetadata(0.0, PositionPropertyChanged, CoercePositionProperty));

        /// <summary>
        /// Registers the value property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(MultiRangeSlider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
        /// Gets the slider thumb position.
        /// </summary>
        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            private set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the slider value.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        #endregion

        #region Dependency Properties: Validation

        /// <summary>
        /// Invalidates the thumb position limits.
        /// </summary>
        /// <param name="d">The dependency object providing the property to invalidate.</param>
        /// <param name="baseValue">The value to invalidate.</param>
        /// <returns>The invalidated value.</returns>
        private static object CoercePositionProperty(DependencyObject d, object baseValue)
        {
            MultiRangeSlider slider = (MultiRangeSlider)d;
            double value = (double)baseValue;

            value = Math.Max(value, 0);

            if (slider.Orientation == Orientation.Horizontal)
            {
                value = Math.Min(value, slider.ActualWidth - slider._Thumb.ActualWidth / 2);
            }
            else
            {
                value = Math.Min(value, slider.ActualHeight - slider._Thumb.ActualHeight / 2);
            }

            return value;
        }

        #endregion

        #region Dependency Properties: Callbacks

        /// <summary>
        /// Updates the value when the thumb position is changed.
        /// </summary>
        /// <param name="d">The dependency object providing the property that is changed.</param>
        /// <param name="e"></param>
        private static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiRangeSlider slider = (MultiRangeSlider)d;

            if (slider.Orientation == Orientation.Horizontal)
            {
                slider.Value = (((slider.Max - slider.Min) / (slider.ActualWidth - slider._Thumb.ActualWidth / 2)) * slider.Position) + slider.Min;
            }
            else
            {
                slider.Value = (((slider.Max - slider.Min) / (slider.ActualHeight - slider._Thumb.ActualHeight / 2)) * slider.Position) + slider.Min;
            }

            Console.WriteLine(slider.Value);
        }

        #endregion

        #endregion






        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MultiRangeSlider), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));



        #region Overrides

        /// <summary>
        /// Extends the base method to get the slider template parts.
        /// </summary>
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

            // Initialize height and width
            if (Orientation == Orientation.Horizontal)
            {
                Margin = new Thickness(_Thumb.Width / 2, 0, _Thumb.Width / 2, 0);

                MinHeight = 21;
                MinWidth = 50;
            }
            else
            {
                Margin = new Thickness(0, _Thumb.Height / 2, 0, _Thumb.Height / 2);

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
            //    Position = ((Value / (Max - Min)) * (ActualWidth - _Thumb.ActualWidth)) - Min;
            //}
            //else
            //{
            //    Position = ((Value / (Max - Min)) * (ActualHeight - _Thumb.ActualHeight)) - Min;
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

            double interval;

            if (Orientation == Orientation.Horizontal)
            {
                interval = ((ActualWidth - _Thumb.ActualWidth) / (Max - Min));
            }
            else
            {
                interval = ((ActualHeight - _Thumb.ActualHeight) / (Max - Min));
            }

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

                    if (Orientation == Orientation.Horizontal)
                        Position = ActualWidth;
                    else
                        Position = ActualHeight;
                    e.Handled = true;
                    break;

                case Key.Tab:

                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }

        /// <summary>
        /// Increments or decrements the thumb position based on the mouse wheel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int speed = 2;

            if (e.Delta > 0)
            {
                Position += ((Max - Min) / 100) * speed;
            }
            else
            {
                Position -= ((Max - Min) / 100) * speed;
            }
        }

        /// <summary>
        /// Sets the thumb position based on the mouse down position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
                Position = e.GetPosition(this).X;
            else
                Position = ActualHeight - e.GetPosition(this).Y;
        }

        /// <summary>
        /// Handles dragging of the thumb.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
                Position += e.HorizontalChange;
            else
                Position -= e.VerticalChange;
        }

        #endregion
    }
}
