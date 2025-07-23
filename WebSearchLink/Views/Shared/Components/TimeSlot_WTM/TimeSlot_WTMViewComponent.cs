using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.TimeSlot_WTM
{
    public class TimeSlot_WTMViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public TimeSlot_WTMViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int wtmId)
        {
            var timeSlots = await _context.TimeSlots
                .Where(ts => ts.WhenToMeetId == wtmId)
                .ToListAsync();
            return View("Index" , timeSlots);
        }
    }
}
