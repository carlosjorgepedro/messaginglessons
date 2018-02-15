using System;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Fanout.Receiver.Management
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiveFanoutMessages();
        }

        static void ReceiveFanoutMessages()
        {
            var connection = GetConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            var eventingConsumer = new EventingBasicConsumer(channel);
            eventingConsumer.Received += (sender, args) =>
            {
                var basicProperties = args.BasicProperties;

                Debug.WriteLine(string.Concat("Message received from the exchange ", args.Exchange));
                Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
                Debug.WriteLine(string.Concat("Consumer tag: ", args.ConsumerTag));
                Debug.WriteLine(string.Concat("Delivery tag: ", args.DeliveryTag));
                var message = Encoding.UTF8.GetString(args.Body);
                Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(args.Body)));
                Console.WriteLine(string.Concat("Message received by the management consumer: ", message));
                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicConsume("pt.southbank.queues.management", false, eventingConsumer);
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
