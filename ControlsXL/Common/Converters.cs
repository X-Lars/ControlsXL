using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ControlsXL.Common
{
    /// <summary>
    /// Changes the sign of a numerical value.
    /// </summary>
    public class InvertConverter : IValueConverter
    {
        /// <summary>
        /// Inverts the sign of the specified value.
        /// </summary>
        /// <param name="value">A numerical <see cref="object"/> to convert.</param>
        /// <param name="targetType"><i>Unused parameter.</i></param>
        /// <param name="parameter"><i>Unused parameter.</i></param>
        /// <param name="culture"><i>Unused parameter.</i></param>
        /// <returns>The inverted <paramref name="value"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * -1;
        }

        /// <summary>
        /// <i>Not implemented.</i>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// Converts a <see cref="null"/> value to a <see cref="bool"/>.
    /// </summary>
    public class NullConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="null"/> value to a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">An <see cref="object"/> to convert.</param>
        /// <param name="targetType"><i>Unused parameter.</i></param>
        /// <param name="parameter"><i>Unused parameter.</i></param>
        /// <param name="culture"><i>Unused parameter.</i></param>
        /// <returns>A <see cref="bool"/> containing true if the specified <see cref="object"/> is <see cref="null"/>, false otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        /// <summary>
        /// <i>Not implemented.</i>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// Converts an enumeration value to a <see cref="bool"/> if it contains the provided enumeration value.
    /// </summary>
    public class FlagsConverter : IValueConverter
    {
        /// <summary>
        /// Converts the enumeration to a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">An enumeration to convert.</param>
        /// <param name="targetType"><i>Unused parameter.</i></param>
        /// <param name="parameter">An enumeration value to check.</param>
        /// <param name="culture"><i>Unused parameter.</i></param>
        /// <returns>A <see cref="bool"/> containing true if the enumeration contains the provided enumeration value, false otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType().IsEnum == false || parameter.GetType().IsEnum == false)
                return Binding.DoNothing;

            return ((Enum)value).HasFlag((Enum)parameter);
        }

        /// <summary>
        /// <i>Not implemented.</i>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    

    /// <summary>
    /// Converts a numeric value to a <see cref="Thickness"/> containing only the specified sides.
    /// </summary>
    public class ThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts the numeric value to a <see cref="Thickness"/> containing the specified sides.
        /// </summary>
        /// <param name="value">A numeric value to convert.</param>
        /// <param name="targetType"><i>Unused parameter.</i></param>
        /// <param name="parameter">An <see cref="int"/>, <see cref="double"/>, <see cref="Thickness"/> or space delimited <see cref="string"/> specifying the sides to apply the thickness.</param>
        /// <param name="culture"><i>Unused parameter.</i>.</param>
        /// <returns>A <see cref="Thickness"/> containing the thickness applied to the specified sides.</returns>
        /// <remarks><i>Any non zero value in <paramref name="parameter"/> gets the thickness applied.</i></remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double left, top, right, bottom;

            double margin = (double)value;
            
            if (parameter.GetType() == typeof(string))
            {
                string[] parameters = parameter.ToString().Split(new char[] { ' ' });

                switch (parameters.Length)
                {
                    case 0:
                        return margin;

                    case 1:
                        if (double.TryParse(parameters[0], out bottom) == false) throw new ArgumentException(parameters[0].ToString());
                        left = top = right = bottom;
                        break;

                    case 2:

                        if (double.TryParse(parameters[0], out right) == false) throw new ArgumentException(parameters[0].ToString());
                        if (double.TryParse(parameters[1], out bottom) == false) throw new ArgumentException(parameters[1].ToString());
                        left = right;
                        top = bottom;
                        break;

                    case 4:

                        if (double.TryParse(parameters[0], out left) == false) throw new ArgumentException(parameters[0].ToString());
                        if (double.TryParse(parameters[1], out top) == false) throw new ArgumentException(parameters[1].ToString());
                        if (double.TryParse(parameters[2], out right) == false) throw new ArgumentException(parameters[2].ToString());
                        if (double.TryParse(parameters[3], out bottom) == false) throw new ArgumentException(parameters[3].ToString());
                        break;

                    default:
                        throw new ArgumentException(parameter.ToString());
                }
            }
            else if (parameter.GetType() == typeof(double) || parameter.GetType() == typeof(int))
            {
                left = top = right = bottom = (double)parameter;
            }
            else if (parameter.GetType() == typeof(Thickness))
            {
                Thickness parameters = (Thickness)parameter;
                left = parameters.Left;
                top = parameters.Top;
                right = parameters.Right;
                bottom = parameters.Bottom;
            }
            else
                return null;

            Thickness result = new Thickness();

            result.Left   = left   == 0 ? 0 : margin;
            result.Top    = top    == 0 ? 0 : margin;
            result.Right  = right  == 0 ? 0 : margin;
            result.Bottom = bottom == 0 ? 0 : margin;

            return result;
        }

        /// <summary>
        /// <i>Not implemented.</i>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a <see cref="Thickness"/> to a new <see cref="Thickness"/> containing only the specified borders.
    /// </summary>
    /// <remarks><i>Maintains the provided border thickness, only hides the specified borders, used for dynamic styling.</i></remarks>
    public class AdaptiveThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts the <see cref="Thickness"/> to a new <see cref="Thickness"/> containing only the specified borders.
        /// </summary>
        /// <param name="value">A <see cref="Thickness"/> to convert.</param>
        /// <param name="targetType"><i>Unused parameter.</i></param>
        /// <param name="parameter">An <see cref="int"/>, <see cref="double"/>, <see cref="Thickness"/> or space delimited <see cref="string"/> specifying the borders to hide.</param>
        /// <param name="culture"><i>Unused parameter.</i></param>
        /// <returns>A <see cref="Thickness"/> containing the thickness without the specified borders.</returns>
        /// <remarks><i>If any of the original <see cref="Thickness"/> values is 0 converting has no effect on it.</i></remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double left, top, right, bottom;
            
            Thickness thickness = (Thickness)value;

            if (thickness == null)
                return null;

            if (parameter.GetType() == typeof(string))
            {
                string[] parameters = parameter.ToString().Split(new char[] { ' ' });

                switch (parameters.Length)
                {
                    case 0:
                        return thickness;

                    case 1:
                        if(double.TryParse(parameters[0], out bottom) == false) throw new ArgumentException(parameters[0].ToString());
                        left = top = right = bottom;
                        break;

                    case 2:

                        if(double.TryParse(parameters[0], out right)  == false) throw new ArgumentException(parameters[0].ToString());
                        if(double.TryParse(parameters[1], out bottom) == false) throw new ArgumentException(parameters[1].ToString());
                        left = right;
                        top = bottom;
                        break;

                    case 4:

                        if(double.TryParse(parameters[0], out left)   == false) throw new ArgumentException(parameters[0].ToString());
                        if(double.TryParse(parameters[1], out top)    == false) throw new ArgumentException(parameters[1].ToString());
                        if(double.TryParse(parameters[2], out right)  == false) throw new ArgumentException(parameters[2].ToString());
                        if(double.TryParse(parameters[3], out bottom) == false) throw new ArgumentException(parameters[3].ToString());
                        break;

                    default:
                        throw new ArgumentException(parameter.ToString());
                }
            }
            else if (parameter.GetType() == typeof(double) || parameter.GetType() == typeof(int))
            {
                left = top = right = bottom = (double)parameter;
            }
            else if(parameter.GetType() == typeof(Thickness))
            {
                Thickness parameters = (Thickness)parameter;
                left   = parameters.Left;
                top    = parameters.Top;
                right  = parameters.Right;
                bottom = parameters.Bottom;
            }
            else
                return null;

            Thickness result = new Thickness();
            
            result.Left   = left   == 0 ? 0 : thickness.Left;
            result.Top    = top    == 0 ? 0 : thickness.Top;
            result.Right  = right  == 0 ? 0 : thickness.Right;
            result.Bottom = bottom == 0 ? 0 : thickness.Bottom;

            return result;
        }

        /// <summary>
        /// <i>Not implemented.</i>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
