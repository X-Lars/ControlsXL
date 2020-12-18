using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Stores the list of resource dictionaries containing the dictonary keys for all current appearances.
        /// </summary>
        private static List<ResourceDictionary> _CurrentAppearances = new List<ResourceDictionary>();

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
        static StyleManager() { }

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
            foreach(ResourceDictionary dictionary in _CurrentAppearances)
            {
                dictionaries.Remove(dictionary);
            }

            _CurrentAppearances.Clear();

            // Create a resource dictionary for each control appearance flag
            foreach (ControlAppearance appearance in appearances)
            {
                if ((_Appearance & appearance) == appearance)
                {
                    switch (appearance)
                    {
                        case ControlAppearance.Flat:
                            _CurrentAppearances.Add(new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Flat), UriKind.Relative) });
                            break;
                        case ControlAppearance.Default:
                            _CurrentAppearances.Add(new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Default), UriKind.Relative) });
                            break;
                        case ControlAppearance.Strong:
                            _CurrentAppearances.Add(new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Strong), UriKind.Relative) });
                            break;
                        case ControlAppearance.Rounded:
                            _CurrentAppearances.Add(new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Rounded), UriKind.Relative) });
                            break;
                        case ControlAppearance.Raised:
                            _CurrentAppearances.Add(new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Raised), UriKind.Relative) });
                            break;
                        case ControlAppearance.Shadowed:
                            _CurrentAppearances.Add(new ResourceDictionary() { Source = new Uri(string.Format(_AppearancesUri, ControlAppearance.Shadowed), UriKind.Relative) });
                            break;
                    }
                }
            }

            // Add all dictionaries to the application's merged dictionaries
            foreach(ResourceDictionary dictionary in _CurrentAppearances)
            {
                dictionaries.Add(dictionary);
            }

            // Raise the appearance changed event
            if (AppearanceChanged != null)
            {
                AppearanceChanged(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Invalidates the current appearance by removing colliding appearances.
        /// </summary>
        /// <remarks><i>Remove colliding appearances using XOR assignment operator.</i></remarks>
        private static void InvalidateAppearance()
        {
            // Strong ovveride default
            if (_Appearance.HasFlag(ControlAppearance.Default) && _Appearance.HasFlag(ControlAppearance.Strong))
            {
                // Toggle default off
                _Appearance ^= ControlAppearance.Default;
            }
        }

        #endregion
    }
}
