using StylesXL;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace ControlsXL.Adorners
{
    public class SpinAdorner : Adorner
    {
        private StreamGeometry _ForegroundGeometry = new StreamGeometry();
        private StreamGeometry _BackgroundGeometry = new StreamGeometry();

        private Brush _Foreground = StyleManager.Brush(StylesXL.Brushes.TextSelected);
        private Brush _Background = StyleManager.Brush(StylesXL.Brushes.ControlSelected);

        public SpinAdorner(Control element) : base(element)
        {
            StyleManager.StyleChanged += (o, e) =>
            {
                _Foreground = StyleManager.Brush(StylesXL.Brushes.ControlSelected);
                _Background = StyleManager.Brush(StylesXL.Brushes.ControlSelected);

                InvalidateVisual();
            };

            SnapsToDevicePixels = true;
            IsHitTestVisible = false;
            
        }

        private void UpdateGeometry(Size finalSize)
        {
            // Control Size
            double sx = DesiredSize.Width;
            double sy = finalSize.Height;

            // Glyph Size
            double sg = sy / 1.5;
            double cg = sg / 2;

            using (StreamGeometryContext context = _ForegroundGeometry.Open())
            {
                // Add Sign: Horizontol
                context.BeginFigure(new Point(sx - sg * 2 + 2, -cg), false, false);
                context.LineTo(new Point(sx - sg - 2, -cg), true, false);

                // Add Sign: Vertical
                context.BeginFigure(new Point(sx - sg * 2 + cg, -sg + 2), false, false);
                context.LineTo(new Point(sx - sg * 2 + cg, -2), true, false);

                // Minus Sign
                context.BeginFigure(new Point(sx - sg + 2, -cg), false, false);
                context.LineTo(new Point(sx - 2, -cg), true, false);

                // Separator
                context.BeginFigure(new Point(sx - sg - 1, -2), false, false);
                context.LineTo(new Point(sx - sg + 1, -sg + 2), true, false);
            }

            using (StreamGeometryContext context = _BackgroundGeometry.Open())
            {
                // Top Line
                context.BeginFigure(new Point(0, 0), false, false);
                context.LineTo(new Point(sx, 0), true, false);

                // Bottom Line
                context.BeginFigure(new Point(0, sy), false, false);
                context.LineTo(new Point(sx, sy), true, false);

                context.BeginFigure(new Point(sx - sg * 2, -sg), true, true);
                context.LineTo(new Point(sx, -sg), false, true);
                context.LineTo(new Point(sx, 0), false, true);
                context.LineTo(new Point(sx - sg * 2, 0), false, true);
            }
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return base.GetLayoutClip(layoutSlotSize);
        }
        protected override Size MeasureOverride(Size constraint)
        {

            IsClipEnabled = true;

            return base.MeasureOverride(constraint); ;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateGeometry(AdornedElement.RenderSize);
            
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Debug.Print($"Render: {this.DesiredSize}");
            Debug.Print($"Render: {ActualWidth}");
            //UpdateGeometry(AdornedElement.RenderSize);
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            Rect actualRect = new Rect(AdornedElement.RenderSize);
            //// Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.LimeGreen);
            //renderBrush.Opacity = 0.2;
            Pen desiredPen = new Pen(new SolidColorBrush(Colors.DeepPink), 1);
            Pen actualPen = new Pen(new SolidColorBrush(Colors.LimeGreen), 1);

            //drawingContext.DrawRectangle(renderBrush, null, actualRect);
            //drawingContext.DrawRectangle(null, desiredPen, adornedElementRect);

            //// Control Size
            //double sx = AdornedElement.DesiredSize.Width;
            //double sy = AdornedElement.DesiredSize.Height;

            //// Glyph Size
            //double sg = sy / 1.5;
            //double cg = sg / 2;

            //using (StreamGeometryContext context = _ForegroundGeometry.Open())
            //{
            //    // Add Sign: Horizontol
            //    context.BeginFigure(new Point(sx - sg * 2 + 2, -cg), false, false);
            //    context.LineTo(new Point(sx - sg - 2, -cg), true, false);

            //    // Add Sign: Vertical
            //    context.BeginFigure(new Point(sx - sg * 2 + cg, -sg + 2), false, false);
            //    context.LineTo(new Point(sx - sg * 2 + cg, -2), true, false);

            //    // Minus Sign
            //    context.BeginFigure(new Point(sx - sg + 2, -cg), false, false);
            //    context.LineTo(new Point(sx - 2, -cg), true, false);

            //    // Separator
            //    context.BeginFigure(new Point(sx - sg - 1, -2), false, false);
            //    context.LineTo(new Point(sx - sg + 1, -sg + 2), true, false);
            //}

            //using (StreamGeometryContext context = _BackgroundGeometry.Open())
            //{
            //    // Top Line
            //    context.BeginFigure(new Point(0, 0), false, false);
            //    context.LineTo(new Point(sx, 0), true, false);

            //    // Bottom Line
            //    context.BeginFigure(new Point(0, sy), false, false);
            //    context.LineTo(new Point(sx, sy), true, false);

            //    context.BeginFigure(new Point(sx - sg * 2, -sg), true, true);
            //    context.LineTo(new Point(sx, -sg), false, true);
            //    context.LineTo(new Point(sx, 0), false, true);
            //    context.LineTo(new Point(sx - sg * 2, 0), false, true);
            //}


            drawingContext.DrawGeometry(_Background, new Pen(_Background, 0), _BackgroundGeometry);
            drawingContext.DrawGeometry(_Foreground, new Pen(_Foreground, 1), _ForegroundGeometry);
        }
    }
}
