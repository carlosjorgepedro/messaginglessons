using System;

namespace Sample.Messages
{
    public interface IRegisterCustomer
    {
        Guid Id { get; }
        DateTime RegisteredUtc { get; }
        int Type { get; }
        string Name { get; }
        bool Preferred { get; }
        decimal DefaultDiscount { get; }
        string Address { get; }
    }

    public interface IRegisterCustomer2
    {
        Guid Id { get; }
        DateTime RegisteredUtc { get; }
        int Type { get; }
        string Name { get; }
        bool Preferred { get; }
        decimal DefaultDiscount { get; }
        string Address { get; }
    }

    public interface ICustomerRegistered
    {
        Guid Id { get; }
        DateTime RegisteredUtc { get; }
        string Name { get; }
        string Address { get; }
    }
}
