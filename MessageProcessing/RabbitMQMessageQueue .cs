using Serilog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace MessageProcessing
{
    public class RabbitMQMessageQueue : IMessageQueue
    {
        public void Subscribe(string topic, Action<ServerStatistics> messageHandler)
        {
            var factory = new ConnectionFactory
            {
                HostName = AppConfiguration.HostName,
                UserName = AppConfiguration.UserName,
                Password = AppConfiguration.Password,
            };
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Topics.ServerStatistics.ToString(), ExchangeType.Topic, durable: true);
                    channel.QueueDeclare(queue: topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: topic, exchange: Topics.ServerStatistics.ToString(), routingKey: topic);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = System.Text.Encoding.UTF8.GetString(body);
                        var statistics = DeserializeServerStatistics(message);

                        messageHandler(statistics);
                    };

                    channel.BasicConsume(queue: topic, autoAck: true, consumer: consumer);
                    Log.Information($"Subscribed to topic {topic}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error with Connection: {ex.Message}");
            }
        }
        private ServerStatistics DeserializeServerStatistics(string json)
        {
            return JsonSerializer.Deserialize<ServerStatistics>(json);
        }
    }
}
