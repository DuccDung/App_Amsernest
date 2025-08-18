using System.ComponentModel.DataAnnotations;

namespace WebSearchLink.Models.ModeBase
{
    public class RequestPost
    {
        public string Title { get; set; } = default!;

        public string? Summary { get; set; }

        public string? Content { get; set; }

        public string? Thumbnail { get; set; }
        public int? Type { get; set; }
        public IFormFile? file { get; set; }
    }
}
