using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Threading.Tasks;
using WebSearchLink.Models;
using WebSearchLink.Models.ModeBase;
using WebSearchLink.Service;

namespace WebSearchLink.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbAba3d6Amsernest1234567Context _dbContext;
        private readonly IZoomService _zoomService;
        public AdminController(DbAba3d6Amsernest1234567Context dbContext, IZoomService zoomService)
        {
            _dbContext = dbContext;
            _zoomService = zoomService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult HandleLogin(string username, string password)
        {
            if (username == "admin" && password == "amsernest")
            {
                HttpContext.Session.SetString("userName", username);
                HttpContext.Session.SetString("password", password);

                return RedirectToAction("DashBoard", "Admin");
            }
            if (username == "user" && password == "@123")
            {
                HttpContext.Session.SetString("userName", username);
                HttpContext.Session.SetString("password", password);

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Admin");
        }
        public IActionResult DashBoard()
        {
            var userName = HttpContext.Session.GetString("userName");
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProccessReportChoose([FromBody] DateRangeModel model)
        {
            var data = await _dbContext.Teachers
                .Where(x => x.Remove == false)
                .Select(t => new TeacherWithReportsViewModel
                {
                    Teacher = t,
                    Reports = t.ZoomMeetingReports
                        .Where(r => r.StartTime >= model.from && r.StartTime <= model.to && r.ParticipantsCount > 1 && r.Condition == false && r.Remove == false)
                        .ToList()
                })
                .Where(x => x.Reports.Any())
                .ToListAsync();

            return PartialView("_partalViewReporting", data);
        }
        public async Task<IActionResult> TecherPage()
        {
            var Techers = await _dbContext.Teachers.Where(x => x.Remove == false).ToListAsync();
            return PartialView("TecherPage", Techers);
        }
        [HttpPost]
        public IActionResult ConfirmReport([FromBody] ConfirmReportRequest request)
        {
            string? meetingId = request.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var teacher = _dbContext.ZoomMeetingReports.Where(r => r.Uuid == meetingId && r.Remove == false).Include(r => r.Teacher).Where(r => r.Teacher.Remove == false).FirstOrDefault();


            return PartialView("PartialViewModalConfirm", teacher);
        }
        [HttpPost]
        public IActionResult MeetingDetail([FromBody] ConfirmReportRequest request)
        {
            string? meetingId = request.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var teacher = _dbContext.ZoomMeetingReports.Where(r => r.Uuid == meetingId && r.Remove == false).Include(r => r.Teacher).Where(x => x.Teacher.Remove == false).FirstOrDefault();


            return PartialView("partialViewMeetingDetail", teacher);
        }
        [HttpPost]
        public async Task<IActionResult> HandleLeftMenuMeetingToDay()
        {
            var teachers = await _dbContext.Teachers.Where(x => x.Remove == false)
                .Include(t => t.ZoomMeetingReports)
                .Where(t => t.ZoomMeetingReports.Any(r => r.Remove == false))
                .ToListAsync();

            return PartialView("PartialViewMeetingToday", teachers);
        }
        [HttpPost]
        public async Task<IActionResult> SearchMeetingByTeacher([FromBody] SearchMeetingRequest req)
        {
            var data = await _dbContext.Teachers.Where(x => x.TeacherId == req.teacherId && x.Remove == false)
                .Select(t => new TeacherWithReportsViewModel
                {
                    Teacher = t,
                    Reports = t.ZoomMeetingReports
                            .Where(r => r.StartTime >= req.from && r.StartTime <= req.to && r.ParticipantsCount > 1 && r.Remove == false)
                            .ToList()
                })
                .Where(x => x.Reports.Any())
                .ToListAsync();
            ViewBag.teacherId = req.teacherId;
            return PartialView("PatialViewTableMeetingToDayLeftMenu", data);
        }
        [HttpPost]
        public async Task<IActionResult> DeletedMeeting([FromBody] SearchMeetingRequest req)
        {
            if (req == null || req.teacherId == null)
            {
                return BadRequest(new { message = "Invalid request data!" });
            }
            var data = await _dbContext.Teachers.Where(x => x.Remove == false && x.TeacherId == req.teacherId)
                           .Select(t => new TeacherWithReportsViewModel
                           {
                               Teacher = t,
                               Reports = t.ZoomMeetingReports
                                       .Where(r => r.StartTime >= req.from && r.StartTime <= req.to && r.ParticipantsCount > 1 && r.Remove == true)
                                       .ToList()
                           })
                           .Where(x => x.Reports.Any())
                           .ToListAsync();

            return View("partialViewDeletedMeeting", data);
        }
        public async Task<IActionResult> RestoreMeeting([FromBody] ConfirmReportRequest req)
        {
            string? meetingId = req.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var meeting = await _dbContext.ZoomMeetingReports.FirstOrDefaultAsync(r => r.Uuid == meetingId);
            if (meeting == null || meeting.Remove == false)
            {
                return BadRequest(new { message = "Không tìm thấy cuộc họp với ID đã cho hoặc cuộc họp không bị xóa." });
            }
            meeting.Remove = false;
            _dbContext.ZoomMeetingReports.Update(meeting);
            await _dbContext.SaveChangesAsync();
            return Json(new
            {
                success = true,
                message = "Restore successful"
            });
        }
        [HttpPost]
        public IActionResult SummaryReportToDay()
        {
            return PartialView("summaryMeetingToday");
        }
        [HttpPost]
        public async Task<IActionResult> HandleSammaryReport([FromBody] DateRangeModel req)
        {
            var data = await _dbContext.Teachers.Where(x => x.Remove == false)
                .Select(t => new TeacherWithReportsViewModel
                {
                    Teacher = t,
                    Reports = t.ZoomMeetingReports
                            .Where(r => r.StartTime >= req.from && r.StartTime <= req.to && r.ParticipantsCount > 1 && r.Remove == false)
                            .ToList()
                })
                .Where(x => x.Reports.Any())
                .ToListAsync();
            return PartialView("PartialViewSummaryReport", data);
        }
        [HttpPost]
        public async Task<IActionResult> HandleConfirmTeacher([FromBody] ConfirmTeacher req)
        {
            var teacher = await _dbContext.Teachers.FindAsync(req.teacherId);
            if (teacher == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy giáo viên" });
            }
            teacher.authenticate = true;
            _dbContext.Teachers.Update(teacher);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTeacher([FromBody] ConfirmTeacher req)
        {
            var teacher = await _dbContext.Teachers.FindAsync(req.teacherId);
            return PartialView("ModalConfirmTeacher", teacher);
        }
        [HttpPost]
        public async Task<IActionResult> HandleConfirmMeeting([FromBody] ConfirmReportRequest req)
        {
            string? meetingId = req.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var meeting = await _dbContext.ZoomMeetingReports.FirstOrDefaultAsync(r => r.Uuid == meetingId && r.Remove == false);
            if (meeting == null)
            {
                return BadRequest(new { message = "Không tìm thấy cuộc họp với ID đã cho." });
            }
            meeting.Condition = true;
            meeting.Feedback = req.feedBack;
            _dbContext.ZoomMeetingReports.Update(meeting);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveMeeting([FromBody] ConfirmReportRequest req)
        {
            string? meetingId = req.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var meeting = await _dbContext.ZoomMeetingReports.FirstOrDefaultAsync(r => r.Uuid == meetingId && r.Remove == false);
            if (meeting == null)
            {
                return BadRequest(new { message = "Không tìm thấy cuộc họp với ID đã cho." });
            }

            return PartialView("ModalRemoveMeeting", meeting);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveMeetingTeacher([FromBody] ConfirmReportRequest req)
        {
            string? meetingId = req.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var meeting = await _dbContext.ZoomMeetingReports.FirstOrDefaultAsync(r => r.Uuid == meetingId && r.Remove == false);
            if (meeting == null)
            {
                return BadRequest(new { message = "Không tìm thấy cuộc họp với ID đã cho." });
            }

            return PartialView("ModalRemoveMeetingTeacher", meeting);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTeacher([FromBody] ConfirmTeacher req)
        {
            var teacher = await _dbContext.Teachers.FindAsync(req.teacherId);
            if (teacher == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy giáo viên" });
            }
            return PartialView("ModalRemoveTeacher", teacher);
        }
        [HttpPost]
        public async Task<IActionResult> HandleRemoveTeacher([FromBody] ConfirmTeacher req)
        {
            var teacher = await _dbContext.Teachers.FindAsync(req.teacherId);
            if (teacher == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy giáo viên" });
            }
            teacher.Remove = true;
            _dbContext.Teachers.Update(teacher);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> HandleRemoveMeeting([FromBody] ConfirmReportRequest req)
        {
            string? meetingId = req.meetingId;
            if (string.IsNullOrEmpty(meetingId))
            {
                return BadRequest(new { message = "Meeting ID không được để trống!" });
            }
            var meeting = await _dbContext.ZoomMeetingReports.FirstOrDefaultAsync(r => r.Uuid == meetingId && r.Remove == false);
            if (meeting == null)
            {
                return BadRequest(new { message = "Không tìm thấy cuộc họp với ID đã cho." });
            }
            meeting.Remove = true;
            meeting.Feedback = req.feedBack;
            _dbContext.ZoomMeetingReports.Update(meeting);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> HandleDetailMeeting([FromBody] FixDetailMeetingReport req)
        {
            var meeting = await _dbContext.ZoomMeetingReports
                .Where(r => r.Uuid == req.meetingId && r.Remove == false)
                .FirstOrDefaultAsync();
            if (req.meetingId != null)
            {
                meeting.TeacherId = req.teacherId;
            }
            if (req.typeMeeting != null)
            {
                meeting.TypeTeacher = req.typeMeeting;
            }
            _dbContext.ZoomMeetingReports.Update(meeting);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] SearchMeetingRequest req)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Amsernest");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Meeting Reports");

            // Header  
            worksheet.Cells[1, 1].Value = "Teacher name";
            worksheet.Cells[1, 2].Value = "Type";
            worksheet.Cells[1, 3].Value = "Meeting ID";
            worksheet.Cells[1, 4].Value = "Topic";
            worksheet.Cells[1, 5].Value = "Creation time";
            worksheet.Cells[1, 6].Value = "Duration (min)";
            worksheet.Cells[1, 7].Value = "Link YT";
            worksheet.Cells[1, 8].Value = "Link Download";
            worksheet.Cells[1, 9].Value = "Feedback";
            worksheet.Cells[1, 10].Value = "Wage";

            int row = 2;
            List<TeacherWithReportsViewModel> modelDataFromDatabase; // Corrected variable initialization  

            if (req.teacherId != null)
            {
                modelDataFromDatabase = await _dbContext.Teachers.Where(x => x.TeacherId == req.teacherId && x.Remove == false)
                    .Select(t => new TeacherWithReportsViewModel
                    {
                        Teacher = t,
                        Reports = t.ZoomMeetingReports
                            .Where(r => r.StartTime >= req.from && r.StartTime <= req.to && r.ParticipantsCount > 1 && r.Remove == false)
                            .ToList()
                    })
                    .Where(x => x.Reports.Any())
                    .ToListAsync();
            }
            else
            {
                modelDataFromDatabase = await _dbContext.Teachers.Where(x => x.Remove == false)
                    .Select(t => new TeacherWithReportsViewModel
                    {
                        Teacher = t,
                        Reports = t.ZoomMeetingReports
                            .Where(r => r.StartTime >= req.from && r.StartTime <= req.to && r.ParticipantsCount > 1 && r.Remove == false)
                            .ToList()
                    })
                    .Where(x => x.Reports.Any())
                    .ToListAsync();
            }

            foreach (var item in modelDataFromDatabase)
            {
                foreach (var report in item.Reports)
                {
                    var link = await _dbContext.RecordingFiles
                        .Where(r => r.MeetingUuid == report.Uuid)
                        .Select(r => new { r.Url, r.DownloadUrl })
                        .FirstOrDefaultAsync();

                    worksheet.Cells[row, 1].Value = item.Teacher.FullName;
                    worksheet.Cells[row, 2].Value = report.TypeTeacher == 1 ? "AC" :
                                                    report.TypeTeacher == 2 ? "Coach" : "Khác";
                    worksheet.Cells[row, 3].Value = report.Id;
                    worksheet.Cells[row, 4].Value = report.Topic;
                    worksheet.Cells[row, 5].Value = report.StartTime?.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 6].Value = report.Duration;
                    worksheet.Cells[row, 7].Value = link?.Url ?? "Không tồn tại!";
                    worksheet.Cells[row, 8].Value = link?.DownloadUrl ?? "Không tồn tại!";
                    worksheet.Cells[row, 9].Value = report.Feedback;
                    worksheet.Cells[row, 10].Value = "";

                    row++;
                }
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            string excelName = $"MeetingReports_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}
