using System;
using System.Collections.Generic;

namespace aflevering7777
{
    public class Inventory
    {
        private List<Item> _items = new List<Item>();

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public void PrintInventory()
        {
            Console.WriteLine("📦 Lagerstatus:");
            foreach (var item in _items)
            {
                Console.WriteLine("- " + item.Name + " | Pris: " + item.GetPrice() + " kr.");
            }
        }
    }
}
