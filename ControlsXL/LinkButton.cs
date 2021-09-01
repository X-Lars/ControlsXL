using System.Windows;
using System.Windows.Controls;
using StylesXL;

namespace ControlsXL
{
    /// <summary>
    /// A plain text button with an underline when the mouse hovers over the button.
    /// </summary>
    public class LinkButton : Button
    {
        /// <summary>
        /// Static constructor called before initializing an instance of <see cref="LinkButton"/>.
        /// </summary>
        static LinkButton()
        {
            // Overrides the default style of the inherited Button to use the LinkButton style instead
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkButton), new FrameworkPropertyMetadata(typeof(LinkButton)));

            // Requires style manager
            StyleManager.Initialize();
        }
    }
}
