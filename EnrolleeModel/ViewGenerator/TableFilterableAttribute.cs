using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TableFilterableAttribute : Attribute
    {
        public TableFilterableAttribute() { }
    }

}
