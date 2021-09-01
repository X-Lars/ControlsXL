using System;
using System.Windows;
using System.Windows.Controls;

namespace ControlsXL
{
    /// <summary>
    /// 
    /// </summary>
    public class CircularProgress : ProgressBar
    {
        #region Constructor

        static CircularProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgress), new FrameworkPropertyMetadata(typeof(CircularProgress)));
            StylesXL.StyleManager.Initialize();
        }

        public CircularProgress()
        {
            DataContext = this;
        }
        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

      
        public double IndeterminateAngle
        {
            get { return (double)GetValue(IndeterminateAngleProperty); }
            private set { SetValue(IndeterminateAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndeterminateAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndeterminateAngleProperty = DependencyProperty.Register(nameof(IndeterminateAngle), typeof(double), typeof(CircularProgress), new PropertyMetadata(0.0));



        /// <summary>
        /// Registers the property to set the angle representing the <see cref="CircularProgress"/> progress.
        /// </summary>
        public static readonly DependencyProperty AngleOffsetProperty = DependencyProperty.Register(nameof(AngleOffset), typeof(double), typeof(CircularProgress), new PropertyMetadata(0.0));

        /// <summary>
        /// Registers the property to set the angle representing the max value of the <see cref="CircularProgress"/>.
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(CircularProgress), new FrameworkPropertyMetadata(359.999, FrameworkPropertyMetadataOptions.None, null, CoerceEndAngleProperty, true));

        /// <summary>
        /// Registers the property to determin the value visibility.
        /// </summary>
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register(nameof(ShowValue), typeof(bool), typeof(CircularProgress), new PropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the start angle of the <see cref="CircularProgress"/> indicator.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(CircularProgress), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.None, null, CoerceStartAngleProperty, false));

        /// <summary>
        /// Registers the property to set the text representing the <see cref="CircularProgress"/> progress.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(CircularProgress), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set the thickness of the indicator.
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(CircularProgress), new PropertyMetadata(5.0));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the angle representing the <see cref="CircularProgress"/> progress.
        /// </summary>
        public double AngleOffset
        {
            get { return EndAngle * (Value / (Maximum - Minimum)); }
            private set { SetValue(AngleOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle representing the max value of the <see cref="CircularProgress"/>.
        /// </summary>
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the progress value is shown.
        /// </summary>
        public bool ShowValue
        {
            get { return (bool)GetValue(ShowValueProperty); }
            set { SetValue(ShowValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start angle in degrees of the <see cref="CircularProgress"/> indicator.
        /// </summary>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text representing the <see cref="CircularProgress"/> progress.
        /// </summary>
        private string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the thickness of the indicator.
        /// </summary>
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        #endregion

        #region Dependency Properties: Invalidation

        /// <summary>
        /// Invalidates the <see cref="StartAngleProperty"/>.
        /// </summary>
        /// <param name="d">The dependency object providing the value to invalidate.</param>
        /// <param name="baseValue">The value of the dependency object to invalidate.</param>
        /// <returns>The invalidated value.</returns>
        private static object CoerceStartAngleProperty(DependencyObject d, object baseValue)
        {
            CircularProgress progress = (CircularProgress)d;

            double value = (double)baseValue;

            value = Math.Min(value, progress.EndAngle);
            value = Math.Max(value, 0);

            return value;
        }

        /// <summary>
        /// Invalidates the <see cref="EndAngleProperty"/>.
        /// </summary>
        /// <param name="d">The dependency object providing the value to invalidate.</param>
        /// <param name="baseValue">The value of the dependency object to invalidate.</param>
        /// <returns>The invalidated value.</returns>
        private static object CoerceEndAngleProperty(DependencyObject d, object baseValue)
        {
            CircularProgress progress = (CircularProgress)d;

            double value = (double)baseValue;

            value = Math.Max(value, progress.StartAngle);
            value = Math.Min(value, 359.999);

            return value;
        }

        #endregion

        #endregion

        #region Overrides

        
        /// <summary>
        /// Extends the OnValueChanged method to set the angle and text properties of the <see cref="CircularProgress"/>.
        /// </summary>
        /// <param name="oldValue">The previous progress value.</param>
        /// <param name="newValue">The new progress value.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (!IsIndeterminate)
            {
                //IndeterminateAngle = 0.0;
                AngleOffset = EndAngle * (Value / (Maximum - Minimum));
                Text = $"{Math.Round(Value, 0)} %";
            }
            else
            {
                // Rotation of the control
                IndeterminateAngle = 360 * (Value / 100);
            }
        }

        #endregion
    }
}
