using Newtonsoft.Json;

namespace WebSearchLink.Models
{
    public class ZoomMeetingReportResponses
    {
        [JsonProperty("meetings")]
        public List<ZoomMeetingReportResponse>? ReportResponses;
        [JsonProperty("next_page_token")]
        public string? NextPageToken { get; set; }
    }
    public class ZoomMeetingReportResponse
    {
        [JsonProperty("uuid")]
        public string? Uuid { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("topic")]
        public string? Topic { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("user_email")]
        public string? UserEmail { get; set; }

        [JsonProperty("user_name")]
        public string? UserName { get; set; }

        [JsonProperty("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonProperty("end_time")]
        public DateTime? EndTime { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("participants_count")]
        public int ParticipantsCount { get; set; }

        [JsonProperty("total_minutes")]
        public int TotalMinutes { get; set; }

        [JsonProperty("host_id")]
        public string? HostId { get; set; }

        [JsonProperty("source")]
        public string? Source { get; set; }
 
    }
}
