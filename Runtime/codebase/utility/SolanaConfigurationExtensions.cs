using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Solana.Unity.SDK
{
    /// <summary>
    /// Helper extensions for <see cref="SolanaConfiguration"/> that provide
    /// formatting utilities used by the sample scenes.
    /// </summary>
    public static class SolanaConfigurationExtensions
    {
        private const char SegmentSeparator = ':';
        private const char KeyValueSeparator = '=';

        /// <summary>
        /// Builds a memo payload describing the currently selected token skin.
        /// </summary>
        /// <param name="configuration">The active Solana configuration.</param>
        /// <param name="segments">Arbitrary segments that describe the skin.</param>
        /// <returns>A compact string suitable for the memo instruction.</returns>
        public static string FormatTokenSkinMemo(this SolanaConfiguration configuration, params object[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            var values = new List<string>();

            foreach (var segment in segments)
            {
                AppendSegment(values, segment);
            }

            if (values.Count == 0)
            {
                return string.Empty;
            }

            builder.Append("token-skin");
            builder.Append(SegmentSeparator);
            builder.Append(string.Join(SegmentSeparator, values));

            return builder.ToString();
        }

        private static void AppendSegment(ICollection<string> values, object segment)
        {
            switch (segment)
            {
                case null:
                    return;
                case string s when !string.IsNullOrWhiteSpace(s):
                    values.Add(s.Trim());
                    break;
                case Color color:
                    values.Add(ColorUtility.ToHtmlStringRGB(color));
                    break;
                case Enum enumValue:
                    values.Add(enumValue.ToString());
                    break;
                case IDictionary dictionary:
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        var key = entry.Key?.ToString();
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            continue;
                        }

                        var value = FormatValue(entry.Value);
                        values.Add(string.Concat(key.Trim(), KeyValueSeparator, value));
                    }
                    break;
                case IEnumerable enumerable:
                    foreach (var item in enumerable)
                    {
                        AppendSegment(values, item);
                    }
                    break;
                default:
                    values.Add(segment.ToString());
                    break;
            }
        }

        private static string FormatValue(object value)
        {
            switch (value)
            {
                case null:
                    return string.Empty;
                case string s:
                    return s.Trim();
                case Color color:
                    return ColorUtility.ToHtmlStringRGB(color);
                case Enum enumValue:
                    return enumValue.ToString();
                default:
                    return value.ToString();
            }
        }
    }
}
