using ControlsXL.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace ControlsXL
{
    public class MDIHost : ItemsControl
    {
       
        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MDIHost"/>.
        /// </summary>
        static MDIHost()
        {
            // Overrides the default style of the inherited ItemsControl to use the MDIHost style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIHost), new FrameworkPropertyMetadata(typeof(MDIHost)));
        }
      
   
    }

 
    public class MDIChild : ContentControl
    {
       

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MDIChild"/>.
        /// </summary>
        static MDIChild()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIChild), new FrameworkPropertyMetadata(typeof(MDIChild)));
        }


        private void XItemLoaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(MDIChild), new PropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }





        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        /// <remarks><i>Default has to be -1, -1 to be raise the event on initialize </i></remarks>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(MDIChild), new PropertyMetadata(new Point(-1.0, -1.0), PositionPropertyChanged));


        private static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            MDIChild child = (MDIChild)d;
            Point point = (Point)e.NewValue;

            point.X = Math.Max(0, point.X);
            point.Y = Math.Max(0, point.Y);

            Canvas.SetLeft(child, point.X);
            Canvas.SetTop(child, point.Y);
        }



        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            MDICanvas canvas = VisualTreeHelper.GetParent(this) as MDICanvas;

            if (this.IsSelected == false)
            {
                canvas.DeselectAll();
                this.IsSelected = true;
            }

            // Allow the event to route further
            e.Handled = false;
        }

        protected override Size MeasureOverride(Size constraint)
        {

            return base.MeasureOverride(constraint);
        }
    }

    internal class MDICanvas : Canvas
    {
        static MDICanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDICanvas), new FrameworkPropertyMetadata(typeof(MDICanvas)));
        }

        public IEnumerable<MDIChild> SelectedItems
        {
            get
            {
                var selectedItems = from item in this.Children.OfType<MDIChild>()
                                    where item.IsSelected == true
                                    select item;

                return selectedItems;
            }
        }


        public void DeselectAll()
        {
            foreach (MDIChild child in InternalChildren)
            {
                child.IsSelected = false;
            }
        }


        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in Children)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);

                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                // Get the elements desired size
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;

                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
                else
                {
                    Console.WriteLine("double.IsNan");
                }

            }
            // extra margin
            size.Width += 10;
            size.Height += 10;
            return size;

        }
    }

    public class MDIDecorator : Control
    {
        private Adorner _Adorner;
        private Adorner _MoveAdorner;

        static MDIDecorator()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(xDecorator), new FrameworkPropertyMetadata(typeof(xDecorator)));
        }

        public MDIDecorator()
        {
            Unloaded += XDecoratorUnloaded;
        }

        private void XDecoratorUnloaded(object sender, RoutedEventArgs e)
        {
            if (_Adorner != null)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                if (layer != null)
                {
                    layer.Remove(_MoveAdorner);
                    layer.Remove(_Adorner);

                }

                _Adorner = null;

                _MoveAdorner = null;
            }
        }

        public static readonly DependencyProperty ShowDecoratorProperty = DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(MDIDecorator), new PropertyMetadata(false, ShowAdornerPropertyChanged));

        public bool ShowDecorator
        {
            get { return (bool)GetValue(ShowDecoratorProperty); }
            set { SetValue(ShowDecoratorProperty, value); }
        }

        private static void ShowAdornerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIDecorator decorator = (MDIDecorator)d;

            bool showDecorator = (bool)e.NewValue;

            if (showDecorator)
            {
                decorator.Show();
            }
            else
            {
                decorator.Hide();
            }
        }

        private void Show()
        {
            if (_Adorner == null)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                if (layer != null)
                {
                    MDIChild child = this.DataContext as MDIChild;
                    MDICanvas canvas = VisualTreeHelper.GetParent(child) as MDICanvas;

                    _Adorner = new MDIResizeAdorner(child);
                    _MoveAdorner = new MDIMoveAdorner(child);

                    layer.Add(_MoveAdorner);
                    layer.Add(_Adorner);


                    if (this.ShowDecorator)
                    {
                        _Adorner.Visibility = Visibility.Visible;
                        _MoveAdorner.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _Adorner.Visibility = Visibility.Hidden;
                        _MoveAdorner.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                _Adorner.Visibility = Visibility.Visible;

                //TODO: Adorner collection.
                _MoveAdorner.Visibility = Visibility.Visible;
            }
        }

        private void Hide()
        {
            if (_Adorner != null)
            {
                _Adorner.Visibility = Visibility.Hidden;

                // TODO: Adorner collection
                _MoveAdorner.Visibility = Visibility.Hidden;
            }
        }
    }

    public class MDIResizeAdorner : Adorner
    {
        private VisualCollection _Visuals;
        private MDIResizeChrome _Chrome;

        public MDIResizeAdorner(MDIChild child) : base(child)
        {
            _Chrome = new MDIResizeChrome();
            _Visuals = new VisualCollection(this);
            _Visuals.Add(_Chrome);
            _Chrome.DataContext = child;
        }

        protected override int VisualChildrenCount
        {
            get { return _Visuals.Count; }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _Chrome.Arrange(new Rect(finalSize));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _Visuals[index];
        }
    }

    public class MDIResizeChrome : Control
    {
        static MDIResizeChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIResizeChrome), new FrameworkPropertyMetadata(typeof(MDIResizeChrome)));
        }
    }

    public class MDIMoveAdorner : Adorner
    {
        private VisualCollection _Visuals;
        private MDIMoveChrome _Chrome;

        public MDIMoveAdorner(MDIChild child) : base(child)
        {
            _Chrome = new MDIMoveChrome();
            _Visuals = new VisualCollection(this);
            _Visuals.Add(_Chrome);
            _Chrome.DataContext = child;
        }

        protected override int VisualChildrenCount
        {
            get { return _Visuals.Count; }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _Chrome.Arrange(new Rect(finalSize));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _Visuals[index];
        }

    }

    public class MDIMoveChrome : Control
    {
        static MDIMoveChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIMoveChrome), new FrameworkPropertyMetadata(typeof(MDIMoveChrome)));
        }
    }

    public class MDIMoveHandle : Thumb
    {
        private MDIChild _Child;
        private MDICanvas _Canvas;

        public MDIMoveHandle()
        {
            DragStarted += XMoveThumbDragStarted;
            DragDelta += XMoveThumbDragDelta;
        }

        private void XMoveThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _Child = DataContext as MDIChild;

            if (_Child != null)
            {
                _Canvas = VisualTreeHelper.GetParent(_Child) as MDICanvas;
            }
        }

        private void XMoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {

            if (_Child != null && _Canvas != null && _Child.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;


                foreach (MDIChild item in _Canvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);
                }


                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                foreach (MDIChild item in _Canvas.SelectedItems)
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                    Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                }

                _Canvas.InvalidateMeasure();
                e.Handled = true;
            }
        }

    }

    public class MDIResizeHandle : Thumb
    {
        private MDIChild _Child;
        private MDICanvas _Canvas;

        public MDIResizeHandle()
        {
            DragStarted += new DragStartedEventHandler(XThumbDragStart);
            DragDelta += new DragDeltaEventHandler(XThumbDragDelta);
        }

        private void XThumbDragStart(object sender, DragStartedEventArgs e)
        {
            _Child = DataContext as MDIChild;

            if (_Child != null)
            {
                _Canvas = VisualTreeHelper.GetParent(_Child) as MDICanvas;
            }
        }


        private void XThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_Child != null && _Canvas != null && _Child.IsSelected)
            {
                Console.WriteLine($"[{this.GetType().Name}.{nameof(XThumbDragDelta)}]");
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;
                double dragDeltaVertical, dragDeltaHorizontal;

                foreach (MDIChild item in _Canvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
                }

                foreach (MDIChild item in _Canvas.SelectedItems)
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            item.Height = item.ActualHeight - dragDeltaVertical;
                            break;
                        case VerticalAlignment.Top:
                            dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                            Canvas.SetTop(item, Canvas.GetTop(item) + dragDeltaVertical);
                            item.Height = item.ActualHeight - dragDeltaVertical;
                            break;
                    }

                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + dragDeltaHorizontal);
                            item.Width = item.ActualWidth - dragDeltaHorizontal;
                            break;
                        case HorizontalAlignment.Right:
                            dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            item.Width = item.ActualWidth - dragDeltaHorizontal;
                            break;
                    }
                }

                e.Handled = true;
            }
        }

    }

}
