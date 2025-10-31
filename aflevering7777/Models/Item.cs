namespace aflevering7777
{
    public abstract class Item
    {
        public string Name { get; }
        /// <summary>1=a, 2=b, 3=c (bruges af robotten til x-koordinat)</summary>
        public uint InventoryLocation { get; set; }

        protected Item(string name, uint inventoryLocation = 0)
        {
            Name = name;
            InventoryLocation = inventoryLocation;
        }

        public abstract decimal GetPrice();
    }
}