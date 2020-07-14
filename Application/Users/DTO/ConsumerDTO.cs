
namespace Application.Users
{
    public class ConsumerDTO: User
    {
        public string ConsumerId { get; set; }
        public string TypeOfConsumer { get; set; }
        public string MeterId { get; set; }
        public string MeterCapacity { get; set; }
    }
}
