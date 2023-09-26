namespace MyECommerceApp.Orders;

public enum OrderStatus
{
    Pending,
    Complete,
}

public enum PaymentMethod
{
    Cash,
    CreditCard
}

public class Order
{
    public Guid OrderId { get; private set; }
    public Guid ClientId { get; private set; }
    public string Address { get; private set; }
    public List<Item> Items { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTimeOffset DeliveryDate { get; private set; }

    public class Item
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
        public string Name { get; private set; }

        public Item(Guid orderId, Guid productId, string name, decimal quantity, decimal price)
        {
            OrderId = orderId;
            ProductId = productId;
            Name = name;
            Quantity = quantity;
            Price = price;
        }

        public Item()
        {

        }
    }

    public Order()
    {

    }

    public Order(
        Guid orderId, 
        PaymentMethod paymentMethod,
        DateTimeOffset deliveryDate,
        Guid clientId, 
        string address, 
        (Guid ProductId, string Name, decimal Price, decimal Quantity)[] items)
    {
        OrderId = orderId;
        ClientId = clientId;
        Address = address;
        PaymentMethod= paymentMethod;
        DeliveryDate = deliveryDate;
        Items = new List<Item>();
        foreach (var (ProductId, Name, Price, Quantity) in items)
        {
            AddItem(ProductId, Name, Price, Quantity);
        }
        Total = Items.Sum(item => item.Price*item.Quantity);
        Status = OrderStatus.Pending;
    }

    private void AddItem(Guid productId, string name, decimal price, decimal quantity)
    {
        Items.Add(new Item(OrderId, productId, name, price, quantity));
    }
}

public record OrderRegistered( Guid OrderId, Guid ClientId);
