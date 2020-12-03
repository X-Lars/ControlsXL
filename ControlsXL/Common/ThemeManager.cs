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
    public enum SkinID
    {
        Default,
        Dark
    }

    /// <summary>
    /// Manages the appearance of the custom controls at runtime.
    /// </summary>
    public static class ThemeManager
    {
        #region Fields

        /// <summary>
        /// Stores the applied skin ID.
        /// </summary>
        private static SkinID _SkinID = SkinID.Default;

        /// <summary>
        /// Stores the <see cref="SkinID.Default"/> <see cref="ResourceDictionary"/> path.
        /// </summary>
        private static ResourceDictionary _Default;

        /// <summary>
        /// Stores the <see cref="SkinID.Dark"/> <see cref="ResourceDictionary"/> path.
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
        public static SkinID SkinID
        {
            get { return _SkinID; }
            set
            {
                if(_SkinID != value)
                {
                    _SkinID = value;
                    ApplySkin();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="SkinID.Default"/> skin.
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
        /// Gets the <see cref="ResourceDictionary"/> containing the references of the custom control dictionaries for the <see cref="SkinID.Dark"/> skin.
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
        private static void ApplySkin()
        {
            Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

            dictionaries.Remove(Default);
            dictionaries.Remove(Dark);

            switch (_SkinID)
            {
                case SkinID.Default:
                    dictionaries.Add(Default);
                    break;

                case SkinID.Dark:
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
