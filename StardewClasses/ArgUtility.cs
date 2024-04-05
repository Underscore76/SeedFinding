using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace StardewValley
{

    /// <summary>A utility for working with space-delimited or split argument lists.</summary>
    public static class ArgUtility
    {
        /// <summary>Split space-separated arguments in a string, ignoring extra spaces.</summary>
        /// <param name="value">The value to split.</param>
        /// <returns>Returns an array of the space-delimited arguments, or an empty array if the <paramref name="value" /> was null, empty, or only contains spaces.</returns>
        /// <remarks>For example, this text: <code>A  B C</code> would be split into three values (<c>A</c>, <c>B</c>, and <c>C</c>). See also <see cref="M:StardewValley.ArgUtility.SplitBySpaceQuoteAware(System.String)" />.</remarks>
        public static string[] SplitBySpace(string value)
        {
            return value?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? LegacyShims.EmptyArray<string>();
        }

        /// <inheritdoc cref="M:StardewValley.ArgUtility.SplitBySpace(System.String)" />
        /// <param name="value">The value to split.</param>
        /// <param name="limit">The number of arguments to return. Any remaining arguments by appended to the final argument.</param>
        public static string[] SplitBySpace(string value, int limit)
        {
            return value?.Split(' ', limit, StringSplitOptions.RemoveEmptyEntries) ?? LegacyShims.EmptyArray<string>();
        }

        /// <summary>Split space-separated arguments in a string (ignoring extra spaces), and get a specific argument.</summary>
        /// <param name="value">The value to split.</param>
        /// <param name="index">The index of the value to get.</param>
        /// <param name="defaultValue">The value to return if the <paramref name="index" /> is out of range for the array.</param>
        /// <returns>Returns the value at the given index if the array was non-null and the index is in range, else the <paramref name="defaultValue" />.</returns>
        public static string SplitBySpaceAndGet(string value, int index, string defaultValue = null)
        {
            if (value == null)
            {
                return defaultValue;
            }
            return ArgUtility.Get(value.Split(' ', index + 2, StringSplitOptions.RemoveEmptyEntries), index, defaultValue);
        }

        /// <summary>Split a list of space-separated arguments (ignoring extra spaces), with support for using quotes to protect spaces within an argument.</summary>
        /// <param name="input">The value to split.</param>
        /// <remarks>See remarks on <see cref="M:StardewValley.ArgUtility.SplitBySpaceQuoteAware(System.String)" /> for the quote format details.</remarks>
        public static string[] SplitBySpaceQuoteAware(string input)
        {
            return ArgUtility.SplitQuoteAware(input, ' ', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Split a list of arguments using the given delimiter, with support for using quotes to protect delimiters within an argument.</summary>
        /// <param name="input">The value to split.</param>
        /// <param name="delimiter">The character on which to split the value. This shouldn't be a quote (<c>"</c>) or backslash (<c>\</c>).</param>
        /// <param name="splitOptions">The string split options to apply for the delimiter split.</param>
        /// <param name="keepQuotesAndEscapes">Whether to keep quotes and escape characters in the string. For example, the value <c>Some \"test\" "here"</c> would become <c>Some "test" here</c> if this disabled, or kept as-is (aside from splitting) if it's enabled. This impacts performance and should usually be <c>false</c> unless you need to split each value further while respecting quotes.</param>
        /// <remarks>
        ///   <para>A quote in the text causes any delimiter to be ignored until the next quote. The quotes are removed from the string. For example, this comma-delimited input: <code>"some,text",here</code> will produce two values: <c>some,text</c> and <c>here</c>.</para>
        ///
        ///   <para>A quote character can be escaped by preceding it with a backslash (like <c>\"</c>). Escaped quotes have no effect on delimiters, and aren't removed from the string. For example, this comma-delimited input: <code>some,\"text,here</code> will produce three values: <c>some</c>, <c>"text</c>, and <c>here</c>. Remember that backslashes need to be escaped in C# or JSON strings (e.g. <c>"\\"</c> produces a single backslash).</para>
        ///
        ///   <para>See also <see cref="M:StardewValley.ArgUtility.SplitBySpaceQuoteAware(System.String)" /> which simplifies usage for the most common case used by the game.</para>
        ///
        ///   <para>When an input *doesn't* contain quotes, this is optimized to be almost as fast as just calling <see cref="M:System.String.Split(System.Char,System.StringSplitOptions)" /> directly.</para>
        /// </remarks>
        public static string[] SplitQuoteAware(string input, char delimiter, StringSplitOptions splitOptions = StringSplitOptions.None, bool keepQuotesAndEscapes = false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return LegacyShims.EmptyArray<string>();
            }
            if (!input.Contains('"'))
            {
                return input.Split(delimiter, splitOptions);
            }
            bool shouldTrimEntries = false;
            if (splitOptions.HasFlag(StringSplitOptions.TrimEntries))
            {
                shouldTrimEntries = true;
                splitOptions &= ~StringSplitOptions.TrimEntries;
            }
            bool splitOptionsRemoveEmpty = splitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries);
            string[] segments = input.Split('"');
            List<string> values = new List<string>(segments.Length * 4);
            bool isQuoted = true;
            bool prevEndsWithDelimiter = true;
            string prevValue = null;
            int j = 0;
            for (int last = segments.Length - 1; j <= last; j++)
            {
                isQuoted = !isQuoted;
                string segment = segments[j];
                bool overwritePrev = false;
                bool appendToPrev = false;
                bool endsWithDelimiter = segment.EndsWith(delimiter);
                if (keepQuotesAndEscapes && j != 0)
                {
                    segment = "\"" + segment;
                }
                if (!prevEndsWithDelimiter)
                {
                    if (prevValue.EndsWith('\\'))
                    {
                        segment = (keepQuotesAndEscapes ? (prevValue + segment) : (prevValue.Substring(0, prevValue.Length - 1) + "\"" + segment));
                        isQuoted = !isQuoted;
                        overwritePrev = true;
                    }
                    else if (isQuoted || !segment.StartsWith(delimiter))
                    {
                        appendToPrev = true;
                    }
                    else
                    {
                        segment = segment.Substring(1);
                    }
                }
                if (values.Count == 0)
                {
                    overwritePrev = false;
                    appendToPrev = false;
                }
                if (isQuoted)
                {
                    endsWithDelimiter = false;
                    if (overwritePrev)
                    {
                        values[values.Count - 1] = segment;
                    }
                    else if (appendToPrev)
                    {
                        values[values.Count - 1] += segment;
                        segment = values[values.Count - 1];
                    }
                    else
                    {
                        values.Add(segment);
                    }
                    prevValue = segment;
                    prevEndsWithDelimiter = false;
                    continue;
                }
                if (endsWithDelimiter && !splitOptionsRemoveEmpty && j != last && segment.Length > 0)
                {
                    segment = segment.Substring(0, segment.Length - 1);
                }
                string[] split = segment.Split(delimiter, splitOptions);
                int num = split.Length;
                if (num != 0)
                {
                    if (num == 1 && endsWithDelimiter && split[0] == string.Empty)
                    {
                        prevValue = string.Empty;
                    }
                    else
                    {
                        if (overwritePrev)
                        {
                            values.RemoveAt(values.Count - 1);
                            values.AddRange(split);
                        }
                        else if (appendToPrev)
                        {
                            values[values.Count - 1] += split[0];
                            if (split.Length > 1)
                            {
                                values.AddRange(new ArraySegment<string>(split, 1, split.Length - 1));
                            }
                        }
                        else
                        {
                            values.AddRange(split);
                        }
                        prevValue = split[^1];
                    }
                }
                else
                {
                    prevValue = string.Empty;
                }
                prevEndsWithDelimiter = endsWithDelimiter;
            }
            if (shouldTrimEntries)
            {
                for (int i = values.Count - 1; i >= 0; i--)
                {
                    values[i] = values[i].Trim();
                    if (splitOptionsRemoveEmpty && values[i].Length == 0)
                    {
                        values.RemoveAt(i);
                    }
                }
            }
            return values.ToArray();
        }

        /// <summary>Escape quotes in a string so they're ignored by methods like <see cref="M:StardewValley.ArgUtility.SplitQuoteAware(System.String,System.Char,System.StringSplitOptions,System.Boolean)" />.</summary>
        /// <param name="input">The input string to escape.</param>
        /// <remarks>This isn't idempotent (e.g. calling it twice will result in double-escaped quotes).</remarks>
        public static string EscapeQuotes(string input)
        {
            return input.Replace("\"", "\\\"");
        }

        /// <summary>Get whether an index is within the bounds of the array, regardless of what value is at that position.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="array">The array of arguments to check.</param>
        /// <param name="index">The index to check within the <paramref name="array" />.</param>
        public static bool HasIndex<T>(T[] array, int index)
        {
            if (index >= 0)
            {
                if (array == null)
                {
                    return false;
                }
                return array.Length > index;
            }
            return false;
        }

        /// <summary>Get a string argument by its array index.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="defaultValue">The value to return if the index is out of bounds or invalid.</param>
        /// <param name="allowBlank">Whether to return the argument even if it's null or whitespace. If false, the <paramref name="defaultValue" /> will be returned in that case.</param>
        /// <returns>Returns the selected argument (if the <paramref name="index" /> is found and valid), else <paramref name="defaultValue" />.</returns>
        public static string Get(string[] array, int index, string defaultValue = null, bool allowBlank = true)
        {
            if (index >= 0 && index < array?.Length)
            {
                string value = array[index];
                if (allowBlank || !string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        /// <summary>Get a string argument by its array index, if it's found and valid.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="value">The argument value, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="allowBlank">Whether to match the argument even if it's null or whitespace. If false, it will be treated as invalid in that case.</param>
        /// <returns>Returns whether the argument was successfully found and is valid.</returns>
        public static bool TryGet(string[] array, int index, out string value, out string error, bool allowBlank = true)
        {
            if (array == null)
            {
                value = null;
                error = "argument list is null";
                return false;
            }
            if (index < 0 || index >= array.Length)
            {
                value = null;
                error = ArgUtility.GetMissingRequiredIndexError(array, index);
                return false;
            }
            value = array[index];
            if (!allowBlank && string.IsNullOrWhiteSpace(value))
            {
                value = null;
                error = $"required index {index} has a blank value";
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a string argument by its array index, or a default value if the argument isn't found.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="value">The argument value, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="defaultValue">The value to return if the index is out of bounds or invalid.</param>
        /// <param name="allowBlank">Whether to match the argument even if it's null or whitespace. If false, it will be treated as invalid in that case.</param>
        /// <returns>Returns true if either (a) the argument was found and valid, or (b) the argument was not found so the default value was used. Returns false if the argument was found but isn't in a valid format.</returns>
        public static bool TryGetOptional(string[] array, int index, out string value, out string error, string defaultValue = null, bool allowBlank = true)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                value = defaultValue;
                error = null;
                return true;
            }
            value = array[index];
            if (!allowBlank && string.IsNullOrWhiteSpace(value))
            {
                value = defaultValue;
                error = $"optional index {index} can't have a blank value";
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get an boolean argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static bool GetBool(string[] array, int index, bool defaultValue = false)
        {
            if (!bool.TryParse(ArgUtility.Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>Get a boolean argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetBool(string[] array, int index, out bool value, out string error)
        {
            if (!ArgUtility.TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = false;
                return false;
            }
            if (!bool.TryParse(raw, out value))
            {
                value = false;
                error = ArgUtility.GetValueParseError(array, index, required: true, "a boolean (should be 'true' or 'false')");
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a boolean argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String@,System.String,System.Boolean)" />
        public static bool TryGetOptionalBool(string[] array, int index, out bool value, out string error, bool defaultValue = false)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!bool.TryParse(array[index], out value))
            {
                error = ArgUtility.GetValueParseError(array, index, required: false, "a boolean");
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a direction argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        /*public static int GetDirection(string[] array, int index, int defaultValue = 0)
        {
            if (!Utility.TryParseDirection(ArgUtility.Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }*/

        /// <summary>Get a direction argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        /*public static bool TryGetDirection(string[] array, int index, out int value, out string error)
        {
            if (!ArgUtility.TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = 0;
                return false;
            }
            if (!Utility.TryParseDirection(raw, out value))
            {
                value = 0;
                error = ArgUtility.GetValueParseError(array, index, required: true, "a direction (should be 'up', 'down', 'left', or 'right')");
                return false;
            }
            error = null;
            return true;
        }*/

        /// <summary>Get a direction argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String@,System.String,System.Boolean)" />
        /*public static bool TryGetOptionalDirection(string[] array, int index, out int value, out string error, int defaultValue = 0)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!Utility.TryParseDirection(array[index], out value))
            {
                error = ArgUtility.GetValueParseError(array, index, required: true, "a direction (should be one of 'up', 'down', 'left', or 'right')");
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }*/

        /// <summary>Get an enum argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        /*public static TEnum GetEnum<TEnum>(string[] array, int index, TEnum defaultValue = default(TEnum)) where TEnum : struct
        {
            if (!Utility.TryParseEnum<TEnum>(ArgUtility.Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }*/

        /// <summary>Get an enum argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        /*public static bool TryGetEnum<TEnum>(string[] array, int index, out TEnum value, out string error) where TEnum : struct
        {
            if (!ArgUtility.TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = default(TEnum);
                return false;
            }
            if (!Utility.TryParseEnum<TEnum>(raw, out value))
            {
                Type type = typeof(TEnum);
                value = default(TEnum);
                error = ArgUtility.GetValueParseError(array, index, required: true, $"an enum of type '{type.FullName ?? type.Name}' (should be one of {string.Join(", ", Enum.GetNames(typeof(TEnum)))})");
                return false;
            }
            error = null;
            return true;
        }*/

        /// <summary>Get an enum argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String@,System.String,System.Boolean)" />
        /*public static bool TryGetOptionalEnum<TEnum>(string[] array, int index, out TEnum value, out string error, TEnum defaultValue = default(TEnum)) where TEnum : struct
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!Utility.TryParseEnum<TEnum>(array[index], out value))
            {
                Type type = typeof(TEnum);
                error = ArgUtility.GetValueParseError(array, index, required: false, "an enum of type '" + (type.FullName ?? type.Name) + "'");
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }*/

        /// <summary>Get a float argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static float GetFloat(string[] array, int index, float defaultValue = 0f)
        {
            if (!float.TryParse(ArgUtility.Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>Get a float argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetFloat(string[] array, int index, out float value, out string error)
        {
            if (!ArgUtility.TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = 0f;
                return false;
            }
            if (!float.TryParse(raw, out value))
            {
                value = 0f;
                error = ArgUtility.GetValueParseError(array, index, required: true, "a number");
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a float argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String@,System.String,System.Boolean)" />
        public static bool TryGetOptionalFloat(string[] array, int index, out float value, out string error, float defaultValue = 0f)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!float.TryParse(array[index], out value))
            {
                error = ArgUtility.GetValueParseError(array, index, required: false, "a float");
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get an integer argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static int GetInt(string[] array, int index, int defaultValue = 0)
        {
            if (!int.TryParse(ArgUtility.Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>Get an integer argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetInt(string[] array, int index, out int value, out string error)
        {
            if (!ArgUtility.TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = 0;
                return false;
            }
            if (!int.TryParse(raw, out value))
            {
                value = 0;
                error = ArgUtility.GetValueParseError(array, index, required: true, "an integer");
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get an int argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String@,System.String,System.Boolean)" />
        public static bool TryGetOptionalInt(string[] array, int index, out int value, out string error, int defaultValue = 0)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!int.TryParse(array[index], out value))
            {
                error = ArgUtility.GetValueParseError(array, index, required: false, "an integer");
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a point argument by its array index, if it's found and valid. This reads two consecutive values starting from <paramref name="index" /> for the X and Y values.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetPoint(string[] array, int index, out Point value, out string error)
        {
            if (!ArgUtility.TryGetInt(array, index, out var x, out error) || !ArgUtility.TryGetInt(array, index + 1, out var y, out error))
            {
                value = new Point(-1,-1);
                return false;
            }
            error = null;
            value = new Point(x, y);
            return true;
        }

        /// <summary>Get a rectangle argument by its array index, if it's found and valid. This reads four consecutive values starting from <paramref name="index" /> for the X, Y, width, and height values.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetRectangle(string[] array, int index, out Rectangle value, out string error)
        {
            if (!ArgUtility.TryGetInt(array, index, out var x, out error) || !ArgUtility.TryGetInt(array, index + 1, out var y, out error) || !ArgUtility.TryGetInt(array, index + 2, out var width, out error) || !ArgUtility.TryGetInt(array, index + 3, out var height, out error))
            {
                value = Rectangle.Empty;
                return false;
            }
            error = null;
            value = new Rectangle(x, y, width, height);
            return true;
        }

        /// <summary>Get a vector argument by its array index, if it's found and valid. This reads two consecutive values starting from <paramref name="index" /> for the X and Y values.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="value">The argument value, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="integerOnly">Whether the X and Y values must be integers.</param>
        /// <returns>Returns whether the argument was successfully found and is valid.</returns>
        public static bool TryGetVector2(string[] array, int index, out Vector2 value, out string error, bool integerOnly = false)
        {
            float x2;
            float y2;
            if (integerOnly)
            {
                if (ArgUtility.TryGetInt(array, index, out var x, out error) && ArgUtility.TryGetInt(array, index + 1, out var y, out error))
                {
                    value = new Vector2(x, y);
                    return true;
                }
            }
            else if (ArgUtility.TryGetFloat(array, index, out x2, out error) && ArgUtility.TryGetFloat(array, index + 1, out y2, out error))
            {
                value = new Vector2(x2, y2);
                return true;
            }
            value = Vector2.Zero;
            return false;
        }

        /// <summary>Get all arguments from the given index as a concatenated string.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static string GetRemainder(string[] array, int index, string defaultValue = null, char delimiter = ' ')
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                return defaultValue;
            }
            if (array.Length - index == 1)
            {
                return array[index];
            }
            return string.Join(delimiter, array[index..]);
        }

        /// <summary>Get all arguments starting from the given index as a concatenated string, if the index is found.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index of the first argument to include within the <paramref name="array" />.</param>
        /// <param name="value">The concatenated argument values, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="delimiter">The delimiter with which to concatenate values.</param>
        /// <returns>Returns whether at least one argument was successfully found.</returns>
        public static bool TryGetRemainder(string[] array, int index, out string value, out string error, char delimiter = ' ')
        {
            if (array == null)
            {
                value = null;
                error = "argument list is null";
                return false;
            }
            if (index < 0 || index >= array.Length)
            {
                value = null;
                error = ArgUtility.GetMissingRequiredIndexError(array, index);
                return false;
            }
            if (array.Length - index == 1)
            {
                value = array[index];
            }
            else
            {
                value = string.Join(delimiter, array[index..]);
            }
            error = null;
            return true;
        }

        /// <summary>Get all arguments starting from the given index as a concatenated string, or a default value if the index isn't in the array.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index of the first argument to include within the <paramref name="array" />.</param>
        /// <param name="value">The concatenated argument values, if found and valid.</param>
        /// <param name="defaultValue">The value to return if the index is out of bounds or invalid.</param>
        /// <param name="delimiter">The delimiter with which to concatenate values.</param>
        /// <returns>Returns true.</returns>
        public static bool TryGetOptionalRemainder(string[] array, int index, out string value, string defaultValue = null, char delimiter = ' ')
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                value = defaultValue;
                return true;
            }
            if (array.Length - index == 1)
            {
                value = array[index];
            }
            else
            {
                value = string.Join(delimiter, array[index..]);
            }
            return true;
        }

        /// <summary>Get an error message indicating that an array doesn't contain a required index.</summary>
        /// <param name="array">The array being indexed.</param>
        /// <param name="index">The index in the array being searched for.</param>
        private static string GetMissingRequiredIndexError(string[] array, int index)
        {
            return array.Length switch
            {
                0 => $"required index {index} not found (list is empty)",
                1 => $"required index {index} not found (list has a single value at index 0)",
                _ => $"required index {index} not found (list has indexes 0 through {array.Length - 1})",
            };
        }

        /// <summary>Get an error message indicating that an array index contains a value that can't be parsed.</summary>
        /// <param name="array">The array being indexed.</param>
        /// <param name="index">The index in the array being parsed.</param>
        /// <param name="required">Whether the argument is required.</param>
        /// <param name="typeSummary">A brief summary of the type being parsed, like "a boolean (one of 'true' or 'false')".</param>
        private static string GetValueParseError(string[] array, int index, bool required, string typeSummary)
        {
            return $"{(required ? "required" : "optional")} index {index} has value '{array[index]}', which can't be parsed as {typeSummary}";
        }
    }
}