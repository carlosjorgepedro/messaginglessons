using System;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.OneWayMessage.Receiver
{
    class Program
    {
        static IModel _channelForEventing;

        static void Main()
        {
            ReceiveMessageWithEvents();
            //            ReceiveSingleOneWayMessage();
        }

        static ConnectionFactory GetConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "accounting",
                UserName = "accountant",
                Password = "accountant"
            };
        }


        static void ReceiveSingleOneWayMessage()
        {
            var connection = GetConnectionFactory().CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            var basicConsumer = new OneWayMessageReceiver(channel);
            channel.BasicConsume("my.first.queue", false, basicConsumer);
        }

        static void ReceiveMessageWithEvents()
        {
            var connection = GetConnectionFactory().CreateConnection();
            _channelForEventing = connection.CreateModel();
            _channelForEventing.BasicQos(0, 1, false);
            var eventingConsumer = new EventingBasicConsumer(_channelForEventing);
            eventingConsumer.Received += (sender, args) =>
            {
                var basicProperties = args.BasicProperties;
                Console.WriteLine("Message received by the event based consumer. Check the debug window for details.");
                Debug.WriteLine(string.Concat("Message received from the exchange ", args.Exchange));
                Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
                Debug.WriteLine(string.Concat("Consumer tag: ", args.ConsumerTag));
                Debug.WriteLine(string.Concat("Delivery tag: ", args.DeliveryTag));
                Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(args.Body)));
                _channelForEventing.BasicAck(args.DeliveryTag, false);
            };
            _channelForEventing.BasicConsume("my.first.queue", false, eventingConsumer);
        }
    }
}
