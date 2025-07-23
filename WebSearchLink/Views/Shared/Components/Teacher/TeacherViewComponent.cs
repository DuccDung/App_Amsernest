using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.Teacher
{
    public class TeacherViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _dbContext;
        public TeacherViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _dbContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var teachers = await _dbContext.Teachers.ToListAsync();
            return View("TeacherView", teachers);
        }
    }
}
