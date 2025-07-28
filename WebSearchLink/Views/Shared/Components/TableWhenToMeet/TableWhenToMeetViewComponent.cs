using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;
using WebSearchLink.Models.ScheduleModels;

namespace WebSearchLink.Views.Shared.Components.TableWhenToMeet
{
    public class TableWhenToMeetViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public TableWhenToMeetViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetInt32("W_user");
            var userEmail = HttpContext.Session.GetString("W_email");

            var listData = new List<WhenToMeet>();
            if (userId != null)
            {
                listData = await _context.WhenToMeets
                   .Where(wtm => wtm.CreatedBy == userId && wtm.condition == true)
                   .Select(wtm => new WhenToMeet
                   {
                       Id = wtm.Id,
                       Title = wtm.Title,
                       Description = wtm.Description,
                       CreatedBy = wtm.CreatedBy,
                       CreatedAt = wtm.CreatedAt,
                       CreatedByAdmin = new Admin
                       {
                           Id = userId ?? 0,
                           Email = userEmail,
                       }
                   })
                   .ToListAsync();
            }

            return View("index", listData);
        }
    }
}
