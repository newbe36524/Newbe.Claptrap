using System.Text;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class MessageSerializer : IMessageSerializer
    {
        public byte[] Serialize(string source)
        {
            var re = Encoding.UTF8.GetBytes(source);
            return re;
        }

        public string Deserialize(byte[] bytes)
        {
            var re = Encoding.UTF8.GetString(bytes);
            return re;
        }
    }
}