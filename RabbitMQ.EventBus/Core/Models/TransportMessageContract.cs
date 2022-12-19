namespace RabbitMQ.EventBus.Core.Models
{
    public class TransportMessageContract<T>
    {
        public string MicroServiceName { get; set; }
        public string ServiceName { get; set; }
        public T Message { get; set; }
    }
}
