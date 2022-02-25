using ControlsXL.Interfaces;
using StylesXL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using StylesXL.Extensions;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ControlsXL.Adorners
{
    public class NextItemsAdorner : Adorner
    {
        private VisualCollection _Visuals;
        private ContentPresenter _ContentPresenter;
        
        private StackPanel _StackPanel;
        private TextBlock _TextBlockTop;
        private TextBlock _TextBlockCenter = new();
        private TextBlock _TextBlockBottom;
        private INextItemsAdorner _Parent;
        
        private Brush _Selected;
        private Brush _Tint;
        private Brush _OpacityUpper;
        private Brush _OpacityLower;

        public NextItemsAdorner(INextItemsAdorner element) : base ((UIElement)element)
        {
            StyleManager.StyleChanged += (o, e) => Initialize();

            _ContentPresenter = new ContentPresenter();
            _Visuals = new VisualCollection(this);
            _Parent = element;

            Initialize();
        }

        public void Initialize()
        {
            _Selected     = StyleManager.Brush(StylesXL.Brushes.ControlSelected).Tint(0.25);
            _Tint         = StyleManager.Brush(StylesXL.Brushes.Tint);
            _OpacityUpper = StyleManager.Brush(StylesXL.Brushes.OpacityFadeUpper);
            _OpacityLower = StyleManager.Brush(StylesXL.Brushes.OpacityFadeLower);

            _Visuals.Clear();

            _StackPanel = new StackPanel();

            _TextBlockTop = new TextBlock() { Text = _Parent.Next };
            _TextBlockTop.TextAlignment = TextAlignment.Center;
            _TextBlockTop.Background = _Tint;
            _TextBlockTop.Foreground = new SolidColorBrush(Colors.White);
            _TextBlockTop.Padding = new Thickness(0, 0, 0, 3);
            _TextBlockTop.OpacityMask = _OpacityUpper;

            _TextBlockCenter = new TextBlock { Height = _Parent.ActualHeight };

            _TextBlockBottom = new TextBlock() { Text = _Parent.Previous };
            _TextBlockBottom.Background = _Tint;
            _TextBlockBottom.Foreground = new SolidColorBrush(Colors.White);
            _TextBlockBottom.TextAlignment = TextAlignment.Center;
            _TextBlockBottom.Padding = new Thickness(0, 3, 0, 0);
            _TextBlockBottom.OpacityMask = _OpacityLower;

            _StackPanel.Children.Add(_TextBlockTop);
            _StackPanel.Children.Add(_TextBlockCenter);
            _StackPanel.Children.Add(_TextBlockBottom);

            Content = _StackPanel;

            _Visuals.Add(_ContentPresenter);

            IsHitTestVisible = false;
        }

        public void Update()
        {
            _TextBlockTop.Text = _Parent.Next;
            _TextBlockBottom.Text = _Parent.Previous;

            if (string.IsNullOrEmpty(_Parent.Next))
            {
                _TextBlockTop.Visibility = Visibility.Collapsed;
            }
            else
                _TextBlockTop.Visibility = Visibility.Visible;

            if (string.IsNullOrEmpty(_Parent.Previous))
            {
                _TextBlockBottom.Visibility = Visibility.Collapsed;
            }
            else
                _TextBlockBottom.Visibility = Visibility.Visible;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _TextBlockTop.Width = _Parent.ActualWidth;
            _TextBlockCenter.Height = _Parent.ActualHeight;
            _TextBlockBottom.Width = _Parent.ActualWidth;

            _ContentPresenter.Measure(constraint);
            return _ContentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            
            _ContentPresenter.Arrange(new Rect(0, -_TextBlockTop.ActualHeight - _TextBlockTop.Margin.Bottom, finalSize.Width, finalSize.Height));
            return _ContentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index)
        { return _Visuals[index]; }

        protected override int VisualChildrenCount
        { get { return _Visuals.Count; } }

        public object Content
        {
            get { return _ContentPresenter.Content; }
            set { _ContentPresenter.Content = value; }
        }
    }
}
