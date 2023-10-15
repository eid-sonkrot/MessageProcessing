namespace MessageProcessing
{
    public interface ISignalRClient
    {
        Task SendAnomalyAlert(ServerStatistics data);
        Task SendHighUsageAlert(ServerStatistics data);
    }
}
