using System;

namespace DContainer.Internals.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DelegateAttribute : Attribute
    {
        public Type On { get; set; }
        public Type By { get; set; }

        public DelegateAttribute(Type on, Type by)
        {
            On = on;
            By = by;
        }
    }
}
