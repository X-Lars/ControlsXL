using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlsXL.Common
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the number of decimal places of a <see cref="double"/>.
        /// </summary>
        /// <param name="value">The value to get the number of decimal places.</param>
        /// <returns>The number of decimal places.</returns>
        /// <remarks><i>Trailing zero's are not counted.</i></remarks>
        public static int GetDecimalPlaces(this double value)
        {
            Debug.Assert(value <= int.MaxValue);

            value = Math.Abs(value); //make sure it is positive.
            value -= (int)value;     //remove the integer part of the number.
            var decimalPlaces = 0;
            while (value > 0)
            {
                decimalPlaces++;
                value *= 10;
                value -= (int)value;
            }
            return decimalPlaces;
        }
    }
}
