using AuthMicroService.Controllers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.EventBus;
using RabbitMQ.EventBus.Core;
using RabbitMQ.EventBus.Core.Models;
using System.Text;

namespace AuthMicroService.RPC
{
    public class RpcServer
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        public RpcServer(IRabbitMQPersistentConnection persistentConnection, IServiceScopeFactory factory, IServiceProvider serviceProvider)
        {
            _persistentConnection = persistentConnection;
            _serviceProvider = serviceProvider;
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
                ReceivedEventAsync(model, ea, channel);
            };
        }
        private async Task ReceivedEventAsync(object sender, BasicDeliverEventArgs ea, IModel channel)
        {
            string response = null;

            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                var data = JsonConvert.DeserializeObject<dynamic>(message);
                string result = null;

                if (ea.RoutingKey == $"{EventBusConstants.RdcPublishQueue}")
                {
                    //Business processes
                    using IServiceScope scope = _serviceProvider.CreateScope();
                    var authenticateController = scope.ServiceProvider.GetRequiredService<AuthenticateController>();

                    if (data.MicroServiceName != "AuthMicroService")
                    {
                        return;
                    }

                    switch (Convert.ToString(data.ServiceName))
                    {
                        case "ValidateToken":
                            result = authenticateController.ValidateToken(Convert.ToString(data.Message));
                            break;
                    }
                }
                response = JsonConvert.SerializeObject(new Response() { Success = true, Message = result });
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
