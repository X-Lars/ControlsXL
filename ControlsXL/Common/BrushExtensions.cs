using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace ControlsXL
{
    public static class BrushExtensions
    {
        public static Brush Shade(this Brush instance, double factor = 0.5)
        {
            SolidColorBrush brush = (SolidColorBrush)(instance);

            factor = Math.Min(factor, 1);
            factor = Math.Max(factor, 0);

            Color color = Color.FromArgb(brush.Color.A,
                                  (byte)(brush.Color.R * (1 - factor)),
                                  (byte)(brush.Color.G * (1 - factor)),
                                  (byte)(brush.Color.B * (1 - factor)));

            return new SolidColorBrush(color);
        }

        public static Brush Tint(this Brush instance, double factor = 0.5)
        {
            SolidColorBrush brush = (SolidColorBrush)instance;

            factor = Math.Min(factor, 1);
            factor = Math.Max(factor, 0);

            Color color = Color.FromArgb(brush.Color.A,
                                  (byte)(brush.Color.R + (255 - brush.Color.R) * factor),
                                  (byte)(brush.Color.G + (255 - brush.Color.G) * factor),
                                  (byte)(brush.Color.B + (255 - brush.Color.B) * factor));

            return new SolidColorBrush(color);
        }

        public static Brush Alpha(this Brush instance, double factor = 0.5)
        {
            SolidColorBrush brush = (SolidColorBrush)instance;

            factor = Math.Min(factor, 1);
            factor = Math.Max(factor, 0);

            Color color = Color.FromArgb((byte)(factor * 255), brush.Color.R, brush.Color.G, brush.Color.B);

            return new SolidColorBrush(color);
        }
    }
}
