using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSearchLink.Models.ScheduleModels;
[Table("WhenToMeet")]
public class WhenToMeet
{
    public int Id { get; set; }
    [Column("title")]
    public string? Title { get; set; }
    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    public bool condition { get; set; } = true;

    [ForeignKey("CreatedBy")] // <-- đây mới là đúng
    public Admin? CreatedByAdmin { get; set; }

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
