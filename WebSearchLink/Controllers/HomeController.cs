using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http;
using WebSearchLink.Models;
using WebSearchLink.Service;

namespace WebSearchLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbAba3d6Amsernest1234567Context _dbContext; 
        private readonly HttpClient _httpClient; 
        private readonly IZoomService _zoomService;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, DbAba3d6Amsernest1234567Context dbContext , IZoomService zoomService)
        {
            _logger = logger;
            _dbContext = dbContext; 
            _httpClient = httpClientFactory.CreateClient();
            _zoomService = zoomService;
        }
        public IActionResult Login()    
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("userName");
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }
            var result = await _dbContext.RecordingFiles
                .Include(x => x.MeetingUu)
                .Select(x =>
                    new RecordingDto
                    {
                        FileId = x.FileId,
                        FileName = x.FileName,
                        YoutubeLink = x.Url,
                        MeetingTopic = x.MeetingUu != null ? x.MeetingUu.Topic : null, 
                        StartTime = x.MeetingUu != null && x.MeetingUu.StartTime.HasValue
                                    ? x.MeetingUu.StartTime.Value
                                    : DateTime.MinValue 
                    }
                ).ToListAsync();

            return View(result);
        }
        public async Task<IActionResult> SearchLink(string? value, DateTime? date)
        {
            string formattedDate = date?.ToString("dd/MM/yyyy") ?? "";
            var query = _dbContext.RecordingFiles
                .Include(x => x.MeetingUu)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(value))
            {
                query = query.Where(x => x.MeetingUu != null &&
                                         x.MeetingUu.Topic != null &&
                                         x.MeetingUu.Topic.Contains(value));
            }

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(x => x.MeetingUu != null &&
                                         x.MeetingUu.StartTime.HasValue &&
                                         x.MeetingUu.StartTime.Value.Date == targetDate);
            }

            var result = await query.Select(x => new RecordingDto
            {
                FileId = x.FileId,
                FileName = x.FileName,
                YoutubeLink = x.Url,
                MeetingTopic = x.MeetingUu != null ? x.MeetingUu.Topic : null, 
                StartTime = x.MeetingUu != null && x.MeetingUu.StartTime.HasValue
                            ? x.MeetingUu.StartTime.Value
                            : DateTime.MinValue 
            }).ToListAsync();

            return View(result);
        }
        public async Task<IActionResult> Check()
        {
            await _zoomService.DownloadReportFormZoomToDbAsync();
            return Content("Thành Công");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
