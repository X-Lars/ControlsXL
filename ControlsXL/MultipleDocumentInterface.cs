using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace ControlsXL
{
    //TODO: Multiple window selection


    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = PART_MDIHOST_TABS, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_MDIHOST_MENU, Type = typeof(Menu))]
    public class MDIHost : ItemsControl
    {
        #region Constants

        #region Constants: Template Parts

        /// <summary>
        /// The template part name of the <see cref="MDIHost"/> header tabs area in xaml.
        /// </summary>
        public const string PART_MDIHOST_TABS = "PART_MDIHostTabs";

        /// <summary>
        /// The template part name of the <see cref="MDIHost"/> menu in xaml.
        /// </summary>
        public const string PART_MDIHOST_MENU = "PART_MDIHostMenu";

        #endregion

        /// <summary>
        /// The height of the <see cref="MDIHost"/> header area.
        /// </summary>
        public const double MDIHOST_HEADER_HEIGHT = 24;

        #endregion

        #region Fields

        /// <summary>
        /// Stores the original title of the main window.
        /// </summary>
        private string _OriginalTitle;

        /// <summary>
        /// Stores a reference to the <see cref="MDIHost"/> menu.
        /// </summary>
        private Menu _Menu;

        private readonly Dispatcher _Dispatcher;

        #endregion

        #region Events

        /// <summary>
        /// Provides the signature for the <see cref="SelectionChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MDIChildEventArgs"/> containing event data.</param>
        public delegate void SelectionChangedHandler(object sender, MDIChildEventArgs e);

        /// <summary>
        /// Raised when a <see cref="MDIChild"/> window is selected or deselected.
        /// </summary>
        public event SelectionChangedHandler SelectionChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="MDIHost"/>.
        /// </summary>
        static MDIHost()
        {
            // Overrides the default style of the inherited ItemsControl to use the MDIHost style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIHost), new FrameworkPropertyMetadata(typeof(MDIHost)));
            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initalizes a new <see cref="MDIHost"/> instance.
        /// </summary>
        public MDIHost()
        {
            // Cannot access the title at design time
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _OriginalTitle = Application.Current.MainWindow.Title;
            }

            // Bind the commands
            CommandBindings.Add(new CommandBinding(_CloseCommand, OnCloseCommand, CanExecuteCloseCommand));
            CommandBindings.Add(new CommandBinding(_MinimizeCommand, OnMinimizeCommand, CanExecuteMinimizeCommand));
            CommandBindings.Add(new CommandBinding(_MaximizeCommand, OnMaximizeCommand, CanExecuteMaximizeCommand));
            CommandBindings.Add(new CommandBinding(_RestoreCommand, OnRestoreCommand, CanExecuteRestoreCommand));

            // Bind the command keyboard shortcuts
            InputBindings.Add(new InputBinding(_CloseCommand, new KeyGesture(Key.F4, ModifierKeys.Control, "CTRL + F4")));
            InputBindings.Add(new InputBinding(_CloseCommand, new KeyGesture(Key.W, ModifierKeys.Control, "CTRL + W")));

            _Dispatcher = Application.Current.Dispatcher;
        }

        #endregion

        #region Commands

        #region Commands: Registrations

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _CloseCommand = new RoutedUICommand(nameof(CloseCommand), nameof(CloseCommand), typeof(MDIHost));

        /// <summary>
        /// Defines the command to minimize the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _MinimizeCommand = new RoutedUICommand(nameof(MinimizeCommand), nameof(MinimizeCommand), typeof(MDIHost));

        /// <summary>
        /// Defines the command to maximize the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _MaximizeCommand = new RoutedUICommand(nameof(MaximizeCommand), nameof(MaximizeCommand), typeof(MDIHost));

        /// <summary>
        /// Defines the command to restore the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _RestoreCommand = new RoutedUICommand(nameof(RestoreCommand), nameof(RestoreCommand), typeof(MDIHost));

        #endregion

        #region Commands: Properties

        /// <summary>
        /// Gets the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand CloseCommand
        {
            get { return _CloseCommand; }
        }

        /// <summary>
        /// Gets the command to minimize the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand MinimizeCommand
        {
            get { return _MinimizeCommand; }
        }

        /// <summary>
        /// Gets the command to maximize the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand MaximizeCommand
        {
            get { return _MaximizeCommand; }
        }

        /// <summary>
        /// Gets the command to restore the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand RestoreCommand
        {
            get { return _RestoreCommand; }
        }

        #endregion

        #region Commands: Event Handlers

        /// <summary>
        /// Implements the <see cref="CloseCommand"/> to remove the selected <see cref="MDIChild"/> window from the <see cref="MDIHost"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Items.Remove(SelectedChild);
            e.Handled = false;
        }

        /// <summary>
        /// Implements the <see cref="MinimizeCommand"/> to minimize the selected <see cref="MDIChild"/> window.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedChild.State = WindowState.Minimized;
        }

        /// <summary>
        /// Implements the <see cref="MaximizeCommand"/> to maximize the selected <see cref="MDIChild"/> window.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnMaximizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedChild.State = WindowState.Maximized;
        }

        /// <summary>
        /// Implements the <see cref="RestoreCommand"/> to restore the selected <see cref="MDIChild"/> window to it's normal size.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnRestoreCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedChild.State = WindowState.Normal;
        }

        /// <summary>
        /// Determines if the <see cref="CloseCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteCloseCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedChild != null && HasItems;
            e.Handled = true;
        }

        /// <summary>
        /// Determines if the <see cref="MaximizeCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteMaximizeCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedChild != null && SelectedChild.State != WindowState.Maximized && HasItems;
            e.Handled = true;
        }

        /// <summary>
        /// Determines if the <see cref="MinimizeCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteMinimizeCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedChild != null && SelectedChild.State != WindowState.Minimized && HasItems;
            e.Handled = true;
        }

        /// <summary>
        /// Determines if the <see cref="RestoreCommand"/> can be executed.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="CanExecuteRoutedEventArgs"/> containing event data.</param>
        /// <remarks><i>The <see cref="UIElement"/>s bound to the executing command are automatically enabled or disabled based on setting the <code>e.CanExecute</code> parameter of this event handler to true or false.</i></remarks>
        private void CanExecuteRestoreCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedChild != null && SelectedChild.State != WindowState.Normal && HasItems;
            e.Handled = true;
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the selected <see cref="MDIChild"/> window.
        /// </summary>
        public static readonly DependencyProperty SelectedChildProperty = DependencyProperty.Register(nameof(SelectedChild), typeof(MDIChild), typeof(MDIHost), new PropertyMetadata(null, SelectedChildChanged));

        /// <summary>
        /// Registers the property to determine the <see cref="MDIHost"/> menu visibility.
        /// </summary>
        public static readonly DependencyProperty ShowMenuProperty = DependencyProperty.Register("ShowMenu", typeof(bool), typeof(MDIHost), new PropertyMetadata(true));

        /// <summary>
        /// Registers the property to determine the <see cref="MDIHost"/> statusbar visibility.
        /// </summary>
        public static readonly DependencyProperty ShowStatusbarProperty = DependencyProperty.Register("ShowStatusbar", typeof(bool), typeof(MDIHost), new PropertyMetadata(true));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the selected <see cref="MDIChild"/> window.
        /// </summary>
        internal MDIChild SelectedChild
        {
            get { return (MDIChild)GetValue(SelectedChildProperty); }
            set { SetValue(SelectedChildProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="MDIHost"/> menu is visible.
        /// </summary>
        public bool ShowMenu
        {
            get { return (bool)GetValue(ShowMenuProperty); }
            set { SetValue(ShowMenuProperty, value); }
        }

        /// <summary>
        /// Gets or sets wheter the <see cref="MDIHost"/> statusbar is visible.
        /// </summary>
        public bool ShowStatusbar
        {
            get { return (bool)GetValue(ShowStatusbarProperty); }
            set { SetValue(ShowStatusbarProperty, value); }
        }

        #endregion

        #region Dependency Properties: Callbacks

        private static readonly object _Lock = new object();
        /// <summary>
        /// Callback function for the <see cref="SelectedChildProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void SelectedChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIHost host = (MDIHost)d;

            MDIChild activatedChild   = (MDIChild)e.NewValue;
            MDIChild deactivatedChild = (MDIChild)e.OldValue;

            if (activatedChild == deactivatedChild)
                return;

            if (deactivatedChild != null)
            {
                Debug.Print($"[{nameof(MDIHost)}.{nameof(SelectedChildChanged)}] Deactivate: {deactivatedChild.Title}");
            }

            if (activatedChild != null)
            {
                Debug.Print($"[{nameof(MDIHost)}.{nameof(SelectedChildChanged)}] Activate: {activatedChild.Title}");
                host.SetTitle(activatedChild.Title);

                // TODO: Bring to front
            }
            
            host.SelectionChanged?.Invoke(host, new MDIChildEventArgs(activatedChild, deactivatedChild));
        }

        #endregion

        #endregion

        #region Properties
    
        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the  mouse wheel event of the <see cref="MDIChild"/> window tabs to scroll to the tabs horizontally.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseWheelEventArgs"/> containing event data.</param>
        private void MDIHostTabsPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;

            if (e.Delta > 0)
            {
                viewer.LineLeft();
            }
            else
            {
                viewer.LineRight();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the title of the application window to include the <see cref="MDIChild.Title"/>.
        /// </summary>
        /// <param name="title">A <see cref="string"/> containing the text to append to the title.</param>
        private void SetTitle(string title = null)
        {
            // Title of the main window cannot be accessed at design time
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (title != null)
                {
                    Application.Current.MainWindow.Title = $"{_OriginalTitle} - {title}";
                }
                else
                {
                    Application.Current.MainWindow.Title = _OriginalTitle;
                }
            }
        }

        /// <summary>
        /// Sets the selected child, called from <see cref="MDIChild"/> when the child is activated.
        /// </summary>
        /// <param name="child"></param>
        public void SelectChild(MDIChild child)
        {
            SelectedChild = child;
        }

        public void ShowWindow(Type type)
        {
            if (!type.IsSubclassOf(typeof(MDIChild)))
                return;

            foreach (MDIChild child in Items)
            {
                // Prevent opening a duplicate window
                if (child.GetType() == type)
                {
                    SelectChild(child);
                    //child.IsSelected = true;
                    return;
                }
            }

            MDIChild window = (MDIChild)Activator.CreateInstance(type);

            Items.Add(window);
        }

        /// <summary>
        /// Invalidates the <see cref="MDIHost"/> menu.
        /// </summary>
        private void InvalidateMenu()
        {
            //if (Items == null)
            //    return;

            if (_Menu != null)
            {
                _Menu.Items.Clear();

                MenuItem menuRoot = new MenuItem { Header = "Windows" };

                _Menu.Items.Add(menuRoot);

                for (int i = 0; i < Items.Count; i++)
                {
                    MDIChild child = (MDIChild)Items[i];

                    MenuItem item = new MenuItem { Header = child.Title };

                    if (string.IsNullOrEmpty(child.Title))
                        child.Title = $"Window {i + 1}";

                    item.Click += (s, a) => { child.IsSelected = true; };
                    MenuItem rootItem = (MenuItem)_Menu.Items[0];
                    rootItem.Items.Add(item);
                }
            }
        }
        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            
            ScrollViewer tabsViewer = GetTemplateChild(PART_MDIHOST_TABS) as ScrollViewer;

            tabsViewer.PreviewMouseWheel += MDIHostTabsPreviewMouseWheel;

            _Menu = GetTemplateChild(PART_MDIHOST_MENU) as Menu;

            
            InvalidateMenu();

            base.OnApplyTemplate();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            
            if (_Menu != null)
                InvalidateMenu();

            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.Print($"[{nameof(MDIHost)}.{nameof(OnItemsChanged)}] Added: {e.NewItems[0]}");
                    break;

                case NotifyCollectionChangedAction.Remove:

                    // If no items left restore the original main window title
                    if (Items.Count == 0) SetTitle();
                    // TODO: Move to 

                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

       

        #endregion
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
            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initalizes a new <see cref="MDIChild"/> window.
        /// </summary>
        public MDIChild()
        {
            // Initialize the command bindings
            CommandBindings.Add(new CommandBinding(_CommandClose, OnCloseCommand));
            CommandBindings.Add(new CommandBinding(_CommandMinimize, OnMinimizeCommand));
            CommandBindings.Add(new CommandBinding(_CommandMaximize, OnMaximizeCommand));
            CommandBindings.Add(new CommandBinding(_CommandRestore, OnRestoreCommand));

            InputBindings.Add(new InputBinding(CloseCommand, new KeyGesture(Key.F4, ModifierKeys.Control, "CTRL + F4")));
            InputBindings.Add(new InputBinding(CloseCommand, new KeyGesture(Key.W , ModifierKeys.Control, "CTRL + W")));
        }

        #endregion

        #region Events

        private static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent("Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MDIChild));

        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        #endregion

        #region Commands

        #region Commands: Registrations

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _CommandClose = new RoutedUICommand(nameof(CloseCommand), nameof(CloseCommand), typeof(MDIChild));

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _CommandMinimize = new RoutedUICommand(nameof(MinimizeCommand), nameof(MinimizeCommand), typeof(MDIChild));

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _CommandMaximize = new RoutedUICommand(nameof(MaximizeCommand), nameof(MaximizeCommand), typeof(MDIChild));

        /// <summary>
        /// Defines the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        private static RoutedUICommand _CommandRestore = new RoutedUICommand(nameof(RestoreCommand), nameof(RestoreCommand), typeof(MDIChild));

        #endregion

        #region Command: Properties

        /// <summary>
        /// Gets the command to close the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand CloseCommand
        {
            get { return _CommandClose; }
        }

        /// <summary>
        /// Gets the command to minimize the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand MinimizeCommand
        {
            get { return _CommandMinimize; }
        }

        /// <summary>
        /// Gets the command to maximize the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand MaximizeCommand
        {
            get { return _CommandMaximize; }
        }

        /// <summary>
        /// Gets the command to restore the <see cref="MDIChild"/> window.
        /// </summary>
        public static ICommand RestoreCommand
        {
            get { return _CommandRestore; }
        }

        #endregion

        #region Command: Handlers

        /// <summary>
        /// Implements the <see cref="CloseCommand"/> to remove the selected <see cref="MDIChild"/> window from the <see cref="MDIHost"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        protected virtual void OnCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            // IMPORTANT: The child has to be selected before it is removed otherwise the Z index is not updated
            if (!IsSelected)
                IsSelected = true;

            this.RaiseEvent(new RoutedEventArgs(CloseEvent));

            

            this.Host.Items.Remove(this);
        }

        /// <summary>
        /// Implements the <see cref="MinimizeCommand"/> to minimize the selected <see cref="MDIChild"/> window.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsSelected)
                IsSelected = true;

            this.State = WindowState.Minimized;
        }

        /// <summary>
        /// Implements the <see cref="MaximizeCommand"/> to maximize the selected <see cref="MDIChild"/> window.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnMaximizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsSelected)
                IsSelected = true;

            this.State = WindowState.Maximized;
        }

        /// <summary>
        /// Implements the <see cref="RestoreCommand"/> to restore the selected <see cref="MDIChild"/> window.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnRestoreCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsSelected)
                IsSelected = true;

            this.State = WindowState.Normal;
        }
 
        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to determin if the <see cref="MDIChild"/> window is selected.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MDIChild), new PropertyMetadata(true, IsSelectedPropertyChangedCallback));
        
        /// <summary>
        /// Registers the property to determin the position of the <see cref="MDIChild"/> window.
        /// </summary>
        /// <remarks><i>Default has to be -1, -1 to be raise the event on initialize </i></remarks>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(Point), typeof(MDIChild), new PropertyMetadata(new Point(-1.0, -1.0), PositionPropertyChangedCallback));

        /// <summary>
        /// Registers the property to set the status bar visibility.
        /// </summary>
        public static readonly DependencyProperty ShowStatusBarProperty = DependencyProperty.Register(nameof(ShowStatusBar), typeof(bool), typeof(MDIChild), new PropertyMetadata(false));

        /// <summary>
        /// Registers the property to determin the state of the <see cref="MDIChild"/> window.
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(WindowState), typeof(MDIChild), new PropertyMetadata(WindowState.Normal, StatePropertyChangedCallback));

        /// <summary>
        /// Registers the property to determin the title of the <see cref="MDIChild"/> window.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MDIChild), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Registers the property to set the <see cref="MDIChild"/> window's widget.
        /// </summary>
        public static readonly DependencyProperty WidgetProperty = DependencyProperty.Register(nameof(Widget), typeof(UIElement), typeof(MDIChild));

        #endregion

        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets whether the <see cref="MDIChild"/> window is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
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
        /// Gets or sets the status bar visibility.
        /// </summary>
        public bool ShowStatusBar
        {
            get { return (bool)GetValue(ShowStatusBarProperty); }
            set { SetValue(ShowStatusBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets the state of the <see cref="MDIChild"/> window.
        /// </summary>
        public WindowState State
        {
            get { return (WindowState)GetValue(StateProperty); }
            internal set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="MDIChild"/> window.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the widget of the <see cref="MDIChild"/> window.
        /// </summary>
        public UIElement Widget
        {
            get { return (UIElement)GetValue(WidgetProperty); }
            set { SetValue(WidgetProperty, value); }
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
            if (e.OldValue == e.NewValue)
                return;

            bool isSelected = (bool)e.NewValue;

            if (isSelected)
            {
                
                MDICanvas.Select((MDIChild)d);
                
                //MDIChild child = (MDIChild)d;

                //// Cannot acces caption at design time
                //if (!DesignerProperties.GetIsInDesignMode(child))
                //{
                //    // Set the caption of the main window
                //    child.Host.Title = child.Title;
                //}

                //// Get current top child state
                //MDIChild topChild = child.Host.Items.Cast<MDIChild>().OrderByDescending(i => i.ZIndex).FirstOrDefault();

                //if (topChild.State == WindowState.Maximized)
                //{
                //    topChild.State = WindowState.Normal;
                //    child.State = WindowState.Maximized;
                //}

                //foreach (MDIChild item in child.Host.Items)
                //{

                //    // Move all items in front of the current item one step back
                //    if (item.ZIndex > child.ZIndex)
                //    {
                //        item.ZIndex--;
                //        Canvas.SetZIndex(item, item.ZIndex);
                //    }

                //    // Ensure every child is deselected
                //    if (item != child)
                //        item.IsSelected = false;
                //}

                //child.ZIndex = child.Host.Items.Count;
                //Canvas.SetZIndex(child, child.ZIndex);

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

        #region Methods

        private void ChangeState(WindowState previousState, WindowState newState)
        {
            // If the previous state of the window was maximized it was already selected
            if (previousState != WindowState.Maximized)
            {
                // Ensure the window is selected before changing it's state
                if (!IsSelected)
                    IsSelected = true;
            }

            // Store the normal size to be able to restore the window
            if (previousState == WindowState.Normal)
            {
                _NormalSize = new Rect(Math.Max(Canvas.GetLeft(this), 0), Math.Max(Canvas.GetTop(this), 0), Width, Height);
            }

            switch (newState)
            {
                case WindowState.Normal:

                    // TODO: Move to MDICanvas
                    Width = _NormalSize.Width;
                    Height = _NormalSize.Height;

                    Position = new Point(_NormalSize.X, _NormalSize.Y);

                    ToolTip = null;

                    break;

                case WindowState.Minimized:

                    double positionX = 0;

                    // TODO: Move to MDICanvas
                    foreach (MDIChild child in Host.Items)
                    {
                        if (child.State == WindowState.Minimized && child.IsSelected == false)
                        {
                            positionX = Math.Max(positionX, child.Position.X + child.ActualWidth);
                        }
                    }

                    Width = MinWidth + BorderThickness.Left + BorderThickness.Right;
                    Height = MinHeight + BorderThickness.Top + BorderThickness.Bottom;

                    //Position = new Point(positionX, canvas.ActualHeight - Height);

                    ToolTip = Title;
                    

                    break;

                case WindowState.Maximized:


                    //TODO: Change width to ensure update????? solving issue second maximize not working?????
                    // Enforces a layout pass
                    Position = new Point(-1, -1);

                    ToolTip = null;

                    break;
            }

            //InvalidateMeasure();
            //InvalidateArrange();
            //UpdateLayout();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            MDICanvas canvas = VisualTreeHelper.GetParent(this) as MDICanvas;

            if (IsSelected == false)
                IsSelected = true;

            ((MDIHost)this.Parent).SelectedChild = this;

            //Focus();

            // Allow the event to route further
            e.Handled = false;
        }

        #endregion
    }

    /// <summary>
    /// The visual component of the <see cref="MDIHost"/> to handle layout and rendering of it's collection of <see cref="MDIChild"/> windows.
    /// </summary>
    public class MDICanvas : Canvas
    {
        #region Fields
        
        /// <summary>
        /// Stores a reference to parent <see cref="MDIHost"/> containing the <see cref="MDICanvas"/>.
        /// </summary>
        private static MDIHost _Host;

        /// <summary>
        /// Stores a static reference to the <see cref="MDICanvas"/>.
        /// </summary>
        private static MDICanvas _Instance;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the <see cref="MDICanvas"/> and initializes the static reference to itself.
        /// </summary>
        public MDICanvas()
        {
            _Instance = this;
        }
 
        #endregion

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
        public IEnumerable<MDIChild> MinimizedChildren
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
        private bool HasMaximizedChild
        {
            get 
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return false;

                return Children.OfType<MDIChild>().Any(i => i.State == WindowState.Maximized); 
            }
        }

        /// <summary>
        /// Gets the maximized <see cref="MDIChild"/> window if any.
        /// </summary>
        private MDIChild MaximizedChild
        {
            get { return Children.OfType<MDIChild>().Where(i => i.State == WindowState.Maximized).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the top most <see cref="MDIChild"/> window if any.
        /// </summary>
        private static MDIChild TopChild
        {
            get { return _Instance.Children.OfType<MDIChild>().OrderByDescending(i => GetZIndex(i)).FirstOrDefault(); }
        }

        /// <summary>
        /// Selects the top <see cref="MDIChild"/> window if any.
        /// </summary>
        private void SelectTopChild()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Children.OfType<MDIChild>().OrderByDescending(i => GetZIndex(i)).FirstOrDefault(i => i.IsSelected = true);
            }
        }

        #endregion

        public void DeselectAll()
        {
            foreach (MDIChild child in Children)
            {
                child.IsSelected = false;
            }
        }

        /// <summary>
        /// Selects the provided <see cref="MDIChild"/>
        /// </summary>
        /// <param name="child">The <see cref="MDIChild"/> to select</param>
        public static void Select(MDIChild child)
        {
            if (TopChild.State == WindowState.Maximized)
            {
                TopChild.State = WindowState.Normal;
                child.State = WindowState.Maximized;
            }

            int c = Canvas.GetZIndex(child);

            foreach (MDIChild item in _Instance.Children)
            {
                if(item == null)
                {
                    //TopChild.IsSelected = true;
                    _Host.SelectedChild = TopChild;
                    return;
                }

                int z = Canvas.GetZIndex(item);
                if (z > c)
                {
                    z--;
                    Canvas.SetZIndex(item, z);
                }

                if(item != child)
                {
                    item.IsSelected = false;
                }
            }
            Canvas.SetZIndex(child, _Instance.Children.Count);
            _Host.SelectedChild = child;
            
        }

        public MDIChild ActiveChild
        {
            get { return Children.OfType<MDIChild>().Where(child => child.IsSelected).FirstOrDefault(); }
            set 
            {
                DeselectAll();

                value.IsSelected = true;
            }
        }

        #region Overrides

        /// <summary>
        /// Initializes the static reference to the <see cref="MDIHost"/> containing the <see cref="MDICanvas"/>.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _Host = (MDIHost)TemplatedParent;
            _Host.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, MDIChildEventArgs e)
        {
            Debug.Print($"[{nameof(MDICanvas)}.{nameof(SelectionChanged)}]");
            // TODO: Bring selected child to front
            
        }

        /// <summary>
        /// Handles changes in the collection of visual children of the <see cref="MDICanvas"/>.
        /// </summary>
        /// <param name="visualAdded">A reference to the <see cref="DependencyObject"/> that is added.</param>
        /// <param name="visualRemoved">A reference to the <see cref="DependencyObject"/> that is removed.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            //if(!DesignerProperties.GetIsInDesignMode(this))
            //{ 
            // Don't run this code in design time
            

            if (visualAdded != null)
            {
                if (Children != null)
                {
                    MDIChild child = (MDIChild)visualAdded;

                    SetZIndex(child, GetZIndex(TopChild) + 1);
                    DeselectAll();

                    child.IsSelected = true;

                    Debug.Print($"[{nameof(MDICanvas)}.{nameof(OnVisualChildrenChanged)}] Added: {child.Title}");
                }
            }

            
            if (visualRemoved != null)
            {
                Debug.Print($"[{nameof(MDICanvas)}.{nameof(OnVisualChildrenChanged)}] Removed: {((MDIChild)visualRemoved).Title}");

                _Host.SelectedChild = TopChild;
                SelectTopChild();
            }
        }




        // TODO: make scroll bars invisible if all items fit by offsetting item
        // TODO: ensure minimized windows do not enable the scrolbars, try wrapping and stacking
        // TODO: foreach sorting problem, minimized items are switcht based on position inside the collection

        /// <summary>
        /// Handles the first layout pass to meassure or override the desired size of the <see cref="MDICanvas"/> and it's descendants.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>A <see cref="Size"/> structure containing the desired size of the <see cref="MDICanvas"/>.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            if (!HasMaximizedChild)
            {
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
                        // TODO: Minimized children
                        size.Height = Math.Max(size.Height, top + desiredSize.Height);
                        size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    }
                }
            }
            else
            {
                // Return a width and height of 0 to make the scrollbars dissapeare
                return new Size(0, 0);
            }

            return size;
        }

        /// <summary>
        /// Handles the second layout pass to arrange or override the arrangement of the <see cref="MDICanvas"/> and it's descendants.
        /// </summary>
        /// <param name="arrangeSize"></param>
        /// <returns>A <see cref="Size"/> structure containing the final size of the <see cref="MDICanvas"/>.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double offsetX = 0;

            if(HasMaximizedChild)
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    MDIChild maximizedChild = MaximizedChild;

                    maximizedChild.Width = arrangeSize.Width;
                    maximizedChild.Height = arrangeSize.Height;
                    maximizedChild.Position = new Point(0, 0);

                    maximizedChild.Arrange(new Rect(maximizedChild.Position.X, maximizedChild.Position.Y, maximizedChild.Width, maximizedChild.Height));
                }

            }
            else
            {
                foreach (MDIChild child in MinimizedChildren)
                {
                    child.Position = new Point(offsetX, ActualHeight - child.Height);
                    offsetX += child.Width;
                }
            }

            return base.ArrangeOverride(arrangeSize);
        }

        #endregion
    }

    public class MDIChildEventArgs : EventArgs
    {
        public MDIChildEventArgs(MDIChild activated, MDIChild deactivated) : base()
        {
            Activated = activated;
            Deactivated = deactivated;
        }

        public MDIChild Activated { get; private set; }
        public MDIChild Deactivated { get; private set; }
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



        public bool CanMove
        {
            get { return (bool)GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanMove.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanMoveProperty =
            DependencyProperty.Register("CanMove", typeof(bool), typeof(MDIMoveHandle), new PropertyMetadata(true));



        public MDIMoveHandle()
        {
            //RenderTransformOrigin = new Point(0.5, 0.5);

            SizeChanged += MDIMoveHandleSizeChanged;
            DragStarted += XMoveThumbDragStarted;
            DragDelta += XMoveThumbDragDelta;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            _Child = DataContext as MDIChild;

            if (_Child != null)
            {
                _Child.IsSelected = true;
                _Child.BringIntoView();
            }
            e.Handled = false;
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDoubleClick(e);

           
            _Child = DataContext as MDIChild;

            if(_Child != null)
            {
                switch(_Child.State)
                {
                    case WindowState.Normal:
                        _Child.State = WindowState.Maximized;
                        break;
                    case WindowState.Minimized:
                    case WindowState.Maximized:
                        _Child.State = WindowState.Normal;
                        break;
                }

                
                //_Child.State = WindowState.Maximized;
            }

        }
        private Point _Start;
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
            if (!CanMove)
                return;

            _Child = DataContext as MDIChild;

            if (_Child != null)
            {
                _Canvas = VisualTreeHelper.GetParent(_Child) as MDICanvas;
            }
            else
            {
                _Canvas = DataContext as MDICanvas;

                //_Canvas.BringIntoView(new Rect(_Start.X, _Start.Y, _Canvas.Width, _Canvas.Height));
                //e.Handled = true;
            }
        }

        private void XMoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!CanMove)
                return;


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
                // TODO: Canvas Dragging (check scrollviewer panning mode?)

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
                //    Canvas.SetLeft(item, deltaHorizontal);
                //    Canvas.SetTop(item, deltaVertical);
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
