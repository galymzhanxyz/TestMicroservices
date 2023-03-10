using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.EventBus;
using RabbitMQ.EventBus.Core;
using RabbitMQ.EventBus.Core.Models;
using System.Text;

namespace BlogMicroService.RPC
{
    public class RpcServer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        public RpcServer(IRabbitMQPersistentConnection persistentConnection, IServiceScopeFactory factory)
        {
            _persistentConnection = persistentConnection;
        }
        public void Consume(string queue)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            var channel = _persistentConnection.CreateModel();

            var args = new Dictionary<string, object>();
            //args.Add("x-single-active-consumer", true);

            channel.QueueDeclare(
                   queue: queue,
                   durable: true,
                   exclusive: false,
                   autoDelete: false,
                   arguments: args);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                ReceivedEvent(model, ea, channel);
            };
        }
        private void ReceivedEvent(object sender, BasicDeliverEventArgs ea, IModel channel)
        {
            string response = null;

            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                var data = JsonConvert.DeserializeObject<dynamic>(message);

                if (ea.RoutingKey == $"{EventBusConstants.RdcPublishQueue}")
                {
                    //Business processes
                    if (data.MicroServiceName != nameof(BlogMicroService))
                    {
                        return;
                    }
                }
                response = JsonConvert.SerializeObject(new Response() { Success = true, Message = "Message received from server" });
            }
            catch (Exception ex)
            {
                //logging
                response = JsonConvert.SerializeObject(new Response() { Success = false, Message = "Failure" });
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response ?? string.Empty);
                channel.BasicPublish(
                    exchange: "",
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        }
        public void Disconnect()
        {
            _persistentConnection.Dispose();
        }
    }
}
