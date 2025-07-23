using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.TableReportToDay
{
    public class TableReportToDayViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _dbContext;
        public TableReportToDayViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _dbContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var data = await _dbContext.Teachers
                .Select(t => new TeacherWithReportsViewModel
                {
                    Teacher = t,
                    Reports =t.ZoomMeetingReports
                            .Where(r => r.StartTime >= today && r.StartTime <= tomorrow && r.ParticipantsCount > 1 && r.Condition == false && r.Remove == false)
                            .ToList()
                })
                .Where(x => x.Reports.Any())
                .ToListAsync();

            return View("TableReportToDay", data);
        }
    }
}
