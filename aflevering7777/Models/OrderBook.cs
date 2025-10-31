using System.Collections.Generic;
using System.Linq;

namespace aflevering7777
{
    /// <summary>Enkel ordrekø til GUI’en.</summary>
    public class OrderBook
    {
        private readonly Queue<Order> _queue = new();

        public void AddOrder(Order order) => _queue.Enqueue(order);

        /// <summary>Returnér linjerne for næste ordre i køen (og fjern ordren fra køen).</summary>
        public IEnumerable<OrderLine> ProcessNextOrder()
        {
            if (_queue.Count == 0) yield break;
            var next = _queue.Dequeue();
            foreach (var line in next.OrderLines) yield return line;
        }

        public bool HasOrders => _queue.Any();
    }
}