using System;
using System.Collections.Generic;

namespace WebSearchLink.Models.ScheduleModels;

public partial class UserTimeSlot
{
    public int UserId { get; set; }

    public int TimeslotId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual TimeSlot Timeslot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
