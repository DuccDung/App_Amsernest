namespace WebSearchLink.Models
{
    public class TeacherWithReportsViewModel
    {
        public Teacher Teacher { get; set; } = null!;
        public List<ZoomMeetingReport> Reports { get; set; } = new List<ZoomMeetingReport>();
    }

}
