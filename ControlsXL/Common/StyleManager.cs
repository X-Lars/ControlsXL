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
    /// Manages the appearance of the custom controls at runtime.
    /// </summary>
    public static class StyleManager
    {
        #region Fields

        /// <summary>
        /// Stores the applied skin ID.
        /// </summary>
        private static SkinStyles _SkinStyle = SkinStyles.Default;

        /// <summary>
        /// Stores the <see cref="SkinStyles.Default"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _Default;

        /// <summary>
        /// Stores the <see cref="SkinStyles.Dark"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _Dark;

        #endregion

        #region Events

        /// <summary>
        /// Defines the event to raise when the applied skin ID is changed.
        /// </summary>
        public static event EventHandler SkinChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the skin to apply.
        /// </summary>
        public static SkinStyles SkinStyle
        {
            get { return _SkinStyle; }
            set
            {
                if(_SkinStyle != value)
                {
                    _SkinStyle = value;
                    ApplySkinStyle();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="SkinStyles.Default"/> skin.
        /// </summary>
        private static ResourceDictionary Default 
        { 
            get
            {
                if(_Default  == null)
                {
                    _Default = new ResourceDictionary();
                    _Default.Source = new Uri("/ControlsXL;Component/Skins/Default.xaml", UriKind.Relative);
                }

                return _Default;
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="SkinStyles.Dark"/> skin.
        /// </summary>
        private static ResourceDictionary Dark
        {
            get
            {
                if(_Dark == null)
                {
                    _Dark = new ResourceDictionary();
                    _Dark.Source = new Uri("/ControlsXL;Component/Skins/Dark.xaml", UriKind.Relative);
                }

                return _Dark;
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Applies the currently selected skin ID.
        /// </summary>
        private static void ApplySkinStyle()
        {
            Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

            dictionaries.Remove(Default);
            dictionaries.Remove(Dark);

            switch (_SkinStyle)
            {
                case SkinStyles.Default:
                    dictionaries.Add(Default);
                    break;

                case SkinStyles.Dark:
                    dictionaries.Add(Dark);
                    break;
            }

            // Raise the skin changed event
            if(SkinChanged != null)
            {
                SkinChanged(null, EventArgs.Empty);
            }
        }
                
        #endregion
    }
}
