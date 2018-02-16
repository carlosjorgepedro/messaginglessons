namespace Configuration
{
    public class BusSettings
    {
        public string HostUrl => "rabbitmq://localhost:5672/accounting";
        public string UserName => "accountant";
        public string Password => "accountant";
        public string Queue => "mycompany.domains.queues";

    }
}
