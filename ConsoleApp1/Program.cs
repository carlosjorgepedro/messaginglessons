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
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "accounting",
                UserName = "accountant",
                Password = "accountant"
            };

            var connection = connectionFactory.CreateConnection();
            Console.WriteLine($"Connection open: {connection.IsOpen }");


            var channel = connection.CreateModel();
            Console.WriteLine($"Channel open: {channel.IsOpen }");


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
            channel.BasicPublish(address,  properties, Encoding.UTF8.GetBytes("this is my first message."));
            

            channel.Close();
            connection.Close();

            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));
            Console.WriteLine("Main done...");
            Console.ReadKey();
        }
    }
}
