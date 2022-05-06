using System;

namespace Fano.ASCIITableUtil
{
    public class TableHeader<T>
    {
        public string Header { get; }
        public Func<T, string> PropertySelector { get; }

        public TableHeader(string header, Func<T, string> propertySelector)
        {
            Header = header;
            PropertySelector = propertySelector;
        }
    }
}