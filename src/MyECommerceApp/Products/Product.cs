using MyECommerceApp.Infrastructure.ExceptionHandling;

namespace MyECommerceApp.Products;

public class Product
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsEnabled { get; private set; }
    public Product(Guid productId, string name, string description, decimal price, bool any)
    {
        if (any)
        {
            throw new DomainException(ExceptionCodes.Duplicated);
        }
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        IsEnabled = false;
    }

    public Product()
    {

    }
    public void Enable()
    {
        IsEnabled = true;
    }
}

