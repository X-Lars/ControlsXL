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
    /// Defines the available control styles.
    /// </summary>
    /// <remarks><i><b>IMPORTANT: </b> The name of the enumeration values are use in the <see cref="ResourceDictionary.Source"/> uri and have to match the file names in the styles folder.</i></remarks>
    public enum ControlStyle
    {
        Default,
        Dark
    }

    /// <summary>
    /// Defines the available control appearances.
    /// </summary>
    /// <remarks><i><b>IMPORTANT: </b> The name of the enumeration values are use in the <see cref="ResourceDictionary.Source"/> uri and have to match the file names in the appearances folder.</i></remarks>
    public enum ControlAppearance
    {
        Default,
        Flat
    }

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
        /// Applies the currently selected skin.
        /// </summary>
        private static void ApplySkinStyle()
        {
            Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

            dictionaries.Remove(_CurrentStyle);

            _CurrentStyle.Source = new Uri($"/ControlsXL;Component/Styles/{Style}.xaml", UriKind.Relative);

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

            _CurrentAppearance.Source = new Uri($"/ControlsXL;Component/Appearances/{Appearance}.xaml", UriKind.Relative);

            dictionaries.Add(_CurrentAppearance);

            // Raise the appearance changed event
            if (AppearanceChanged != null)
            {
                AppearanceChanged(null, EventArgs.Empty);
            }
        }

        #endregion
    }
}
