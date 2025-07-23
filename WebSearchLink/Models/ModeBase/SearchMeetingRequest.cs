namespace WebSearchLink.Models.ModeBase
{
    public class SearchMeetingRequest
    {
        public int? teacherId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
