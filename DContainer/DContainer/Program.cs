using DContainer.Internals;
using DContainer.TestData;
using DContainer.TestData.Decorators;
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
            container.Register<Logger, Logger>();
            container.Register<IExporter, DataExporter>();

            ITaxService service = container.Resolve<ITaxService>();

            Console.WriteLine(service.PrepareReport());
            Console.WriteLine("Once more:");
            Console.WriteLine(service.PrepareReport());

            var exportResult = ((IExporter)service).Export();
            Console.WriteLine(exportResult);

            Console.ReadKey();
        }
    }
}
