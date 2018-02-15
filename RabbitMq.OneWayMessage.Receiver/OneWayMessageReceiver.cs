using System;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMq.OneWayMessage.Receiver
{
    public class OneWayMessageReceiver : DefaultBasicConsumer
    {
        readonly IModel _channel;
        public OneWayMessageReceiver(IModel channel)
        {
            _channel = channel;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            Console.WriteLine("Message received by the consumer. Check the debug window for details.");
            Debug.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Debug.WriteLine(string.Concat("Content type: ", properties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body)));
            //_channel.BasicAck(deliveryTag, false);
        }
    }
}