using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Text.Json;

namespace MyECommerceApp.Infrastructure.Messaging
{
    public class EventPublisher
    {
        private readonly IAmazonSimpleNotificationService _snsClient;

        private readonly string _topicArn;

        public EventPublisher(IAmazonSimpleNotificationService snsClient, string topicArn)
        {
            _snsClient = snsClient;
            _topicArn = topicArn;
        }

        public Task Publish<T>(T message)
        {
            var @event = new PublishRequest
            {
                TopicArn = _topicArn,
                Message = JsonSerializer.Serialize(message),
            };

            @event.MessageAttributes.Add("event", new MessageAttributeValue() { StringValue = typeof(T).Name.ToLower(), DataType = "String" });

            return _snsClient.PublishAsync(@event);
        }
    }
}
