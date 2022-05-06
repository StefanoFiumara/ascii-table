using System;

namespace Fano.ASCIITableUtil
{
    public class AsciiIgnoreColumnAttribute : Attribute { }

    public class AsciiColumnIndexAttribute : Attribute
    {
        public int Index { get; }

        public AsciiColumnIndexAttribute(int index)
        {
            Index = index;
        }
    }

    public class AsciiHeaderNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public AsciiHeaderNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
