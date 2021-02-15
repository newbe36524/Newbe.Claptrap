namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public static class TopicHelper
    {
        public static string GetExchangeName(string claptrapTypeCode)
        {
            return $"claptrap.{claptrapTypeCode}.event";
        }

        public static string GetQueueName(string claptrapTypeCode)
        {
            return $"claptrap.{claptrapTypeCode}.event";
        }

        public static string GetRouteKey(string claptrapTypeCode, string eventTypeCode)
        {
            return $"claptrap.{claptrapTypeCode}.event.{eventTypeCode}";
        }

        public static string GetSubscribeKey(string claptrapTypeCode)
        {
            return $"claptrap.{claptrapTypeCode}.event.*";
        }
    }
}