
using WebSearchLink.Service;

namespace WebSearchLink.Models
{
    public class ScheduledUploadService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduledUploadService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1.5);
        public ScheduledUploadService(IServiceProvider serviceProvider, ILogger<ScheduledUploadService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) // Added 'async' keyword
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var zoomService = scope.ServiceProvider.GetRequiredService<IZoomService>();
                var youtubeService = scope.ServiceProvider.GetRequiredService<IYouTubeService>();
                try
                {
                    // 1. Download Zoom
                    await zoomService.GetNewRecordingsAsync();
                    var downloadResults = await zoomService.SaveNewRecordingsAsync();
                    _logger.LogInformation("Zoom download completed: " + downloadResults.Message);

                    // 2. Upload YouTube
                    var uploadResults = await youtubeService.UploadAllVideosInFolderAsync();
                    foreach (var result in uploadResults)
                    {
                        _logger.LogInformation("Upload: " + result);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong ScheduledUploadService.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                    await zoomService.DownloadReportFormZoomToDbAsync();
                    _logger.LogInformation("Report sync completed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Zoom report download.");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
