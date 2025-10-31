namespace aflevering7777
{
    public class OrderLine
    {
        public Item Item { get; }
        public int Quantity { get; }

        public OrderLine(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public decimal GetLinePrice() => Item.GetPrice() * Quantity;
    }
}