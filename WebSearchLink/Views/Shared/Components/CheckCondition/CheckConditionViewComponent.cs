using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.CheckCondition
{
    public class CheckConditionViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _dbContext;
        public CheckConditionViewComponent(DbAba3d6Amsernest1234567Context dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(string uuId)
        {
            var isCheck = await _dbContext.ZoomMeetingReports
                .Where(r => r.Uuid == uuId)
                .FirstOrDefaultAsync();

            if (isCheck == null || !isCheck.Condition)
            {
                return View("NotConfirmed", isCheck?.Uuid); 
            }
            return View("Confirmed");
        }
    }
}
