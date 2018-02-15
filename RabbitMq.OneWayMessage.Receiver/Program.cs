using RabbitMQ.Client;

namespace RabbitMq.OneWayMessage.Receiver
{
    class Program
    {
        static void Main()
        {
            ReceiveSingleOneWayMessage();
        }

        static void ReceiveSingleOneWayMessage()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "accounting",
                UserName = "accountant",
                Password = "accountant"
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            var basicConsumer = new OneWayMessageReceiver(channel);
            channel.BasicConsume("my.first.queue", false, basicConsumer);
        }
    }
}
