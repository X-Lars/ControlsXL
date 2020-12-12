using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    [TemplatePart(Name = PART_MDIHOST_HEADER)]
    [TemplatePart(Name = PART_MDIHOST_CONTENT)]
    [TemplatePart(Name = PART_MDIHOST_MINIMIZED_CONTENT)]
    [TemplatePart(Name = PART_MDIHOST_CONTENT_VIEWER)]
    public class MDIHost : ItemsControl
    {
        private const string PART_MDIHOST_HEADER = "PART_MDIHostHeader";
        private const string PART_MDIHOST_CONTENT = "PART_MDIHostContent";
        private const string PART_MDIHOST_MINIMIZED_CONTENT = "PART_MDIHostMinimizedContent";
        private const string PART_MDIHOST_CONTENT_VIEWER = "PART_MDIHostContentViewer";

        static MDIHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIHost), new FrameworkPropertyMetadata(typeof(MDIHost)));
        }
        
        public MDIHost()
        {
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });
            MinimizedCollection.Add(new Button { Content = "Very long title" });

            SizeChanged += MDIHost_SizeChanged;
            Loaded += MDIHost_Loaded;

           
        }



        private void MDIHost_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            //if (window != null)
            //{
            //    window.Activated += MDIHostActivated;
            //    window.Deactivated += MDIHostDeactivated;
            //}

            //_Canvas.Width = _Canvas.ActualWidth;
            //_Canvas.Height = _Canvas.ActualHeight;

            //_Canvas.HorizontalAlignment = HorizontalAlignment.Left;
            //_Canvas.VerticalAlignment = VerticalAlignment.Top;

            //InvalidateContentSize();
            //InvalidateMeasure();
        }

        private void MDIHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_Canvas.InvalidateMeasure();
        }

        public ObservableCollection<MDIChild> Children { get; set; } = new ObservableCollection<MDIChild>();
        public ObservableCollection<Button> MinimizedCollection { get; set; } = new ObservableCollection<Button>();
        private MDICanvas _Canvas;
        private ScrollViewer _Viewer;
       
        public override void OnApplyTemplate()
        {
            //_Element = GetTemplateChild(PART_MDIHOST_MINIMIZED_CONTENT) as UIElement;
            //_Element.PreviewMouseWheel += _Element_PreviewMouseWheel;

            _Canvas = GetTemplateChild(PART_MDIHOST_CONTENT) as MDICanvas;
            _Viewer = GetTemplateChild(PART_MDIHOST_CONTENT_VIEWER) as ScrollViewer;

            if (_Canvas == null)
            {
                Console.WriteLine("No CANVAS!");
                return;
            }

            //InvalidateContentSize();


            base.OnApplyTemplate();
        }

        private void _Element_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = (ScrollViewer)sender;

            if (viewer == null)
                return;

            if (e.Delta > 0)
                viewer.LineLeft();
            else
                viewer.LineRight();

            e.Handled = true;

            
        }

       

        /// <summary>
        /// Invalidates the content area size to ensure all content is visible.
        /// </summary>
        internal void InvalidateContentSize()
        {
            Point trackingPoint = new Point(0, 0);

            // Get the minimum dimensions
            for (int i = 0; i < Children.Count; i++)
            {
                MDIChild child = this.Children[i];

                Point maxChildPoint = child.Position;

                // Get the bottom right corner of the window
                maxChildPoint.X += child.Width;
                maxChildPoint.Y += child.Height;

                if (maxChildPoint.X > trackingPoint.X)
                    trackingPoint.X = maxChildPoint.X;

                if (maxChildPoint.Y > trackingPoint.Y)
                    trackingPoint.Y = maxChildPoint.Y;
            }


            // Apply the minimum dimensions
            if (_Canvas.Width != trackingPoint.X)
                _Canvas.Width = trackingPoint.X;

            if (_Canvas.Height != trackingPoint.Y)
                _Canvas.Height = trackingPoint.Y;

            
            //_Canvas.Measure(new Size(0, 0));

            //_Viewer.CanContentScroll = true;
            //_Viewer.InvalidateMeasure();
            //_Viewer.InvalidateArrange();
            //_Viewer.InvalidateScrollInfo();
            //_Viewer.InvalidateVisual();
            //InvalidateArrange();
            //InvalidateMeasure();
            //_Canvas.InvalidateMeasure();
        }




        //#region Constants

        //#region Constants: Template Parts

        ///// <summary>
        ///// The template part name of the menu area in xaml.
        ///// </summary>
        //private const string PART_MDIHOST_HEADER = "PART_MDIHostHeader";

        ///// <summary>
        ///// The template part name of the content area in xaml.
        ///// </summary>
        //private const string PART_MDIHOST_CONTENT = "PART_MDIHostContent";

        //#endregion

        //#endregion

        //#region Fields

        ///// <summary>
        ///// Contains the <see cref="MDIHost"/> content.
        ///// </summary>
        //private Canvas _Content = new Canvas();

        ///// <summary>
        ///// Contains the <see cref="MDIHost"/> header area.
        ///// </summary>
        //private DockPanel _Header = new DockPanel();

        ///// <summary>
        ///// Contains the <see cref="MDIHost"/> header minimize, maximize and close buttons.
        ///// </summary>
        //private Border _HeaderButtons = new Border();

        ///// <summary>
        ///// Contains the <see cref="MDIHost"/> header menu button.
        ///// </summary>
        //private Border _HeaderMenu = new Border();

        ///// <summary>
        ///// Contains the <see cref="MDIHost"/> content area.
        ///// </summary>
        //private ScrollViewer _ContentArea = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

        ///// <summary>
        ///// Contains the <see cref="MDIHost"/> overall layout.
        ///// </summary>
        //private Grid _Layout = new Grid();

        //#endregion

        //#region Constructor

        ///// <summary>
        ///// Static constructor called before initializing an instance of <see cref="MDIHost"/>.
        ///// </summary>
        //static MDIHost()
        //{
        //    // Overrides the default style of the inherited HeaderedItemsControl to use the SideBar style instead.
        //    //DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIHost), new FrameworkPropertyMetadata(typeof(MDIHost)));
        //}

        ///// <summary>
        ///// Creates and initializes a new instance of <see cref="MDIHost"/>.
        ///// </summary>
        ///// <remarks><i></i></remarks>
        //public MDIHost()
        //{
        //    Children = new ObservableCollection<MDIChild>();
        //    Children.CollectionChanged += MDIChildCollectionChanged;

        //    // Initialize the MDI host
        //    Initialize();

        //    // Hookup the MDI host event listeners
        //    this.Loaded += MDIHostLoaded;
        //    this.KeyDown += MDIHostKeyDown;
        //    this.SizeChanged += MDIHostSizeChanged;
        //}



        //#endregion

        //#region Dependency Properties

        ///// <summary>
        ///// Registers the property to set the active child window.
        ///// </summary>
        //public static readonly DependencyProperty ActiveChildProperty = DependencyProperty.Register(nameof(ActiveChild), typeof(MDIChild), typeof(MDIHost), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, ActiveChildChangedCallback));

        //#endregion

        //#region Properties

        ///// <summary>
        ///// Gets or sets the collection of all <see cref="MDIChild"/> windows.
        ///// </summary
        //public ObservableCollection<MDIChild> Children { get; set; }

        ///// <summary>
        ///// Gets or sets the active <see cref="MDIChild"/> window.
        ///// </summary>
        //public MDIChild ActiveChild
        //{
        //    get { return (MDIChild)GetValue(ActiveChildProperty); }
        //    set { SetValue(ActiveChildProperty, value); }
        //}

        //#endregion

        //#region Callbacks

        ///// <summary>
        ///// Handles the property changed event of the <see cref="ActiveChildProperty"/>.
        ///// </summary>
        ///// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        ///// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containint event data.</param>
        ///// <returns></returns>
        //private static void ActiveChildChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        //#endregion

        //#region Event Listeners

        ///// <summary>
        ///// Handles the <see cref="MDIHost"/> loaded event.
        ///// </summary>
        ///// <param name="sender">The <see cref="object"/> that raised the event.</param>
        ///// <param name="e">An <see cref="RoutedEventArgs"/> containing event data.</param>
        //private void MDIHostLoaded(object sender, RoutedEventArgs e)
        //{
        //    _Content.Width = _Content.ActualWidth;
        //    _Content.Height = _Content.ActualHeight;
        //    _Content.VerticalAlignment = VerticalAlignment.Top;
        //    _Content.HorizontalAlignment = HorizontalAlignment.Left;

        //    InvalidateContentSize();
        //}

        ///// <summary>
        ///// Handles the <see cref="MDIHost"/> size changed event.
        ///// </summary>
        ///// <param name="sender">The <see cref="object"/> that raised the event.</param>
        ///// <param name="e">An <see cref="SizeChangedEventArgs"/> containing event data.</param>
        //private void MDIHostSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (Children.Count == 0)
        //        return;

        //    for (int i = 0; i < Children.Count; i++)
        //    {
        //        MDIChild child = Children[i];

        //        if (child.State == WindowState.Maximized)
        //        {
        //            child.Width = this.ActualWidth;
        //            child.Height = this.ActualHeight;
        //        }

        //        if (child.State == WindowState.Minimized)
        //        {
        //            child.Position.Offset(0, e.NewSize.Height - e.PreviousSize.Height);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Handles the <see cref="MDIHost"/> key down event.
        ///// </summary>
        ///// <param name="sender">The <see cref="object"/> that raised the event.</param>
        ///// <param name="e">A <see cref="KeyEventArgs"/> containing event data.</param>
        //private void MDIHostKeyDown(object sender, KeyEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Handles changes to <see cref="MDIHost"/>'s child windows collection.
        ///// </summary>
        ///// <param name="sender">The <see cref="object"/> that raised the event.</param>
        ///// <param name="e">A <see cref="NotifyCollectionChangedEventArgs"/> containing event data.</param>
        //private void MDIChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:

        //            // Get the newly added child window
        //            MDIChild newChild = Children[e.NewStartingIndex];

        //            newChild.Host = this;

        //            if (this.ActiveChild != null && this.ActiveChild.State == WindowState.Maximized)
        //            {
        //                // Wait until the child loaded event is raised to set it's state to maximized
        //                newChild.Loaded += (s, a) => newChild.State = WindowState.Maximized;
        //            }

        //            // Wait until the child loaded event is raised to set the new child window as the active window
        //            newChild.Loaded += (s, a) => this.ActiveChild = newChild;

        //            _Content.Children.Add(newChild);

        //            break;

        //        case NotifyCollectionChangedAction.Remove:

        //            MDIChild oldChild = (MDIChild)e.OldItems[0];

        //            Children.Remove(oldChild);

        //            break;

        //        case NotifyCollectionChangedAction.Reset:

        //            _Content.Children.Clear();

        //            break;
        //    }

        //    InvalidateContentSize();
        //}

        //#endregion

        //#region Methods

        ///// <summary>
        ///// Initializes the <see cref="MDIHost"/> layout and visuals.
        ///// </summary>
        //private void Initialize()
        //{
        //    Background = (Brush)StyleManager.GetStyleValue(Styles.BackgroundBrush);

        //    // Set the header dock panel elements
        //    _Header.Children.Add(_HeaderMenu);
        //    _Header.Children.Add(_HeaderButtons);

        //    // Set the header area children's dock positions
        //    DockPanel.SetDock(_HeaderMenu, Dock.Left);
        //    DockPanel.SetDock(_HeaderButtons, Dock.Right);

        //    // Set the content area scroll viewer content
        //    _ContentArea.Content = _Content;

        //    // Define the layout grid rows
        //    _Layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        //    _Layout.RowDefinitions.Add(new RowDefinition());

        //    // Set the layout grid content
        //    _Layout.Children.Add(_Header);
        //    _Layout.Children.Add(_ContentArea);

        //    // Assign the layout grid content to rows
        //    Grid.SetRow(_Header, 0);
        //    Grid.SetRow(_ContentArea, 1);

        //    // Assign the layout to the MDI host
        //    this.Content = _Layout;
        //}

        ///// <summary>
        ///// Invalidates the content area size to ensure all content is visible.
        ///// </summary>
        //internal void InvalidateContentSize()
        //{
        //    Point trackingPoint = new Point(0, 0);

        //    // Get the minimum dimensions
        //    for (int i = 0; i < Children.Count; i++)
        //    {
        //        MDIChild child = this.Children[i];

        //        Point maxChildPoint = child.Position;

        //        // Get the bottom right corner of the window
        //        maxChildPoint.X += child.Width;
        //        maxChildPoint.Y += child.Height;

        //        if (maxChildPoint.X > trackingPoint.X)
        //            trackingPoint.X = maxChildPoint.X;

        //        if (maxChildPoint.Y > trackingPoint.Y)
        //            trackingPoint.Y = maxChildPoint.Y;
        //    }

        //    // Apply the minimum dimensions
        //    if (_Content.Width != trackingPoint.X)
        //        _Content.Width = trackingPoint.X;

        //    if (_Content.Height != trackingPoint.Y)
        //        _Content.Height = trackingPoint.Y;
        //}

        //#endregion

        //#region Overrides
        //#endregion
    }


    
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = PART_MDICHILD_HEADER)]
    public class MDIChild : UserControl, INotifyPropertyChanged
    {
        private const string PART_MDICHILD_HEADER = "PART_MDIChildHeader";
        public const double MDICHILD_HEADER_HEIGHT = 24;


       
        static MDIChild()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIChild), new FrameworkPropertyMetadata(typeof(MDIChild)));
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MDIChild), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        //public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(MDIChild), new PropertyMetadata(null));
        //public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(MDIChild), new PropertyMetadata(new Point(0, 0)));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        //public UIElement Content
        //{
        //    get { return (UIElement)GetValue(ContentProperty); }
        //    set { SetValue(ContentProperty, value); }
        //}
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set
            {
                //Margin = new Thickness(value.X, value.Y, 0, 0);
                SetValue(PositionProperty, value);
                //X = value.X;
                //Y = value.Y;
                //SetValue(XProperty, value.X);
                //SetValue(YProperty, value.Y);
                //NotifyPropertyChanged(nameof(Position));
                //NotifyPropertyChanged(nameof(X));
                //NotifyPropertyChanged(nameof(Y));
            }
        }

        // Using a DependencyProperty as the backing store for Point.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(Point), typeof(MDIChild), new FrameworkPropertyMetadata(new Point(0.0, 0.0), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));



        //public double X
        //{
        //    get { return (double)GetValue(XProperty); }
        //    set
        //    {
        //        SetValue(XProperty, value);
        //        //Position = new Point(value, Position.Y);
        //        SetValue(PositionProperty, new Point(value, Position.Y));
        //        //NotifyPropertyChanged(nameof(Position));
        //        //NotifyPropertyChanged(nameof(X));
        //    }
        //}

        //// Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty XProperty =
        //    DependencyProperty.Register(nameof(X), typeof(double), typeof(MDIChild), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsArrange));



        //public double Y
        //{
        //    get { return (double)GetValue(YProperty); }
        //    set
        //    {
        //        SetValue(YProperty, value);
        //        Position = new Point(Position.X, value);
        //        //SetValue(PositionProperty, new Point(Position.X, value));
        //        //NotifyPropertyChanged(nameof(Position));
        //        //NotifyPropertyChanged(nameof(X));
        //    }
        //}

        //// Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty YProperty =
        //    DependencyProperty.Register(nameof(Y), typeof(double), typeof(MDIChild), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MDIHost Host { get; internal set; }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Thumb header = (Thumb)Template.FindName("HeaderThumb", this);

            if (header != null)
            {
                header.DragStarted += Header_DragStarted;
                header.DragDelta += Header_DragDelta;
                header.DragCompleted += Header_DragCompleted;
            }
            else
            {
                Console.WriteLine("Header Thumb not found!");
            }
            
        }

        private void Header_DragCompleted(object sender, DragCompletedEventArgs e)
        {
        }

        private void Header_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Console.WriteLine("Drag Delta");

            double posX = Position.X + e.HorizontalChange;
            double posY = Position.Y + e.VerticalChange;

            if (posX < 0)
                posX = 0;

            if (posY < 0)
                posY = 0;

            Position = new Point(posX, posY);

            UpdateLayout();
        }

        

        private void Header_DragStarted(object sender, DragStartedEventArgs e)
        {
            Console.WriteLine("Drag Start");
        }


        //#region Constants

        //private const string PART_MDICHILD_HEADER = "PART_MDIChildHeader";
        ///// <summary>
        ///// The height of the <see cref="MDIChild"/> header.
        ///// </summary>
        //public const double MDICHILD_HEADER_HEIGHT = 24;

        ///// <summary>
        ///// The minimized width of the <see cref="MDIChild"/> to ensure visibility of caption elements.
        ///// </summary>
        //internal const double MDICHILD_MINIMIZED_WIDTH = 100;

        ///// <summary>
        ///// The minimized height of the <see cref="MDIChild"/> to ensure caption visibility.
        ///// </summary>
        //internal const double MDICHILD_MINIMIZED_HEIGHT = 50;

        //#endregion

        //#region Fields

        //private Panel _Header;

        ///// <summary>
        ///// Stores the dimensions of the <see cref="MDIChild"/> window normal state.
        ///// </summary>
        //private Rect _NormalDimensions;

        //#endregion

        //#region Constructor

        ///// <summary>
        ///// Static constructor called before initializing an instance of <see cref="MDIChild"/>.
        ///// </summary>
        //static MDIChild()
        //{
        //    // Overrides the default style of the inherited UserControl to use the MDIChild style instead.
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIChild), new FrameworkPropertyMetadata(typeof(MDIChild)));
        //}

        ///// <summary>
        ///// Creates and initializes a new instance of <see cref="MDIChild"/>.
        ///// </summary>
        //public MDIChild()
        //{
        //    this.Loaded += MDIChildLoaded;
        //    this.KeyDown += MDIChildKeyDown;
        //    this.GotFocus += MDIChildGotFocus;
        //}

        //#endregion

        //#region Dependency Properties

        ///// <summary>
        ///// Registers the property to set the <see cref="MDIChild"/>'s window title.
        ///// </summary>
        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MDIChild));

        ///// <summary>
        ///// Registers the property to set the state of the <see cref="MDIChild"/> window.
        ///// </summary>
        ///// <remarks><i>Defaults to <see cref="WindowState.Normal"/>.</i></remarks>
        //public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(WindowState), typeof(MDIChild), new PropertyMetadata(WindowState.Normal, StateChangedCallback));


        ///// <summary>
        ///// Registers the property to set the position of the <see cref="MDIChild"/> window.
        ///// </summary>
        ///// <remarks><i>The position is the top left corner of the window and defaults to [0, 0].</i></remarks>
        //public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(MDIChild), new PropertyMetadata(new Point(0, 0)));

        ///// <summary>
        ///// Registers the property to set the content of the <see cref="MDIChild"/> window.
        ///// </summary>
        //public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(MDIChild), new PropertyMetadata(null));

        //#endregion

        //#region Properties

        ///// <summary>
        ///// Gets a reference to the <see cref="MDIHost"/> containing this <see cref="MDIChild"/> window.
        ///// </summary>
        //public MDIHost Host { get; internal set; }

        ///// <summary>
        ///// Gets or sets the title of the <see cref="MDIChild"/> window.
        ///// </summary>
        //public string Title
        //{
        //    get { return (string)GetValue(TitleProperty); }
        //    set { SetValue(TitleProperty, value); }
        //}

        ///// <summary>
        ///// Gets or sets the content of the <see cref="MDIChild"/> window.
        ///// </summary>
        //public UIElement Content
        //{
        //    get { return (UIElement)GetValue(ContentProperty); }
        //    set { SetValue(ContentProperty, value); }
        //}

        ///// <summary>
        ///// Gets or sets the state of the <see cref="MDIChild"/> window.
        ///// </summary>
        //public WindowState State
        //{
        //    get { return (WindowState)GetValue(StateProperty); }
        //    set { SetValue(StateProperty, value); }
        //}

        ///// <summary>
        ///// Gets or sets the position of the <see cref="MDIChild"/> window.
        ///// </summary>
        ///// <remarks><i>The position is the top left corner of the window.</i></remarks>
        //public Point Position
        //{
        //    get { return (Point)GetValue(PositionProperty); }
        //    set { SetValue(PositionProperty, value); }
        //}


        //#endregion

        //#region Event Handlers

        //private void MDIChildLoaded(object sender, RoutedEventArgs e)
        //{
        //    //FrameworkElement element = this;

        //    //while (element != null)
        //    //{
        //    //    element = (FrameworkElement)element.Parent;

        //    //    if(element.GetType() == typeof(MDIHost))
        //    //        element.
        //    //}
        //}

        //private void MDIChildGotFocus(object sender, RoutedEventArgs e)
        //{

        //}

        //private void MDIChildKeyDown(object sender, KeyEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        //#region Callbacks

        ///// <summary>
        ///// Handles the <see cref="StateProperty"/> changed event.
        ///// </summary>
        ///// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        ///// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        //private static void StateChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    MDIChild child = (MDIChild)sender;
        //    MDIHost host = child.Host;

        //    WindowState newState = (WindowState)e.NewValue;
        //    WindowState oldState = (WindowState)e.OldValue;

        //    if (child.Host == null || newState == oldState)
        //        return;


        //    switch(newState)
        //    {
        //        case WindowState.Normal:

        //            child.Position = new Point(child._NormalDimensions.X, child._NormalDimensions.Y);
        //            child.Width = child._NormalDimensions.Width;
        //            child.Height = child._NormalDimensions.Height;

        //            break;

        //        case WindowState.Maximized:
        //            break;

        //        case WindowState.Minimized:
        //            break;
        //    }
        //}


        //#endregion

        //#region Overrides

        //public override void OnApplyTemplate()
        //{
        //    _Header = (Panel)GetTemplateChild(PART_MDICHILD_HEADER);

        //    _Header.PreviewMouseLeftButtonDown += MDIChildPreviewMouseLeftButtonDown;

        //    base.OnApplyTemplate();
        //}

        //private void MDIChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Console.WriteLine("header clicked");
        //}

        //#endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class MDICanvas : Canvas, IScrollInfo
    {
        #region Constants

        /// <summary>
        /// The offset to scroll when using the scroll bar buttons.
        /// </summary>
        private const double DEFAULT_SCROLL_OFFSET = 10;

        /// <summary>
        /// The offset to scroll when using the mouse wheel.
        /// </summary>
        private const double DEFAULT_SCROLL_WHEEL_OFFSET = DEFAULT_SCROLL_OFFSET * 3;

        #endregion

        #region Fields

        /// <summary>
        /// Stores the <see cref="TranslateTransform"/> to use for scrolling the <see cref="MDICanvas"/>.
        /// </summary>
        private TranslateTransform _Transfrom = new TranslateTransform();

        /// <summary>
        /// A reference to the <see cref="ScrollViewer"/> element that encloses the <see cref="MDICanvas"/>.
        /// </summary>
        private ScrollViewer _Owner;

        /// <summary>
        /// See <see cref="CanHorizontallyScroll"/> for information.
        /// </summary>
        private bool _CanScrollHorizontal = false;

        /// <summary>
        /// See <see cref="CanVerticallyScroll"/> for information.
        /// </summary>
        private bool _CanScrollVertical = false;

        /// <summary>
        /// Stores the extent of the <see cref="MDICanvas"/>.
        /// </summary>
        /// <remarks><i>This is the size of the <see cref="MDICanvas"/>.</i></remarks>
        private Size _Extent = new Size(0, 0);

        /// <summary>
        /// Stores the viewport of the <see cref="MDICanvas"/>.
        /// </summary>
        /// <remarks><i>This is the visible part of the <see cref="MDICanvas"/>.</i></remarks>
        private Size _Viewport = new Size(0, 0);

        /// <summary>
        /// Stores the offset of the <see cref="MDICanvas"/>.
        /// </summary>
        /// <remarks><i>This is the amount the viewport is offset from the origin of the <see cref="MDICanvas"/>.</i></remarks>
        private Point _Offset;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates and initializes a new instance of <see cref="MDICanvas"/>.
        /// </summary>
        public MDICanvas()
        {
            this.RenderTransform = _Transfrom;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invalidates the provided offset to be within the vertical viewport bounds.
        /// </summary>
        /// <param name="offset">A <see cref="double"/> containting the offset to invalidate.</param>
        /// <returns>A <see cref="double"/> within vertical the viewport bounds.</returns>
        private double InvalidateVerticalOffset(double offset)
        {
            return Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
        }

        /// <summary>
        /// Invalidates the provided offset to be within the horizontal viewport bounds.
        /// </summary>
        /// <param name="offset">A <see cref="double"/> containting the offset to invalidate.</param>
        /// <returns>A <see cref="double"/> within the horizontal viewport bounds.</returns>
        private double InvalidateHorizontalOffset(double offset)
        {
            return Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
        }

        /// <summary>
        /// Applies the scroll offset to the <see cref="MDICanvas"/>.
        /// </summary>
        /// <param name="offsetX">A <see cref="double"/> containing the amount of horizontal scroll offset.</param>
        /// <param name="offsetY">A <see cref="double"/> containing the amount of vertical scroll offset.</param>
        /// <remarks><i>Scrolling is done by applying a <see cref="TranslateTransform"/> to the entire <see cref="MDICanvas"/>.</i></remarks>
        private void ApplyScrollOffset(double offsetX, double offsetY)
        {
            _Transfrom.X = -offsetX;
            _Transfrom.Y = -offsetY;

            if (_Owner != null)
                _Owner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Calculates viewport and extent sizes and invalidates scroll data.
        /// </summary>
        /// <param name="viewport">A <see cref="Size"/> structure providing the viewport size.</param>
        /// <param name="extent">A <see cref="Size"/> structure providing the extent size.</param>
        /// <remarks><i>Called by the <see cref="MeasureOverride"/> method to calculate viewport and extent sizes and invalidate scroll data.</i></remarks>
        private void UpdateScrollInfo(Size viewport, Size extent)
        {
            if (double.IsInfinity(viewport.Width) || double.IsNaN(viewport.Width))
                viewport.Width = extent.Width;

            if (double.IsInfinity(viewport.Height) || double.IsNaN(viewport.Height))
                viewport.Height = extent.Height;

            _Viewport = viewport;
            _Extent   = extent;

            _Offset.X = InvalidateHorizontalOffset(_Offset.X);
            _Offset.Y = InvalidateVerticalOffset(_Offset.Y);

            if (_Owner != null)
                _Owner.InvalidateScrollInfo();
        }

        /// <summary>
        /// Calculates the required size to show all content.
        /// </summary>
        /// <returns>a <see cref="Size"/> structure containing the size required to show all content.</returns>
        private Size CalculateExtent()
        {
            Size extent = new Size(0, 0);

            extent.Width = InternalChildren.OfType<MDIChild>().Max(m => m.Width + m.Position.X);
            extent.Height = InternalChildren.OfType<MDIChild>().Max(m => m.Height + m.Position.Y);

            return extent;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Supplies the parent of the <see cref="MDICanvas"/> the required size to display all content.
        /// </summary>
        /// <param name="viewport">A <see cref="Size"/> structure providing the available size.</param>
        /// <returns>A <see cref="Size"/> structure containing the size required to display all content of the <see cref="MDICanvas"/>.</returns>
        /// <remarks><i>The <paramref name="viewport"/> parameter returns unmodified.</i></remarks>
        protected override Size MeasureOverride(Size viewport)
        {
            if (InternalChildren.Count == 0)
            {
                return Size.Empty;
            }

            Size extent = CalculateExtent();

            UpdateScrollInfo(viewport, extent);

            return viewport;
        }

        /// <summary>
        /// Arranges the <see cref="MDIChild"/>s inside the <see cref="MDICanvas"/>.
        /// </summary>
        /// <param name="arrangeSize">A <see cref="Size"/> structure providing the available size for arranging child elements.</param>
        /// <returns>A <see cref="Size"/> structure containing the size of the <see cref="MDICanvas"/> and its arranged child elements.</returns>
        /// <remarks><i>The <paramref name="arrangeSize"/> parameter returns unmodified.</i></remarks>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            int count = InternalChildren.Count;

            if (count == 0)
            {
                return arrangeSize;
            }

            foreach (MDIChild child in InternalChildren)
            {
                // TODO: Minimized / Maximized / Normal states
                child.Arrange(new Rect(child.Position.X, child.Position.Y, child.Width, child.Height));
            }

            ApplyScrollOffset(HorizontalOffset, VerticalOffset);

            return arrangeSize;
        }

        #endregion

        #region IScrollInfo

        #region IScrollInfo: Properties

        /// <summary>
        /// Gets or sets a reference to the <see cref="ScrollViewer"/> element enclosing the <see cref="MDICanvas"/>.
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        /// <summary>
        /// Gets or sets whether the horizontal scrollbar is enabled.
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get { return _CanScrollHorizontal; }
            set { _CanScrollHorizontal = value; }
        }

        /// <summary>
        /// Gets or sets whether the vertical scrollbar is enabled.
        /// </summary>
        public bool CanVerticallyScroll
        {
            get { return _CanScrollVertical; }
            set { _CanScrollVertical = value; }
        }

        /// <summary>
        /// Gets the horizontal scroll offset.
        /// </summary>
        public double HorizontalOffset
        {
            get { return _Offset.X; }
        }

        /// <summary>
        /// Gets the vertical scroll offset.
        /// </summary>
        public double VerticalOffset
        {
            get { return _Offset.Y; }
        }

        /// <summary>
        /// Gets the extent width.
        /// </summary>
        /// <remarks><i>This is the width of the entire <see cref="MDICanvas"/>.</i></remarks>
        public double ExtentWidth
        {
            get { return _Extent.Width; }
        }

        /// <summary>
        /// Gets the extent height.
        /// </summary>
        /// <remarks><i>This is the height of the entire <see cref="MDICanvas"/>.</i></remarks>
        public double ExtentHeight
        {
            get { return _Extent.Height; }
        }

        /// <summary>
        /// Gets the width of the viewport.
        /// </summary>
        /// <remarks><i>This is the width of the visible part of the <see cref="MDICanvas"/>.</i></remarks>
        public double ViewportWidth
        {
            get { return _Viewport.Width; }
        }

        /// <summary>
        /// Gets the height of the viewport.
        /// </summary>
        /// <remarks><i>This is the height of the visible part of the <see cref="MDICanvas"/>.</i></remarks>
        public double ViewportHeight
        {
            get { return _Viewport.Height; }
        }

        #endregion

        #region IScrollInfo: Methods

        /// <summary>
        /// Scrolls the content up by using the scrollbar up button.
        /// </summary>
        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - DEFAULT_SCROLL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content down by using the scrollbar down button.
        /// </summary>
        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + DEFAULT_SCROLL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content to the top by clicking inside the scrollbar.
        /// </summary>
        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        /// <summary>
        /// Scrolls the content to the bottom by clicking inside the scrollbar.
        /// </summary>
        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        /// <summary>
        /// Scrolls the content up by using the mouse wheel.
        /// </summary>
        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - DEFAULT_SCROLL_WHEEL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content down by using the mouse wheel.
        /// </summary>
        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + DEFAULT_SCROLL_WHEEL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content left by clicking the scrollbar left button.
        /// </summary>
        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - DEFAULT_SCROLL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content right by clicking the scrollbar right button.
        /// </summary>
        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + DEFAULT_SCROLL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content to the left end by clicking inside the scrollbar.
        /// </summary>
        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        /// <summary>
        /// Scrolls the content to the right end by clicking inside the scrollbar.
        /// </summary>
        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        /// <summary>
        /// Scrolls the content left by using the mouse wheel.
        /// </summary>
        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - DEFAULT_SCROLL_WHEEL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content right by using the mouse wheel.
        /// </summary>
        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + DEFAULT_SCROLL_WHEEL_OFFSET);
        }

        /// <summary>
        /// Scrolls the content horizontal by the specified offset.
        /// </summary>
        /// <param name="offset">A <see cref="double"/> specifying the offset to scroll to.</param>
        public void SetHorizontalOffset(double offset)
        {
            offset = InvalidateHorizontalOffset(offset);

            if (offset != _Offset.X)
            {
                _Offset.X = offset;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Scrolls the content vertical by the specified offset.
        /// </summary>
        /// <param name="offset">A <see cref="double"/> specifying the offset to scroll to.</param>
        public void SetVerticalOffset(double offset)
        {
            offset = InvalidateVerticalOffset(offset);

            if (offset != _Offset.Y)
            {
                _Offset.Y = offset;
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Scrolls the specified item into view.
        /// </summary>
        /// <param name="visual">A <see cref="Visual"/> representing the item scroll into view.</param>
        /// <param name="rectangle">A <see cref="Rect"/> structure identifying the area to make visible.</param>
        /// <returns>A <see cref="Rect"/> structure containing the visible area.</returns>
        /// <remarks><i>Not implemented, returns the <paramref name="area"/> paramater unmodified.</i></remarks>
        public Rect MakeVisible(Visual visual, Rect area)
        {
            return area;
        }

        #endregion

        #endregion
    }
}
