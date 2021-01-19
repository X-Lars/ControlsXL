using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ControlsXL.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Compares a <see cref="string"/> to the specified filter, mimics the LIKE operator.
        /// </summary>
        /// <param name="source">The <see cref="string"/> to compare.</param>
        /// <param name="filter">A <see cref="string"/> specifying the pattern to match.</param>
        /// <param name="prefixWildcard">Specifies if a wildcard prefix is added to the filter.</param>
        /// <returns>A <see cref="bool"/> containing true if the <paramref name="source"/> matches the <paramref name="filter"/> pattern.</returns>
        /// <remarks><i>Use ' ' or '*' as wildcard for any number of characters, use '?' as single character wildcard.</i></remarks>
        public static bool Like(this string source, string filter, bool prefixWildcard = false)
        {
            if (prefixWildcard == false)
            {
                return new Regex(@"^.*" + Regex.Escape(filter).Replace(@"\?", ".").Replace(@"\ ", ".*").Replace(@"\*", ".*") + ".*$", RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(source);
            }
            else
            {
                return new Regex(@"^" + Regex.Escape(filter).Replace(@"\?", ".").Replace(@"\ ", ".*").Replace(@"\*", ".*") + ".*$", RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(source);
            }
        }
    }
}
