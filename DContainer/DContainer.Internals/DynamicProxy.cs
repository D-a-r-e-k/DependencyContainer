using ImpromptuInterface;
using System;
using System.Dynamic;

namespace DContainer.Internals
{
    public class DynamicProxy : DynamicObject
    {
        private Type _typeToBeWrapped;
        private IContainer _container;

        private object _wrappedObject;

        public static object Create(Type type, IContainer container)
        {
            if (!type.IsInterface)
                throw new ArgumentException("T1 must be an Interface");

            return Impromptu.ActLike(new DynamicProxy(type, container), type);
        }

        private DynamicProxy(Type type, IContainer container)
        {
            _typeToBeWrapped = type;
            _container = container;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                if (_wrappedObject == null)
                {
                    _wrappedObject = _container.Resolve(_typeToBeWrapped);
                }

                result = _wrappedObject.GetType().GetMethod(binder.Name).Invoke(_wrappedObject, args);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
