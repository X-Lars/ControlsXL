using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ControlsXL.Common
{
    public static class ColorExtensions
    {
        public static double Brightness(this Color color)
        {
            return Math.Sqrt(color.R * color.R * .241 +
                             color.G * color.G * .691 +
                             color.B * color.B * .068)/ 255;
        }
    }
}
