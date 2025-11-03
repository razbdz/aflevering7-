namespace aflevering7777
{
    public abstract class Item
    {
        public string Name { get; }
       
        public uint InventoryLocation { get; set; }

        protected Item(string name, uint inventoryLocation = 0)
        {
            Name = name;
            InventoryLocation = inventoryLocation;
        }

        public abstract decimal GetPrice();
    }
}