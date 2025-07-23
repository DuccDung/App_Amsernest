using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSearchLink.Models.ScheduleModels;
[Table("TimeSlots")]
public partial class TimeSlot
{
    [Key]
    public int Id { get; set; }
    [Column("slot_date")]
    public DateOnly SlotDate { get; set; }
    [Column("start_time")]
    public TimeOnly StartTime { get; set; }
    [Column("end_time")]
    public TimeOnly EndTime { get; set; }
    
    public bool? IsBooked { get; set; }

    public DateTime? CreatedAt { get; set; }

    [Column("when_to_meet_id")]
    public int WhenToMeetId { get; set; }

    [ForeignKey("WhenToMeetId")]
    public WhenToMeet? WhenToMeet { get; set; }


    public virtual ICollection<UserTimeSlot> UserTimeSlots { get; set; } = new List<UserTimeSlot>();
}
