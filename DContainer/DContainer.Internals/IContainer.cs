using System;

namespace DContainer.Internals
{
    public interface IContainer
    {
        void Register<TTypeToResolve, TConcrete>();
        void Register<TTypeToResolve, TConcrete>(Configuration configuration);
        ITypeToResolve Resolve<ITypeToResolve>();
        object Resolve(Type typeToResolve);
    }
}
