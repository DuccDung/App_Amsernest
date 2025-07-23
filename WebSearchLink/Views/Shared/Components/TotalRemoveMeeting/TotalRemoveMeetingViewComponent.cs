using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.TotalRemoveMeeting
{
    public class TotalRemoveMeetingViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public TotalRemoveMeetingViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int teacherId)
        {
            var cnt = await _context.ZoomMeetingReports
                .Where(r => r.TeacherId == teacherId && r.Remove == true).CountAsync();
            var model = new WebSearchLink.Models.ModeBase.TeacherId_Cnt
            {
                TeacherId = teacherId,
                Cnt = cnt
            };
            return View("index" , model);
        }
    }
}
