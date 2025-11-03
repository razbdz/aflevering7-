using System.Collections.Generic;
using System.Linq;

namespace aflevering7777
{
   
    public class OrderBook
    {
        private readonly Queue<Order> _queue = new();

        public void AddOrder(Order order) => _queue.Enqueue(order);

   
        public IEnumerable<OrderLine> ProcessNextOrder()
        {
            if (_queue.Count == 0) yield break;
            var next = _queue.Dequeue();
            foreach (var line in next.OrderLines) yield return line;
        }

        public bool HasOrders => _queue.Any();
    }
}