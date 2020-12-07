using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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

    /// <summary>
    /// The SideBar is a container for SideBarSections
    /// 
    /// </summary>
    [TemplatePart(Name = PART_SIDEBAR_MINIMIZED_SECTIONS)]
    [TemplatePart(Name = PART_SIDEBAR_OVERFLOW_MENU)]
    [TemplatePart(Name = PART_SIDEBAR_COMMON)]
    [TemplatePart(Name = PART_SIDEBAR_COLLAPSED_CONTENT_POPUP)]
    [TemplatePart(Name = PART_SIDEBAR_COLLAPSED_CONTENT_BUTTON)]
    public class SideBar : HeaderedItemsControl
    {
        #region Constants

        /// <summary>
        /// The template part name of the minimized sections in xaml.
        /// </summary>
        private const string PART_SIDEBAR_MINIMIZED_SECTIONS = "PART_SideBarMinimizedSections";

        /// <summary>
        /// The template part name of the overflow menu in xaml.
        /// </summary>
        private const string PART_SIDEBAR_OVERFLOW_MENU = "PART_SideBarOverflowMenu";

        /// <summary>
        /// The template part name of the side bar common area in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COMMON = "PART_SideBarCommon";

        /// <summary>
        /// The template part name of the collapsed side bar content in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COLLAPSED_CONTENT_POPUP = "PART_SideBarCollapsedContentPopup";

        /// <summary>
        /// The template part name of the collapsed side bar content button in xaml.
        /// </summary>
        private const string PART_SIDEBAR_COLLAPSED_CONTENT_BUTTON = "PART_SideBarCollapsedContentButton";

        /// <summary>
        /// The height of <see cref="SideBarSection"/>s.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_HEIGHT = 32;

        /// <summary>
        /// The minimized dimensions of <see cref="SideBarSection"/>s.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_MINIMIZED_DIMENSIONS = SIDEBAR_SECTION_HEIGHT;

        /// <summary>
        /// The factor of a <see cref="SideBarSection"/>'s image scale.
        /// </summary>
        public const double SIDEBAR_SECTION_IMAGE_SCALE_FACTOR = 0.8;

        /// <summary>
        /// The default square dimensions of a <see cref="SideBarSection"/>'s image.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_IMAGE_DIMENSIONS = SIDEBAR_SECTION_HEIGHT * SIDEBAR_SECTION_IMAGE_SCALE_FACTOR;

        /// <summary>
        /// The default image margins of a <see cref="SideBarSection"/>'s image.
        /// </summary>
        public const double SIDEBAR_SECTION_IMAGE_MARGINS = (SIDEBAR_SECTION_HEIGHT * (1 - SIDEBAR_SECTION_IMAGE_SCALE_FACTOR)) / 2;

        /// <summary>
        /// The minimum width of the <see cref="SideBar"/> before it collapses.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        private const double SIDEBAR_COLLAPSE_WIDTH = 100;

        /// <summary>
        /// The width of the <see cref="SideBarSection"/>s selection indicator.
        /// </summary>
        /// <remarks><i>The value is in device independent units (1/96th inch per unit).</i></remarks>
        public const double SIDEBAR_SECTION_INDICATOR_WIDTH = 5;

        #endregion

        #region Enumerations

        /// <summary>
        /// Defines the possible values of <see cref="SideBar"/> dock positions.
        /// </summary>
        public enum SideBarDockPositions
        {
            /// <summary>
            /// Aligns the <see cref="SideBar"/> left.
            /// </summary>
            Left = 0,

            /// <summary>
            /// Aligns the <see cref="SideBar"/> right.
            /// </summary>
            Right = 2
        }

        #endregion

        #region Fields

        /// <summary>
        /// Stores all <see cref="SideBarSection"/>s.
        /// </summary>
        private ObservableCollection<SideBarSection> _SideBarSections;

        /// <summary>
        /// Stores minimized <see cref="SideBarSection"/>s.
        /// </summary>
        private Collection<SideBarSection> _MinimizedSideBarSections;

        /// <summary>
        /// Stores maximized <see cref="SideBarSection"/>s.
        /// </summary>
        private Collection<SideBarSection> _MaximizedSideBarSections;

        /// <summary>
        /// Stores overflow menu items.
        /// </summary>
        private Collection<SideBarSection> _OverflowMenuItems;

        /// <summary>
        /// Stores option buttons.
        /// </summary>
        private Collection<ButtonBase> _OptionButtons;

        /// <summary>
        /// Stores the width to expand to when the <see cref="SideBar"/> is collapsed.
        /// </summary>
        private double _LastWidth = double.PositiveInfinity;

        /// <summary>
        /// Stores a reference to the template part in xaml.
        /// </summary>
        private FrameworkElement _PARTSideBarMinimizedSections;

        /// <summary>
        /// Stores a reference to the overflow menu popup template part in xaml.
        /// </summary>
        private Popup _PARTOverflowMenu;

        /// <summary>
        /// Stores a reference to the collapsed content popup template part in xaml.
        /// </summary>
        private Popup _PARTCollapsedContentPopup;

        /// <summary>
        /// Stores a reference to the side bar common template part in xaml.
        /// </summary>
        private FrameworkElement _PARTSideBarCommon;

        /// <summary>
        /// Stores the common section.
        /// </summary>
        private object _CommonSection;

        /// <summary>
        /// Stores a reference to the collapsed content button template part in xaml.
        /// </summary>
        private ToggleButton _PARTCollapsedContentButton;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="SideBar"/>.
        /// </summary>
        static SideBar()
        {
            // Overrides the default style of the inherited HeaderedItemsControl to use the SideBar style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SideBar), new FrameworkPropertyMetadata(typeof(SideBar)));
        }

        /// <summary>
        /// Creates and initializes a new <see cref="SideBar"/> control.
        /// </summary>
        public SideBar() : base()
        {
            _OverflowMenuItems = new Collection<SideBarSection>();
            _OptionButtons = new Collection<ButtonBase>();
            _CommonSection = new object();

            SetValue(OverflowMenuItemsPropertyKey, _OverflowMenuItems);
            SetValue(OptionButtonsPropertyKey, _OptionButtons);
            SetValue(CommonSectionProperty, _CommonSection);

            _SideBarSections = new ObservableCollection<SideBarSection>();

            _MinimizedSideBarSections = new Collection<SideBarSection>();
            _MaximizedSideBarSections = new Collection<SideBarSection>();

            // Bind the side bar commands
            CommandBindings.Add(new CommandBinding(_RoutedUICommandCollapseSideBar, OnSideBarCollapse));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandResizeSideBar, OnSideBarResize));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandResizeSections, OnSectionsResize));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandResizeCommon, OnCommonResize));

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.SizeChanged += new SizeChangedEventHandler(SideBarSizeChanged);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Registers the event to raise when the <see cref="SelectedSection"/> changes.
        /// </summary>
        public static readonly RoutedEvent SelectedSectionChangedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(SelectedSectionChangedRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<SideBarSection>), typeof(SideBar));

        /// <summary>
        /// Registers the event to raise when the <see cref="SideBar"/> is collapsed.
        /// </summary>
        public static readonly RoutedEvent CollapsedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(CollapsedRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SideBar));

        /// <summary>
        /// Registers the event to raise when the <see cref="SideBar"/> is expanded.
        /// </summary>
        public static readonly RoutedEvent ExpandedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(ExpandedRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SideBar));

        /// <summary>
        /// Defines the event to raise when the <see cref="SelectedSection"/> changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SideBarSection> SelectedSectionChanged
        {
            add { AddHandler(SelectedSectionChangedRoutedEvent, value); }
            remove { RemoveHandler(SelectedSectionChangedRoutedEvent, value); }
        }

        /// <summary>
        /// Registers the event to raise when the <see cref="_PARTCollapsedContentPopup"/> is opened.
        /// </summary>
        public static readonly RoutedEvent PopupOpenedRoutedEvent = EventManager.RegisterRoutedEvent("PopupOpenedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SideBar));

        /// <summary>
        /// Registers the event to raise when the <see cref="_PARTCollapsedContentPopup"/> is closed.
        /// </summary>
        public static readonly RoutedEvent PopupClosedRoutedEvent = EventManager.RegisterRoutedEvent("PopupClosedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SideBar));

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

        /// <summary>
        /// Registers the property to set the dock position of the <see cref="SideBar"/>.
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty = DependencyProperty.Register(nameof(DockPosition), typeof(SideBarDockPositions), typeof(SideBar), new UIPropertyMetadata(SideBarDockPositions.Left));

        /// <summary>
        /// Registers the property to get the collapsed width of the <see cref="SideBar"/>.
        /// </summary>
        public static readonly DependencyProperty CollapsedWidthProperty = DependencyProperty.Register(nameof(CollapsedWidth), typeof(double), typeof(SideBar), new UIPropertyMetadata(SIDEBAR_SECTION_MINIMIZED_DIMENSIONS));

        /// <summary>
        /// Registers the specialized read only property to get the content of collapsed <see cref="SideBarSection"/>s.
        /// </summary>
        private static readonly DependencyPropertyKey CollapsedContentPropertyKey = DependencyProperty.RegisterReadOnly("CollapsedContent", typeof(object), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to get the expanded state of the <see cref="SideBar"/>.
        /// </summary>
        public static readonly DependencyProperty IsExpandendProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SideBar), new UIPropertyMetadata(true, IsExpandedChangedCallback));

        /// <summary>
        /// Registers the property to get the overflow menu visibility..
        /// </summary>
        public static readonly DependencyProperty IsOverflowMenuVisibleProperty = DependencyProperty.Register("IsOverflowMenuVisible", typeof(bool), typeof(SideBar), new UIPropertyMetadata(false, IsOverflowMenuVisibleChangedCallback));

        /// <summary>
        /// Registers the specialized read only property to get the maximized <see cref="SideBarSection"/>s.
        /// </summary>
        private static readonly DependencyPropertyKey MaximizedSectionsPropertyKey = DependencyProperty.RegisterReadOnly("MaximizedSections", typeof(Collection<SideBarSection>), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the minimized <see cref="SideBarSection"/>s.
        /// </summary>
        public static readonly DependencyPropertyKey MinimizedSectionsPropertyKey = DependencyProperty.RegisterReadOnly("MinimizedSections", typeof(Collection<SideBarSection>), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the overflow menu items.
        /// </summary>
        private static readonly DependencyPropertyKey OverflowMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("OverflowMenuItems", typeof(Collection<SideBarSection>), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the specialized read only property to get the option buttons.
        /// </summary>
        private static readonly DependencyPropertyKey OptionButtonsPropertyKey = DependencyProperty.RegisterReadOnly("OptionButtons", typeof(Collection<ButtonBase>), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to set the overflow menu button visibility.
        /// </summary>
        private static readonly DependencyProperty IsOverflowMenuButtonVisibleProperty = DependencyProperty.Register("IsOverflowMenuButtonVisible", typeof(bool), typeof(SideBar), new UIPropertyMetadata(false, IsOverFlowmenuButtonVisibleChangedCallback));

        /// <summary>
        /// Register the property to get the visibility of the collapsed content popup.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedContentPopupVisibleProperty = DependencyProperty.Register("IsCollapsedContentPopupVisible", typeof(bool), typeof(SideBar), new UIPropertyMetadata(false, IsCollapsedContentPopupVisibleChangedCallback));

        /// <summary>
        /// Registers the specialized read only property to get the <see cref="SideBarSection"/>'s content.
        /// </summary>
        private static readonly DependencyPropertyKey SectionContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SectionContent), typeof(object), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to set the <see cref="SideBar"/>'s common section.
        /// </summary>
        private static readonly DependencyProperty CommonSectionProperty = DependencyProperty.Register(nameof(CommonSection), typeof(object), typeof(SideBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Registers the property to hide <see cref="SideBarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty ShowSectionsProperty = DependencyProperty.Register("ShowSections", typeof(bool), typeof(SideBar), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to show the common <see cref="SideBarSection"/>
        /// </summary>
        public static readonly DependencyProperty ShowCommonProperty = DependencyProperty.Register("ShowCommon", typeof(bool), typeof(SideBar), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to set the number of visible <see cref="SideBarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty VisibleSectionsProperty = DependencyProperty.Register(nameof(VisibleSections), typeof(int), typeof(SideBar), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, VisibleSectionsChangedCallback));

        /// <summary>
        /// Registers the property to set the selected <see cref="SideBarSection"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedSectionProperty = DependencyProperty.Register(nameof(SelectedSection), typeof(SideBarSection), typeof(SideBar), new UIPropertyMetadata(null, SelectedSectionChangedCallback));

        /// <summary>
        /// Registers the property to set the index of the selected <see cref="SideBarSection"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(SideBar), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedIndexChangedCallback));

        /// <summary>
        /// Registers the property to get the maximized <see cref="SideBarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty MaximizedSectionsProperty = MaximizedSectionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the minimized <see cref="SideBarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty MinimizedSectionsProperty = MinimizedSectionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the <see cref="SideBarSection"/>'s content.
        /// </summary>
        public static readonly DependencyProperty SectionContentProperty = SectionContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the overflow menu items.
        /// </summary>
        public static readonly DependencyProperty OverflowMenuItemsProperty = OverflowMenuItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the content of collapsed <see cref="SideBarSection"/>s.
        /// </summary>
        public static readonly DependencyProperty CollapsedContentProperty = CollapsedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Registers the property to get the option buttons.
        /// </summary>
        public static readonly DependencyProperty OptionButtonsProperty = OptionButtonsPropertyKey.DependencyProperty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="SideBarSection"/>'s.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<SideBarSection> Sections
        {
            get { return _SideBarSections; }
        }

        /// <summary>
        /// Gets the collection of overflow menu items.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<SideBarSection> OverflowMenuItems
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

        /// <summary>
        /// Gets or sets whether the collapsed content popup is visible.
        /// </summary>
        public bool IsCollapsedContentPopupVisible
        {
            get { return (bool)GetValue(IsCollapsedContentPopupVisibleProperty); }
            set { SetValue(IsCollapsedContentPopupVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="SideBar"/>'s common section.
        /// </summary>
        public object CommonSection
        {
            get { return GetValue(CommonSectionProperty); }
            set { SetValue(CommonSectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the collapsed <see cref="SideBar"/>.
        /// </summary>
        public double CollapsedWidth
        {
            get { return (double)GetValue(CollapsedWidthProperty); }
            set { SetValue(CollapsedWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the selected <see cref="SideBarSection"/>.
        /// </summary>
        internal object SectionContent
        {
            get { return GetValue(SectionContentProperty); }
            set { SetValue(SectionContentPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the content of collapsed <see cref="SideBarSection"/>s.
        /// </summary>
        internal object CollapsedContent
        {
            get { return GetValue(CollapsedContentProperty); }
            set { SetValue(CollapsedContentPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets the dock position of the side bar.
        /// </summary>
        /// <remarks><i>Only left and right docking is supported.</i></remarks>
        public SideBarDockPositions DockPosition
        {
            get { return (SideBarDockPositions)GetValue(DockPositionProperty); }
            set { SetValue(DockPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of visible <see cref="SideBarSection"/>s.
        /// </summary>
        public int VisibleSections
        {
            get { return (int)GetValue(VisibleSectionsProperty); }
            set { SetValue(VisibleSectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected <see cref="SideBarSection"/>.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected <see cref="SideBarSection"/>.
        /// </summary>
        public SideBarSection SelectedSection
        {
            get { return (SideBarSection)GetValue(SelectedSectionProperty); }
            set { SetValue(SelectedSectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="SideBar"/> is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandendProperty); }
            set { SetValue(IsExpandendProperty, value); }
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
        /// Gets or sets whether to show the <see cref="SideBarSection"/>s.
        /// </summary>
        public bool ShowSections
        {
            get { return (bool)GetValue(ShowSectionsProperty); }
            set { SetValue(ShowSectionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to show the common <see cref="SideBarSection"/>.
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

        #region Commands

        /// <summary>
        /// Defines the command to collapse the side bar.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandCollapseSideBar = new RoutedUICommand("CollapseSideBar", nameof(CollapseSideBarCommand), typeof(SideBar));

        /// <summary>
        /// Defines the command to resize the side bar.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandResizeSideBar = new RoutedUICommand("ResizeSideBar", nameof(ResizeSideBarCommand), typeof(SideBar));

        /// <summary>
        /// Defines the command to resize the side bar sections.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandResizeSections = new RoutedUICommand("ResizeSections", nameof(ResizeSectionsCommand), typeof(SideBar));

        /// <summary>
        /// Defines the command to resize the side bar common area.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandResizeCommon = new RoutedUICommand("ResizeCommon", nameof(ResizeCommonCommand), typeof(SideBar));

        /// <summary>
        /// Gets the command to collapse the <see cref="SideBar"/>.
        /// </summary>
        public static RoutedUICommand CollapseSideBarCommand { get { return _RoutedUICommandCollapseSideBar; } }

        /// <summary>
        /// Gets the command to resize the <see cref="SideBar"/>.
        /// </summary>
        public static RoutedUICommand ResizeSideBarCommand { get { return _RoutedUICommandResizeSideBar; } }

        /// <summary>
        /// Gets the command to resize the <see cref="SideBar"/> common area.
        /// </summary>
        public static RoutedUICommand ResizeCommonCommand { get { return _RoutedUICommandResizeCommon; } }

        /// <summary>
        /// Gets the command to resize the sections.
        /// </summary>
        public static RoutedUICommand ResizeSectionsCommand { get { return _RoutedUICommandResizeSections; } }

        #endregion

        #region Command Event Handlers

        /// <summary>
        /// Handles the <see cref="CollapseSideBarCommand"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSideBarCollapse(object sender, ExecutedRoutedEventArgs e)
        {
            // Toggles the expanded state by an XOR boolean assignment
            this.IsExpanded ^= true;
        }

        /// <summary>
        /// Handles the <see cref="ResizeSideBarCommand"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnSideBarResize(object sender, ExecutedRoutedEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if (control != null)
            {
                // Attach the mouse up button event handler to the control that started the resize be notified when resizing has been done
                control.PreviewMouseUp += new MouseButtonEventHandler(RemoveResizeEventListeners);
            }

            // Attach the mouse move event handler to the side bar
            this.PreviewMouseMove += new MouseEventHandler(ResizeSideBar);
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
            this.PreviewMouseMove -= ResizeSideBar;
            this.PreviewMouseMove -= ResizeSections;
            this.PreviewMouseMove -= ResizeCommon;
        }

        /// <summary>
        /// Handles the resize side bar command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeSideBar(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(this);

                double width = 0;

                // Determine the resize direction
                if (this.DockPosition == SideBarDockPositions.Left)
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
                this.PreviewMouseMove -= ResizeSideBar;
            }
        }

        /// <summary>
        /// Handles the resize section command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeSections(object sender, MouseEventArgs e)
        {
            
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                // TODO: Resize sections function
                Point mousePosition = e.GetPosition(this);

                double height = this.ActualHeight - SIDEBAR_SECTION_HEIGHT - mousePosition.Y - 1;

                // TODO:
                if(mousePosition.Y > _PARTSideBarCommon.ActualHeight)
                this.VisibleSections = (int)(height / SIDEBAR_SECTION_HEIGHT);
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
                if (height > this.ActualHeight - GetMinimumSideBarSectionsAreaHeight())
                    height = this.ActualHeight - GetMinimumSideBarSectionsAreaHeight();

                // Prevent scaling over the common area header
                if (height < SIDEBAR_SECTION_HEIGHT + 5)
                    height = SIDEBAR_SECTION_HEIGHT + 5;

                _PARTSideBarCommon.Height = height;
            }
            else
            {
                this.PreviewMouseMove -= ResizeCommon;
            }
        }

        /// <summary>
        /// Handels the <see cref="SideBar.SizeChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="SizeChangedEventArgs"/> containing event data.</param>
        private void SideBarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitializeSideBarSections();
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
            SideBar sideBar = (SideBar)sender;

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
            SideBar sideBar = (SideBar)sender;

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
            SideBar sideBar = (SideBar)sender;

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
        /// Handles the property changed event of the <see cref="IsExpandendProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void IsExpandedChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SideBar sideBar = (SideBar)sender;

            bool isExpanded = (bool)e.NewValue;

            sideBar.InvalidateContentVisibility();
            sideBar.InitializeOverflowMenu();

            if (isExpanded)
            {
                sideBar.IsCollapsedContentPopupVisible = false;
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
            SideBar sideBar = (SideBar)sender;

            for (int i = 0; i < sideBar._SideBarSections.Count; i++)
            {
                SideBarSection section = sideBar._SideBarSections[i];

                section.IsSelected = (section == (SideBarSection)e.NewValue);

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

            sideBar.RaiseEvent(new RoutedPropertyChangedEventArgs<SideBarSection>((SideBarSection)e.OldValue, (SideBarSection)e.NewValue, SelectedSectionChangedRoutedEvent));
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="VisibleSectionsProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void VisibleSectionsChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SideBar sideBar = sender as SideBar;

            if (sideBar != null)
            {
                sideBar.InitializeSideBarSections();
            }
        }

        /// <summary>
        /// Handles the property changed event of the <see cref="SelectedIndexProperty"/>.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> that raised the event.</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> containing event data.</param>
        private static void SelectedIndexChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SideBar sideBar = sender as SideBar;

            if (sideBar != null)
                sideBar.InitializeSideBarSections();
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

            // Gets a reference to the element in xaml
            _PARTSideBarMinimizedSections = GetTemplateChild(PART_SIDEBAR_MINIMIZED_SECTIONS) as FrameworkElement;
            _PARTOverflowMenu = GetTemplateChild(PART_SIDEBAR_OVERFLOW_MENU) as Popup;
            _PARTSideBarCommon = GetTemplateChild(PART_SIDEBAR_COMMON) as FrameworkElement;
            _PARTCollapsedContentPopup = GetTemplateChild(PART_SIDEBAR_COLLAPSED_CONTENT_POPUP) as Popup;
            _PARTCollapsedContentButton = GetTemplateChild(PART_SIDEBAR_COLLAPSED_CONTENT_BUTTON) as ToggleButton;

            if(_PARTCollapsedContentPopup != null)
            {
                _PARTCollapsedContentPopup.Opened += CollapsedContentPopupOpened;
                _PARTCollapsedContentPopup.Closed += CollapsedContentPopupClosed;
            }

            if(_PARTCollapsedContentButton != null)
            {
                _PARTCollapsedContentButton.PreviewMouseLeftButtonUp += CollapsedContentButtonMouseUpPreview;
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

            InitializeSideBarSections();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes the side bar sections.
        /// </summary>
        private void InitializeSideBarSections()
        {
            // Reset the sidebar section collections
            _MinimizedSideBarSections = new Collection<SideBarSection>();
            _MaximizedSideBarSections = new Collection<SideBarSection>();

            // Initialize the number of visible sections
            int maximizedSections = this.VisibleSections;
            int minimizedSections = GetMinimizedSectionsDisplayCount();

            // If the sum of minimized and maximized sections is smaller than the total number of sections, the overflow menu button is visible
            IsOverflowMenuButtonVisible = (maximizedSections + minimizedSections) < this.Sections.Count;

            // Loop through all side bar sections
            for (int i = 0; i < _SideBarSections.Count; i++)
            {
                SideBarSection section = _SideBarSections[i];

                // Set common section properties
                section.SideBar = this;
                section.Height = SIDEBAR_SECTION_HEIGHT;

                if(maximizedSections > 0)
                {
                    section.IsMaximized = true;

                    // Add the section to the maximized section collection
                    _MaximizedSideBarSections.Add(section);

                    maximizedSections--;
                }
                else
                {
                    section.IsMaximized = false;

                    if (this._MinimizedSideBarSections.Count < minimizedSections)
                    {
                        // Add the section to the minimized section collection
                        _MinimizedSideBarSections.Add(section);
                    }
                }

                // Sets the selected property
                section.IsSelected = i == this.SelectedIndex;

                if (section.IsSelected)
                    this.SelectedSection = section;
            }

            // Update the side bar section collections
            SetValue(MaximizedSectionsPropertyKey, _MaximizedSideBarSections);
            SetValue(MinimizedSectionsPropertyKey, _MinimizedSideBarSections);
        }

        /// <summary>
        /// Initializes the overflow menu.
        /// </summary>
        private void InitializeOverflowMenu()
        {
            Collection<SideBarSection> overflowMenuItems = new Collection<SideBarSection>();

            if(_OverflowMenuItems.Count > 0)
            {
                foreach(SideBarSection item in _OverflowMenuItems)
                {
                    overflowMenuItems.Add(item);
                }
            }

            int visibleItems = _MaximizedSideBarSections.Count + (this.IsExpanded ? _MinimizedSideBarSections.Count : 0);

            for (int i = visibleItems; i < _SideBarSections.Count; i++)
            {
                SideBarSection section = _SideBarSections[i];

                section.Clicked += new RoutedEventHandler(MenuItemClickedEventHandler);
                section.IsMaximized = true;
                overflowMenuItems.Add(section);
            }

            SetValue(OverflowMenuItemsPropertyKey, overflowMenuItems);
        }

        /// <summary>
        /// Calculates the maximum number of minimized <see cref="SideBarSection"/>s that can be displayed.
        /// </summary>
        /// <returns>An <see cref="int"/> containing the number of minimized <see cref="SideBarSection"/>s that can be displayed.</returns>
        private int GetMinimizedSectionsDisplayCount()
        {
            if (_PARTSideBarMinimizedSections == null)
                return 0;
            
            return (int)Math.Floor((_PARTSideBarMinimizedSections.ActualWidth
                                    - (SelectedSection.IsMaximized ? 0 : SIDEBAR_SECTION_INDICATOR_WIDTH) 
                                    - SIDEBAR_SECTION_MINIMIZED_DIMENSIONS) 
                                    / SIDEBAR_SECTION_MINIMIZED_DIMENSIONS);
        }

        /// <summary>
        /// Calculates the minimum height of the side bar sections area to show all visible <see cref="SideBarSection"/>s.
        /// </summary>
        /// <returns></returns>
        private int GetMinimumSideBarSectionsAreaHeight()
        {
            return (int)(_MaximizedSideBarSections.Count * SIDEBAR_SECTION_HEIGHT) + 
                   (int)(_MinimizedSideBarSections.Count > 0 ? (1 * SIDEBAR_SECTION_MINIMIZED_DIMENSIONS) : 0) + 
                   (int)SIDEBAR_SECTION_INDICATOR_WIDTH * 2 +
                   (int)SIDEBAR_SECTION_HEIGHT;
        }

        /// <summary>
        /// Invalidates the content to be visible after the side bar expandend state is changed.
        /// </summary>
        /// <remarks><i>Ensures visibility of content when a <see cref="SideBarSection"/> is selected while the <see cref="SideBar"/> is collapsed.</i></remarks>
        private void InvalidateContentVisibility()
        {
            // Store the selected section content temporary
            object tempContent = SelectedSection != null ? SelectedSection.Content : null;

            // Reset the section content
            SectionContent = null;

            // Reassign the section content
            CollapsedContent = IsExpanded ? null : tempContent;
            SectionContent = IsExpanded ? tempContent : null;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="SideBarSection.Clicked"/> event.
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

    /// <summary>
    /// 
    /// </summary>
    public class SideBarSection : HeaderedContentControl
    {
        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="SideBarSection"/>.
        /// </summary>
        static SideBarSection()
        {
            // Overrides the default style of the inherited HeaderedContentControl to use the SideBarSection style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SideBarSection), new FrameworkPropertyMetadata(typeof(SideBarSection)));
        }

        /// <summary>
        /// Creates and initializes a new <see cref="SideBarSection"/> control.
        /// </summary>
        public SideBarSection() : base()
        {
            // Add a mouse click event listener to the sidebar section
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SideBarSectionClicked));
        }
        
        #endregion

        #region Routed Events

        /// <summary>
        /// Registers the event to raise when the <see cref="SideBarSection"/> is clicked.
        /// </summary>
        public static readonly RoutedEvent SideBarSectionClickRoutedEvent = EventManager.RegisterRoutedEvent(nameof(SideBarSectionClickRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SideBarSection));

        /// <summary>
        /// Defines the event raised when the <see cref="SideBarSection"/> is clicked.
        /// </summary>
        public event RoutedEventHandler Clicked
        {
            add { AddHandler(SideBarSectionClickRoutedEvent, value); }
            remove { RemoveHandler(SideBarSectionClickRoutedEvent, value); }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Registers the property to check if the <see cref="SideBarSection"/> is maximized.
        /// </summary>
        public static readonly DependencyProperty IsMaximizedProperty = DependencyProperty.Register(nameof(IsMaximized), typeof(bool), typeof(SideBarSection), new UIPropertyMetadata(true));

        /// <summary>
        /// Registers the property to check if the <see cref="SideBarSection"/> is selected.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SideBarSection), new UIPropertyMetadata(false, IsSelectedChangedCallback));

        /// <summary>
        /// Registers the property to set the <see cref="Image"/> of the <see cref="SideBarSection"/>.
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(SideBarSection), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Geometry"/> data for the <see cref="SideBarSection"/> button image.
        /// </summary>
        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register(nameof(Vector), typeof(Geometry), typeof(SideBarSection), new UIPropertyMetadata(null));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a reference to the parent of the <see cref="SideBarSection"/>.
        /// </summary>
        internal SideBar SideBar { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="SideBarSection"/> is maximized.
        /// </summary>
        public bool IsMaximized
        {
            get { return (bool)GetValue(IsMaximizedProperty); }
            set { SetValue(IsMaximizedProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="SideBarSection"/> is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="ImageSource"/> of the <see cref="SideBarSection"/>'s image.
        /// </summary>
        /// <remarks><i>The <see cref="Image"/> property has priorty over the <see cref="Vector"/> property.</i></remarks>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vector <see cref="Geometry"/> of the <see cref="SideBarSection"/>'s image.
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
            SideBarSection section = (SideBarSection)sender;
            if((bool)e.NewValue)
            {
                section.SideBar.SelectedSection = section;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="Clicked"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> containing event data.</param>
        private void SideBarSectionClicked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = e.OriginalSource as ToggleButton;

            if (button != null)
                button.IsChecked = true;

            if (SideBar != null)
                SideBar.SelectedSection = this;

            RaiseEvent(new RoutedEventArgs(SideBarSectionClickRoutedEvent));
        }

      

        #endregion
    }

    //    / <summary>
    //    / 
    //    / </summary>
    //    public class SideBarCommonSection : HeaderedContentControl
    //    {
    //        #region Constructor

    //        / <summary>
    //        / Static constructor called before initializing an instance of<see cref= "SideBarSection" />.
    //        / </ summary >
    //        static SideBarCommonSection()
    //        {
    //            Overrides the default style of the inherited HeaderedContentControl to use the SideBarSection style instead.
    //           DefaultStyleKeyProperty.OverrideMetadata(typeof(SideBarSection), new FrameworkPropertyMetadata(typeof(SideBarSection)));
    //        }

    //        / <summary>
    //        / Creates and initializes a new <see cref = "SideBarSection" /> control.
    //        / </ summary >
    //        public SideBarCommonSection() : base()
    //        {
    //            Add a mouse click event listener to the sidebar section
    //           AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SideBarCommonSectionClicked));
    //        }

    //        #endregion

    //        #region Routed Events

    //        / <summary>
    //        / Registers the event to raise when the<see cref="SideBarSection"/> is clicked.
    //        / </summary>
    //        public static readonly RoutedEvent SideBarCommonSectionClickRoutedEvent = EventManager.RegisterRoutedEvent(nameof(SideBarCommonSectionClickRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SideBarSection));

    //        / <summary>
    //        / Defines the event raised when the<see cref="SideBarSection"/> is clicked.
    //        / </summary>
    //        public event RoutedEventHandler Clicked
    //    {
    //        add { AddHandler(SideBarCommonSectionClickRoutedEvent, value); }
    //        remove { RemoveHandler(SideBarCommonSectionClickRoutedEvent, value); }
    //    }

    //        #endregion

    //        #region Dependency Properties

    //        / <summary>
    //        / Registers the property to set the<see cref="Image"/> of the <see cref = "SideBarSection" />.
    //        / </ summary >
    //        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(SideBarSection), new UIPropertyMetadata(null));

    //        / <summary>
    //        / Gets or sets the<see cref="Geometry"/> data for the<see cref="SideBarSection"/> button image.
    //        / </summary>
    //        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register(nameof(Vector), typeof(Geometry), typeof(SideBarSection), new UIPropertyMetadata(null));

    //        #endregion

    //        #region Properties

    //        / <summary>
    //        / Gets or sets a reference to the parent of the<see cref="SideBarSection"/>.
    //        / </summary>
    //        internal SideBar SideBar { get; set; }

    //        / <summary>
    //        / Gets or sets the<see cref="ImageSource"/> of the <see cref = "SideBarSection" /> 's image.
    //        / </ summary >
    //        / < remarks >< i > The < see cref= "Image" /> property has priorty over the <see cref = "Vector" /> property.</ i ></ remarks >
    //        public ImageSource Image
    //    {
    //        get { return (ImageSource)GetValue(ImageProperty); }
    //        set { SetValue(ImageProperty, value); }
    //    }

    //        / <summary>
    //        / Gets or sets the vector<see cref="Geometry"/> of the<see cref="SideBarSection"/>'s image.
    //        / </summary>
    //        / <remarks><i>The<see cref="Image"/> property has priorty over the<see cref="Vector"/> property.</i></remarks>
    //        public Geometry Vector
    //    {
    //        get { return (Geometry)GetValue(VectorProperty); }
    //        set { SetValue(VectorProperty, value); }
    //    }

    //        #endregion

    //        #region Event Handlers

    //        / <summary>
    //        / Handles the<see cref="Clicked"/> event.
    //        / </summary>
    //        / <param name = "sender" > The < see cref= "object" /> that raised the event.</param>
    //        / <param name = "e" > A < see cref= "RoutedEventArgs" /> containing event data.</param>
    //        private void SideBarCommonSectionClicked(object sender, RoutedEventArgs e)
    //    {
    //        ToggleButton button = e.OriginalSource as ToggleButton;


    //        RaiseEvent(new RoutedEventArgs(SideBarCommonSectionClickRoutedEvent));
    //    }

    //    #endregion
    //}
}
