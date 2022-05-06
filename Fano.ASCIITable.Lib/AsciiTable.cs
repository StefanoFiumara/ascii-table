using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fano.ASCIITableUtil
{
    public class IgnoreColumnAttribute : Attribute { }

    public static class AsciiTable
    {
        public static string Create<T>(List<T> items, bool useIndex = false)
        {
            var headers = new List<(string header, Func<T, string> propertySelector)>();

            var properties = typeof(T).GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(IgnoreColumnAttribute)))
                {
                    headers.Add((propertyInfo.Name, p => $"{propertyInfo.GetValue(p, null)}"));
                }
            }

            return Create(items, headers, useIndex);

        }

        public static string Create<T>(List<T> items, List<(string header, Func<T, string> propertySelector)> headers, bool useIndex = false)
        {
            var sb = new StringBuilder();

            var headerPaddings = CalculateColumnPadding(items, headers, useIndex);

            sb.AppendLine(string.Join(" | ", headerPaddings.Select(f => f.header.PadRight(f.padding))));

            var underlines = new List<string>();

            foreach ((_, int padding) in headerPaddings)
            {
                var line = new string('=', padding);
                underlines.Add(line);
            }

            sb.AppendLine(string.Join(" | ", underlines));

            foreach (var item in items)
            {
                var formattedProps = new List<string>();

                if (useIndex)
                {
                    var padding = headerPaddings.Single(h => h.header == "#").padding;
                    formattedProps.Add($"{items.IndexOf(item) + 1}".PadRight(padding));
                }

                foreach ((string header, var propertySelector) in headers)
                {
                    var padding = headerPaddings.Single(h => h.header == header).padding;
                    formattedProps.Add(propertySelector(item).PadRight(padding));
                }

                sb.AppendLine(string.Join(" | ", formattedProps));
            }

            return sb.ToString();
        }

        private static List<(string header, int padding)> CalculateColumnPadding<T>(List<T> items, List<(string header, Func<T, string> propertySelector)> headers, bool useIndex = false)
        {
            var paddings = new List<(string header, int padding)>();

            if (items.Count == 0)
            {
                var fixedHeaders = headers.Select(h => (h.header, padding: h.header.Length)).ToList();

                if (useIndex)
                {
                    fixedHeaders.Add(("#", 1));
                }

                return fixedHeaders;
            }

            if (useIndex)
            {
                var width = items.Select(t => $"{items.IndexOf(t) + 1}".Length).Max() + 1;
                paddings.Add(("#", width));
            }

            foreach (var (header, selector) in headers)
            {
                var width = items.Select(t => selector(t).Length).Max();

                width = width > header.Length ? width : header.Length;

                paddings.Add((header, width));
            }

            return paddings;
        }
    }
}
