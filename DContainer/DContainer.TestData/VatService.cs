namespace DContainer.TestData
{
    public class VatService : IVatService
    {
        private const decimal VatPercentageValue = 22;

        public decimal VatPercentage => VatPercentageValue;

        public VatService() { }
    }
}
