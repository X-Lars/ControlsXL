using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    [Flags]
    public enum ControlAppearance
    {
        Flat = 0,
        Default = 1,
        Strong  = 2,
        Rounded   = 4,
        Raised  = 8,
        Shadowed  = 16

    }

    internal static class Styles 
    {
        internal const string BorderThickness = "BorderThickness";

        internal const string ControlBrush = "ControlBrush";
    }
}
