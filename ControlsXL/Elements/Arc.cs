using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ControlsXL.Elements
{
    /// <summary>
    /// Defines an arc shaped element for use in custom controls.
    /// </summary>
    public class Arc : Shape
    {
        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the start angle of the <see cref="Arc"/>.
        /// </summary>
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register(nameof(Start), typeof(double), typeof(Arc), new FrameworkPropertyMetadata(90.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Registers the property to set the end angle of the <see cref="Arc"/>.
        /// </summary>
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register(nameof(End), typeof(double), typeof(Arc), new FrameworkPropertyMetadata(-270.0, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Dependency Properties: Implementation

        #endregion

        /// <summary>
        /// Gets or sets the start angle of the <see cref="Arc"/>.
        /// </summary>
        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end angle of the <see cref="Arc"/>.
        /// </summary>
        public double End
        {
            get { return (double)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        #endregion

        /// <summary>
        /// Defines the geometry of the arc based on it's properties.
        /// </summary>
        /// <returns>The geometry to draw the arc.</returns>
        private Geometry GetGeometry()
        {
            
            double start = Start + 90;
            double end = 90 - End;

            // Max width and height of the arc accounting the stroke thickness
            double maxWidth = Math.Max(0, (RenderSize.Width - StrokeThickness));
            double maxHeight = Math.Max(0, (RenderSize.Height - StrokeThickness));

            double startX = maxWidth / 2 * Math.Cos(start * Math.PI / 180);
            double startY = maxHeight / 2 * Math.Sin(start * Math.PI / 180);

            double endX = maxWidth / 2 * Math.Cos(end * Math.PI / 180);
            double endY = maxHeight / 2 * Math.Sin(end * Math.PI / 180);

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(new Point((RenderSize.Width / 2) + startX, 
                    (RenderSize.Height / 2) - startY), true, false);

                context.ArcTo(new Point((RenderSize.Width / 2) + endX, (RenderSize.Height / 2) - endY), 
                    new Size(maxWidth / 2, maxHeight / 2),
                    0
                    , (start  - end) > 180
                    , SweepDirection.Clockwise
                    , true
                    , false);
            }

            //geometry.Transform = new TranslateTransform(StrokeThickness / 2, StrokeThickness / 2);
            
            return geometry;
        }

        /// <summary>
        /// Calculates the x and y coordinates of a point on the arc at the specified <paramref name="angle"/>.
        /// </summary>
        /// <param name="angle">The angele to retrun the point for.</param>
        /// <returns>A <see cref="Point"/> containing the x and y coordinates on the arc at the specified angle.</returns>
        private Point GetPoint(double angle)
        {
            // Get the angle in degrees from radians
            double degrees = angle * (Math.PI / 180);

            // Get the radius from the center of the stroke
            double radiusX = (RenderSize.Width  - StrokeThickness) / 2;
            double radiusY = (RenderSize.Height - StrokeThickness) / 2;

            // Get the point on the arc at the specified angle
            double x = radiusX + radiusX * Math.Cos(degrees);
            double y = radiusY - radiusY * Math.Sin(degrees);

            return new Point(x, y);
        }

        #region Overrides

        /// <summary>
        /// Implements the abstract <see cref="Shape.DefiningGeometry"/> method to return the geometry of the arc.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get { return GetGeometry(); }
        }

        #endregion
    }
}
