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
    /// <summary>
    /// A button with a marker bar.
    /// </summary>
    /// <remarks><i>When the <see cref="ButtonMode"/> is <see cref="ButtonModes.Standard"/> changing the <see cref="MarkerButton"/>.IsChecked property has no effect and always returns false.</i></remarks>
    /// <example>
    /// ButtonMode: Standard, Toggle
    /// MarkerPlacement: Left, Right, Top, Bottom
    /// MarkerSize: 5
    /// </example>
    public class MarkerButton : ToggleButton
    {
        #region Constants

        /// <summary>
        /// Default width or height of the marker depending on the <see cref="MarkerPlacement"/> property.
        /// </summary>
        private const double DEFAULT_MARKER_SIZE = 5;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MarkerButton"/>.
        /// </summary>
        static MarkerButton()
        {
            // Overrides the default style of the inherited ToggleButton to use the MarkerButton style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkerButton), new FrameworkPropertyMetadata(typeof(MarkerButton)));
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Registers the property to set the <see cref="MarkerButton"/> mode.
        /// </summary>
        /// <remarks><i>Defaults to <see cref="ButtonModes.Standard"/>.</i></remarks>
        public static readonly DependencyProperty ButtonModeProperty = DependencyProperty.Register(nameof(ButtonMode), typeof(ButtonModes), typeof(MarkerButton), new UIPropertyMetadata(ButtonModes.Standard));
        
        /// <summary>
        /// Registers the property to set the placment of the marker.
        /// </summary>
        /// <remarks><i>Defaults to <see cref="Dock.Bottom"/>.</i></remarks>
        public static readonly DependencyProperty MarkerPlacementProperty = DependencyProperty.Register(nameof(MarkerPlacement), typeof(Dock), typeof(MarkerButton), new PropertyMetadata(Dock.Bottom));
        
        /// <summary>
        /// Registers the property to set the size of the marker .
        /// </summary>
        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register(nameof(MarkerSize), typeof(double), typeof(MarkerButton), new PropertyMetadata(DEFAULT_MARKER_SIZE));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="MarkerButton"/> mode.
        /// </summary>
        public ButtonModes ButtonMode
        {
            get { return (ButtonModes)GetValue(ButtonModeProperty); }
            set { SetValue(ButtonModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the placement of the marker.
        /// </summary>
        public Dock MarkerPlacement
        {
            get { return (Dock)GetValue(MarkerPlacementProperty); }
            set { SetValue(MarkerPlacementProperty, value); }
        }

        /// <summary>
        /// Gets or set the size of the marker.
        /// </summary>
        public double MarkerSize
        {
            get { return (double)GetValue(MarkerSizeProperty); }
            set { SetValue(MarkerSizeProperty, value); }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Handles the mouse click event.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();

            // Prevents setting the checked state if the marker button is used as a normal button            
            if (ButtonMode == ButtonModes.Standard)
                IsChecked = false;
        }

        /// <summary>
        /// Handles the checked event.
        /// </summary>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        protected override void OnChecked(RoutedEventArgs e)
        {
            // Prevents setting the checked state if the marker button is used as a normal button            
            if (ButtonMode == ButtonModes.Toggle)
                base.OnChecked(e);
        }

        #endregion
    }
}
