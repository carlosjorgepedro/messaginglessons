using System;
using System.Threading.Tasks;
using Configuration;
using Sample.Messages;

namespace MassTransit.AnotherReceiver
{
    public class RegisterCustomerConsumer : IConsumer<IRegisterCustomer>, IConsumer<IRegisterCustomer2>
    {
        public Task Consume(ConsumeContext<IRegisterCustomer> context)
        {
            var newCustomer = context.Message;
            Console.WriteLine("A new customer has signed up, it's time to register it. Details: ");
            Console.WriteLine(newCustomer.Id);


            context.Publish<ICustomerRegistered>(new
            {
                newCustomer.Id,
                newCustomer.Address,
                newCustomer.RegisteredUtc,
                newCustomer.Name
            });

            return Task.FromResult(context.Message);

        }


        public Task Consume(ConsumeContext<IRegisterCustomer2> context)
        {
            var newCustomer = context.Message;
            Console.WriteLine("A new customer has signed up, it's time to register it. Details: ");


            Console.WriteLine(newCustomer.Id);



            context.Publish<ICustomerRegistered>(new
            {
                newCustomer.Id,
                newCustomer.Address,
                newCustomer.RegisteredUtc,
                newCustomer.Name
            });

            return Task.FromResult(context.Message);

        }
    }

    class Program
    {
        static void Main()
        {
            Console.Title = "This is the other customer registration command receiver.";
            Console.WriteLine("CUSTOMER REGISTRATION COMMAND RECEIVER2.");
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

                rabbit.ReceiveEndpoint(host, hostSettings.Queue,
                    conf => { conf.Consumer<RegisterCustomerConsumer>(); });
            });

            busControl.Start();
            Console.ReadKey();
            busControl.Stop();
        }
    }
}
