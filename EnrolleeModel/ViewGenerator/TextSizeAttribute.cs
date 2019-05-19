using System;

namespace ViewGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TextSizeAttribute : Attribute
    {
        public int Width;
        public int Height;
        public bool Multiline;

        public TextSizeAttribute() { }

        public TextSizeAttribute(int width)
        {
            Width = width;
        }

        public TextSizeAttribute(int width, bool multiline = false, int height = 0)
        {
            Width = width;
            Height = height;
            Multiline = multiline;
        }
    }
}
