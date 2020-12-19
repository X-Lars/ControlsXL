using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlsXL
{
    /// <summary>
    /// Provide data for dynamic resources at design time.
    /// </summary>
    public class DesignTimeResourceDictionary : ResourceDictionary
    {
        #region Fields

        /// <summary>
        /// Stores the <see cref="DesignTimeResourceDictionary"/> source uri.
        /// </summary>
        private Uri _DesignTimeSource;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the design time source.
        /// </summary>
        public Uri DesignTimeSource
        {
            get
            {
                return this._DesignTimeSource;
            }

            set
            {
                this._DesignTimeSource = value;

                // Prevents adding of the resource dictionary at runtime
                if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                {
                    base.Source = _DesignTimeSource;
                }
            }
        }

        /// <summary>
        /// Gets or sets the run uniform resource identifier (URI) to load resources from.
        /// </summary>
        /// <remarks><i>Overrides the base <see cref="ResourceDictionary.Source"/> property to force use of the <see cref="DesignTimeSource"/> property.</i></remarks>
        /// <exception cref="Exception">When the source property is used</exception>
        public new Uri Source
        {
            get
            {
                return this._DesignTimeSource;
            }

            set
            {
                this._DesignTimeSource = value;

                // Prevents adding of the resource dictionary at runtime
                if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                {
                    base.Source = _DesignTimeSource;
                }
            }
        }

        #endregion
    }
}
