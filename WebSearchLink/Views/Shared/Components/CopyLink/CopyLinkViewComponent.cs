using Microsoft.AspNetCore.Mvc;

namespace WebSearchLink.Views.Shared.Components.CopyLink
{
    public class CopyLinkViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int adminId, int wtmId)
        {
            var link = Url.Action("WTM_UserLogin", "WhenToMeet", new { adminId = adminId, wtmId = wtmId });
            var fullLink = "https://localhost:7168" + link;
            return View("Index", fullLink);
        }
    }
}
