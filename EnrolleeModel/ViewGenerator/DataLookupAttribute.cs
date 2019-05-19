using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataLookupAttribute: Attribute
    {
        public string ValueMember;
        public string LookupMember;

        public DataLookupAttribute() { }

        public DataLookupAttribute(string valueMember, string lookupMember)
        {
            ValueMember = valueMember;
            LookupMember = lookupMember;
        }
    }
}
