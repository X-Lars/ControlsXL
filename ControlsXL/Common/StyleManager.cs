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
        #region Constants

        /// <summary>
        /// Defines the base uri for all resources.
        /// </summary>
        private const string _BaseUri = "/ControlsXL;Component/Resources/";

        /// <summary>
        /// Defines the uri for all appearances.
        /// </summary>
        /// <remarks><i>Use a string format to inject the filename. <code>string.format(_AppearanceUri, <filename>)</code></i></remarks>
        private const string _AppearancesUri = _BaseUri + "Appearances/{0}.xaml";

        /// <summary>
        /// Defines the uri for all styles.
        /// </summary>
        /// <remarks><i>Use a string format to inject the filename. <code>string.format(_AppearanceUri, <filename>)</code></i></remarks>
        private const string _StylesUri = _BaseUri + "Styles/{0}.xaml";

        #endregion

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
        private static Dictionary<ControlAppearance, ResourceDictionary> _CurrentAppearances = new Dictionary<ControlAppearance, ResourceDictionary>();
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
                    InvalidateAppearance();
                    ApplyAppearance();
                }
            }
        }

        public static void InvalidateAppearance()
        {
            if(_Appearance.HasFlag(ControlAppearance.Default) && _Appearance.HasFlag(ControlAppearance.Strong))
            {
                // Toggle default off
                _Appearance ^= ControlAppearance.Default;
            }
        }

        public static void RemoveAppearance(ControlAppearance appearance)
        {
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

            _CurrentStyle.Source = new Uri(string.Format(_StylesUri, Style), UriKind.Relative);

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

            Array appearances = Enum.GetValues(typeof(ControlAppearance));

            // Remove all current appearance dictionaries
            foreach(ResourceDictionary dictionary in _CurrentAppearances.Values)
            {
                dictionaries.Remove(dictionary);
            }

            _CurrentAppearances.Clear();

            foreach (ControlAppearance appearance in appearances)
            {
                if ((_Appearance & appearance) == appearance)
                {
                    switch (appearance)
                    {
                        case ControlAppearance.Flat:
                            _CurrentAppearances.Add(ControlAppearance.Flat, new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Flat), UriKind.Relative) });
                            break;
                        case ControlAppearance.Default:
                            _CurrentAppearances.Add(ControlAppearance.Default, new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Default), UriKind.Relative) });
                            break;
                        case ControlAppearance.Strong:
                            _CurrentAppearances.Add(ControlAppearance.Strong, new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Strong), UriKind.Relative) });
                            break;
                        case ControlAppearance.Round:
                            _CurrentAppearances.Add(ControlAppearance.Round, new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Round), UriKind.Relative) });
                            break;
                    }
                }
            }

            foreach(ResourceDictionary dictionary in _CurrentAppearances.Values)
            {
                dictionaries.Add(dictionary);
            }

            // Raise the appearance changed event
            if (AppearanceChanged != null)
            {
                AppearanceChanged(null, EventArgs.Empty);
            }

            //dictionaries.Remove(_CurrentAppearance);

            //_CurrentAppearance.Source = new Uri($"/ControlsXL;Component/Resources/Appearances/{Appearance}.xaml", UriKind.Relative);

            //dictionaries.Add(_CurrentAppearance);

            //// Raise the appearance changed event
            //if (AppearanceChanged != null)
            //{
            //    AppearanceChanged(null, EventArgs.Empty);
            //}

            // Remove?
            //Application.Current.MainWindow.InvalidateArrange();
            //Application.Current.MainWindow.InvalidateMeasure();
            //Application.Current.MainWindow.InvalidateVisual();

        }

        #endregion
    }
}
