using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ControlsXL
{

    /// <summary>
    /// The Sidebar is a container for SidebarSections
    /// 
    /// </summary>
    [TemplatePart(Name = PART_SIDEBAR_MINIMIZED_SECTIONS)]
    [TemplatePart(Name = PART_SIDEBAR_OVERFLOW_MENU)]
    [TemplatePart(Name = PART_SIDEBAR_COMMON)]
    [TemplatePart(Name = PART_SIDEBAR_HEADER)]
    [TemplatePart(Name = PART_SIDEBAR_COLLAPSED_CONTENT_POPUP)]
    [TemplatePart(Name = PART_SIDEBAR_COLLAPSED_CONTENT_BUTTON)]
    [TemplatePart(Name = PART_SIDEBAR_COLLAPSED_COMMON_POPUP)]
    [TemplatePart(Name = PART_SIDEBAR_COLLAPSED_COMMON_BUTTON)]
    public class Sidebar : HeaderedItemsControl
    {
        #region Constants

        #region Constants: Template Parts

        /// <summary>
        /// The template part name of the minimized sections in xaml.
        /// </summary>
        private const string PART_SIDEBAR_MINIMIZED_SECTIONS = "PART_SidebarMinimizedSections";

        /// <summary>
        /// The template part name of the overflow menu in xaml.
        /// </summary>
        private const string PART_SIDEBAR_OVERFLOW_MENU = "PART_SidebarOverflowMenu";

        /// <summary>
        /// The template part name of the side bar common area in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COMMON = "PART_SidebarCommon";

        /// <summary>
        /// The template part name of the collapsed side bar content in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COLLAPSED_CONTENT_POPUP = "PART_SidebarCollapsedContentPopup";

        /// <summary>
        /// The template part name of the collapsed side bar content button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COLLAPSED_CONTENT_BUTTON = "PART_SidebarCollapsedContentButton";

        /// <summary>
        /// The template part name of the collapsed side bar common area content in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COLLAPSED_COMMON_POPUP = "PART_SidebarCollapsedCommonPopup";

        /// <summary>
        /// The template part name of the collapsed side bar common area button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COLLAPSED_COMMON_BUTTON = "PART_SidebarCollapsedCommonButton";

        /// <summary>
        /// The template part name of the side bar header button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_HEADER = "PART_SidebarHeader";

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
        private ToggleButton _PARTCollapsedContentButton;

        /// <summary>
        /// Stores a reference to the collapsed content popup template part in xaml.
        /// </summary>
        private Popup _PARTCollapsedContentPopup;

        /// <summary>
        /// Stores a reference to the collapsed common area button template part in xaml.
        /// </summary>
        private ToggleButton _PARTCollapsedCommonButton;

        /// <summary>
        /// Stores a reference to the collapsed common area popup template part in xaml.
        /// </summary>
        private Popup _PARTCollapsedCommonPopup;

        /// <summary>
        /// Stores a reference to the overflow menu popup template part in xaml.
        /// </summary>
        private Popup _PARTOverflowMenu;

        /// <summary>
        /// Stores a reference to the side bar common template part in xaml.
        /// </summary>
        private FrameworkElement _PARTSidebarCommon;

        /// <summary>
        /// Stores a reference to the template side bar minimized sections part in xaml.
        /// </summary>
        private FrameworkElement _PARTSidebarMinimizedSections;

        /// <summary>
        /// Stores a reference to the side bar header template part in xaml.
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
            SetValue(CommonSectionProperty, _CommonSection);

            _SidebarSections = new ObservableCollection<SidebarSection>();

            _MinimizedSidebarSections = new Collection<SidebarSection>();
            _MaximizedSidebarSections = new Collection<SidebarSection>();

            // Bind the side bar commands
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

        /// <summary>
        /// Registers the event to raise when the <see cref="SelectedSection"/> changes.
        /// </summary>
        public static readonly RoutedEvent SelectedSectionChangedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(SelectedSectionChangedRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<SidebarSection>), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when the <see cref="Sidebar"/> is collapsed.
        /// </summary>
        public static readonly RoutedEvent CollapsedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(CollapsedRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when the <see cref="Sidebar"/> is expanded.
        /// </summary>
        public static readonly RoutedEvent ExpandedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(ExpandedRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Defines the event to raise when the <see cref="SelectedSection"/> changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SidebarSection> SelectedSectionChanged
        {
            add { AddHandler(SelectedSectionChangedRoutedEvent, value); }
            remove { RemoveHandler(SelectedSectionChangedRoutedEvent, value); }
        }

        /// <summary>
        /// Registers the event to raise when the <see cref="_PARTCollapsedContentPopup"/> is opened.
        /// </summary>
        public static readonly RoutedEvent PopupOpenedRoutedEvent = EventManager.RegisterRoutedEvent("PopupOpenedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Registers the event to raise when the <see cref="_PARTCollapsedContentPopup"/> is closed.
        /// </summary>
        public static readonly RoutedEvent PopupClosedRoutedEvent = EventManager.RegisterRoutedEvent("PopupClosedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

        /// <summary>
        /// Defines the event to raise when the <see cref="_PARTCollapsedContentPopup"/> is opened.
        /// </summary>
        public event RoutedEventHandler PopupOpened
        {
            add { AddHandler(PopupOpenedRoutedEvent, value); }
            remove { RemoveHandler(PopupOpenedRoutedEvent, value); }
        }

        /// <summary>
        /// Defines the event to raise when the <see cref="_PARTCollapsedContentPopup"/> is closed.
        /// </summary>
        public event RoutedEventHandler PopupClosed
        {
            add { AddHandler(PopupClosedRoutedEvent, value); }
            remove { RemoveHandler(PopupClosedRoutedEvent, value); }
        }

       

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Collections

        /// <summary>
        /// Registers the specialized read only property to get the maximized <see cref="SidebarSection"/>s.
        /// </summary>
        private static readonly DependencyPropertyKey MaximizedSectionsPropertyKey = DependencyProperty.RegisterReadOnly("MaximizedSections", typeof(Collection<SidebarSection>), typeof(Sidebar), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Registers the property to get the maximized <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty MaximizedSectionsProperty = MaximizedSectionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the specialized read only property to get the minimized <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyPropertyKey MinimizedSectionsPropertyKey = DependencyProperty.RegisterReadOnly("MinimizedSections", typeof(Collection<SidebarSection>), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to get the minimized <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty MinimizedSectionsProperty = MinimizedSectionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the specialized read only property to get the overflow menu items.
        /// </summary>
        private static readonly DependencyPropertyKey OverflowMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("OverflowMenuItems", typeof(Collection<SidebarSection>), typeof(Sidebar), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Registers the property to get the overflow menu items.
        /// </summary>
        public static readonly DependencyProperty OverflowMenuItemsProperty = OverflowMenuItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the specialized read only property to get the option buttons.
        /// </summary>
        private static readonly DependencyPropertyKey OptionButtonsPropertyKey = DependencyProperty.RegisterReadOnly("OptionButtons", typeof(Collection<ButtonBase>), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to get the option buttons.
        /// </summary>
        public static readonly DependencyProperty OptionButtonsProperty = OptionButtonsPropertyKey.DependencyProperty;

        #endregion

        /// <summary>
        /// Registers the property to set the dock position of the <see cref="Sidebar"/>.
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty = DependencyProperty.Register(nameof(DockPosition), typeof(SidebarDockPositions), typeof(Sidebar), new UIPropertyMetadata(SidebarDockPositions.Left));

        /// <summary>
        /// Registers the property to get the collapsed width of the <see cref="Sidebar"/>.
        /// </summary>
        public static readonly DependencyProperty CollapsedWidthProperty = DependencyProperty.Register(nameof(CollapsedWidth), typeof(double), typeof(Sidebar), new UIPropertyMetadata(SIDEBAR_SECTION_MINIMIZED_DIMENSIONS));

        /// <summary>
        /// Registers the specialized read only property to get the content of collapsed <see cref="SidebarSection"/>s.
        /// </summary>
        private static readonly DependencyPropertyKey CollapsedContentPropertyKey = DependencyProperty.RegisterReadOnly("CollapsedContent", typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the content of the collapsed common area.
        /// </summary>
        private static readonly DependencyPropertyKey CollapsedCommonContentPropertyKey = DependencyProperty.RegisterReadOnly("CollapsedCommonContent", typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to get the expanded state of the <see cref="Sidebar"/>.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(true, IsExpandedChangedCallback));

        /// <summary>
        /// Registers the property to get the overflow menu visibility..
        /// </summary>
        public static readonly DependencyProperty IsOverflowMenuVisibleProperty = DependencyProperty.Register("IsOverflowMenuVisible", typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsOverflowMenuVisibleChangedCallback));
       
        /// <summary>
        /// Registers the property to set the overflow menu button visibility.
        /// </summary>
        private static readonly DependencyProperty IsOverflowMenuButtonVisibleProperty = DependencyProperty.Register("IsOverflowMenuButtonVisible", typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsOverFlowmenuButtonVisibleChangedCallback));

        /// <summary>
        /// Register the property to get the visibility of the collapsed content popup.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedContentPopupVisibleProperty = DependencyProperty.Register("IsCollapsedContentPopupVisible", typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsCollapsedContentPopupVisibleChangedCallback));

        /// <summary>
        /// Register the property to get the visibility of the collapsed common content area popup.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedCommonContentPopupVisibleProperty = DependencyProperty.Register(nameof(IsCollapsedCommonContentPopupVisible), typeof(bool), typeof(Sidebar), new UIPropertyMetadata(false, IsCollapsedCommonPopupVisibleChangedCallback));

        /// <summary>
        /// Registers the specialized read only property to get the <see cref="SidebarSection"/>'s content.
        /// </summary>
        private static readonly DependencyPropertyKey SectionContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SectionContent), typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to set the <see cref="Sidebar"/>'s common section.
        /// </summary>
        private static readonly DependencyProperty CommonSectionProperty = DependencyProperty.Register(nameof(CommonSection), typeof(object), typeof(Sidebar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to hide <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty ShowSectionsProperty = DependencyProperty.Register("ShowSections", typeof(bool), typeof(Sidebar), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to show the common <see cref="SidebarSection"/>
        /// </summary>
        public static readonly DependencyProperty ShowCommonProperty = DependencyProperty.Register("ShowCommon", typeof(bool), typeof(Sidebar), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the number of visible <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty VisibleSectionsProperty = DependencyProperty.Register(nameof(VisibleSections), typeof(int), typeof(Sidebar), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, VisibleSectionsChangedCallback));

        /// <summary>
        /// Registers the property to set the selected <see cref="SidebarSection"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedSectionProperty = DependencyProperty.Register(nameof(SelectedSection), typeof(SidebarSection), typeof(Sidebar), new UIPropertyMetadata(null, SelectedSectionChangedCallback));

        /// <summary>
        /// Registers the property to set the index of the selected <see cref="SidebarSection"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(Sidebar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedIndexChangedCallback));

        /// <summary>
        /// Registers the property to get the <see cref="SidebarSection"/>'s content.
        /// </summary>
        public static readonly DependencyProperty SectionContentProperty = SectionContentPropertyKey.DependencyProperty;
        
        /// <summary>
        /// Registers the property to get the content of collapsed <see cref="SidebarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty CollapsedContentProperty = CollapsedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the collapsed common content.
        /// </summary>
        public static readonly DependencyProperty CollapsedCommonContentProperty = CollapsedCommonContentPropertyKey.DependencyProperty;

      
        #endregion

        #region Properties

        #region Properties: Bindable

        /// <summary>
        /// Gets the collection of all <see cref="SidebarSection"/>s.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<SidebarSection> Sections
        {
            get { return _SidebarSections; }
        }

        /// <summary>
        /// Gets the collection of maximized <see cref="SidebarSection"/>s.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<SidebarSection> MaximizedSections
        {
            get { return _MaximizedSidebarSections; }
        }

        /// <summary>
        /// Gets the collection of minimized <see cref="SidebarSection"/>s.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<SidebarSection> MinimizedSections
        {
            get { return _MinimizedSidebarSections; }
        }

        /// <summary>
        /// Gets the collection of overflow menu items.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<SidebarSection> OverflowMenuItems
        {
            get { return _OverflowMenuItems; }
        }

        /// <summary>
        /// Gets the collection of option buttons.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

        internal object CollapsedCommonContent
        {
            get { return GetValue(CollapsedCommonContentProperty); }
            set { SetValue(CollapsedCommonContentPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the content of the selected <see cref="SidebarSection"/>.
        /// </summary>
        internal object SectionContent
        {
            get { return GetValue(SectionContentProperty); }
            set { SetValue(SectionContentPropertyKey, value); }
        }

        #endregion

        #region Properties: Public

        /// <summary>
        /// Gets or sets whether the collapsed content popup is visible.
        /// </summary>
        public bool IsCollapsedContentPopupVisible
        {
            get { return (bool)GetValue(IsCollapsedContentPopupVisibleProperty); }
            set { SetValue(IsCollapsedContentPopupVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the collapsed common content area popup is visible.
        /// </summary>
        public bool IsCollapsedCommonContentPopupVisible
        {
            get { return (bool)GetValue(IsCollapsedCommonContentPopupVisibleProperty); }
            set { SetValue(IsCollapsedCommonContentPopupVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Sidebar"/>'s common section.
        /// </summary>
        public object CommonSection
        {
            get { return GetValue(CommonSectionProperty); }
            set { SetValue(CommonSectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the collapsed <see cref="Sidebar"/>.
        /// </summary>
        public double CollapsedWidth
        {
            get { return (double)GetValue(CollapsedWidthProperty); }
            set { SetValue(CollapsedWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dock position of the side bar.
        /// </summary>
        /// <remarks><i>Only left and right docking is supported.</i></remarks>
        public SidebarDockPositions DockPosition
        {
            get { return (SidebarDockPositions)GetValue(DockPositionProperty); }
            set { SetValue(DockPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of visible <see cref="SidebarSection"/>s.
        /// </summary>
        public int VisibleSections
        {
            get { return (int)GetValue(VisibleSectionsProperty); }
            set { SetValue(VisibleSectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected <see cref="SidebarSection"/>.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected <see cref="SidebarSection"/>.
        /// </summary>
        public SidebarSection SelectedSection
        {
            get { return (SidebarSection)GetValue(SelectedSectionProperty); }
            set { SetValue(SelectedSectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Sidebar"/> is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the overflow menu is visible.
        /// </summary>
        public bool IsOverflowMenuVisible
        {
            get { return (bool)GetValue(IsOverflowMenuVisibleProperty); }
            set { SetValue(IsOverflowMenuVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to show the <see cref="SidebarSection"/>s.
        /// </summary>
        public bool ShowSections
        {
            get { return (bool)GetValue(ShowSectionsProperty); }
            set { SetValue(ShowSectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to show the common <see cref="SidebarSection"/>.
        /// </summary>
        public bool ShowCommon
        {
            get { return (bool)GetValue(ShowCommonProperty); }
            set { SetValue(ShowCommonProperty, value); }
        }

        /// <summary>
        /// Gets or sets wheter the overflow menu button is visible.
        /// </summary>
        public bool IsOverflowMenuButtonVisible
        {
            get { return (bool)GetValue(IsOverflowMenuButtonVisibleProperty); }
            set { SetValue(IsOverflowMenuButtonVisibleProperty, value); }
        }
        #endregion

        #endregion

        #region Commands

        #region Commands: Registration

        /// <summary>
        /// Defines the command to collapse the side bar.
        /// </summary>
        private static RoutedUICommand _CollapseCommand = new RoutedUICommand(nameof(CollapseCommand), nameof(CollapseCommand), typeof(Sidebar));

        /// <summary>
        /// Defines the command to resize the side bar.
        /// </summary>
        private static RoutedUICommand _ResizeSidebarCommand = new RoutedUICommand(nameof(ResizeSidebarCommand), nameof(ResizeSidebarCommand), typeof(Sidebar));

        /// <summary>
        /// Defines the command to resize the side bar sections.
        /// </summary>
        private static RoutedUICommand _ResizeSectionsCommand = new RoutedUICommand(nameof(ResizeSectionsCommand), nameof(ResizeSectionsCommand), typeof(Sidebar));

        /// <summary>
        /// Defines the command to resize the side bar common area.
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
        /// Gets the command to resize the sections.
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

            // Attach the mouse move event handler to the side bar
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

            // Attach the mouse move event handler to the side bar
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

            // Remove the mouse move event handlers from the side bar
            this.PreviewMouseMove -= ResizeSidebar;
            this.PreviewMouseMove -= ResizeSections;
            this.PreviewMouseMove -= ResizeCommon;
        }

        /// <summary>
        /// Handles the resize side bar command mouse movement.
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

                // Determine if the side bar collapses
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
                // Remove the mouse move event handler from the side bar
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
                Thickness border = (Thickness)StyleManager.GetStyleValue(Styles.BorderThickness);

                double height = this.ActualHeight - SIDEBAR_DIMENSION_CONSTANT - mousePosition.Y;

                // TODO:
                if (mousePosition.Y > (_PARTSidebarCommon.ActualHeight + SIDEBAR_DIMENSION_CONSTANT))
                {
                    this.VisibleSections = (int)(height / SIDEBAR_DIMENSION_CONSTANT);
                }
                else
                {
                    
                }
            }
            else
            {
                // Remove the mouse move event handler from the side bar
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

                // Prevent scaling over the side bar sections area
                if (height > this.ActualHeight - GetMinimumSidebarSectionsAreaHeight())
                    height = this.ActualHeight - GetMinimumSidebarSectionsAreaHeight();

                // Prevent scaling over the common area header
                if (height < SIDEBAR_SECTION_HEIGHT + 5)
                    height = SIDEBAR_SECTION_HEIGHT + 5;

                _PARTSidebarCommon.Height = height;
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
        /// Handles the property changed event of the <see cref="IsOverflowMenuVisibleProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsOverflowMenuVisibleChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)sender;

            if ((bool)e.NewValue)
                sideBar.InitializeOverflowMenu();
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="IsOverflowMenuButtonVisibleProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsOverFlowmenuButtonVisibleChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)sender;

            if ((bool)e.NewValue)
                sideBar.InitializeOverflowMenu();
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="IsCollapsedContentPopupVisibleProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsCollapsedContentPopupVisibleChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)sender;

            if(sideBar._PARTCollapsedContentPopup != null)
            {
                sideBar._PARTCollapsedContentPopup.StaysOpen = true;
                sideBar._PARTCollapsedContentPopup.IsOpen = (bool)e.NewValue;
            }

            // If popup is open
            if((bool)e.NewValue)
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupOpenedRoutedEvent));
            }
            else
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupClosedRoutedEvent));
            }
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="IsCollapsedCommonContentPopupVisibleProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsCollapsedCommonPopupVisibleChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)sender;

            if (sideBar._PARTCollapsedCommonPopup != null)
            {
                sideBar._PARTCollapsedCommonPopup.StaysOpen = true;
                sideBar._PARTCollapsedCommonPopup.IsOpen = (bool)e.NewValue;
            }

            // If popup is open
            if ((bool)e.NewValue)
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupOpenedRoutedEvent));
            }
            else
            {
                sideBar.RaiseEvent(new RoutedEventArgs(PopupClosedRoutedEvent));
            }
        }


        /// <summary>
        /// Handles the property changed event of the <see cref="IsExpandedProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsExpandedChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)sender;

            bool isExpanded = (bool)e.NewValue;

            sideBar.InvalidateContentVisibility();
            sideBar.InitializeOverflowMenu();

            if (isExpanded)
            {
                sideBar.IsCollapsedContentPopupVisible = false;
                sideBar.IsCollapsedCommonContentPopupVisible = false;

                sideBar.MaxWidth = sideBar._LastWidth;
                sideBar.RaiseEvent(new RoutedEventArgs(CollapsedRoutedEvent));
            }
            else
            {
                sideBar._LastWidth = sideBar.MaxWidth;
                sideBar.MaxWidth = sideBar.CollapsedWidth;
                sideBar.RaiseEvent(new RoutedEventArgs(ExpandedRoutedEvent));
                
            }
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="SelectedSectionProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void SelectedSectionChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = (Sidebar)sender;

            for (int i = 0; i < sideBar._SidebarSections.Count; i++)
            {
                SidebarSection section = sideBar._SidebarSections[i];

                section.IsSelected = (section == (SidebarSection)e.NewValue);

                if(section.IsSelected)
                {
                    // Set the index of the section to the side bar
                    sideBar.SelectedIndex = i;

                    // Add the content of the selected section to the side bar
                    sideBar.SectionContent = sideBar.IsExpanded ? section.Content : null;

                    // Add the content of the selected collapsed section to the side bar
                    sideBar.CollapsedContent = sideBar.IsExpanded ? null : section.Content;
                    
                }
            }

            
            sideBar.RaiseEvent(new RoutedPropertyChangedEventArgs<SidebarSection>((SidebarSection)e.OldValue, (SidebarSection)e.NewValue, SelectedSectionChangedRoutedEvent));
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="VisibleSectionsProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void VisibleSectionsChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = sender as Sidebar;

            if (sideBar != null)
            {
                sideBar.InitializeSidebarSections();
            }
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="SelectedIndexProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void SelectedIndexChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Sidebar sideBar = sender as Sidebar;

            if (sideBar != null)
                sideBar.InitializeSidebarSections();
        }

       
        #endregion

        #region Overrides

        /// <summary>
        /// Handles the <see cref="FrameworkElement.OnApplyTemplate"/>, called when the side bar template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if(_PARTCollapsedContentPopup != null)
            {
                _PARTCollapsedContentPopup.Opened -= CollapsedContentPopupOpened;
                _PARTCollapsedContentPopup.Closed -= CollapsedContentPopupClosed;
            }

            if(_PARTCollapsedCommonPopup != null)
            {
                _PARTCollapsedCommonPopup.Opened -= CollapsedCommonPopupOpened;
                _PARTCollapsedCommonPopup.Closed -= CollapsedCommonPopupClosed;
            }

            // Gets a reference to the element in xaml
            _PARTSidebarMinimizedSections = GetTemplateChild(PART_SIDEBAR_MINIMIZED_SECTIONS) as FrameworkElement;
            _PARTOverflowMenu = GetTemplateChild(PART_SIDEBAR_OVERFLOW_MENU) as Popup;
            _PARTSidebarCommon = GetTemplateChild(PART_SIDEBAR_COMMON) as FrameworkElement;

            _PARTCollapsedContentPopup = GetTemplateChild(PART_SIDEBAR_COLLAPSED_CONTENT_POPUP) as Popup;
            _PARTCollapsedContentButton = GetTemplateChild(PART_SIDEBAR_COLLAPSED_CONTENT_BUTTON) as ToggleButton;

            _PARTCollapsedCommonPopup = GetTemplateChild(PART_SIDEBAR_COLLAPSED_COMMON_POPUP) as Popup;
            _PARTCollapsedCommonButton = GetTemplateChild(PART_SIDEBAR_COLLAPSED_COMMON_BUTTON) as ToggleButton;

            _PARTSidebarHeader = GetTemplateChild(PART_SIDEBAR_HEADER) as Button;

            if(_PARTCollapsedContentPopup != null)
            {
                _PARTCollapsedContentPopup.Opened += CollapsedContentPopupOpened;
                _PARTCollapsedContentPopup.Closed += CollapsedContentPopupClosed;
            }

            if(_PARTCollapsedContentButton != null)
            {
                _PARTCollapsedContentButton.PreviewMouseLeftButtonUp += CollapsedContentButtonMouseUpPreview;
            }

            if(_PARTCollapsedCommonPopup != null)
            {
                _PARTCollapsedCommonPopup.Opened += CollapsedCommonPopupOpened;
                _PARTCollapsedCommonPopup.Closed += CollapsedCommonPopupClosed;
            }

            if(_PARTCollapsedCommonButton != null)
            {
                _PARTCollapsedCommonButton.PreviewMouseLeftButtonUp += CollapsedCommonButtonMouseUpPreview;
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Handles the <see cref="FrameworkElement.Initialized"/> event, called when the side bar is initialized.
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
        /// Initializes the side bar sections.
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

            // Loop through all side bar sections
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

            // Update the side bar section collections
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
            if (_PARTSidebarMinimizedSections == null)
                return 0;

            // Get dynamic applied border thickness style value
            Thickness border = (Thickness)StyleManager.GetStyleValue(Styles.BorderThickness);

            return (int)Math.Floor((_PARTSidebarMinimizedSections.ActualWidth - SIDEBAR_SECTION_IMAGE_DIMENSIONS) 
                                  / (SIDEBAR_SECTION_IMAGE_DIMENSIONS + border.Right));
        }

        /// <summary>
        /// Calculates the minimum height of the side bar sections area to show all visible <see cref="SidebarSection"/>s.
        /// </summary>
        /// <returns>An <see cref="double"/> containing the height to maintain visibility of all maximized <see cref="SidebarSection"/>s.</returns>
        private double GetMinimumSidebarSectionsAreaHeight()
        {
            // Get dynamic applied border thickness style value
            Thickness border = (Thickness)StyleManager.GetStyleValue(Styles.BorderThickness);

            return (_MaximizedSidebarSections.Count * SIDEBAR_DIMENSION_CONSTANT + border.Top)
                   + SIDEBAR_SECTION_IMAGE_DIMENSIONS + border.Top 
                   + SIDEBAR_DIMENSION_CONSTANT + border.Top 
                   + SIDEBAR_HANDLE_DIMENSION_CONSTANT;
        }

        /// <summary>
        /// Invalidates the content to be visible after the side bar expandend state is changed.
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
            object tempCommonContent = this.CommonSection;

            // Reset the section content
            this.CommonSection = null;

            // Reassign the section content
            //this.CollapsedCommonContent =  tempCommonContent;
            this.CommonSection = tempCommonContent;


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
        /// Handles the <see cref="Popup.Closed"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void CollapsedContentPopupClosed(object sender, EventArgs e)
        {
            IsCollapsedContentPopupVisible = false;
            Mouse.Capture(null);
        }

        /// <summary>
        /// Handles the <see cref="Popup.Opened"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void CollapsedContentPopupOpened(object sender, EventArgs e)
        {
            IsCollapsedContentPopupVisible = true;
            
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Handles the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapsedContentButtonMouseUpPreview(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);

            Mouse.Capture(null);
            _PARTCollapsedContentPopup.StaysOpen = false;
        }

        /// <summary>
        /// Handles the <see cref="Popup.Closed"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void CollapsedCommonPopupClosed(object sender, EventArgs e)
        {
            IsCollapsedCommonContentPopupVisible = false;
            Mouse.Capture(null);
        }

        /// <summary>
        /// Handles the <see cref="Popup.Opened"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void CollapsedCommonPopupOpened(object sender, EventArgs e)
        {
            IsCollapsedCommonContentPopupVisible = true;

            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Handles the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapsedCommonButtonMouseUpPreview(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);

            Mouse.Capture(null);
            _PARTCollapsedCommonPopup.StaysOpen = false;
        }


        #endregion

        //protected override IEnumerator LogicalChildren
        //{
        //    get
        //    {
        //        return GetLogicalChildren().GetEnumerator();
        //    }
        //}

        //protected virtual IEnumerable GetLogicalChildren()
        //{
        //    foreach (var section in Sections) yield return section;
        //    if (SelectedSection != null) yield return SelectedSection.Content;
        //}

    }

    //
    /// <summary>
    /// Defines all possible <see cref="SidebarSection"/>'s <see cref="SidebarSection.StatusFlags"/> values.
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
