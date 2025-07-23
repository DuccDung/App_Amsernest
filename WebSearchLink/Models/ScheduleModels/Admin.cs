using System;
using System.Collections.Generic;

namespace WebSearchLink.Models.ScheduleModels;

public partial class Admin
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }
    public virtual ICollection<WhenToMeet> CreatedWTMs { get; set; } = new List<WhenToMeet>();

}
