using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlsXL
{


    public class Expander : HeaderedContentControl
    {
        #region Constructor

        static Expander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlsXL.Expander), new FrameworkPropertyMetadata(typeof(ControlsXL.Expander)));
            StylesXL.StyleManager.Initialize();
            
        }

        public Expander() : base()
        {
            CommandBindings.Add(new CommandBinding(ToggleStateCommand, OnToggleStateCommand));
            
        }

        
        #endregion

        #region Commands

        #region Commands: Registration

        private static ICommand _ToggleStateCommand = new RoutedUICommand(nameof(ToggleStateCommand), nameof(ToggleStateCommand), typeof(ControlsXL.Expander));



        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderAlignmentProperty); }
            set { SetValue(HeaderAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderAlignmentProperty = DependencyProperty.Register(nameof(HeaderAlignment), typeof(HorizontalAlignment), typeof(ControlsXL.Expander), new PropertyMetadata(HorizontalAlignment.Left));



        
        public HorizontalAlignment HeaderButtonAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderButtonAlignmentProperty); }
            set { SetValue(HeaderButtonAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderButtonAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderButtonAlignmentProperty =
            DependencyProperty.Register(nameof(HeaderButtonAlignment), typeof(HorizontalAlignment), typeof(ControlsXL.Expander), new PropertyMetadata(HorizontalAlignment.Right));



        #endregion

        #region Commands: Properties

        public static ICommand ToggleStateCommand
        {
            get { return _ToggleStateCommand; }
        }

        #endregion

        #region Commands: Handlers

        private void OnToggleStateCommand(object sender, ExecutedRoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }


        #endregion

        #endregion

        #region Dependency Properties

        #region Dependency Properties: Registration

        /// <summary>
        /// Registers the property to set the <see cref="Expander"/> state.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ControlsXL.Expander), new PropertyMetadata(true));


        #endregion
        #region Dependency Properties: Implementation

        /// <summary>
        /// Gets or sets the <see cref="Expander"/> state.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion
        #endregion
    }
}
