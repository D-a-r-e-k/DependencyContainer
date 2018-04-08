using DContainer.Internals.Attributes;
using ImpromptuInterface;
using System;
using System.Dynamic;

namespace DContainer.Internals
{
    public class DynamicProxy : DynamicObject
    {
        private DelegateAttribute _delegateAttribute;
        private DecorateAttribute _decorateAttribute;

        private Type _typeToBeWrapped;
        private IContainer _container;

        private object _wrappedObject;
        private object _decorator;
        private object _delegateObject;

        // Creation method will be called using reflection as not in all cases T is known during compile time
        public static T Create<T>(IContainer container, DelegateAttribute delegateAttribute,
            DecorateAttribute decorateAttribute) where T: class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("T must be an Interface");
            }

            if (delegateAttribute != null && !delegateAttribute.On.IsInterface)
            {
                throw new ArgumentException("Interface expected");
            }

            return new DynamicProxy(typeof(T), container, delegateAttribute, decorateAttribute).ActLike<T>(delegateAttribute.On);
        }

        private DynamicProxy(Type type, IContainer container, DelegateAttribute delegateAttribute,
            DecorateAttribute decorateAttribute)
        {
            _typeToBeWrapped = type;
            _container = container;
            _delegateAttribute = delegateAttribute;
            _decorateAttribute = decorateAttribute;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                if (_wrappedObject == null)
                {
                    _wrappedObject = _container.Resolve(_typeToBeWrapped);
                }

                if (_decorateAttribute != null)
                {
                    _decorator = _container.Resolve(_decorateAttribute.Type);

                    _decorator.GetType().GetMethod(_decorateAttribute.Before)
                        .Invoke(_decorator, new object[] { $"Before {binder.Name}" });
                }

                if (_wrappedObject.GetType().GetMethod(binder.Name) == null)
                {
                    if (_delegateObject == null)
                    {
                        _delegateObject = _container.Resolve(_delegateAttribute.On);                       
                    }

                    result = _delegateObject.GetType().GetMethod(binder.Name).Invoke(_delegateObject, args);
                }
                else
                {
                    result = _wrappedObject.GetType().GetMethod(binder.Name).Invoke(_wrappedObject, args);
                }

                if (_decorator != null)
                {
                    _decorator.GetType().GetMethod(_decorateAttribute.After)
                        .Invoke(_decorator, new object[] { $"After {binder.Name}" });
                }

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
