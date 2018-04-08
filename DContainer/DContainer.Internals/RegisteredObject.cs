using System;
using System.Collections.Generic;
using System.Linq;

namespace DContainer.Internals
{
    public class RegisteredObject
    {
        public Configuration Configuration { get; set; }

        public object Proxy { get; set; }
        public object Instance { get; set; }

        public Type TypeToResolve { get; set; }
        public Type ConcreteType { get; set; }

        public RegisteredObject(Type typeToResolve, Type concreteType, Configuration configuration)
        {
            Configuration = configuration;
            TypeToResolve = typeToResolve;
            ConcreteType = concreteType;
        }

        public void CreateInstance(IEnumerable<object> parameters)
        {
            Instance = ConcreteType.GetConstructors().First().Invoke(parameters.ToArray());
        }
    }
}
