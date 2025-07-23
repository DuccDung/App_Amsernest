using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.Link
{
    public class LinkViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public LinkViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string uuId)
        {
            var recordingLink =await _context.RecordingFiles
                .Where(r => r.MeetingUuid == uuId)
                .Select(r => r.Url)
                .ToListAsync();

            return View("Link" , recordingLink);
        }
    }
}
