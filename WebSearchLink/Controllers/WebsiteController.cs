using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebSearchLink.Models;
using WebSearchLink.Models.ScheduleModels;

namespace WebSearchLink.Controllers
{
    public class WebsiteController : Controller
    {
        private readonly DbAba3d6Amsernest1234567Context _context;
        public WebsiteController(DbAba3d6Amsernest1234567Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<List<Posts>> model = new List<List<Posts>>()
            {
                new List<Posts>(), // index 0
                new List<Posts>(), // index 1
                new List<Posts>()  // index 2
            };


            List<Posts> posts = _context.Posts.Where(p => p.Condition == true).ToList();
            var post_1 = posts.Where(p => p.Type == 1).ToList();
            var post_2 = posts.Where(p => p.Type == 2).ToList();
            var post_3 = posts.Where(p => p.Type == 3).ToList();
            model?[0].AddRange(post_1);
            model?[1].AddRange(post_2);
            model?[2].AddRange(post_3);
            return View(model);
        }
        public IActionResult AboutTeacher()
        {

            return View();
        }
        public IActionResult AcademicsNews()
        {
            List<Posts> posts = _context.Posts.Where(p => p.Condition == true && p.Type == 2).ToList();
            return View(posts);
        }
        public IActionResult CampusNews()
        {
            List<Posts> posts = _context.Posts.Where(p => p.Condition == true && p.Type == 1).ToList();
            return View(posts);
        }
        public IActionResult TecherNews()
        {
            List<Posts> posts = _context.Posts.Where(p => p.Condition == true && p.Type == 3).ToList();
            return View(posts);
        }
    }
}
