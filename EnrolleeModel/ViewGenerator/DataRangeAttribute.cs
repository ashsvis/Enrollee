using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataRangeAttribute : Attribute
    {
        public double Low;
        public double High;

        public DataRangeAttribute() { }

        public DataRangeAttribute(double high)
        {
            High = high;
        }
        public DataRangeAttribute(double low, double high)
        {
            Low = low;
            High = high;
        }
    }
}
