namespace aflevering7777
{
    public class UnitItem : Item
    {
        private decimal PricePerUnit;

        public UnitItem(string name, decimal pricePerUnit, uint inventoryLocation = 0)
            : base(name, inventoryLocation)
        {
            PricePerUnit = pricePerUnit;
        }

        public override decimal GetPrice() => PricePerUnit;
    }
}