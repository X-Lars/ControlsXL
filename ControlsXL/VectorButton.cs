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
    /// A button with a vector based symbol inside.
    /// </summary>
    public class VectorButton : ButtonBase
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
            // Overrides the default style of the inherited ButtonBase to use the VectorButton style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorButton), new FrameworkPropertyMetadata(typeof(VectorButton)));
        }

        /// <summary>
        /// Creates and initializes a new <see cref="VectorButton"/> control.
        /// </summary>
        public VectorButton() : base()
        {
            // Register the mouse click event handler
            AddHandler(ClickEvent, new RoutedEventHandler(VectorButtonClicked));
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Registers a property to set the <see cref="Geometry"/> path defining the vector graphic.
        /// </summary>
        /// <remarks><i>Defaults to <see cref="DEFAULT_VECTOR_GRAPHIC"/>.</i></remarks>
        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register("Vector", typeof(Geometry), typeof(VectorButton), new UIPropertyMetadata(Geometry.Parse(DEFAULT_VECTOR_GRAPHIC)));

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

        #endregion

        #region Events

        /// <summary>
        /// Registers an event to capture mouse click events.
        /// </summary>
        private static readonly RoutedEvent VectorButtonClickedRoutedEvent = EventManager.RegisterRoutedEvent("VectorButtonClickedEventHandler", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VectorButton));

        /// <summary>
        /// Handles the mouse click event.
        /// </summary>
        public event RoutedEventHandler VectorButtonClickedEventHandler
        {
            add { AddHandler(VectorButtonClickedRoutedEvent, value); }
            remove { RemoveHandler(VectorButtonClickedRoutedEvent, value); }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the vector button clicked event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        protected virtual void VectorButtonClicked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(VectorButtonClickedRoutedEvent));
        }

        #endregion
    }
}
