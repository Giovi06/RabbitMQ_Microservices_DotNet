namespace MqLibrary
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMq";

        public string Uri { get; set; } = "localhost";
        public string ExchangeName { get; set; } = string.Empty;
        public string ClientProvidedName { get; set; } = string.Empty;
    }
}
