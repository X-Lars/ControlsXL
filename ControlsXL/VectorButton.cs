using ControlsXL.Common;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ControlsXL
{
    /// <summary>
    /// A button with a vector based symbol inside.
    /// </summary>
    /// <remarks><i>When the <see cref="ButtonMode"/> is <see cref="ButtonModes.Standard"/> changing the <see cref="VectorButton"/>.IsChecked property has no effect and always returns false.</i></remarks>
    /// <example>
    /// Vector: M0,0 L4,4 0,8 M4,0 L8,4 4,8
    /// Stroke: Blue
    /// Fill: Red
    /// StrokeThickness: 1
    /// Scale: 0.8
    /// ButtonMode: Standard, Toggle
    /// </example>
    public class VectorButton : ToggleButton
    {
        #region Constants

        /// <summary>
        /// The default image vector shown when no vector is supplied by the user.
        /// </summary>
        /// <remarks><i>Displays a square with a cross inside.</i></remarks>
        public const string DEFAULT_VECTOR_GRAPHIC = "F1 M 22,54L 22,22L 54,22L 54,54L 22,54 Z M 26,26L 26,50L 50,50L 50,26L 26,26 Z M 30.755,27.65L 38,34.895L 45.2449,27.6501L 48.3499,30.7551L 41.105,38L 48.35,45.245L 45.245,48.35L 38,41.105L 30.755,48.35L 27.65,45.245L 34.895,38L 27.65,30.755L 30.755,27.65 Z";

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="VectorButton"/>.
        /// </summary>
        static VectorButton()
        {
            // Overrides the default style of the inherited ToggleButton to use the VectorButton style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorButton), new FrameworkPropertyMetadata(typeof(VectorButton)));

            // Requires style manager
            StylesXL.StyleManager.Initialize();
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Registers a property to set the <see cref="Geometry"/> path defining the vector graphic.
        /// </summary>
        /// <remarks><i>Defaults to <see cref="DEFAULT_VECTOR_GRAPHIC"/>.</i></remarks>
        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register("Vector", typeof(Geometry), typeof(VectorButton), new FrameworkPropertyMetadata(Geometry.Parse(DEFAULT_VECTOR_GRAPHIC), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Registers a property to set the brush to stroke the vector graphic.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(VectorButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers a property to set the brush to fill the vector graphic.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(VectorButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers a property to set the stroke thickness of the vector graphic.
        /// </summary>
        /// <remarks><i>Defaults to 1.</i></remarks>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(VectorButton), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Registers a property to set the scale of the vector graphic.
        /// </summary>
        /// <remarks><i>Defaults to 1.</i></remarks>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(VectorButton), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Registers the property to set the <see cref="VectorButton"/> mode.
        /// </summary>
        /// <remarks><i>Defaults to <see cref="ButtonModes.Standard"/>.</i></remarks>
        public static readonly DependencyProperty ButtonModeProperty = DependencyProperty.Register(nameof(ButtonMode), typeof(ButtonModes), typeof(VectorButton), new UIPropertyMetadata(ButtonModes.Standard));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Geometry"/> path defining the vector to display inside the button.
        /// </summary>
        public Geometry Vector
        {
            get { return (Geometry)GetValue(VectorProperty); }
            set { SetValue(VectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to stroke the vector graphic.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to fill the vector graphic.
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke thickness of the vector graphic.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scale of the vector graphic.
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="VectorButton"/> mode.
        /// </summary>
        public ButtonModes ButtonMode
        {
            get { return (ButtonModes)GetValue(ButtonModeProperty); }
            set { SetValue(ButtonModeProperty, value); }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Handles the mouse click event.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();

            // Prevent checked state when the button mode is standard
            if (ButtonMode == ButtonModes.Standard)
                IsChecked = false;
        }

        /// <summary>
        /// Handles the checked event.
        /// </summary>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        protected override void OnChecked(RoutedEventArgs e)
        {
            // Prevent checked state when the button mode is standard
            if (ButtonMode == ButtonModes.Toggle)
                base.OnChecked(e);
        }

        #endregion
    }
}
