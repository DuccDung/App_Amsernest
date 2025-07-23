using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.LinkDownLoad
{
    public class LinkDownloadViewComponent : ViewComponent
    {

        private readonly DbAba3d6Amsernest1234567Context _context;
        public LinkDownloadViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string uuId)
        {
            var recordingLink = await _context.RecordingFiles
                .Where(r => r.MeetingUuid == uuId)
                .Select(r => r.DownloadUrl)
                .ToListAsync();

            return View("LinkDown", recordingLink);
        }
    }
}
