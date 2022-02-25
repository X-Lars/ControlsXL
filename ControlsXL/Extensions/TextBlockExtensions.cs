using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlsXL.Extensions
{
    internal static class TextBlockExtensions
    {
        internal static double GetTextWidth(this TextBlock instance, int padding = 0)
        {
            var formattedText = new FormattedText(instance.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(instance.FontFamily, instance.FontStyle, instance.FontWeight, instance.FontStretch), instance.FontSize, Brushes.Black,
                new NumberSubstitution(), TextFormattingMode.Ideal, 1);
            
            formattedText.Trimming = TextTrimming.None;
            
            return formattedText.Width + padding;
        }
    }
}
