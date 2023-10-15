using Microsoft.Extensions.Configuration;

namespace MessageProcessing
{
    public static class AppConfiguration
    {
        public static string HostName { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string ServerIdentifier { get; set; }
        public static string signalRHubUrl {get; set;}
        public static double MemoryUsageThresholdPercentage { get; set; }
        public static double MemoryUsageAnomalyThresholdPercentage { get; set; }
        public static double CpuUsageAnomalyThresholdPercentage { get; set; }
        public static double CpuUsageThresholdPercentage { get; set; }
        public static string ConnectionString { get; set; }
        public static string DatabaseName { get; set; }
        public static string CollectionName { get; set; }
        public static void Load(IConfiguration configuration)
        {
            HostName = configuration[$"{ConfigurationItem.MessageQueue.ToString()}:{MessageQueueItem.HostName.ToString()}"];
            UserName = configuration[$"{ConfigurationItem.MessageQueue}:{MessageQueueItem.UserName}"];
            Password = configuration[$"{ConfigurationItem.MessageQueue}:{MessageQueueItem.Password}"];
            ServerIdentifier = configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{ConfigurationItem.ServerIdentifier}"];
            MemoryUsageThresholdPercentage = double.Parse(configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{Thresholds.MemoryUsageThresholdPercentage}"]);
            MemoryUsageAnomalyThresholdPercentage= double.Parse(configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{Thresholds.MemoryUsageAnomalyThresholdPercentage}"]);
            CpuUsageAnomalyThresholdPercentage= double.Parse(configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{Thresholds.CpuUsageAnomalyThresholdPercentage}"]);
            CpuUsageThresholdPercentage= double.Parse(configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{Thresholds.CpuUsageThresholdPercentage}"]);
            signalRHubUrl = configuration[$"{ConfigurationItem.SignalRConfig}:SignalRUrl"];
            ConnectionString = configuration["ConnectionString"];
            DatabaseName = configuration["DatabaseName"];
            CollectionName = configuration["CollectionName"];
        }
    }
}