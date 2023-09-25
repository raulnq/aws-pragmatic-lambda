using MyECommerceApp.Infrastructure.ExceptionHandling;

namespace MyECommerceApp.ShoppingCart;

public class ShoppingCartItem
{
    public Guid ShoppingCartItemId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }

    public ShoppingCartItem(Guid shoppingCartItemId, Guid clientId, Guid productId, decimal quantity, bool any)
    {
        if (any)
        {
            throw new DomainException(ExceptionCodes.Duplicated);
        }
        ShoppingCartItemId = shoppingCartItemId;
        ClientId= clientId;
        ProductId = productId;
        Quantity= quantity;
    }

    public ShoppingCartItem()
    {

    }
}
