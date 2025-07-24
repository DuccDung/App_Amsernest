
using Microsoft.AspNetCore.Mvc;
using WebSearchLink.Service;

namespace WebSearchLink.Models
{
    public class ScheduledUploadService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduledUploadService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1.5);
        private readonly IWebHostEnvironment _env;
        public ScheduledUploadService(IServiceProvider serviceProvider, ILogger<ScheduledUploadService> logger, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _env = env;
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
                    if (CheckVideos() == true)
                    {
                        var uploadResultsStillExist = await youtubeService.UploadAllVideosInFolderAsync();
                        foreach (var result in uploadResultsStillExist)
                        {
                            _logger.LogInformation("Upload: " + result);
                            await Task.Delay(_interval, stoppingToken);
                        }
                    }
                    else
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

        public bool CheckVideos()
        {
            var videoPath = Path.Combine(_env.WebRootPath, "videos");
            if (!Directory.Exists(videoPath))
            {
                return false; // Thư mục không tồn tại, coi như không có video
            }

            var files = Directory.GetFiles(videoPath, "*.*", SearchOption.TopDirectoryOnly)
                                 .Where(f => f.EndsWith(".mp4") || f.EndsWith(".avi") || f.EndsWith(".mov") || f.EndsWith(".mkv"))
                                 .ToList();

            if (files.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
