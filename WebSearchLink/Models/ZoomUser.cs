using Newtonsoft.Json;

namespace WebSearchLink.Models
{
    public class ZoomUsers
    {
        [JsonProperty("users")]
        public List<ZoomUser>? Users { get; set; }
    }
    public class ZoomUser
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("first_name")]
        public string? FirstName { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("pmi")]
        public long? Pmi { get; set; }

        [JsonProperty("timezone")]
        public string? TimeZone { get; set; }

        [JsonProperty("verified")]
        public int Verified { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("last_login_time")]
        public DateTime? LastLoginTime { get; set; }

    }

}
