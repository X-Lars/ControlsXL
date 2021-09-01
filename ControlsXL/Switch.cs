using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ControlsXL
{
    /// <summary>
    /// Defines all available <see cref="Switch"/> styles .
    /// </summary>
    public enum SwitchStyles
    {
        Round,
        Square
    }

    /// <summary>
    /// 
    /// </summary>
    public class Switch : ToggleButton
    {
        #region Constructore

        /// <summary>
        /// Static constructor to set the <see cref="Switch"/> style.
        /// </summary>
        static Switch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch), new FrameworkPropertyMetadata(typeof(Switch)));
            StylesXL.StyleManager.Initialize();
        }

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the labels inline.
        /// </summary>
        public static readonly DependencyProperty InlineLabelsProperty = DependencyProperty.Register(nameof(InlineLabels), typeof(bool), typeof(Switch), new PropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the inline label offset.
        /// </summary>
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register(nameof(LabelOffset), typeof(double), typeof(Switch), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Registers the property to set the off label.
        /// </summary>
        public static readonly DependencyProperty OffLabelProperty = DependencyProperty.Register(nameof(OffLabel), typeof(string), typeof(Switch), new PropertyMetadata("Off"));

        /// <summary>
        /// Registers the property to set the on label.
        /// </summary>
        public static readonly DependencyProperty OnLabelProperty = DependencyProperty.Register(nameof(OnLabel), typeof(string), typeof(Switch), new PropertyMetadata("On"));

        /// <summary>
        /// Registers the property to set the orientation of the switch.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Switch), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Registers the property to set the size of the switch.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(double), typeof(Switch), new PropertyMetadata(23.0));

        /// <summary>
        /// Registers the property to set the style of the switch.
        /// </summary>
        public static readonly DependencyProperty SwitchStyleProperty = DependencyProperty.Register(nameof(SwitchStyle), typeof(SwitchStyles), typeof(Switch), new PropertyMetadata(SwitchStyles.Round));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets whether to show the labels inline.
        /// </summary>
        public bool InlineLabels
        {
            get { return (bool)GetValue(InlineLabelsProperty); }
            set { SetValue(InlineLabelsProperty, value); }
        }

        /// <summary>
        /// Gets the label offset for the inline labels.
        /// </summary>
        public double LabelOffset
        {
            get { return (double)GetValue(LabelOffsetProperty); }
            private set { SetValue(LabelOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the off label.
        /// </summary>
        public string OffLabel
        {
            get { return (string)GetValue(OffLabelProperty); }
            set { SetValue(OffLabelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the on label.
        /// </summary>
        public string OnLabel
        {
            get { return (string)GetValue(OnLabelProperty); }
            set { SetValue(OnLabelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation of the switch.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the switch.
        /// </summary>
        /// <remarks><i>For horizontal layout the size defines the height, for vertical layout the size defines the width.</i></remarks>
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style of the switch.
        /// </summary>
        /// <remarks><i>Not implemented.</i></remarks>
        public SwitchStyles SwitchStyle
        {
            get { return (SwitchStyles)GetValue(SwitchStyleProperty); }
            set { SetValue(SwitchStyleProperty, value); }
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// Extends the base method to bind the switch event listeners.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += SwitchLoaded;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the loaded event to initialize size dependent properties.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        private void SwitchLoaded(object sender, RoutedEventArgs e)
        {
            LabelOffset = ActualHeight / 3;
        }

        /// <summary>
        /// Handles the mouse wheel event to switch the button state.
        /// </summary>
        /// <param name="e">A <see cref="MouseWheelEventArgs"/> containing event data.</param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                IsChecked = true;
            }
            else
            {
                IsChecked = false;
            }
        }

        #endregion
    }
}
