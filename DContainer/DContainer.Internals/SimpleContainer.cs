using System;
using System.Collections.Generic;
using System.Linq;

namespace DContainer.Internals
{
    public class SimpleContainer : IContainer
    {
        private readonly List<RegisteredObject> registeredObjects = new List<RegisteredObject>();

        public void Register<TTypeToResolve, TConcrete>()
        {
            Register<TTypeToResolve, TConcrete>(Configuration.Eager);
        }

        public void Register<TTypeToResolve, TConcrete>(Configuration configuration)
        {
            registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), typeof(TConcrete), configuration));
        }

        public ITypeToResolve Resolve<ITypeToResolve>()
        {
            return (ITypeToResolve)(object)ResolveObject(typeof(ITypeToResolve));
        }

        public object Resolve(Type typeToResolve)
        {
            return ResolveObject(typeToResolve);
        }

        private RegisteredObject GetRegisteredObject(Type typeToResolve)
        {
            var registeredObject = registeredObjects.FirstOrDefault(o => o.TypeToResolve == typeToResolve);

            if (registeredObject == null)
            {
                throw new Exception(string.Format(
                    "The type {0} has not been registered", typeToResolve.Name));
            }

            return registeredObject;
        }

        private object ResolveObject<T>()
        {
            var registeredObject = GetRegisteredObject(typeof(T));

            if (registeredObject.Configuration == Configuration.Lazy)
            {
                return DynamicProxy.Create(registeredObject.TypeToResolve, this);
            }

            return GetInstance(registeredObject);
        }

        private object ResolveObject(Type typeToResolve)
        {
            var registeredObject = registeredObjects.FirstOrDefault(o => o.TypeToResolve == typeToResolve);

            if (registeredObject == null)
            {
                throw new Exception(string.Format(
                    "The type {0} has not been registered", typeToResolve.Name));
            }

            if (registeredObject.Configuration == Configuration.Lazy)
            {
                return DynamicProxy.Create(registeredObject.TypeToResolve, this);
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
                    .Where(x => x.CanWrite && registeredObjects.Any(y => y.TypeToResolve == x.PropertyType));

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
