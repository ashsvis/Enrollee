using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataPasswordAttribute : Attribute
    {
        public Char PasswordChar { get; set; }

        public DataPasswordAttribute() { }

        public DataPasswordAttribute(char passwordChar)
        {
            PasswordChar = passwordChar;
        }
    }
}
