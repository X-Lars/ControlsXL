using StylesXL;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ControlsXL.Adorners
{
    public class ScrollAdorner : Adorner
    {
        private StreamGeometry _Geometry = new StreamGeometry();
        private Brush _Brush = StyleManager.Brush(StylesXL.Brushes.ControlSelected);

        public ScrollAdorner(UIElement element) : base(element) 
        {
            StyleManager.StyleChanged += (o, e) => 
            {
                _Brush = StyleManager.Brush(StylesXL.Brushes.ControlSelected);
                InvalidateVisual();
            };

            SnapsToDevicePixels = true;
            IsHitTestVisible = false;
        }

        private void UpdateGeometry(Size finalSize)
        {
            double sx = finalSize.Width;
            double sy = finalSize.Height;
            double cx = sx / 2;

            using (StreamGeometryContext context = _Geometry.Open())
            {
                // Top Line
                context.BeginFigure(new Point(0, 0), false, false);
                context.LineTo(new Point(sx, 0), true, false);

                // Top Arrow
                context.BeginFigure(new Point(cx - 2, 0), true, true);
                context.LineTo(new Point(cx, -3), true, true);
                context.LineTo(new Point(cx + 2, 0), true, true);

                // Bottom Line
                context.BeginFigure(new Point(0, sy), false, false);
                context.LineTo(new Point(sx, sy), true, false);

                // Bottom Arrow
                context.BeginFigure(new Point(cx - 2, sy), true, true);
                context.LineTo(new Point(cx, sy + 3), true, true);
                context.LineTo(new Point(cx + 2, sy), true, true);
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            IsClipEnabled = true;
            return base.MeasureOverride(constraint);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateGeometry(finalSize);

            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(_Brush, new Pen(_Brush, 1), _Geometry);
        }

    }
}
