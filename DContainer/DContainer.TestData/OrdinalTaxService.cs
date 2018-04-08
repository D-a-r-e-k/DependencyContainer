using DContainer.Internals.Attributes;
using DContainer.TestData.Decorators;

namespace DContainer.TestData
{
    [Decorate(typeof(Logger), "Log", "Log")]
    [Delegate(typeof(IExporter), typeof(DataExporter))]
    public class OrdinalTaxService : ITaxService
    {
        private readonly IVatService _vatService;

        public ILegislationService LegislationService { get; set; }

        public OrdinalTaxService(IVatService vatService)
        {
            _vatService = vatService;
        }

        public string PrepareReport()
        {
            return $"\nREPORT:\nLegislation: {LegislationService.GetSummary()}\nVat: {_vatService.VatPercentage}\n";
        }
    }
}
