using WebSearchLink.Models;

namespace WebSearchLink.Service
{
    public interface IZoomService
    {
        Task<ResponseModel<string>> GetAccessTokenZoom(); // get access token from zoom
        Task<ResponseModel<string>> RefeshAccessTokenZoom(); // refresh access token from zoom
        Task<ResponseModel<ZoomRecordingFile>> GetNewRecordingsAsync(); // get new recordings link from zoom
        Task<ResponseModel<string>> SaveNewRecordingsAsync(); // save new recordings video link to database
        // handle report zoom
        Task<ResponseModel<ZoomUsers>> GetUserAsync();
        Task<ResponseModel<ZoomMeetingReportResponses>> GetReportingFilesToWeekAsync();
        Task DownloadReportFormZoomToDbAsync();
        //  Task<ResponseModel<ZoomMeetingReportResponses>> GetReportingFilesChooseAsync(DateTime from, DateTime to);
    }
}
