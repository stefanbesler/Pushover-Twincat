using Pushover.Twincat;

namespace Pushover
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var server = new PushoverService(25733, "Twinpack Pushover Server", _logger);
                await server.ConnectServerAndWaitAsync(stoppingToken);
                await Task.Delay(10000);
            }
        }
    }
}
