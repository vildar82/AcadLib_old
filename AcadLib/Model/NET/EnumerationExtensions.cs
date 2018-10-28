namespace Extensions
{
    using System;
    using System.Threading;
    using AcadLib;
    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods to make working with Enum values easier
    /// http://hugoware.net/blog/enumeration-extensions-2-0
    /// </summary>
    [PublicAPI]
    [Obsolete]
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Checks if an enumerated type contains a value
        /// </summary>
        public static bool Has<T>([NotNull] this Enum value, T check)
        {
            var type = value.GetType();

            // determine the values
            var parsed = new _Value(check, type);
            if (parsed.Signed.HasValue)
            {
                return (Convert.ToInt64(value) & parsed.Signed.Value) == parsed.Signed.Value;
            }

            if (parsed.Unsigned.HasValue)
            {
                return (Convert.ToUInt64(value) & parsed.Unsigned.Value) == parsed.Unsigned.Value;
            }

            return false;
        }

        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        [NotNull]
        public static T Include<T>([NotNull] this Enum value, T append)
        {
            var type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(append, type);
            if (parsed.Signed.HasValue)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned.HasValue)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Checks if an enumerated type is missing a value
        /// </summary>
        public static bool Missing<T>([NotNull] this Enum obj, T value)
        {
            return !Has(obj, value);
        }

        /// <summary>
        /// Removes an enumerated type and returns the new value
        /// </summary>
        [NotNull]
        public static T Remove<T>([NotNull] this Enum value, T remove)
        {
            var type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(remove, type);
            if (parsed.Signed.HasValue)
            {
                result = Convert.ToInt64(value) & ~(long)parsed.Signed;
            }
            else if (parsed.Unsigned.HasValue)
            {
                result = Convert.ToUInt64(value) & ~(ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        // class to simplfy narrowing values between
        // a ulong and long since either value should
        // cover any lesser value
        private class _Value
        {
            public long? Signed;
            public ulong? Unsigned;
            private static readonly Type _UInt32 = typeof(long);

            // cached comparisons for tye to use
            private static readonly Type _UInt64 = typeof(ulong);

            public _Value(object value, [NotNull] Type type)
            {
                // make sure it is even an enum to work with
                if (!type.IsEnum)
                {
                    throw new
                        ArgumentException("Value provided is not an enumerated type!");
                }

                // then check for the enumerated value
                var compare = Enum.GetUnderlyingType(type);

                // if this is an unsigned long then the only
                // value that can hold it would be a ulong
                if (compare == _UInt32 || compare == _UInt64)
                {
                    Unsigned = Convert.ToUInt64(value);
                }

                // otherwise, a long should cover anything else
                else
                {
                    Signed = Convert.ToInt64(value);
                }
            }
        }
    }
}