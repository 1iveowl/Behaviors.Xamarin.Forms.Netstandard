using System;
using System.Collections.Generic;
using System.Text;

namespace Behaviors
{
    internal static class TypeConverterHelper
    {
        public static object Convert(string value, string destinationTypeFullName)
        {
            if (string.IsNullOrWhiteSpace(destinationTypeFullName))
            {
                throw new ArgumentNullException(destinationTypeFullName);
            }

            var scope = GetScope(destinationTypeFullName);

            if (string.Equals(scope, "System", StringComparison.Ordinal))
            {
                if (string.Equals(destinationTypeFullName, typeof(string).FullName, StringComparison.Ordinal))
                {
                    return value;
                }
                else if (string.Equals(destinationTypeFullName, typeof(bool).FullName, StringComparison.Ordinal))
                {
                    return bool.Parse(value);
                }
                else if (string.Equals(destinationTypeFullName, typeof(int).FullName, StringComparison.Ordinal))
                {
                    return int.Parse(value);
                }
                else if (string.Equals(destinationTypeFullName, typeof(double).FullName, StringComparison.Ordinal))
                {
                    return double.Parse(value);
                }
            }

            return null;
        }

        private static string GetScope(string name)
        {
            var indexOfLastPeriod = name.LastIndexOf('.');
            if (indexOfLastPeriod != name.Length - 1)
            {
                return name.Substring(0, indexOfLastPeriod);
            }
            return name;
        }
    }
}
