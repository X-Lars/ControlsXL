using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlsXL
{
    /// <summary>
    /// Defines the available skins.
    /// </summary>
    /// <remarks><i><b>IMPORTANT: </b> The name of the enumeration values are use in the <see cref="ResourceDictionary.Source"/> uri and have to match the file names in the skins folder.</i></remarks>
    public enum SkinStyles
    {
        Default,
        Dark
    }

    /// <summary>
    /// Defines the available appearances.
    /// </summary>
    /// <remarks><i><b>IMPORTANT: </b> The name of the enumeration values are use in the <see cref="ResourceDictionary.Source"/> uri and have to match the file names in the appearance folder.</i></remarks>
    public enum Appearances
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
        /// Stores the applied skin.
        /// </summary>
        private static SkinStyles _SkinStyle = SkinStyles.Default;

        /// <summary>
        /// Stores the applied appearance.
        /// </summary>
        private static Appearances _Appearance = Appearances.Default;

        /// <summary>
        /// Stores the <see cref="ResourceDictionary"/> containing the dictionary keys for the current skin.
        /// </summary>
        private static ResourceDictionary _CurrentSkin = new ResourceDictionary();

        /// <summary>
        /// Stores the <see cref="ResourceDictionary"/> containing the dictionary keys for the current appearance.
        /// </summary>
        private static ResourceDictionary _CurrentAppearance = new ResourceDictionary();

        #endregion

        #region Events

        /// <summary>
        /// Defines the event to raise when the applied skin style is changed.
        /// </summary>
        public static event EventHandler SkinChanged;

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
        /// Gets or sets the skin to apply.
        /// </summary>
        public static SkinStyles SkinStyle
        {
            get { return _SkinStyle; }
            set
            {
                if (_SkinStyle != value)
                {
                    _SkinStyle = value;
                    ApplySkinStyle();
                }
            }
        }

        /// <summary>
        /// Gets or sets the appearance to apply.
        /// </summary>
        public static Appearances Appearance
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

            dictionaries.Remove(_CurrentSkin);

            _CurrentSkin.Source = new Uri($"/ControlsXL;Component/Skins/{SkinStyle}.xaml", UriKind.Relative);

            dictionaries.Add(_CurrentSkin);            

            // Raise the skin changed event
            if (SkinChanged != null)
            {
                SkinChanged(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Applies the currently selected appearance.
        /// </summary>
        private static void ApplyAppearance()
        {
            Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

            dictionaries.Remove(_CurrentAppearance);

            _CurrentAppearance.Source = new Uri($"/ControlsXL;Component/Appearance/{Appearance}.xaml", UriKind.Relative);

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
