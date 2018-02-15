using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqApp1
{
    class Program
    {
        static void Main()
        {
            var connection = GetConnection();
            Console.WriteLine($"Connection open: {connection.IsOpen }");

            var channel = connection.CreateModel();
            Console.WriteLine($"Channel open: {channel.IsOpen }");

            SetHeadersExchange(connection, channel);

            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));
            Console.WriteLine("Main done...");
            Console.ReadKey();
        }

        static void SetHeadersExchange(IConnection connection, IModel channel)
        {
            const string queue = "pt.southbank.queue.headers";
            const string exchange = "pt.southbank.exchange.headers";

            channel.ExchangeDeclare(exchange, ExchangeType.Headers, true, false, null);
            channel.QueueDeclare(queue, true, false, false, null);
            var headersMatchAll = new Dictionary<string, object>
            {
                {"x-match", "all"},
                {"category", "animal"},
                {"type", "mammal"}
            };

            channel.QueueBind(queue, exchange, string.Empty, headersMatchAll);

            var headersMatchAny = new Dictionary<string, object>
            {
                {"x-match", "any"},
                {"category", "plant"},
                {"type", "tree"}
            };
            channel.QueueBind(queue, exchange, string.Empty, headersMatchAny);

            var address = new PublicationAddress(ExchangeType.Headers, exchange, "");

            var properties = channel.CreateBasicProperties();
            var messageHeaders = new Dictionary<string, object>
            {
                {"category", "animal"},
                {"type", "insect"}
            };
            properties.Headers = messageHeaders;
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("Hello from the world of insects"));

            properties.Headers["category"] = "animal";
            properties.Headers["type"] = "mammal";
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("Hello from the world of mammals"));

            properties.Headers["category"] = "animal";
            properties.Headers.Remove("type");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("Hello from the world of animals"));

            properties.Headers["category"] = "fungi";
            properties.Headers["type"] = "champignon";
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("Hello from the world of champignons"));

            properties.Headers["category"] = "sad";
            properties.Headers["type"] = "tree";
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("Hello from the world of sad trees"));

            properties.Headers["category"] = "animal";
            properties.Headers["type"] = "mammal";
            properties.Headers["color"] = "pink";
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("Hello from the world of pink mammals"));

            channel.Close();
            connection.Close();

        }


        static void SetupDirectExchangeWithRoutingKey(IConnection connection, IModel channel)
        {


            channel.ExchangeDeclare("pt.southbank.routing.exchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("pt.southbank.routing.queue", true, false, false, null);
            channel.QueueDeclare("pt.southbank.routing.other", true, false, false, null);
            channel.QueueBind("pt.southbank.routing.queue", "pt.southbank.routing.exchange", "asia");
            channel.QueueBind("pt.southbank.routing.queue", "pt.southbank.routing.exchange", "americas");
            channel.QueueBind("pt.southbank.routing.queue", "pt.southbank.routing.exchange", "europe");
            channel.QueueBind("pt.southbank.routing.other", "pt.southbank.routing.exchange", "australia");

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            var address = new PublicationAddress(ExchangeType.Direct, "pt.southbank.routing.exchange", "asia");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("The latest news from Asia!"));

            address = new PublicationAddress(ExchangeType.Direct, "pt.southbank.routing.exchange", "europe");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("The latest news from Europe!"));

            address = new PublicationAddress(ExchangeType.Direct, "pt.southbank.routing.exchange", "americas");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("The latest news from the Americas!"));

            address = new PublicationAddress(ExchangeType.Direct, "pt.southbank.routing.exchange", "africa");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("The latest news from Africa!"));

            address = new PublicationAddress(ExchangeType.Direct, "pt.southbank.routing.exchange", "australia");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("The latest news from Australia!"));

            channel.Close();
            connection.Close();
        }



        static void SetupRpc(IModel channel)
        {
            channel.QueueDeclare("pt.southbank.queues.rpc", true, false, false, null);
            SendRpcMessagesBankAndForth(channel);
        }


        static void SendRpcMessagesBankAndForth(IModel channel)
        {
            var rpcResponseQueue = channel.QueueDeclare().QueueName;
            var correlationId = $"{Guid.NewGuid()}";

            var properties = channel.CreateBasicProperties();
            properties.ReplyTo = rpcResponseQueue;
            properties.CorrelationId = correlationId;
            Console.WriteLine("Enter your message and press Enter.");
            var message = Console.ReadLine();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(string.Empty, "pt.southbank.queues.rpc", properties, messageBytes);
            var rpcConsumer = new EventingBasicConsumer(channel);
            rpcConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {

                var responseFromConsumer = null as string;
                var props = basicDeliveryEventArgs.BasicProperties;
                if (props != null && props.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(basicDeliveryEventArgs.Body);
                    responseFromConsumer = response;
                }
                channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                Console.WriteLine("Response: {0}", responseFromConsumer);
                Console.WriteLine("Enter your message and press Enter.");
                message = Console.ReadLine();
                messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", "pt.southbank.queues.rpc", properties, messageBytes);
            };
            channel.BasicConsume(rpcResponseQueue, false, rpcConsumer);
        }





        static void SetupFanoutExchange(IConnection connection, IModel channel)
        {

            //declare the common exchange
            channel.ExchangeDeclare("pt.southbank.fanout.exchange", ExchangeType.Fanout, true, false, null);
            //declare queues
            channel.QueueDeclare("pt.southbank.queues.accounting", true, false, false, null);
            channel.QueueDeclare("pt.southbank.queues.management", true, false, false, null);
            //bind queues to exchange
            channel.QueueBind("pt.southbank.queues.accounting", "pt.southbank.fanout.exchange", string.Empty, null);
            channel.QueueBind("pt.southbank.queues.management", "pt.southbank.fanout.exchange", string.Empty, null);

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

