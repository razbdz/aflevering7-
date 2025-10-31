using System.Collections.Generic;
using System.Linq;

namespace aflevering7777
{
    public class Order
    {
        public Customer Customer { get; }
        public List<OrderLine> OrderLines { get; } = new();

        public Order(Customer customer) => Customer = customer;

        public void AddOrderLine(OrderLine line) => OrderLines.Add(line);

        public decimal GetTotalOrderPrice() =>
            OrderLines.Sum(l => l.GetLinePrice());
    }
}