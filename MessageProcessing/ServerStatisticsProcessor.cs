using Microsoft.Extensions.Hosting;
using Serilog;
using System.Text.Json;

namespace MessageProcessing
{
    public class ServerStatisticsProcessor : IHostedService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IRepository _repository;
        private readonly ISignalRClient _signalRClient;
        private readonly double _mb = 1024.0 * 1024.0;
        private ServerStatistics _previousStatistics;

        public ServerStatisticsProcessor(IMessageQueue messageQueue,IRepository repository,ISignalRClient signalRClient)
        {
            _messageQueue = messageQueue;
            _repository = repository;
            _signalRClient = signalRClient;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _messageQueue.Subscribe("ServerStatistics", async message =>
            {
                try
                {
                    // Persist the received server statistics using the repository
                    await _repository.InsertAsync(message);
                    // Perform anomaly detection and alerting
                    await DetectAndSendAlerts(message);
                    _previousStatistics = message;
                }
                catch (Exception ex)
                {
                    Log.Error($"Error processing server statistics: {ex.Message}");
                }
            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        private async Task DetectAndSendAlerts(ServerStatistics currentStatistics)
        {
            if (_previousStatistics is not null)
            {
                if (IsMemoryAnomaly(currentStatistics, _previousStatistics))
                {
                    await _signalRClient.SendAnomalyAlert(currentStatistics);
                }

                if (IsCpuAnomaly(currentStatistics, _previousStatistics))
                {
                    await _signalRClient.SendAnomalyAlert(currentStatistics);
                }
            }
            if (IsMemoryHighUsage(currentStatistics) || IsCpuHighUsage(currentStatistics))
            {
                await _signalRClient.SendHighUsageAlert(currentStatistics);
            }
        }
        private bool IsMemoryAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            var memoryUsageThreshold = previous.MemoryUsage * (1 + AppConfiguration.MemoryUsageAnomalyThresholdPercentage);

            return current.MemoryUsage > memoryUsageThreshold;
        }
        private bool IsCpuAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            var cpuUsageThreshold = previous.CpuUsage * (1 + AppConfiguration.CpuUsageAnomalyThresholdPercentage);

            return current.CpuUsage > cpuUsageThreshold;
        }
        private bool IsMemoryHighUsage(ServerStatistics statistics)
        {
            var memoryUsagePercentage = statistics.MemoryUsage / (statistics.MemoryUsage + statistics.AvailableMemory);

            return memoryUsagePercentage > AppConfiguration.MemoryUsageThresholdPercentage;
        }
        private bool IsCpuHighUsage(ServerStatistics statistics)
        {
            return statistics.CpuUsage > AppConfiguration.CpuUsageThresholdPercentage;
        }
    }
}
