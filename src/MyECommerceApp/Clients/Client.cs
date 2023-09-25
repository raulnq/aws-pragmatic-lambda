namespace MyECommerceApp.Clients
{
    public class Client
    {
        public Guid ClientId { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public Client(Guid clientId, string name, string address, string phoneNumber)
        {
            ClientId = clientId;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }
    }
}
