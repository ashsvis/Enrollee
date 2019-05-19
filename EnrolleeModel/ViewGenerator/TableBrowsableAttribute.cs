using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TableBrowsableAttribute : Attribute
    {
        public bool Browsable { get; set; }

        public TableBrowsableAttribute() { }

        public TableBrowsableAttribute(bool browsable = true)
        {
            Browsable = browsable;
        }
    }

}
