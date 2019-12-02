namespace Scorpio.Api
{
    public class RabbitMqConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }
        public string MyQueueName { get; set; }
        public int MessageTTL { get; set; }
    }

    public class MongoDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
