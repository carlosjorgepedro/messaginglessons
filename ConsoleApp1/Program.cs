using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            var connection = GetConnection();
            Console.WriteLine($"Connection open: {connection.IsOpen }");

            var channel = connection.CreateModel();
            Console.WriteLine($"Channel open: {channel.IsOpen }");

            SetupFanoutExchange(connection, channel);

            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));
            Console.WriteLine("Main done...");
            Console.ReadKey();
        }


        static void SetupFanoutExchange(IConnection connection, IModel channel)
        {

            //declare the common exchange
            channel.ExchangeDeclare("pt.southbank.fanout.exchange", ExchangeType.Fanout, true, false, null);
            //declare queues
            channel.QueueDeclare("pt.southbank.queues.accounting", true, false, false, null);
            channel.QueueDeclare("pt.southbank.queues.management", true, false, false, null);
            //bind queues to exchange
            channel.QueueBind("pt.southbank.queues.accounting","pt.southbank.fanout.exchange", string.Empty, null);
            channel.QueueBind("pt.southbank.queues.management","pt.southbank.fanout.exchange", string.Empty, null);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            var address = new PublicationAddress(ExchangeType.Fanout, "pt.southbank.fanout.exchange", "null");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("A new huge order has just come in worth $1M!!!!!"));

            channel.Close();
            connection.Close();
        }


        static IConnection GetConnection()
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
            return connection;
        }

        static void SetUpDirectExchange(IConnection connection, IModel channel)
        {
            //Create exchange
            channel.ExchangeDeclare("my.first.exchange", ExchangeType.Direct, true, false, null);
            //Create Queue where messsages should be stored
            channel.QueueDeclare("my.first.queue", true, false, false, null);
            //Link queue to exchange 
            channel.QueueBind("my.first.queue", "my.first.exchange", string.Empty);


            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            var address = new PublicationAddress(ExchangeType.Direct, "my.first.exchange", string.Empty);
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("this is my first message."));


            channel.Close();
            connection.Close();
        }
    }
}

