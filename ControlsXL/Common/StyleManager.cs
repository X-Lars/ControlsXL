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
    public enum SkinStyles
    {
        Default,
        Dark
    }

    /// <summary>
    /// Defines the available appearnces.
    /// </summary>
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
        /// Stores the <see cref="SkinStyles.Default"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _DefaultSkin;

        /// <summary>
        /// Stores the <see cref="SkinStyles.Dark"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _DarkSkin;

        /// <summary>
        /// Stores the <see cref="Appearances.Default"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _DefaultAppearance;

        /// <summary>
        /// Stores the <see cref="Appearances.Flat"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _FlatAppearance;

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

        #region Properties

        /// <summary>
        /// Gets or sets the skin to apply.
        /// </summary>
        public static SkinStyles SkinStyle
        {
            get { return _SkinStyle; }
            set
            {
                
                    _SkinStyle = value;
                    ApplySkinStyle();
                
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
                
                    _Appearance = value;
                    ApplyAppearance();
                
            }
        }
        
        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="SkinStyles.Default"/> skin.
        /// </summary>
        private static ResourceDictionary DefaultSkin 
        { 
            get
            {
                if(_DefaultSkin  == null)
                {
                    _DefaultSkin = new ResourceDictionary();
                    _DefaultSkin.Source = new Uri("/ControlsXL;Component/Skins/Default.xaml", UriKind.Relative);
                }

                return _DefaultSkin;
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="SkinStyles.Dark"/> skin.
        /// </summary>
        private static ResourceDictionary DarkSkin
        {
            get
            {
                if(_DarkSkin == null)
                {
                    _DarkSkin = new ResourceDictionary();
                    _DarkSkin.Source = new Uri("/ControlsXL;Component/Skins/Dark.xaml", UriKind.Relative);
                }

                return _DarkSkin;
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="Appearances.Default"/> appearance.
        /// </summary>
        private static ResourceDictionary DefaultAppearance
        {
            get
            {
                if(_DefaultAppearance == null)
                {
                    _DefaultAppearance = new ResourceDictionary();
                    _DefaultAppearance.Source = new Uri("/ControlsXL;Component/Appearance/Default.xaml", UriKind.Relative);
                }

                return _DefaultAppearance;
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="Appearances.Flat"/> appearance.
        /// </summary>
        private static ResourceDictionary FlatAppearance
        {
            get
            {
                if (_FlatAppearance == null)
                {
                    _FlatAppearance = new ResourceDictionary();
                    _FlatAppearance.Source = new Uri("/ControlsXL;Component/Appearance/Flat.xaml", UriKind.Relative);
                }

                return _FlatAppearance;
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

            dictionaries.Remove(DefaultSkin);
            dictionaries.Remove(DarkSkin);

            switch (_SkinStyle)
            {
                case SkinStyles.Default:
                    dictionaries.Add(DefaultSkin);
                    break;

                case SkinStyles.Dark:
                    dictionaries.Add(DarkSkin);
                    break;
            }

            // Raise the skin changed event
            if(SkinChanged != null)
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

            dictionaries.Remove(DefaultAppearance);
            dictionaries.Remove(FlatAppearance);

            switch(_Appearance)
            {
                case Appearances.Default:
                    dictionaries.Add(DefaultAppearance);
                    break;

                case Appearances.Flat:
                    dictionaries.Add(FlatAppearance);
                    break;
            }

            // Raise the appearance changed event
            if(AppearanceChanged != null)
            {
                AppearanceChanged(null, EventArgs.Empty);
            }
        }

        #endregion
    }
}
