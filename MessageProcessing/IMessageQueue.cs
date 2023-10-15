namespace MessageProcessing
{
    public interface IMessageQueue
    {
        void Subscribe(string topic, Action<ServerStatistics> message);
    }
}
