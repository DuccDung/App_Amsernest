using Microsoft.AspNetCore.Mvc;
using WebSearchLink.Models;

namespace WebSearchLink.Views.Shared.Components.ConfirmTeacher
{
    public class ConfirmTeacherViewComponent : ViewComponent
    {
        private readonly DbAba3d6Amsernest1234567Context _dbContext;
        public ConfirmTeacherViewComponent(DbAba3d6Amsernest1234567Context context)
        {
            _dbContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int teacherId)
        {
            var teacher =await _dbContext.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                return Content("Teacher not found");
            }
            if(teacher.authenticate == true)
            {
                return View("Confirmed" , teacher.TeacherId);
            }
            return View("unConfirm" , teacher.TeacherId);
        }
    }
}
