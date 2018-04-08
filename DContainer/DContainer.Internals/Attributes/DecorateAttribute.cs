using System;

namespace DContainer.Internals.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DecorateAttribute : Attribute
    {
        public Type Type { get; set; }
        public string Before { get; set; }
        public string After { get; set; }

        public DecorateAttribute(Type type, string before, string after)
        {
            Type = type;
            Before = before;
            After = after;
        }
    }
}
