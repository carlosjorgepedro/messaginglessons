using System;
using Configuration;
using Sample.Messages;

namespace MassTransit.Publisher
{
    class Program
    {
        static void Main()
        {
            RunMassTransitPublisher();
        }


        static void RunMassTransitPublisher()
        {
            var busSettings = new BusSettings();
            var busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
                {
                    rabbit.Host(new Uri(busSettings.HostUrl), settings =>
                    {
                        settings.Password(busSettings.Password);
                        settings.Username(busSettings.UserName);
                    });
                });

            ISendEndpoint EndPointFunc()
            {
                var task = busControl.GetSendEndpoint(new Uri($"{busSettings.HostUrl}/{busSettings.Queue}"));
                task.Wait();
                return task.Result;
            }

            var endPoint = EndPointFunc();
            endPoint.Send<IRegisterCustomer>(new
            {
                Address = "New Street",
                Id = Guid.NewGuid(),
                Preferred = true,
                RegisteredUtc = DateTime.UtcNow,
                Name = "Nice people LTD",
                Type = 1,
                DefaultDiscount = 0
            }).Wait();

            Console.ReadKey();
        }
    }

}

