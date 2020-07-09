namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IMessageSerializer
    {
        byte[] Serialize(string source);
        string Deserialize(byte[] bytes);
    }
}