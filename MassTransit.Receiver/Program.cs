using System;
using Configuration;
using Sample.Messages;

namespace MassTransit.Receiver
{
    class Program
    {
        static void Main()
        {
            Console.Title = "This is the customer registration command receiver.";
            Console.WriteLine("CUSTOMER REGISTRATION COMMAND RECEIVER.");
            RunMassTransitReceiverWithRabbit();
        }

        static void RunMassTransitReceiverWithRabbit()
        {
            var hostSettings = new BusSettings();
            var busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var host = rabbit.Host(new Uri(hostSettings.HostUrl),
                    settings =>
                    {
                        settings.Password(hostSettings.Password);
                        settings.Username(hostSettings.UserName);
                    });

                rabbit.ReceiveEndpoint(host, hostSettings.Queue , conf =>
                {
                    
                    conf.Consumer<RegisterCustomerConsumer>();
                });
            });

            busControl.Start();
            Console.ReadKey();
            busControl.Stop();
        }
    }
}
