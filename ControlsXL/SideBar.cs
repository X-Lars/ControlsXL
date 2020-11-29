using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public class SideBar : HeaderedItemsControl
    {
        

        #region Constants

        /// <summary>
        /// The default height and minimum width when minimized of <see cref="SideBarSection"/>s.
        /// </summary>
        private const double DEFAULT_SECTION_SQUARE_DIMENSIONS = 32;

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
            this._SideBarSections = new ObservableCollection<SideBarSection>();

            this._MinimizedSideBarSections = new Collection<SideBarSection>();
            this._MaximizedSideBarSections = new Collection<SideBarSection>();

            // Bind the side bar commands
            CommandBindings.Add(new CommandBinding(_RoutedUICommandResizeSideBar, OnResizeSideBar));
            CommandBindings.Add(new CommandBinding(_RoutedUICommandResizeSections, OnResizeSections));
        }

        #endregion

        

        #region Properties

        /// <summary>
        /// Registers the property to set the dock position of the side bar.
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty = DependencyProperty.Register("DockPosition", typeof(SideBarDockPositions), typeof(SideBar), new UIPropertyMetadata(SideBarDockPositions.Left));

        /// <summary>
        /// Gets or sets the dock position of the side bar.
        /// </summary>
        /// <remarks><i>Only left and right docking is supported.</i></remarks>
        public SideBarDockPositions DockPosition
        {
            get { return (SideBarDockPositions)GetValue(DockPositionProperty); }
            set { SetValue(DockPositionProperty, value); }
        }

        #endregion

        #region Properties

        #endregion

        #region Commands

        /// <summary>
        /// Defines the command to resize the side bar.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandResizeSideBar = new RoutedUICommand("ResizeSideBar", "ResizeSideBarCommand", typeof(SideBar));

        /// <summary>
        /// Defines the command to resize the side bar sections.
        /// </summary>
        private static RoutedUICommand _RoutedUICommandResizeSections = new RoutedUICommand("ResizeSections", "ResizeSectionsCommand", typeof(SideBar));

        /// <summary>
        /// Gets the command to resize the side bar.
        /// </summary>
        public static RoutedUICommand ResizeSideBarCommand { get { return _RoutedUICommandResizeSideBar; } }

        /// <summary>
        /// Gets the command to resize the sections.
        /// </summary>
        public static RoutedUICommand ResizeSectionsCommand { get { return _RoutedUICommandResizeSections; } }

        #endregion

        #region Command Event Handlers

        /// <summary>
        /// Handles the resize side bar command.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnResizeSideBar(object sender, ExecutedRoutedEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if (control != null)
            {
                // Attach the mouse up button event handler to the control that started the resize be notified when resizing has been done
                control.PreviewMouseUp += new MouseButtonEventHandler(RemoveResizeEventListeners);
            }

            // Attach the mouse move event handler to the side bar
            this.PreviewMouseMove += new MouseEventHandler(ResizeSideBarEventListener);
        }

        /// <summary>
        /// Handles the resize sections command.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> containing event data.</param>
        private void OnResizeSections(object sender, ExecutedRoutedEventArgs e)
        {
            Control control = e.OriginalSource as Control;

            if (control != null)
            {
                // Attach the mouse up button event handler to the control that started the resize be notified when resizing has been done
                control.PreviewMouseUp += new MouseButtonEventHandler(RemoveResizeEventListeners);
            }

            // Attach the mouse move event handler to the side bar
            this.PreviewMouseMove += new MouseEventHandler(ResizeSectionsEventListener);
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
            this.PreviewMouseMove -= ResizeSideBarEventListener;
            this.PreviewMouseMove -= ResizeSectionsEventListener;
        }

        /// <summary>
        /// Handles the resize side bar command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeSideBarEventListener(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                // TODO: Resize function
            }
            else
            {
                // Remove the mouse move event handler from the side bar
                this.PreviewMouseMove -= ResizeSideBarEventListener;
            }
        }

        /// <summary>
        /// Handles the resize section command mouse movement.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that raised the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> containing event data.</param>
        private void ResizeSectionsEventListener(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                // TODO: Resize sections function
            }
            else
            {
                // Remove the mouse move event handler from the side bar
                this.PreviewMouseMove -= ResizeSectionsEventListener;
            }
        }


        #endregion

        #region Event Handlers

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class SideBarSection : HeaderedContentControl
    {
        #region Fields
        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="SideBarSection"/>.
        /// </summary>
        static SideBarSection()
        {
            // Overrides the default style of the inherited HeaderedContentControl to use the SideBarSection style instead.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SideBarSection), new FrameworkPropertyMetadata(typeof(SideBarSection)));
        }

        public SideBarSection() : base()
        {
        }

        #endregion
    }
}
