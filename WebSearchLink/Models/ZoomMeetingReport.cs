using System.ComponentModel.DataAnnotations;

namespace WebSearchLink.Models
{
    public class ZoomMeetingReport
    {
        [Key]
        [StringLength(100)]
        public string Uuid { get; set; } = null!;

        public long Id { get; set; }

        [StringLength(255)]
        public string? Topic { get; set; }

        public int Type { get; set; }

        [StringLength(255)]
        public string? UserEmail { get; set; }

        [StringLength(255)]
        public string? UserName { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? Duration { get; set; }

        public int? ParticipantsCount { get; set; }

        public int? TotalMinutes { get; set; }

        [StringLength(100)]
        public string? HostId { get; set; }

        [StringLength(50)]
        public string? Source { get; set; }

        public bool Condition { get; set; }
        public bool Remove { get; set; }
        public string? Feedback { get; set; }

        // Foreign Key
        public int? TeacherId { get; set; }
        public int? TypeTeacher { get; set; }

        public Teacher? Teacher { get; set; }
    }
}
