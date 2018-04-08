namespace DContainer.TestData
{
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
            return $"REPORT:\nLegislation: {LegislationService.GetSummary()}\nVat: {_vatService.VatPercentage}\n";
        }
    }
}
