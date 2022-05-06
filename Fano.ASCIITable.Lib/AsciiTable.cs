using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fano.ASCIITableUtil
{
    public static class AsciiTable
    {
        /// <summary>
        /// Creates a formatted ASCII table with the given list.
        /// The type's public properties are used as header names.
        /// </summary>
        /// <param name="items">The items to populate the table with.</param>
        /// <param name="useIndex">If true, adds an additional column to index each item</param>
        /// <returns>A string containing the formatted ASCII table</returns>
        public static string Create<T>(IList<T> items, bool useIndex = false)
        {
            var headers = new SortedList<int, TableHeader<T>>();

            var properties = typeof(T).GetProperties();

            int index = 1000;

            foreach (var propertyInfo in properties)
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(AsciiIgnoreColumnAttribute)))
                {
                    var indexAttr = propertyInfo.GetCustomAttribute<AsciiColumnIndexAttribute>();
                    int currentIndex = indexAttr?.Index ?? index++;

                    var headerNameAttr = propertyInfo.GetCustomAttribute<AsciiHeaderNameAttribute>();
                    var headerName = headerNameAttr?.DisplayName ?? propertyInfo.Name;

                    var header = new TableHeader<T>(headerName, p => $"{propertyInfo.GetValue(p, null)}");
                    headers.Add(currentIndex, header);
                }
            }

            return Create(items, headers.Values, useIndex);
        }

        /// <summary>
        /// Creates a formatted ASCII table with the given list.
        /// The headers parameter is used to determine which properties to show and in which order.
        /// </summary>
        /// <param name="items">The items to populate the table with.</param>
        /// <param name="headers">A list of table headers and their property value selectors</param>
        /// <param name="useIndex">If true, adds an additional column to index each item</param>
        /// <returns>A string containing the formatted ASCII table</returns>
        public static string Create<T>(IList<T> items, IList<TableHeader<T>> headers, bool useIndex = false)
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

                foreach (var header in headers)
                {
                    var padding = headerPaddings.Single(h => h.header == header.Name).padding;
                    formattedProps.Add(header.ValueSelector(item).PadRight(padding));
                }

                sb.AppendLine(string.Join(" | ", formattedProps));
            }

            return sb.ToString();
        }

        private static List<(string header, int padding)> CalculateColumnPadding<T>(IList<T> items, IList<TableHeader<T>> headers, bool useIndex = false)
        {
            var paddings = new List<(string header, int padding)>();

            if (items.Count == 0)
            {
                var fixedHeaders = headers.Select(h => (h.Name, padding: h.Name.Length)).ToList();

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

            foreach (var header in headers)
            {
                var width = items.Select(t => header.ValueSelector(t).Length).Max();

                width = width > header.Name.Length ? width : header.Name.Length;

                paddings.Add((header.Name, width));
            }

            return paddings;
        }
    }
}
