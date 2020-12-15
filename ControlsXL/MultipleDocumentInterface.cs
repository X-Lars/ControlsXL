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
using System.Windows.Data;
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

        public MDIHost()
        {
            
        }


        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine($"[{this.GetType().Name}.{nameof(OnItemsChanged)}] Add");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine($"[{this.GetType().Name}.{nameof(OnItemsChanged)}] Remove");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine($"[{this.GetType().Name}.{nameof(OnItemsChanged)}] Replace");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine($"[{this.GetType().Name}.{nameof(OnItemsChanged)}] Reset");
                    break;
                case NotifyCollectionChangedAction.Move:
                    Console.WriteLine($"[{this.GetType().Name}.{nameof(OnItemsChanged)}] Move");
                    break;
            }
        }
    }

 
    /// <summary>
    /// 
    /// </summary>
    public class MDIChild : ContentControl
    {

        #region Constants

        /// <summary>
        /// Defines the size of the <see cref="MDIChild"/> window header.
        /// </summary>
        public const double MDICHILD_HEADER_HEIGHT = 24;

        /// <summary>
        /// Defines the width of a minimized <see cref="MDIChild"/> window.
        /// </summary>
        public const double MDICHILD_MINIMIZED_WIDTH = 150;

        /// <summary>
        /// Defines the height of a minimized <see cref="MDIChild"/> window.
        /// </summary>
        public const double MDICHILD_MINIMIZED_HEIGHT = MDICHILD_HEADER_HEIGHT;
       
        #endregion

        #region Fields

        /// <summary>
        /// Stores the normal size of a <see cref="MDIChild"/> window.
        /// </summary>
        private Rect _NormalSize;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MDIChild"/>.
        /// </summary>
        static MDIChild()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIChild), new FrameworkPropertyMetadata(typeof(MDIChild)));
        }

        /// <summary>
        /// 
        /// </summary>
        public MDIChild()
        {
            // Initialize the command bindings
            CommandBindings.Add(new CommandBinding(_RoutedUICommandClose, OnCloseCommand));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandMinimize, OnMinimizeCommand));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandMaximize, OnMaximizeCommand));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandRestore, OnRestoreCommand));

            _RoutedUICommandClose.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Control));
        }

        
        #endregion

        #region Commands

        #region Commands: Registrations

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandClose = new RoutedUICommand(nameof(Close), nameof(Close), typeof(MDIChild));

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandMinimize = new RoutedUICommand(nameof(Minimize), nameof(Minimize), typeof(MDIChild));

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandMaximize = new RoutedUICommand(nameof(Maximize), nameof(Maximize), typeof(MDIChild));

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandRestore = new RoutedUICommand(nameof(Restore), nameof(Restore), typeof(MDIChild));

        #endregion

        #region Command: Properties

        /// <summary>
        /// Gets the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        public static RoutedUICommand Close
        {
            get { return _RoutedUICommandClose; }
        }

        /// <summary>
        /// Gets the command to minimize the <see cref="MDIChild"/> window.
        /// </summary>
        public static RoutedUICommand Minimize
        {
            get { return _RoutedUICommandMinimize; }
        }

        /// <summary>
        /// Gets the command to maximize the <see cref="MDIChild"/> window.
        /// </summary>
        public static RoutedUICommand Maximize
        {
            get { return _RoutedUICommandMaximize; }
        }

        /// <summary>
        /// Gets the command to restore the <see cref="MDIChild"/> window.
        /// </summary>
        public static RoutedUICommand Restore
        {
            get { return _RoutedUICommandRestore; }
        }

        #endregion

        #region Command: Handlers

        private void OnCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Host.Items.Remove(this);
        }

        private void OnMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.State = WindowState.Minimized;
        }

        private void OnMaximizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.State = WindowState.Maximized;
        }

        private void OnRestoreCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.State = WindowState.Normal;
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to determin if the <see cref="MDIChild"/> window can be resized.
        /// </summary>
        public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register(nameof(CanResize), typeof(bool), typeof(MDIChild), new PropertyMetadata(true));

        /// <summary>
        /// Registers the property to determin if the <see cref="MDIChild"/> window is selected.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MDIChild), new PropertyMetadata(false, IsSelectedPropertyChangedCallback));

        
        /// <summary>
        /// Registers the property to determin the position of the <see cref="MDIChild"/> window.
        /// </summary>
        /// <remarks><i>Default has to be -1, -1 to be raise the event on initialize </i></remarks>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(Point), typeof(MDIChild), new PropertyMetadata(new Point(-1.0, -1.0), PositionPropertyChangedCallback));

        /// <summary>
        /// Registers the property to determin the state of the <see cref="MDIChild"/> window.
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(WindowState), typeof(MDIChild), new PropertyMetadata(WindowState.Normal, StatePropertyChangedCallback));

        /// <summary>
        /// Registers the property to determin the title of the <see cref="MDIChild"/> window.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MDIChild), new PropertyMetadata(string.Empty));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets whether the <see cref="MDIChild"/> window can be resized.
        /// </summary>
        public bool CanResize
        {
            get { return (bool)GetValue(CanResizeProperty); }
            private set { SetValue(CanResizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="MDIChild"/> window is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position of the <see cref="MDIChild"/> window.
        /// </summary>
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the state of the <see cref="MDIChild"/> window.
        /// </summary>
        public WindowState State
        {
            get { return (WindowState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="MDIChild"/> window.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region Dependency Properties: Callbacks

        /// <summary>
        /// Handles changes made to the <see cref="IsSelectedProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsSelectedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIChild child = (MDIChild)d;

            bool isSelected = (bool)e.NewValue;

            if (isSelected)
            {
                int maxIndex = 0;

                foreach(MDIChild item in child.Host.Items)
                {
                    int index = Panel.GetZIndex(item);

                    if (index > maxIndex)
                    {
                        maxIndex = index;
                        index--;
                    }

                    if (item != child)
                        item.IsSelected = false;

                    Console.WriteLine(index);
                }

                Panel.SetZIndex(child, maxIndex + 1);
            }
        }

        /// <summary>
        /// Handles changes made to the <see cref="PositionProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void PositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            MDIChild child = (MDIChild)d;
            Point point = (Point)e.NewValue;

            point.X = Math.Max(0, point.X);
            point.Y = Math.Max(0, point.Y);

            Canvas.SetLeft(child, point.X);
            Canvas.SetTop(child, point.Y);
        }

        /// <summary>
        /// Handles changes made to the <see cref="StateProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void StatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIChild child = (MDIChild)d;

            WindowState previousState = (WindowState)e.OldValue;
            WindowState newState = (WindowState)e.NewValue;

            if (newState == previousState)
                return;

            child.ChangeState(previousState, newState);
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="MDIHost"/> of this instance.
        /// </summary>
        private MDIHost Host
        {
            get { return this.Parent as MDIHost; }
        }

        #endregion


        #region Event Handlers


        #endregion

        #region Methods

        private void ChangeState(WindowState previousState, WindowState newState)
        {
            // Store the normal size to be able to restore the window
            if (previousState == WindowState.Normal)
            {
                _NormalSize = new Rect(Canvas.GetLeft(this), Canvas.GetTop(this), ActualWidth, ActualHeight);
                Console.WriteLine($"NORMAL STATE STORED [{Position.X},{Position.Y}]");
            }

            MDICanvas canvas = VisualTreeHelper.GetParent(this) as MDICanvas;

            if(previousState == WindowState.Minimized)
            {
                int offset = 0;
                double positionX = 0;

                foreach(MDIChild child in Host.Items)
                {
                    if(child.State == WindowState.Minimized)
                    {
                        child.Position = new Point(positionX, child.Position.Y);
                        positionX += child.ActualWidth;
                    }
                }
            }

            switch (newState)
            {
                case WindowState.Normal:

                    CanResize = true;

                    Width = _NormalSize.Width;
                    Height = _NormalSize.Height;

                    Position = new Point(_NormalSize.X, _NormalSize.Y);

                    ToolTip = null;

                    break;

                case WindowState.Minimized:

                    CanResize = false;

                    double positionX = 0;

                    foreach(MDIChild child in Host.Items)
                    {
                        if (child.State == WindowState.Minimized && child.IsSelected == false)
                        {
                            positionX = Math.Max(positionX, child.Position.X + child.ActualWidth);
                        }
                    }

                    Width = MinWidth + BorderThickness.Left + BorderThickness.Right;
                    Height = MinHeight + BorderThickness.Top + BorderThickness.Bottom;

                    Position = new Point(positionX, canvas.ActualHeight - Height);
                    
                    ToolTip = Title;
                    

                    break;

                case WindowState.Maximized:

                    CanResize = false;

                    ToolTip = null;

                    break;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            MDICanvas canvas = VisualTreeHelper.GetParent(this) as MDICanvas;

            if (this.IsSelected == false)
            {
                canvas.DeselectAll();
                this.IsSelected = true;
            }

            Focus();

            // Allow the event to route further
            e.Handled = false;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            
            switch (e.Key)
            {
                case Key.F4:

                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        Host.Items.Remove(this);
                    
                    break;
            }
        }

    }

    internal class MDICanvas : Canvas
    {
        #region Fields

        /// <summary>
        /// Stores a reference to the <see cref="MDIHost"/> owning the <see cref="MDICanvas"/>.
        /// </summary>
        private MDIHost _Host;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates and initializes a new <see cref="MDICanvas"/> instance.
        /// </summary>
        public MDICanvas()
        {
            _Host = (MDIHost)this.TemplatedParent;
        }

        #endregion

       
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if(visualAdded != null)
                Console.WriteLine($"[{this.GetType().Name}.{nameof(OnVisualChildrenChanged)}] Added: {visualAdded}");

            if (visualRemoved != null)
                Console.WriteLine($"[{this.GetType().Name}.{nameof(OnVisualChildrenChanged)}] Removed: {visualRemoved}");
        }

        #region Properties

        /// <summary>
        /// Gets an enumerable list of selected <see cref="MDIChild"/> windows.
        /// </summary>
        public IEnumerable<MDIChild> SelectedItems
        {
            get
            {
                return from item in Children.OfType<MDIChild>() 
                       where item.IsSelected == true 
                       select item;
            }
        }

        /// <summary>
        /// Gets an enumerable list of minimized <see cref="MDIChild"/> windows.
        /// </summary>
        public IEnumerable<MDIChild> MinimizedItems
        {
            get
            {
                return from item in Children.OfType<MDIChild>()
                       where item.State == WindowState.Minimized
                       select item;
            }
        }

        /// <summary>
        /// Gets whether any of the <see cref="MDIChild"/> windows is maximized.
        /// </summary>
        private bool HasMaximizedItem
        {
            get { return this.Children.OfType<MDIChild>().Any(i => i.State == WindowState.Minimized); }
        }

        #endregion

        public void DeselectAll()
        {
            foreach (MDIChild child in InternalChildren)
            {
                child.IsSelected = false;
            }
        }


        // TODO: make scroll bars invisible if all items fit by offsetting item
        // TODO: ensure minimized windows do not enable the scrolbars, try wrapping and stacking
        // TODO: foreach sorting problem, minimized items are switcht based on position inside the collection
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            Console.WriteLine($"Constraint: {constraint.Width}x{constraint.Height}");

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
                    if (((MDIChild)element).State != WindowState.Minimized)
                    {
                        size.Height = Math.Max(size.Height, top + desiredSize.Height);
                    }
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                }
                else
                {
                    Console.WriteLine("double.IsNan");
                }

            }

            Console.WriteLine($"Measure Actual: {ActualWidth}x{ActualHeight}");
            Console.WriteLine($"Measure Normal: {Width}x{Height}");
            // extra margin
            //size.Width += 10;
            //size.Height += 10;


            return size;

        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Console.WriteLine(HasMaximizedItem);

            double positionX = 0;
            double offsetX = 0;

            foreach(MDIChild child in MinimizedItems)
            {
                child.Position = new Point(offsetX, ActualHeight - child.Height);
                offsetX += child.Width;
            }
            

            //foreach (MDIChild child in Host.Items)
            //{
            //    if (child.State == WindowState.Minimized && child.IsSelected == false)
            //    {
            //        positionX = Math.Max(positionX, child.Position.X + child.ActualWidth);
            //    }
            //}

            //Width = MinWidth + BorderThickness.Left + BorderThickness.Right;
            //Height = MinHeight + BorderThickness.Top + BorderThickness.Bottom;

            //Position = new Point(positionX, canvas.ActualHeight - Height);

            foreach (MDIChild child in Children)
            {
                Console.WriteLine(child.Title);
            }



            Console.WriteLine($"Arrange: {arrangeSize.Width}x{arrangeSize.Height}");
            Console.WriteLine($"Actual: {ActualWidth}x{ActualHeight}");
            Console.WriteLine($"Normal: {Width}x{Height}");
            Console.WriteLine($"Desired: {DesiredSize.Width}x{DesiredSize.Height}");
            return base.ArrangeOverride(arrangeSize);
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
                    //layer.Remove(_MoveAdorner);
                    layer.Remove(_Adorner);

                }

                _Adorner = null;
                //_MoveAdorner = null;
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
                    //_MoveAdorner = new MDIMoveAdorner(child);

                    //layer.Add(_MoveAdorner);
                    layer.Add(_Adorner);


                    if (this.ShowDecorator)
                    {
                        _Adorner.Visibility = Visibility.Visible;
                        //_MoveAdorner.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _Adorner.Visibility = Visibility.Hidden;
                        //_MoveAdorner.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                _Adorner.Visibility = Visibility.Visible;

                //TODO: Adorner collection.
                //_MoveAdorner.Visibility = Visibility.Visible;
            }
        }

        private void Hide()
        {
            if (_Adorner != null)
            {
                _Adorner.Visibility = Visibility.Hidden;

                // TODO: Adorner collection
                //_MoveAdorner.Visibility = Visibility.Hidden;
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
        public static double ADORNER_SIDE_DIMENSIONS = 5;
        public static double ADORNER_CORNER_DIMENSIONS = 10;
        public static readonly Thickness ADORNER_MARGIN = new Thickness(-4);
        public static readonly Thickness ADORNER_CORNER_MARGIN = new Thickness(-2.5);

        static MDIResizeChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIResizeChrome), new FrameworkPropertyMetadata(typeof(MDIResizeChrome)));
        }
    }

    //public class MDIMoveAdorner : Adorner
    //{
    //    private VisualCollection _Visuals;
    //    private MDIMoveChrome _Chrome;

    //    public MDIMoveAdorner(MDIChild child) : base(child)
    //    {
    //        _Chrome = new MDIMoveChrome();
    //        _Visuals = new VisualCollection(this);
    //        _Visuals.Add(_Chrome);
    //        _Chrome.DataContext = child;
    //    }

    //    protected override int VisualChildrenCount
    //    {
    //        get { return _Visuals.Count; }
    //    }

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        _Chrome.Arrange(new Rect(finalSize));

    //        return finalSize;
    //    }

    //    protected override Visual GetVisualChild(int index)
    //    {
    //        return _Visuals[index];
    //    }

    //}

    //public class MDIMoveChrome : Control
    //{
    //    static MDIMoveChrome()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIMoveChrome), new FrameworkPropertyMetadata(typeof(MDIMoveChrome)));
    //    }
    //}

    internal class MDIMoveHandle : Thumb
    {
        private MDIChild _Child;
        private MDICanvas _Canvas;

        public MDIMoveHandle()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);

            SizeChanged += MDIMoveHandleSizeChanged;
            DragStarted += XMoveThumbDragStarted;
            DragDelta += XMoveThumbDragDelta;
        }

        private void MDIMoveHandleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(IsDragging)
            {
                Width = e.PreviousSize.Width;
                Height = e.PreviousSize.Height;
            }
        }

        private void XMoveThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _Child = DataContext as MDIChild;

            if (_Child != null)
            {
                _Canvas = VisualTreeHelper.GetParent(_Child) as MDICanvas;
            }
            else
            {
                _Canvas = DataContext as MDICanvas;
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
            else if (_Canvas != null)
            {
                // TODO: Canvas Dragging
                //double minLeft = double.MaxValue;
                //double minTop = double.MaxValue;

                //foreach (MDIChild item in _Canvas.Children)
                //{
                //    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                //    minTop = Math.Min(Canvas.GetTop(item), minTop);
                //}


                //double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                //double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                //Console.WriteLine(deltaHorizontal);
                //foreach (MDIChild item in _Canvas.Children)
                //{
                //    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                //    Canvas.SetTop(item, Canvas.GetTop(item) +  deltaVertical);
                //}

                //_Canvas.InvalidateArrange();
                //e.Handled = true;

            }
        }

    }

    public class MDIResizeHandle : Thumb
    {
        private MDIChild _Child;
        private MDICanvas _Canvas;

        public MDIResizeHandle()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);

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
