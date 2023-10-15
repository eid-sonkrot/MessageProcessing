using Microsoft.AspNetCore.SignalR.Client;

namespace MessageProcessing
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _hubConnection;

        public SignalRClient(string signalRHubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(signalRHubUrl)
                .Build();
        }
        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
        }
        public async Task SendAnomalyAlert(ServerStatistics data)
        {
            await _hubConnection.SendAsync("SendAnomalyAlert", data);
        }
        public async Task SendHighUsageAlert(ServerStatistics data)
        {
            await _hubConnection.SendAsync("SendHighUsageAlert", data);
        }
    }
}