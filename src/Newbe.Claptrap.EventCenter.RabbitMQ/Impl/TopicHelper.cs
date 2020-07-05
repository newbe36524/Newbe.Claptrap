namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public static class TopicHelper
    {
        public static string GetExchangeName(string claptrapTypeCode)
            => $"claptrap.{claptrapTypeCode}.event";

        public static string GetQueueName(string claptrapTypeCode)
            => $"claptrap.{claptrapTypeCode}.event";

        public static string GetRouteKey(string claptrapTypeCode, string eventTypeCode)
            => $"claptrap.{claptrapTypeCode}.event.{eventTypeCode}";

        public static string GetSubscribeKey(string claptrapTypeCode)
            => $"claptrap.{claptrapTypeCode}.event.*";
    }
}