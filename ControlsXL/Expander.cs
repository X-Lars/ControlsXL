using System;
using System.Collections.Generic;
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
   

    public class Expander : HeaderedContentControl
    {
        #region Constructor

        static Expander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Expander), new FrameworkPropertyMetadata(typeof(Expander)));
        }

        public Expander()
        {
            CommandBindings.Add(new CommandBinding(ToggleStateCommand, OnToggleStateCommand));
        }

        
        #endregion

        #region Commands

        #region Commands: Registration

        private static RoutedUICommand _ToggleStateCommand = new RoutedUICommand(nameof(ToggleStateCommand), nameof(ToggleStateCommand), typeof(Expander));



        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderAlignmentProperty); }
            set { SetValue(HeaderAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderAlignmentProperty = DependencyProperty.Register(nameof(HeaderAlignment), typeof(HorizontalAlignment), typeof(Expander), new PropertyMetadata(HorizontalAlignment.Left));



        public HorizontalAlignment HeaderButtonAlignment
        {
            get { return (HorizontalAlignment)GetValue(ButtonAlignmentProperty); }
            set { SetValue(ButtonAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonAlignmentProperty = DependencyProperty.Register(nameof(HeaderButtonAlignment), typeof(HorizontalAlignment), typeof(Expander), new PropertyMetadata(HorizontalAlignment.Right));




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
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Expander), new PropertyMetadata(true));


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
