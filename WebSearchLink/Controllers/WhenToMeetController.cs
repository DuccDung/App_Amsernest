using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Globalization;
using WebSearchLink.Models;
using WebSearchLink.Models.ModeBase;
using WebSearchLink.Models.ScheduleModels;

namespace WebSearchLink.Controllers
{
    public class WhenToMeetController : Controller
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public WhenToMeetController(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }
        public IActionResult LoginWhenToMeet()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HandleRegister([FromBody] AuthWTM auth)
        {
            if (auth == null)
            {
                return Json(new { success = false });
            }
            Admin admin = new Admin
            {
                Name = auth.UserName,
                Email = auth.Email,
                PasswordHash = auth.Password
            };
            await _context.AddAsync(admin);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> HandleDeleteWTM(int Id)
        {
            var whenToMeet = await _context.WhenToMeets.FirstOrDefaultAsync(wtm => wtm.Id == Id);
            if (whenToMeet != null)
            {
                whenToMeet.condition = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "WhenToMeet not found." });
        }
        [HttpPost]
        public async Task<IActionResult> HandleLogin([FromBody] AuthWTM auth)
        {
            if (auth == null)
            {
                return Json(new { success = false });
            }
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == auth.Email && a.PasswordHash == auth.Password);
            if (admin == null)
            {
                return Json(new { success = false });
            }
            HttpContext.Session.SetInt32("W_user", admin.Id);
            HttpContext.Session.SetString("W_email", admin.Email ?? "NULL");
            var url = Url.Action("AdminDashBoard", "WhenToMeet");
            return Json(new { redirectUrl = url });
        }

        public async Task<IActionResult> GetTimeSlots()
        {
            await Task.CompletedTask;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddSlot(int meetId, DateTime date, TimeOnly timeStart, TimeOnly timeEnd)
        {
            var timeSlot = new TimeSlot
            {
                SlotDate = DateOnly.FromDateTime(date), 
                StartTime = timeStart,
                EndTime = timeEnd,
                WhenToMeetId = meetId,
            };

            await _context.AddAsync(timeSlot);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        public async Task<IActionResult> GetAdminDetails(int adminId)
        {
            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
            {
                return NotFound();
            }
            return Json(admin);
        }
        public async Task<IActionResult> AdminDashBoard()
        {
            await Task.CompletedTask;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HandleAddWTM([FromBody] WhenToMeet req)
        {
            var userId = HttpContext.Session.GetInt32("W_user");
            var userEmail = HttpContext.Session.GetString("W_email");
            if (userId == null || req == null)
            {
                return Json(new { success = false });
            }
            var whenToMeet = new WhenToMeet
            {
                Title = req.Title,
                Description = req.Description,
                CreatedBy = userId ?? 0,
                CreatedAt = DateTime.Now
            };
            await _context.WhenToMeets.AddAsync(whenToMeet);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = whenToMeet.Id, email = userEmail, title = whenToMeet.Title, description = whenToMeet.Description, create_at = whenToMeet.CreatedAt });
        }
        public async Task<IActionResult> WTM_Detail(int wtmId)
        {
            await Task.CompletedTask;
            return PartialView("WTM_Detail", wtmId);
        }
        [HttpPost]
        public async Task<IActionResult> WTM_BookingData([FromBody] WTM_User req)
        {
            var whenToMeet = await _context.WhenToMeets
                .Include(ts => ts.CreatedByAdmin)
                .Include(ts => ts.TimeSlots)
                .FirstOrDefaultAsync(ts => ts.Id == req.whenToMeetId);

            if (whenToMeet == null)
            {
                return NotFound();
            }

            var bookingData = whenToMeet.TimeSlots
                .GroupBy(ts => ts.SlotDate.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(ts => new
                    {
                        id = ts.Id,
                        start_time = ts.StartTime.ToString("HH:mm"),
                        end_time = ts.EndTime.ToString("HH:mm")
                    }).ToList()
                );

            return Ok(bookingData);
        }
        [HttpPost]
        public async Task<IActionResult> UserDateSlotStatus([FromBody] SlotCheckRequest req)
        {
            var dateOnly = DateOnly.ParseExact(req.date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var slots = await _context.TimeSlots
                .Where(ts => ts.SlotDate == dateOnly && ts.WhenToMeetId == req.whenToMeetId)
                .Select(ts => ts.Id)
                .ToListAsync();

            var userSlotIds = await _context.UserTimeSlots
                .Where(us => us.UserId == req.userId && slots.Contains(us.TimeslotId))
                .Select(us => us.TimeslotId)
                .ToListAsync();
            return Ok(new
            {
                totalSlots = slots.Count,
                userSelectedSlotIds = userSlotIds
            });
        }
        public async Task<IActionResult> WTM_UserChooseTimeSlot(int adminId, int userId, int wtmId)
        {
            var admin = await _context.Admins.FindAsync(adminId);
            var model = new WTM_User
            {
                adminId = adminId,
                adminName = admin?.Name ?? "Unknown Admin",
                userId = userId,
                whenToMeetId = wtmId
            };
            await Task.CompletedTask;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> InitCookie([FromBody] UserRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.UserName))
            {
                return Json(new { message = "Invalid input data." });
            }

            var newUser = new User { Name = data.UserName };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            SetCookie("userId", newUser.Id.ToString(), 365);

            return Json(new { userId = newUser.Id, message = "Account created successfully.", redirectUrl = Url.Content("/WhenToMeet/WTM_UserChooseTimeSlot") });
        }

        // Helper method to set cookies
        private void SetCookie(string name, string value, int days)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(days),
                HttpOnly = true,
                Secure = false // Set to true if using HTTPS
            };
            Response.Cookies.Append(name, value, options);
        }

        public class UserRequest
        {
            public string UserName { get; set; } = null!;
        }



        public async Task<IActionResult> WTM_UserLogin(int adminId, int wtmId)
        {
            var userId = Request.Cookies["userId"];
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user != null)
                {
                    return RedirectToAction("WTM_UserChooseTimeSlot", new { adminId = adminId, userId = userId, wtmId = wtmId });
                }
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BookTimeSlot([FromBody] BookingRequest request)
        {
            try
            {
                var userTimeSlot = new UserTimeSlot
                {
                    UserId = request.user_id,
                    TimeslotId = request.timeslot_id,
                    JoinedAt = DateTime.UtcNow,
                };

                _context.UserTimeSlots.Add(userTimeSlot);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Booking successful!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error booking time slot.", error = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult CheckUserTimeSlot([FromBody] CheckUserTimeSlotRequest request)
        {
            var existingBooking = _context.UserTimeSlots
                .FirstOrDefault(uts => uts.UserId == request.UserId && uts.TimeslotId == request.TimeSlotId);

            if (existingBooking != null)
            {
                return Ok(new { isBooked = true });
            }

            return Ok(new { isBooked = false });
        }
        [HttpPost]
        public IActionResult CancelBooking([FromBody] CancelBookingRequest request)
        {
            var booking = _context.UserTimeSlots
                .FirstOrDefault(uts => uts.UserId == request.UserId && uts.TimeslotId == request.TimeSlotId);

            if (booking != null)
            {
                _context.UserTimeSlots.Remove(booking);
                _context.SaveChanges();
                return Ok(new { success = true });
            }

            return BadRequest(new { success = false, message = "Booking not found." });
        }

        public class SlotTimeModel
        {
            public int Id { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            public bool IsBooked { get; set; }
        }

        public class SlotDateModel
        {
            public DateOnly SlotDate { get; set; }
            public List<SlotTimeModel>? SlotTimes { get; set; }
        }

        public async Task<IActionResult> ReportChooseTime(int wtmId)
        {
            var listData = await _context.TimeSlots.Where(ts => ts.WhenToMeetId == wtmId).GroupBy(ts => ts.SlotDate)
                            .Select(group => new SlotDateModel
                            {
                                SlotDate = group.Key,
                                SlotTimes = group.Select(ts => new SlotTimeModel
                                {
                                    Id = ts.Id,
                                    StartTime = ts.StartTime,
                                    EndTime = ts.EndTime,
                                }).ToList()
                            })
                            .ToListAsync();

            return View(listData);
        }
        public async Task<IActionResult> TableUserCount(int wtmId)
        {
            await Task.CompletedTask;
            return View();
        }
    }
}
 