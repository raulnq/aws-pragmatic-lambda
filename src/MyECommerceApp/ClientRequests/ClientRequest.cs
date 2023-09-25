using MyECommerceApp.Infrastructure.ExceptionHandling;

namespace MyECommerceApp.ClientRequests
{
    public class ClientRequest
    {
        public Guid ClientRequestId { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public ClientRequestStatus Status { get; private set; }
        public DateTimeOffset RegisteredAt { get; private set; }
        public DateTimeOffset? ApprovedAt { get; private set; }
        public DateTimeOffset? RejectedAt { get; private set; }
        private ClientRequest()
        {

        }
        public ClientRequest(Guid clientRequestId, string name, string address, string phoneNumber, bool any)
        {
            if(any)
            {
                throw new DomainException(ExceptionCodes.Duplicated);
            }
            ClientRequestId = clientRequestId;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Status = ClientRequestStatus.Pending;
            RegisteredAt = DateTimeOffset.UtcNow;
        }

        public void Approve()
        {
            ApprovedAt = DateTimeOffset.UtcNow;
            Status = ClientRequestStatus.Approved;
        }

        public void Reject()
        {
            ApprovedAt = DateTimeOffset.UtcNow;
            Status = ClientRequestStatus.Rejected;
        }
    }

    public enum ClientRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public record ClientRequestApproved(Guid ClientRequestId);
}
