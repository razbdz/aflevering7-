namespace aflevering7777
{
    public class BulkItem : Item
    {
        private decimal PricePerUnit;
        public int Minimum { get; }

        public BulkItem(string name, decimal pricePerUnit, int minimum)
            : base(name, 0)
        {
            PricePerUnit = pricePerUnit;
            Minimum = minimum;
        }

        public BulkItem(string name, decimal pricePerUnit, uint inventoryLocation, int minimum)
            : base(name, inventoryLocation)
        {
            PricePerUnit = pricePerUnit;
            Minimum = minimum;
        }

        public override decimal GetPrice() => PricePerUnit;
    }
}