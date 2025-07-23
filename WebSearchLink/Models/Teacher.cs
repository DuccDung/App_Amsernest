using System.ComponentModel.DataAnnotations;

namespace WebSearchLink.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        [Required]
        [StringLength(255)]
        public string? FullName { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        public int Type { get; set; }

        [StringLength(255)]
        public string? Department { get; set; }

        public bool? authenticate { get; set; }
        public bool Remove { get; set; }

        public ICollection<ZoomMeetingReport>? ZoomMeetingReports { get; set; }
    }
}
