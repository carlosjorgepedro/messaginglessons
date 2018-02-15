using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Rpc.Receiver
{
    class Program
    {
        static void Main()
        {
            var channel = GetConnection().CreateModel();
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);


            consumer.Received += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body);
                channel.BasicAck(args.DeliveryTag, false);
                Console.WriteLine($"Message: {message}\nEnter your response: ");
                var response = Console.ReadLine();
                var responseBytes = Encoding.UTF8.GetBytes(response);
                var properties = channel.CreateBasicProperties();
                properties.CorrelationId = args.BasicProperties.CorrelationId;
                channel.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, properties, responseBytes);
            };

            channel.BasicConsume("pt.southbank.queues.rpc", false, consumer);
        }


        static IConnection GetConnection()
        {
            return new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "accounting",
                UserName = "accountant",
                Password = "accountant"
            }.CreateConnection();
        }
    }
}
