namespace WebSearchLink.Models.ScheduleModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
    [Table("Posts", Schema = "ams83485_sa")]
    public class Posts
    {
        [Key]
        public int PostID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = default!;

        [MaxLength(500)]
        public string? Summary { get; set; }

        public string? Content { get; set; }

        [MaxLength(255)]
        public string? Thumbnail { get; set; }

        public int? Type { get; set; }

        public bool? Condition { get; set; }
    }

}
