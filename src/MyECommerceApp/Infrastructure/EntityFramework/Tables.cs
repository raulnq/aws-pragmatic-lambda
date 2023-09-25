namespace MyECommerceApp.Infrastructure.EntityFramework;

public static class Tables
{
    public static Table ClientRequests = new Table("ClientRequests");

    public static Table Clients = new Table("Clients");

    public static Table Products = new Table("Products");

    public static Table ShoppingCartItems = new Table("ShoppingCartItems");

    public static Table Orders = new Table("Orders");

    public static Table OrderItems = new Table("OrderItems");
}
