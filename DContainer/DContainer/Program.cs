using DContainer.Internals;
using DContainer.TestData;
using System;

namespace DContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = new SimpleContainer();

            container.Register<IVatService, VatService>();
            container.Register<ITaxService, OrdinalTaxService>(Configuration.Lazy);
            container.Register<ILegislationService, LegislationService>();

            ITaxService service = container.Resolve<ITaxService>();

            Console.WriteLine(service.PrepareReport());
            Console.ReadKey();
        }
    }
}
