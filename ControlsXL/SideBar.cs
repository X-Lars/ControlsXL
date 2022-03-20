using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlsXL
{
    /// <summary>
    /// The sidebar provides a section based stacked layout menu.
    /// </summary>
    [TemplatePart(Name = PART_SIDEBAR_COMMON)]
    [TemplatePart(Name = PART_SIDEBAR_COMMON_CONTENT_BUTTON)]
    [TemplatePart(Name = PART_SIDEBAR_COMMON_CONTENT_POPUP)]
    [TemplatePart(Name = PART_SIDEBAR_HEADER)]
    [TemplatePart(Name = PART_SIDEBAR_MINIMIZED_SECTIONS)]
    [TemplatePart(Name = PART_SIDEBAR_OVERFLOW_MENU)]
    [TemplatePart(Name = PART_SIDEBAR_SECTION_CONTENT_POPUP)]
    [TemplatePart(Name = PART_SIDEBAR_SECTION_CONTENT_BUTTON)]
    public class Sidebar : HeaderedItemsControl
    {
        #region Constants

        #region Constants: Template Parts

        /// <summary>
        /// The template part name of the sidebar common area in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COMMON = "PART_SidebarCommon";

        /// <summary>
        /// The template part name of the collapsed sidebar common area button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COMMON_CONTENT_BUTTON = "PART_SidebarCommonContentButton";

        /// <summary>
        /// The template part name of the collapsed sidebar common area content in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COMMON_CONTENT_POPUP = "PART_SidebarCommonContentPopup";

        /// <summary>
        /// The template part name of the sidebar header button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_HEADER = "PART_SidebarHeader";

        /// <summary>
        /// The template part name of the minimized sections in xaml.
        /// </summary>
        private const string PART_SIDEBAR_MINIMIZED_SECTIONS = "PART_SidebarMinimizedSections";

        /// <summary>
        /// The template part name of the overflow menu in xaml.
        /// </summary>
        private const string PART_SIDEBAR_OVERFLOW_MENU = "PART_SidebarOverflowMenu";

        /// <summary>
        /// The template part name of the collapsed sidebar content button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_SECTION_CONTENT_BUTTON = "PART_SidebarSectionContentButton";

        /// <summary>
        /// The template part name of the collapsed sidebar content in xaml.
        /// </summary>
        private const string PART_SIDEBAR_SECTION_CONTENT_POPUP = "PART_SidebarSectionContentPopup";

        #endregion

        /// <summary>
        /// The height of <see cref="SidebarSection"/>s.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_HEIGHT = 32;

        /// <summary>
        /// The minimized dimensions of <see cref="SidebarSection"/>s.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_MINIMIZED_DIMENSIONS = SIDEBAR_SECTION_HEIGHT;

        /// <summary>
        /// The factor of a <see cref="SidebarSection"/>'s image scale.
        /// </summary>
        public const double SIDEBAR_SECTION_IMAGE_SCALE_FACTOR = 0.8;

        /// <summary>
        /// The default square dimensions of a <see cref="SidebarSection"/>'s image.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_IMAGE_DIMENSIONS = SIDEBAR_SECTION_HEIGHT * SIDEBAR_SECTION_IMAGE_SCALE_FACTOR;

        /// <summary>
        /// The default image margins of a <see cref="SidebarSection"/>'s image.
        /// </summary>
        public const double SIDEBAR_SECTION_IMAGE_MARGINS = (SIDEBAR_SECTION_HEIGHT * (1 - SIDEBAR_SECTION_IMAGE_SCALE_FACTOR)) / 2;

        /// <summary>
        /// The minimum width of the <see cref="Sidebar"/> before it collapses.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        private const double SIDEBAR_COLLAPSE_WIDTH = 50;

        public const double SIDEBAR_DIMENSION_CONSTANT = 32;

        public const double SIDEBAR_HANDLE_DIMENSION_CONSTANT = 5;

        #endregion

        #region Enumerations

        /// <summary>
        /// Defines the possible values of <see cref="Sidebar"/> dock positions.
        /// </summary>
        public enum SidebarDockPositions
        {
            /// <summary>
            /// Aligns the <see cref="Sidebar"/> left.
            /// </summary>
            Left = 0,

            /// <summary>
            /// Aligns the <see cref="Sidebar"/> right.
            /// </summary>
            Right = 2
        }

        #endregion

        #region Fields

        #region Fields: Collections

        /// <summary>
        /// Stores all <see cref="SidebarSection"/>s.
        /// </summary>
        private ObservableCollection<SidebarSection> _SidebarSections;

        /// <summary>
        /// Stores minimized <see cref="SidebarSection"/>s.
        /// </summary>
        private Collection<SidebarSection> _MinimizedSidebarSections;

        /// <summary>
        /// Stores maximized <see cref="SidebarSection"/>s.
        /// </summary>
        private Collection<SidebarSection> _MaximizedSidebarSections;

        /// <summary>
        /// Stores overflow menu items.
        /// </summary>
        private Collection<SidebarSection> _OverflowMenuItems;

        /// <summary>
        /// Stores option buttons.
        /// </summary>
        private Collection<ButtonBase> _OptionButtons;

        #endregion

        #region Fields: Partials

        /// <summary>
        /// Stores a reference to the collapsed content button template part in xaml.
        /// </summary>
        private ToggleButton _PARTSectionContentButton;

        /// <summary>
        /// Stores a reference to the collapsed content popup template part in xaml.
        /// </summary>
        private Popup _PARTSectionContentPopup;

        /// <summary>
        /// Stores a reference to the collapsed common area button template part in xaml.
        /// </summary>
        private ToggleButton _PARTCommonContentButton;

        /// <summary>
        /// Stores a reference to the collapsed common area popup template part in xaml.
        /// </summary>
        private Popup _PARTCommonContentPopup;

        /// <summary>
        /// Stores a reference to the overflow menu popup template part in xaml.
        /// </summary>
        private Popup _PARTOverflowMenu;

        /// <summary>
        /// Stores a reference to the sidebar common template part in xaml.
        /// </summary>
        private FrameworkElement _PARTCommonContent;

        /// <summary>
        /// Stores a reference to the template sidebar minimized sections part in xaml.
        /// </summary>
        private FrameworkElement _PARTMinimizedSections;

        /// <summary>
        /// Stores a reference to the sidebar header template part in xaml.
        /// </summary>
        private Button _PARTSidebarHeader;

        #endregion

        /// <summary>
        /// Stores the width to expand to when the <see cref="Sidebar"/> is collapsed.
        /// </summary>
        private double _LastWidth = double.PositiveInfinity;


        /// <summary>
        /// Stores the common section.
        /// </summary>
        private object _CommonSection;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="Sidebar"/>.
        /// </summary>
        static Sidebar()
        {
            // Overrides the default style of the inherited HeaderedItemsControl to use the Sidebar style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Sidebar), new FrameworkPropertyMetadata(typeof(Sidebar)));

            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initializes a new <see cref="Sidebar"/> control.
        /// </summary>
        public Sidebar() : base()
        {
            _OverflowMenuItems = new Collection<SidebarSection>();
            _OptionButtons = new Collection<ButtonBase>();
            _CommonSection = new object();

            SetValue(OverflowMenuItemsPropertyKey, _OverflowMenuItems);
            SetValue(OptionButtonsPropertyKey, _OptionButtons);
            SetValue(CommonContentProperty, _CommonSection);

            _SidebarSections = new ObservableCollection<SidebarSection>();

            _MinimizedSidebarSections = new Collection<SidebarSection>();
            _MaximizedSidebarSections = new Collection<SidebarSection>();

            // Bind the sidebar commands
            CommandBindings.Add(new CommandBinding(_CollapseCommand, OnSidebarCollapse));
            CommandBindings.Add(new CommandBinding(_ResizeSidebarCommand, OnSidebarResize));
            CommandBindings.Add(new CommandBinding(_ResizeSectionsCommand, OnSectionsResize));
            CommandBindings.Add(new CommandBinding(_ResizeCommonCommand, OnCommonResize));

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.SizeChanged += new SizeChangedEventHandler(SidebarSizeChanged);
            }
        }

        #endregion

        #region Events

        #region Events: Registration

        /// <summary>
        /// Registers the event to raise when the <see cref="Sidebar"/> is collapsed.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(nameof(Collapsed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when the <see cref="Sidebar"/> is expanded.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(nameof(Expanded), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when a <see cref="Sidebar"/> popup is closed.
        /// </summary>
        public static readonly RoutedEvent PopupClosedEvent = EventManager.RegisterRoutedEvent(nameof(PopupClosed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when a <see cref="Sidebar"/> popup is opened.
        /// </summary>
        public static readonly RoutedEvent PopupOpenedEvent = EventManager.RegisterRoutedEvent(nameof(PopupOpened), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when the <see cref="SelectedSection"/> property changes.
        /// </summary>
        public static readonly RoutedEvent SectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SectionChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<SidebarSection>), typeof(Sidebar));

        #endregion

        /// <summary>
        /// Defines the event to raise when the <see cref="Sidebar"/> is collapsed.
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        /// <summary>
        /// Defines the event to raise when the <see cref="Sidebar"/> is expanded.
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        /// <summary>
        /// Defines the event to raise when a popup is closed.
        /// </summary>
        public event RoutedEventHandler PopupClosed
        {
            add { AddHandler(PopupClosedEvent, value); }
            remove { RemoveHandler(PopupClosedEvent, value); }
        }

        /// <summary>
        /// Defines the event to raise when a popup is opened.
        /// </summary>
        public event RoutedEventHandler PopupOpened
        {
            add { AddHandler(PopupOpenedEvent, value); }
            remove { RemoveHandler(PopupOpenedEvent, value); }
        }

        /// <summary>
        /// Defines the event to raise when the <see cref="SelectedSection"/> changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SidebarSection> SectionChanged
        {
            add { AddHandler(SectionChangedEvent, value); }
            remove { RemoveHandler(SectionChangedEvent, value); }
        }

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Keys

        /// <summary>
        /// Registers the specialized read only property to get the content of collapsed <see cref="SidebarSection"/>s.
        /// </summary>
        private static readonly DependencyPropertyKey CollapsedContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CollapsedContent), typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the maximized <see cref="SidebarSection"/>s.
        /// </summary>
        private static readonly DependencyPropertyKey MaximizedSectionsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(MaximizedSections), typeof(Collection<SidebarSection>), typeof(Sidebar), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Registers the specialized read only property to get the minimized <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyPropertyKey MinimizedSectionsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(MinimizedSections), typeof(Collection<SidebarSection>), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the overflow menu items.
        /// </summary>
        private static readonly DependencyPropertyKey OverflowMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(OverflowMenuItems), typeof(Collection<SidebarSection>), typeof(Sidebar), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Registers the property to get the overflow menu items.
        /// </summary>
        public static readonly DependencyProperty OverflowMenuItemsProperty = OverflowMenuItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the specialized read only property to get the option buttons.
        /// </summary>
        private static readonly DependencyPropertyKey OptionButtonsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(OptionButtons), typeof(Collection<ButtonBase>), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the <see cref="SidebarSection"/>'s content.
        /// </summary>
        private static readonly DependencyPropertyKey SectionContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SectionContent), typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        #endregion

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to get the content of collapsed <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty CollapsedContentProperty = CollapsedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the collapsed width of the <see cref="Sidebar"/>.
        /// </summary>
        public static readonly DependencyProperty CollapsedWidthProperty = DependencyProperty.Register(nameof(CollapsedWidth), typeof(double), typeof(Sidebar), new UIPropertyMetadata(SIDEBAR_SECTION_MINIMIZED_DIMENSIONS));

        /// <summary>
        /// Registers the property to set the <see cref="Sidebar"/>'s common area content.
        /// </summary>
        private static readonly DependencyProperty CommonContentProperty = DependencyProperty.Register(nameof(CommonContent), typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to set the dock position of the <see cref="Sidebar"/>.
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty = DependencyProperty.Register(nameof(DockPosition), typeof(SidebarDockPositions), typeof(Sidebar), new UIPropertyMetadata(SidebarDockPositions.Left));

        /// <summary>
        /// Registers the property to get the expanded state of the <see cref="Sidebar"/>.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(true, IsExpandedPropertyChanged));

        /// <summary>
        /// Registers the property to get the overflow menu visibility..
        /// </summary>
        public static readonly DependencyProperty IsOverflowMenuVisibleProperty = DependencyProperty.Register(nameof(IsOverflowMenuVisible), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsOverflowMenuVisiblePropertyChanged));
       
        /// <summary>
        /// Registers the property to set the overflow menu button visibility.
        /// </summary>
        public static readonly DependencyProperty IsOverflowMenuButtonVisibleProperty = DependencyProperty.Register(nameof(IsOverflowMenuButtonVisible), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsOverflowMenuButtonVisiblePropertyChanged));

        /// <summary>
        /// Register the property to get the visibility of the collapsed content popup.
        /// </summary>
        public static readonly DependencyProperty IsSectionContentPopupVisibleProperty = DependencyProperty.Register(nameof(IsSectionContentPopupVisible), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsSectionContentPopupVisiblePropertyChanged));

        /// <summary>
        /// Register the property to get the visibility of the collapsed common content area popup.
        /// </summary>
        public static readonly DependencyProperty IsCommonContentPopupVisibleProperty = DependencyProperty.Register(nameof(IsCommonContentPopupVisible), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsCommonContentPopupVisiblePropertyChanged));

        /// <summary>
        /// Registers the property to get the maximized <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty MaximizedSectionsProperty = MaximizedSectionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the minimized <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty MinimizedSectionsProperty = MinimizedSectionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the option buttons.
        /// </summary>
        public static readonly DependencyProperty OptionButtonsProperty = OptionButtonsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the <see cref="SidebarSection"/>'s content.
        /// </summary>
        public static readonly DependencyProperty SectionContentProperty = SectionContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to set the selected <see cref="SidebarSection"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedSectionProperty = DependencyProperty.Register(nameof(SelectedSection), typeof(SidebarSection), typeof(Sidebar), new UIPropertyMetadata(null, SelectedSectionPropertyChanged));

        /// <summary>
        /// Registers the property to set the index of the selected <see cref="SidebarSection"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(Sidebar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedIndexPropertyChanged));

        /// <summary>
        /// Registers the property to show the common <see cref="SidebarSection"/>
        /// </summary>
        public static readonly DependencyProperty ShowCommonAreaProperty = DependencyProperty.Register(nameof(ShowCommonArea), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to hide <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty ShowSectionsProperty = DependencyProperty.Register(nameof(ShowSections), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the number of visible <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty VisibleSectionsProperty = DependencyProperty.Register(nameof(VisibleSections), typeof(int), typeof(Sidebar), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, VisibleSectionsPropertyChanged));

        #endregion

        #endregion

        #region Properties

        #region Properties: Collections

        /// <summary>
        /// Gets the collection of all <see cref="SidebarSection"/>s.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data"), Description("Collection of sidebar sections."), DefaultValue(null)]
        public Collection<SidebarSection> Sections
        {
            get { return _SidebarSections; }
        }
 
        /// <summary>
        /// Gets the collection of option buttons.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data"), Description("Collection of buttons to show when the sidebar is collapsed."), DefaultValue(null)]
        public Collection<ButtonBase> OptionButtons
        {
            get { return (Collection<ButtonBase>)GetValue(OptionButtonsProperty); }
        }

        #endregion

        #region Properties: Internal

        /// <summary>
        /// Gets or sets the content of collapsed <see cref="SidebarSection"/>s.
        /// </summary>
        internal object CollapsedContent
        {
            get { return GetValue(CollapsedContentProperty); }
            set { SetValue(CollapsedContentPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the width of the collapsed <see cref="Sidebar"/>.
        /// </summary>
        internal double CollapsedWidth
        {
            get { return (double)GetValue(CollapsedWidthProperty); }
            set { SetValue(CollapsedWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the collapsed common content area popup is visible.
        /// </summary>
        internal bool IsCommonContentPopupVisible
        {
            get { return (bool)GetValue(IsCommonContentPopupVisibleProperty); }
            set { SetValue(IsCommonContentPopupVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the collapsed content popup is visible.
        /// </summary>
        internal bool IsSectionContentPopupVisible
        {
            get { return (bool)GetValue(IsSectionContentPopupVisibleProperty); }
            set { SetValue(IsSectionContentPopupVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets wheter the overflow menu button is visible.
        /// </summary>
        internal bool IsOverflowMenuButtonVisible
        {
            get { return (bool)GetValue(IsOverflowMenuButtonVisibleProperty); }
            set { SetValue(IsOverflowMenuButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the overflow menu is visible.
        /// </summary>
        internal bool IsOverflowMenuVisible
        {
            get { return (bool)GetValue(IsOverflowMenuVisibleProperty); }
            set { SetValue(IsOverflowMenuVisibleProperty, value); }
        }

        /// <summary>
        /// Gets the collection of minimized <see cref="SidebarSection"/>s.
        /// </summary>
        internal Collection<SidebarSection> MinimizedSections
        {
            get { return _MinimizedSidebarSections; }
        }

        /// <summary>
        /// Gets the collection of maximized <see cref="SidebarSection"/>s.
        /// </summary>
        internal Collection<SidebarSection> MaximizedSections
        {
            get { return _MaximizedSidebarSections; }
        }

        /// <summary>
        /// Gets the collection of overflow menu items.
        /// </summary>
        internal Collection<SidebarSection> OverflowMenuItems
        {
            get { return _OverflowMenuItems; }
        }

        /// <summary>
        /// Gets or sets the content of the selected <see cref="SidebarSection"/>.
        /// </summary>
        internal object SectionContent
        {
            get { return GetValue(SectionContentProperty); }
            set { SetValue(SectionContentPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the selected <see cref="SidebarSection"/>.
        /// </summary>
        internal SidebarSection SelectedSection
        {
            get { return (SidebarSection)GetValue(SelectedSectionProperty); }
            set { SetValue(SelectedSectionProperty, value); }
        }

        #endregion

        #region Properties: Public

        /// <summary>
        /// Gets or sets the <see cref="Sidebar"/>'s common section.
        /// </summary>
        [Category("Data"), Description("Data for the sidebar common area content."), DefaultValue(null)]
        public object CommonContent
        {
            get { return GetValue(CommonContentProperty); }
            set { SetValue(CommonContentProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the dock position of the sidebar.
        /// </summary>
        /// <remarks><i>Only left and right docking is supported.</i></remarks>
        [Category("Layout"), Description("Indicates the dock position of the sidebar on screen."), DefaultValue(SidebarDockPositions.Left)]
        public SidebarDockPositions DockPosition
        {
            get { return (SidebarDockPositions)GetValue(DockPositionProperty); }
            set { SetValue(DockPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected <see cref="SidebarSection"/>.
        /// </summary>
        [Category("Behavior"), Description("Indicates the index of the selected sections of the sidebar."), DefaultValue(int.MaxValue)]
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

         /// <summary>
        /// Gets or sets whether the <see cref="Sidebar"/> is expanded.
        /// </summary>
        [Category("Appearance"), Description("Indicates if the sidebar is shown expanded or collapsed."), DefaultValue(true)]
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to show the <see cref="SidebarSection"/>s.
        /// </summary>
        [Category("Appearance"), Description("Indicates if the sidebar sections are shown or hidden."), DefaultValue(true)]
        public bool ShowSections
        {
            get { return (bool)GetValue(ShowSectionsProperty); }
            set { SetValue(ShowSectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to show the common <see cref="SidebarSection"/>.
        /// </summary>
        [Category("Appearance"), Description("Indicates if the common area above the sidebar sections is shown."), DefaultValue(true)]
        public bool ShowCommonArea
        {
            get { return (bool)GetValue(ShowCommonAreaProperty); }
            set { SetValue(ShowCommonAreaProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of visible <see cref="SidebarSection"/>s.
        /// </summary>
        [Category("Appearance"), Description("Indicates the number of visible sections of the sidebar on screen."), DefaultValue(int.MaxValue)]
        public int VisibleSections
        {
            get { return (int)GetValue(VisibleSectionsProperty); }
            set { SetValue(VisibleSectionsProperty, value); }
        }

        #endregion

        #endregion

        #region Commands

        #region Commands: Registration

        /// <summary>
        /// Defines the command to collapse the sidebar.
        /// </summary>
        private static RoutedUICommand _CollapseCommand = new RoutedUICommand(nameof(CollapseCommand), nameof(CollapseCommand), typeof(Sidebar));

        /// <summary>
        /// Defines the command to resize the sidebar.
        /// </summary>
        private static RoutedUICommand _ResizeSidebarCommand = new RoutedUICommand(nameof(ResizeSidebarCommand), nameof(ResizeSidebarCommand), typeof(Sidebar));

        /// <summary>
        /// Defines the command to resize the sidebar sections.
        /// </summary>
        private static RoutedUICommand _ResizeSectionsCommand = new RoutedUICommand(nameof(ResizeSectionsCommand), nameof(ResizeSectionsCommand), typeof(Sidebar));

        /// <summary>
        /// Defines the command to resize the sidebar common area.
        /// </summary>
        private static RoutedUICommand _ResizeCommonCommand = new RoutedUICommand(nameof(ResizeCommonCommand), nameof(ResizeCommonCommand), typeof(Sidebar));

        #endregion

        #region Commands: Properties

        /// <summary>
        /// Gets the command to collapse the <see cref="Sidebar"/>.
        /// </summary>
        public static RoutedUICommand CollapseCommand 
        { 
            get { return _CollapseCommand; } 
        }

        /// <summary>
        /// Gets the command to resize the <see cref="Sidebar"/>.
        /// </summary>
        public static RoutedUICommand ResizeSidebarCommand 
        { 
            get { return _ResizeSidebarCommand; } 
        }

        /// <summary>
        /// Gets the command to resize the <see cref="Sidebar"/> common area.
        /// </summary>
        public static RoutedUICommand ResizeCommonCommand 
        { 
            get { return _ResizeCommonCommand; } 
        }

        /// <summary>
        /// Gets the command to resize the <see cref="Sidebar"/> sections area.
        /// </summary>
        public static RoutedUICommand ResizeSectionsCommand 
        { 
            get { return _ResizeSectionsCommand; } 
        }

        #endregion

        #endregion

        #region Command Event Handlers

        /// <summary>
        /// Handles the <see cref="CollapseCommand"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSidebarCollapse(object sender, ExecutedRoutedEventArgs e)
        {
            // Toggles the expanded state by an XOR boolean assignment
            this.IsExpanded ^= true;
        }

        /// <summary>
        /// Handles the <see cref="ResizeSidebarCommand"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnSidebarResize(object sender, ExecutedRoutedEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if (control != null)
            {
                // Attach the mouse up button event handler to the control that started the resize be notified when resizing has been done
                control.PreviewMouseUp += new MouseButtonEventHandler(RemoveResizeEventListeners);
            }

            // Attach the mouse move event handler to the sidebar
            this.PreviewMouseMove += new MouseEventHandler(ResizeSidebar);
        }

        /// <summary>
        /// Handles the <see cref="ResizeSectionsCommand"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnSectionsResize(object sender, ExecutedRoutedEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if (control != null)
            {
                // Attach the mouse up button event handler to the control that started the resize be notified when resizing has been done
                control.PreviewMouseUp += new MouseButtonEventHandler(RemoveResizeEventListeners);
            }

            // Attach the mouse move event handler to the sidebar
            this.PreviewMouseMove += new MouseEventHandler(ResizeSections);
        }

        /// <summary>
        /// Handles the <see cref="ResizeCommonCommand"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnCommonResize(object sender, ExecutedRoutedEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if(control != null)
            {
                control.PreviewMouseUp += new MouseButtonEventHandler(RemoveResizeEventListeners);
            }

            this.PreviewMouseMove += new MouseEventHandler(ResizeCommon);
        }

        /// <summary>
        /// Removes event listeners that have been possibly set at the beginning of a resize command.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseButtonEventArgs"/> containing event data.</param>
        private void RemoveResizeEventListeners(object sender, MouseButtonEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if(control != null)
            {
                // Remove the mouse up button event handler from the control that started the resize
                control.PreviewMouseUp -= RemoveResizeEventListeners;
            }

            // Remove the mouse move event handlers from the sidebar
            this.PreviewMouseMove -= ResizeSidebar;
            this.PreviewMouseMove -= ResizeSections;
            this.PreviewMouseMove -= ResizeCommon;
        }

        /// <summary>
        /// Handles the resize sidebar command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeSidebar(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(this);

                double width = 0;

                // Determine the resize direction
                if (this.DockPosition == SidebarDockPositions.Left)
                {
                    // Resize from the right
                    width = mousePosition.X;
                }
                else
                {
                    // Resize from the left
                    width = this.ActualWidth - mousePosition.X;
                }

                // Determine if the sidebar collapses
                if (width < SIDEBAR_COLLAPSE_WIDTH)
                {
                    width = double.NaN;
                    this.IsExpanded = false;
                }
                else
                {
                    this.IsExpanded = true;
                }

                // Determine if the width is greater than the maximum width
                if (this.MaxWidth != double.NaN && width > this.MaxWidth)
                {
                    width = this.MaxWidth;
                }

                // Set the width
                this.Width = width;
            }
            else
            {
                // Remove the mouse move event handler from the sidebar
                this.PreviewMouseMove -= ResizeSidebar;
            }
        }

        /// <summary>
        /// Handles the resize section command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeSections(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Resizing Sections");
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(this);

                // Get dynamic applied border thickness style value
                Thickness border = new Thickness(1);

                if(!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    border = (Thickness)StylesXL.StyleManager.GetStyleValue(StylesXL.Layout.BorderThickness);

                double height = this.ActualHeight - SIDEBAR_DIMENSION_CONSTANT - mousePosition.Y;

                // TODO:
                if (mousePosition.Y > (_PARTCommonContent.ActualHeight + SIDEBAR_DIMENSION_CONSTANT))
                {
                    this.VisibleSections = (int)(height / SIDEBAR_DIMENSION_CONSTANT);
                }
                else
                {
                    
                }
            }
            else
            {
                // Remove the mouse move event handler from the sidebar
                this.PreviewMouseMove -= ResizeSections;
            }
        }

        /// <summary>
        /// Handles the resize common command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeCommon(object sender, MouseEventArgs e)
        {

            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(this);

                double height = mousePosition.Y;

                // Prevent scaling over the sidebar sections area
                if (height > this.ActualHeight - GetMinimumSidebarSectionsAreaHeight())
                    height = this.ActualHeight - GetMinimumSidebarSectionsAreaHeight();

                // Prevent scaling over the common area header
                if (height < SIDEBAR_SECTION_HEIGHT + 5)
                    height = SIDEBAR_SECTION_HEIGHT + 5;

                _PARTCommonContent.Height = height;
            }
            else
            {
                this.PreviewMouseMove -= ResizeCommon;
            }
        }

        /// <summary>
        /// Handels the <see cref="Sidebar.SizeChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="SizeChangedEventArgs"/> containing event data.</param>
        private void SidebarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitializeSidebarSections();
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Handles changes made to the <see cref="IsOverflowMenuVisibleProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsOverflowMenuVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)d;

            if ((bool)e.NewValue)
                sideBar.InitializeOverflowMenu();
        }

        /// <summary>
        /// Handles changes made to the <see cref="IsOverflowMenuButtonVisibleProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsOverflowMenuButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)d;

            if ((bool)e.NewValue)
                sideBar.InitializeOverflowMenu();
        }

        /// <summary>
        /// Handles changes made to the <see cref="IsSectionContentPopupVisibleProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsSectionContentPopupVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)d;

            if(sideBar._PARTSectionContentPopup != null)
            {
                sideBar._PARTSectionContentPopup.StaysOpen = true;
                sideBar._PARTSectionContentPopup.IsOpen = (bool)e.NewValue;
            }

            if((bool)e.NewValue)
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupOpenedEvent));
            }
            else
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupClosedEvent));
            }
        }

        /// <summary>
        /// Handles changes made to the <see cref="IsCommonContentPopupVisibleProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsCommonContentPopupVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)d;

            if (sideBar._PARTCommonContentPopup != null)
            {
                sideBar._PARTCommonContentPopup.StaysOpen = true;
                sideBar._PARTCommonContentPopup.IsOpen = (bool)e.NewValue;
            }

            if ((bool)e.NewValue)
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupOpenedEvent));
            }
            else
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupClosedEvent));
            }
        }

        /// <summary>
        /// Handles changes made to the <see cref="IsExpandedProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)d;

            bool isExpanded = (bool)e.NewValue;

            sideBar.InvalidateContentVisibility();
            sideBar.InitializeOverflowMenu();

            if (isExpanded)
            {
                sideBar.IsSectionContentPopupVisible = false;
                sideBar.IsCommonContentPopupVisible = false;

                sideBar.MaxWidth = sideBar._LastWidth;
                sideBar.RaiseEvent(new RoutedEventArgs(CollapsedEvent));
            }
            else
            {
                sideBar._LastWidth = sideBar.MaxWidth;
                sideBar.MaxWidth = sideBar.CollapsedWidth;
                sideBar.RaiseEvent(new RoutedEventArgs(ExpandedEvent));
                
            }
        }

        /// <summary>
        /// Handles changes made to the <see cref="SelectedSectionProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void SelectedSectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)d;

            for (int i = 0; i < sideBar._SidebarSections.Count; i++)
            {
                SidebarSection section = sideBar._SidebarSections[i];

                section.IsSelected = (section == (SidebarSection)e.NewValue);

                if(section.IsSelected)
                {
                    // Set the index of the section to the sidebar
                    sideBar.SelectedIndex = i;

                    // Add the content of the selected section to the sidebar
                    sideBar.SectionContent = sideBar.IsExpanded ? section.Content : null;

                    // Add the content of the selected collapsed section to the sidebar
                    sideBar.CollapsedContent = sideBar.IsExpanded ? null : section.Content;
                    
                }
            }
            
            sideBar.RaiseEvent(new RoutedPropertyChangedEventArgs<SidebarSection>((SidebarSection)e.OldValue, (SidebarSection)e.NewValue, SectionChangedEvent));
        }

        /// <summary>
        /// Handles changes made to the <see cref="VisibleSectionsProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void VisibleSectionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Sidebar)d).InitializeSidebarSections();
        }

        /// <summary>
        /// Handles Changes made to the <see cref="SelectedIndexProperty"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void SelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Sidebar)d).InitializeSidebarSections();
        }
       
        #endregion

        #region Overrides

        /// <summary>
        /// Sets the XAML references to the template parts and binds the appropriate event handlers when the <see cref="OnApplyTemplate"/> method is called.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Clear all event listeners
            if(_PARTSectionContentPopup != null)
            {
                _PARTSectionContentPopup.Opened -= SidebarPopupOpened;
                _PARTSectionContentPopup.Closed -= SidebarPopupClosed;
            }

            if(_PARTCommonContentPopup != null)
            {
                _PARTCommonContentPopup.Opened -= SidebarPopupOpened;
                _PARTCommonContentPopup.Closed -= SidebarPopupClosed;
            }

            // Get the references to the template parts from xaml
            _PARTCommonContentButton      = GetTemplateChild(PART_SIDEBAR_COMMON_CONTENT_BUTTON) as ToggleButton;
            _PARTCommonContentPopup       = GetTemplateChild(PART_SIDEBAR_COMMON_CONTENT_POPUP) as Popup;
            _PARTSectionContentButton     = GetTemplateChild(PART_SIDEBAR_SECTION_CONTENT_BUTTON) as ToggleButton;
            _PARTSectionContentPopup      = GetTemplateChild(PART_SIDEBAR_SECTION_CONTENT_POPUP) as Popup;
            _PARTOverflowMenu             = GetTemplateChild(PART_SIDEBAR_OVERFLOW_MENU) as Popup;
            _PARTCommonContent        = GetTemplateChild(PART_SIDEBAR_COMMON) as FrameworkElement;
            _PARTSidebarHeader            = GetTemplateChild(PART_SIDEBAR_HEADER) as Button;
            _PARTMinimizedSections = GetTemplateChild(PART_SIDEBAR_MINIMIZED_SECTIONS) as FrameworkElement;

            // Bind the popup event listeners
            if (_PARTCommonContentButton != null)
            {
                _PARTCommonContentButton.PreviewMouseLeftButtonUp += SidebarPopupButtonPreviewMouseUp;
            }

            if (_PARTCommonContentPopup != null)
            {
                _PARTCommonContentPopup.Opened += SidebarPopupOpened;
                _PARTCommonContentPopup.Closed += SidebarPopupClosed;
            }

            if (_PARTSectionContentButton != null)
            {
                _PARTSectionContentButton.PreviewMouseLeftButtonUp += SidebarPopupButtonPreviewMouseUp;
            }

            if (_PARTSectionContentPopup != null)
            {
                _PARTSectionContentPopup.Opened += SidebarPopupOpened;
                _PARTSectionContentPopup.Closed += SidebarPopupClosed;
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Handles the <see cref="Initialized"/> event, called when the sidebar is initialized.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            InitializeSidebarSections();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes the sidebar sections.
        /// </summary>
        private void InitializeSidebarSections()
        {
            // Reset the sidebar section collections
            _MinimizedSidebarSections = new Collection<SidebarSection>();
            _MaximizedSidebarSections = new Collection<SidebarSection>();

            // Initialize the number of visible sections
            int maximizedSections = this.VisibleSections;
            int minimizedSections = GetMinimizedSectionsDisplayCount();

            // If the sum of minimized and maximized sections is smaller than the total number of sections, the overflow menu button is visible
            IsOverflowMenuButtonVisible = (maximizedSections + minimizedSections) < this.Sections.Count;

            // Loop through all sidebar sections
            for (int i = 0; i < _SidebarSections.Count; i++)
            {
                SidebarSection section = _SidebarSections[i];

                // Set common section properties
                section.Sidebar = this;
                section.Height = SIDEBAR_SECTION_HEIGHT;

                // Reset the status flags
                section.StatusFlags = SectionStatusFlags.None;
                section.StatusFlags |= this.IsExpanded == true ? SectionStatusFlags.Expanded : SectionStatusFlags.Collapsed;

                if(maximizedSections > 0)
                {
                    // Maximized section logic
                    section.StatusFlags |= SectionStatusFlags.Maximized;
                    section.Height = SIDEBAR_SECTION_HEIGHT;

                    // Add the section to the maximized section collection
                    _MaximizedSidebarSections.Add(section);

                    maximizedSections--;
                }
                else
                {
                    // Minimized section logic
                    section.StatusFlags |= SectionStatusFlags.Minimized;
                    section.Height = SIDEBAR_SECTION_IMAGE_DIMENSIONS;
                    section.ToolTip = section.Header;

                    if (this._MinimizedSidebarSections.Count < minimizedSections)
                    {
                        // Add the section to the minimized section collection
                        _MinimizedSidebarSections.Add(section);
                    }
                }

                // Sets the selected property
                section.IsSelected = i == this.SelectedIndex;

                if (section.IsSelected)
                    this.SelectedSection = section;
            }

            // Update the sidebar section collections
            SetValue(MaximizedSectionsPropertyKey, _MaximizedSidebarSections);
            SetValue(MinimizedSectionsPropertyKey, _MinimizedSidebarSections);
        }

        /// <summary>
        /// Initializes the overflow menu.
        /// </summary>
        private void InitializeOverflowMenu()
        {
            Collection<SidebarSection> overflowMenuItems = new Collection<SidebarSection>();

            if(_OverflowMenuItems.Count > 0)
            {
                foreach(SidebarSection item in _OverflowMenuItems)
                {
                    overflowMenuItems.Add(item);
                }
            }

            int visibleItems = _MaximizedSidebarSections.Count + (this.IsExpanded ? _MinimizedSidebarSections.Count : 0);

            for (int i = visibleItems; i < _SidebarSections.Count; i++)
            {
                SidebarSection section = _SidebarSections[i];

                section.Clicked += new RoutedEventHandler(MenuItemClickedEventHandler);

                section.StatusFlags &= ~SectionStatusFlags.Minimized;
                section.StatusFlags |= SectionStatusFlags.Maximized;
                section.StatusFlags &= ~SectionStatusFlags.Collapsed;
                section.StatusFlags |= SectionStatusFlags.Expanded;
                section.StatusFlags |= SectionStatusFlags.Overflow;
                section.ToolTip = null;

                overflowMenuItems.Add(section);
            }

            SetValue(OverflowMenuItemsPropertyKey, overflowMenuItems);
        }

        /// <summary>
        /// Calculates the maximum number of minimized <see cref="SidebarSection"/>s that can be displayed.
        /// </summary>
        /// <returns>An <see cref="int"/> containing the number of minimized <see cref="SidebarSection"/>s that can be displayed.</returns>
        private int GetMinimizedSectionsDisplayCount()
        {
            if (_PARTMinimizedSections == null)
                return 0;

            // Get dynamic applied border thickness style value
            Thickness border = new Thickness(1);

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                border = (Thickness)StylesXL.StyleManager.GetStyleValue(StylesXL.Layout.BorderThickness);


            return (int)Math.Floor((_PARTMinimizedSections.ActualWidth - SIDEBAR_SECTION_IMAGE_DIMENSIONS) 
                                  / (SIDEBAR_SECTION_IMAGE_DIMENSIONS + border.Right));
        }

        /// <summary>
        /// Calculates the minimum height of the sidebar sections area to show all visible <see cref="SidebarSection"/>s.
        /// </summary>
        /// <returns>An <see cref="double"/> containing the height to maintain visibility of all maximized <see cref="SidebarSection"/>s.</returns>
        private double GetMinimumSidebarSectionsAreaHeight()
        {
            // Get dynamic applied border thickness style value
            Thickness border = new Thickness(1);

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                border = (Thickness)StylesXL.StyleManager.GetStyleValue(StylesXL.Layout.BorderThickness);


            return (_MaximizedSidebarSections.Count * SIDEBAR_DIMENSION_CONSTANT + border.Top)
                   + SIDEBAR_SECTION_IMAGE_DIMENSIONS + border.Top 
                   + SIDEBAR_DIMENSION_CONSTANT + border.Top 
                   + SIDEBAR_HANDLE_DIMENSION_CONSTANT;
        }

        /// <summary>
        /// Invalidates the content to be visible after the sidebar expandend state is changed.
        /// </summary>
        /// <remarks><i>Ensures visibility of content when a <see cref="SidebarSection"/> is selected while the <see cref="Sidebar"/> is collapsed.</i></remarks>
        private void InvalidateContentVisibility()
        {
            // Store the selected section content temporary
            object tempContent = this.SelectedSection != null ? this.SelectedSection.Content : null;

            // Reset the section content
            this.SectionContent = null;

            // Reassign the section content
            this.CollapsedContent = this.IsExpanded ? null : tempContent;
            this.SectionContent = this.IsExpanded ? tempContent : null;

            // Store the selected section content temporary
            object tempCommonContent = this.CommonContent;

            // Reset the section content
            this.CommonContent = null;

            // Reassign the section content
            this.CommonContent = tempCommonContent;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="SidebarSection.Clicked"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        private void MenuItemClickedEventHandler(object sender, RoutedEventArgs e)
        {
            _PARTOverflowMenu.IsOpen = false;
        }

        /// <summary>
        /// Handles the <see cref="Popup.Closed"/> event for the specified popup.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void SidebarPopupClosed(object sender, EventArgs e)
        {
            Popup popup = (Popup)sender;

            if(popup == _PARTCommonContentPopup)
                IsCommonContentPopupVisible = false;
            else
                IsSectionContentPopupVisible = false;

            Mouse.Capture(null);
        }

        /// <summary>
        /// Handles the <see cref="Popup.Opened"/> event for the specified popup.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void SidebarPopupOpened(object sender, EventArgs e)
        {
            Popup popup = (Popup)sender;

            if (popup == _PARTCommonContentPopup)
                IsCommonContentPopupVisible = true;
            else
                IsSectionContentPopupVisible = true;
            
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Handles the preview mouse up event for the <see cref="Sidebar"/> popup buttons.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseButtonEventArgs"/> containing event data.</param>
        private void SidebarPopupButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // IMPORTANT! The mouse capture has to be released to close the popup when the focus is lost, otherwise the popup has to be closed by its corresponding button
            Mouse.Capture(null);

            _PARTCommonContentPopup.StaysOpen = false;
            _PARTSectionContentPopup.StaysOpen = false;
        }

        #endregion
    }

    //
    /// <summary>
    /// Defines all possible <see cref="SidebarSection.StatusFlags"/> values.
    /// </summary>
    [Flags]
    [TypeConverter(typeof(EnumConverter))]
    public enum SectionStatusFlags
    {
        None      = 0,
        Maximized = 1,
        Minimized = 2,
        Overflow  = 4,
        Expanded  = 8,
        Collapsed = 16
    }

    /// <summary>
    /// 
    /// </summary>
    public class SidebarSection : HeaderedContentControl
    {

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="SidebarSection"/>.
        /// </summary>
        static SidebarSection()
        {
            // Overrides the default style of the inherited HeaderedContentControl to use the SidebarSection style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SidebarSection), new FrameworkPropertyMetadata(typeof(SidebarSection)));
            StylesXL.StyleManager.Initialize();
        }

        /// <summary>
        /// Creates and initializes a new <see cref="SidebarSection"/> control.
        /// </summary>
        public SidebarSection() : base()
        {
            // Add a mouse click event listener to the sidebar section
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SidebarSectionClicked));
        }
        
        #endregion

        #region Routed Events

        /// <summary>
        /// Registers the event to raise when the <see cref="SidebarSection"/> is clicked.
        /// </summary>
        public static readonly RoutedEvent SidebarSectionClickRoutedEvent = EventManager.RegisterRoutedEvent(nameof(SidebarSectionClickRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SidebarSection));

        /// <summary>
        /// Defines the event raised when the <see cref="SidebarSection"/> is clicked.
        /// </summary>
        public event RoutedEventHandler Clicked
        {
            add { AddHandler(SidebarSectionClickRoutedEvent, value); }
            remove { RemoveHandler(SidebarSectionClickRoutedEvent, value); }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Registers the property to check if the <see cref="SidebarSection"/> is selected.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SidebarSection), new UIPropertyMetadata(false, IsSelectedChangedCallback));
        
        /// <summary>
        /// Registers the property to set the <see cref="Image"/> of the <see cref="SidebarSection"/>.
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(SidebarSection), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to set the <see cref="Geometry"/> data for the <see cref="SidebarSection"/> button image.
        /// </summary>
        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register(nameof(Vector), typeof(Geometry), typeof(SidebarSection), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to set the <see cref="StatusFlags"/> of the <see cref="SidebarSection"/>.
        /// </summary>
        public static readonly DependencyProperty StatusFlagsProperty = DependencyProperty.Register(nameof(StatusFlags), typeof(SectionStatusFlags), typeof(SidebarSection), new PropertyMetadata(SectionStatusFlags.None));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a reference to the parent of the <see cref="SidebarSection"/>.
        /// </summary>
        public Sidebar Sidebar { get; internal set; }

        /// <summary>
        /// Gets or sets the <see cref="SidebarSection"/>'s status flags.
        /// </summary>
        public SectionStatusFlags StatusFlags
        {
            get { return (SectionStatusFlags)GetValue(StatusFlagsProperty); }
            internal set { SetValue(StatusFlagsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="SidebarSection"/> is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="ImageSource"/> of the <see cref="SidebarSection"/>'s image.
        /// </summary>
        /// <remarks><i>The <see cref="Image"/> property has priorty over the <see cref="Vector"/> property.</i></remarks>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vector <see cref="Geometry"/> of the <see cref="SidebarSection"/>'s image.
        /// </summary>
        /// <remarks><i>The <see cref="Image"/> property has priorty over the <see cref="Vector"/> property.</i></remarks>
        public Geometry Vector
        {
            get { return (Geometry)GetValue(VectorProperty); }
            set { SetValue(VectorProperty, value); }
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Handles the property changed event of the <see cref="IsSelectedProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>

        private static void IsSelectedChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SidebarSection section = (SidebarSection)sender;

            if((bool)e.NewValue)
            {
                section.Sidebar.SelectedSection = section;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="Clicked"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        private void SidebarSectionClicked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = e.OriginalSource as ToggleButton;

            if (button != null)
                button.IsChecked = true;

            if (Sidebar != null)
                Sidebar.SelectedSection = this;

            RaiseEvent(new RoutedEventArgs(SidebarSectionClickRoutedEvent));
        }

        #endregion
    }

}
