using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sample.Messages;

namespace MassTransit.Receiver
{
    public class RegisterCustomerConsumer : IConsumer<IRegisterCustomer>, IConsumer<IRegisterCustomer2>
    {
        public Task Consume(ConsumeContext<IRegisterCustomer> context)
        {
            var newCustomer = context.Message;
            Console.WriteLine("A new customer has signed up, it's time to register it. Details: ");
            Console.WriteLine(newCustomer.Address);
            Console.WriteLine(newCustomer.Name);
            Console.WriteLine(newCustomer.Id);
            Console.WriteLine(newCustomer.RegisteredUtc);


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
            Console.WriteLine(newCustomer.Address);
            Console.WriteLine(newCustomer.Name);
            Console.WriteLine(newCustomer.Id);
            Console.WriteLine(newCustomer.RegisteredUtc);


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
}