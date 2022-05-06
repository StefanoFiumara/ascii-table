using System;

namespace Fano.ASCIITableUtil
{
    public class TableHeader<T>
    {
        public string Name { get; }
        public Func<T, string> ValueSelector { get; }

        public TableHeader(string name, Func<T, string> valueSelector)
        {
            Name = name;

            ValueSelector = valueSelector;
        }
    }
}
