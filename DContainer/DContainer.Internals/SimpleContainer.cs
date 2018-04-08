using DContainer.Internals.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DContainer.Internals
{
    public class SimpleContainer : IContainer
    {
        private readonly List<RegisteredObject> _registeredObjects = new List<RegisteredObject>();

        public void Register<TTypeToResolve, TConcrete>()
        {
            Register<TTypeToResolve, TConcrete>(Configuration.Eager);
        }

        public void Register<TTypeToResolve, TConcrete>(Configuration configuration)
        {
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), typeof(TConcrete), configuration));
        }

        public ITypeToResolve Resolve<ITypeToResolve>()
        {
            return (ITypeToResolve)ResolveObject(typeof(ITypeToResolve));
        }

        public object Resolve(Type typeToResolve)
        {
            return ResolveObject(typeToResolve);
        }

        private object ResolveObject(Type typeToResolve)
        {
            var registeredObject = _registeredObjects.FirstOrDefault(o => o.TypeToResolve == typeToResolve);

            if (registeredObject == null)
            {
                throw new Exception(string.Format(
                    "The type {0} has not been registered", typeToResolve.Name));
            }

            var decorateAttribute = registeredObject.ConcreteType.GetCustomAttribute(typeof(DecorateAttribute));
            var delegateAttribute = registeredObject.ConcreteType.GetCustomAttribute(typeof(DelegateAttribute));

            if ((registeredObject.Configuration == Configuration.Lazy || decorateAttribute != null || delegateAttribute != null)
                && registeredObject.Proxy == null)
            {
                var method = typeof(DynamicProxy).GetMethod("Create", BindingFlags.Static | BindingFlags.Public)
                                                 .MakeGenericMethod(new Type[] { registeredObject.TypeToResolve });

                registeredObject.Proxy = method.Invoke(null, new object[] { this, delegateAttribute, decorateAttribute });

                return registeredObject.Proxy;
            }

            return GetInstance(registeredObject);
        }

        private object GetInstance(RegisteredObject registeredObject)
        {
            if (registeredObject.Instance == null)
            {
                var parameters = ResolveConstructorParameters(registeredObject);
                registeredObject.CreateInstance(parameters.ToArray());

                var propertiesToSet = registeredObject.ConcreteType
                    .GetProperties()
                    .Where(x => x.CanWrite && _registeredObjects.Any(y => y.TypeToResolve == x.PropertyType));

                foreach (var property in propertiesToSet)
                {
                    property.SetValue(registeredObject.Instance, ResolveObject(property.PropertyType));
                }
            }
            return registeredObject.Instance;
        }

        private IEnumerable<object> ResolveConstructorParameters(RegisteredObject registeredObject)
        {
            var constructorInfo = registeredObject.ConcreteType.GetConstructors().First();
            foreach (var parameter in constructorInfo.GetParameters())
            {
                yield return ResolveObject(parameter.ParameterType);
            }
        }
    }
}
