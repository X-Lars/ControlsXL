using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlsXL
{
    /// <summary>
    /// Manages the appearance of the custom controls at runtime.
    /// </summary>
    public static class StyleManager
    {
        #region Fields

        /// <summary>
        /// Stores the applied style.
        /// </summary>
        private static ControlStyle _Style = ControlStyle.Default;

        /// <summary>
        /// Stores the applied appearance.
        /// </summary>
        private static ControlAppearance _Appearance = ControlAppearance.Default;

        /// <summary>
        /// Stores the <see cref="ResourceDictionary"/> containing the dictionary keys for the current style.
        /// </summary>
        private static ResourceDictionary _CurrentStyle = new ResourceDictionary();

        /// <summary>
        /// Stores the <see cref="ResourceDictionary"/> containing the dictionary keys for the current appearance.
        /// </summary>
        private static ResourceDictionary _CurrentAppearance = new ResourceDictionary();

        #endregion

        #region Events

        /// <summary>
        /// Defines the event to raise when the applied style is changed.
        /// </summary>
        public static event EventHandler StyleChanged;

        /// <summary>
        /// Defines the event to raise when the applied appearance is changed.
        /// </summary>
        public static event EventHandler AppearanceChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor called before the instance of <see cref="StyleManager"/> is created.
        /// </summary>
        static StyleManager()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the style to apply.
        /// </summary>
        public static ControlStyle Style
        {
            get { return _Style; }
            set
            {
                if (_Style != value)
                {
                    _Style = value;
                    ApplySkinStyle();
                }
            }
        }

        /// <summary>
        /// Gets or sets the appearance to apply.
        /// </summary>
        public static ControlAppearance Appearance
        {
            get { return _Appearance; }
            set
            {
                if (_Appearance != value)
                {
                    _Appearance = value;
                    ApplyAppearance();
                }
            }
        }
   
        #endregion

        #region Methods

        /// <summary>
        /// Gets the value of the specified component resource from the current application runtime resources.
        /// </summary>
        /// <param name="ID">A <see cref="string"/> specifying the component resource key ID.</param>
        /// <returns>A <see cref="object"/> containing the requested component's value.</returns>
        internal static object GetStyleValue(string ID)
        {
            return Application.Current.FindResource(new ComponentResourceKey(typeof(Styles), ID));
        }

        /// <summary>
        /// Applies the currently selected skin.
        /// </summary>
        private static void ApplySkinStyle()
        {
            Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

            dictionaries.Remove(_CurrentStyle);

            _CurrentStyle.Source = new Uri($"/ControlsXL;Component/Resources/Styles/{Style}.xaml", UriKind.Relative);

            dictionaries.Add(_CurrentStyle);            

            // Raise the skin changed event
            if (StyleChanged != null)
            {
                StyleChanged(null, EventArgs.Empty);
            }

        }

        /// <summary>
        /// Applies the currently selected appearance.
        /// </summary>
        private static void ApplyAppearance()
        {
            Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

            dictionaries.Remove(_CurrentAppearance);

            _CurrentAppearance.Source = new Uri($"/ControlsXL;Component/Resources/Appearances/{Appearance}.xaml", UriKind.Relative);

            dictionaries.Add(_CurrentAppearance);

            // Raise the appearance changed event
            if (AppearanceChanged != null)
            {
                AppearanceChanged(null, EventArgs.Empty);
            }

            Application.Current.MainWindow.InvalidateArrange();
            Application.Current.MainWindow.InvalidateMeasure();
            Application.Current.MainWindow.InvalidateVisual();

        }

        #endregion
    }
}
