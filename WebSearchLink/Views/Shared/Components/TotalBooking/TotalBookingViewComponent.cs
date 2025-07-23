using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.TotalBooking
{
    public class TotalBookingViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public TotalBookingViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int stId)
        {
            var cnt = await _context.UserTimeSlots
                .CountAsync(uts => uts.TimeslotId == stId);
            return View("index" , cnt);
        }
    }
}
