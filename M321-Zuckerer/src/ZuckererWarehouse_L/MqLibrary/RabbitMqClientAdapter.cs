using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqLibrary
{
    public class RabbitMqClientAdapter
    {
        private readonly RabbitMqOptions _options;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;

        public RabbitMqClientAdapter(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            _connectionFactory = new()
            {
                Uri = new Uri(_options.Uri),
                ClientProvidedName = _options.ClientProvidedName
            };
            _connection = _connectionFactory.CreateConnection();
        }

        public IModel CreateModel() => _connection.CreateModel();
        
    }
}
