using StylesXL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ControlsXL.Adorners
{
    public abstract class BaseAdorner : Adorner
    {
        private VisualCollection _Visuals;
        private ContentPresenter _ContentPresenter;

        public BaseAdorner(UIElement element) : base(element)
        {
            StyleManager.StyleChanged += StyleChanged;

            _ContentPresenter = new ContentPresenter();
            _Visuals = new VisualCollection(this);
        }

        public virtual void StyleChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }


        protected override Size MeasureOverride(Size constraint)
        {
            _ContentPresenter.Measure(constraint);
            return _ContentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {

            _ContentPresenter.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return _ContentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index)
        { 
            return _Visuals[index]; 
        }

        protected override int VisualChildrenCount
        { 
            get { return _Visuals.Count; } 
        }

        public object Content
        {
            get { return _ContentPresenter.Content; }
            set { _ContentPresenter.Content = value; }
        }
    }
}
