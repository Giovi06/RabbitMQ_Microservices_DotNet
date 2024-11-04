using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MqLibrary
{
    public class MqSender : IDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly IModel _channel;

        public MqSender(IOptions<RabbitMqOptions> options, IModel channel)
        {
            _options = options.Value;
            _channel = channel;

            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic);
        }

        public void SendAsJson<T>(string routingKey, T obj)
        {
            SendString(routingKey, JsonSerializer.Serialize(obj));
        }

        public void SendString(string routingKey, string message)
        {
            SendBytes(routingKey, Encoding.UTF8.GetBytes(message));
        }

        public void SendBytes(string routingKey, byte[] bytes)
        {
            _channel.BasicPublish(exchange: _options.ExchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: bytes);
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}
