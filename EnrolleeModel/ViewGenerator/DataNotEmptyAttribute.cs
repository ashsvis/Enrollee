using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataNotEmptyAttribute : Attribute
    {
        public DataNotEmptyAttribute() { }
    }
}
