using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WebSearchLink.Models.ScheduleModels;

namespace WebSearchLink.Models;

public partial class DbAba3d6Amsernest1234567Context : DbContext
{
    private readonly IConfiguration _configuration;
    public DbAba3d6Amsernest1234567Context(DbContextOptions<DbAba3d6Amsernest1234567Context> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<GoogleToken> GoogleTokens { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<RecordingFile> RecordingFiles { get; set; }
    public virtual DbSet<Teacher> Teachers { get; set; }
    public virtual DbSet<ZoomMeetingReport> ZoomMeetingReports { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTimeSlot> UserTimeSlots { get; set; }
    public DbSet<WhenToMeet> WhenToMeets { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InformationMeetingContext"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WhenToMeet>()
     .HasOne(w => w.CreatedByAdmin)
     .WithMany(a => a.CreatedWTMs)
     .HasForeignKey(w => w.CreatedBy)
     .OnDelete(DeleteBehavior.Restrict);



        modelBuilder.Entity<ZoomMeetingReport>()
           .HasOne(z => z.Teacher)
           .WithMany(t => t.ZoomMeetingReports)
           .HasForeignKey(z => z.TeacherId)
           .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<GoogleToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GoogleTo__3214EC072880BADA");

            entity.ToTable("GoogleToken");

            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Scope).HasMaxLength(500);
            entity.Property(e => e.TokenType).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(100);
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("PK__Meeting__BDA103F59A105013");

            entity.ToTable("Meeting");

            entity.Property(e => e.Uuid).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Topic).HasMaxLength(255);
        });

        modelBuilder.Entity<RecordingFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__Recordin__6F0F98BF47C1F6BE");

            entity.ToTable("RecordingFile");

            entity.Property(e => e.FileId).HasMaxLength(100);
            entity.Property(e => e.DownloadedAt).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.MeetingUuid).HasMaxLength(100);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.MeetingUu).WithMany(p => p.RecordingFiles)
                .HasForeignKey(d => d.MeetingUuid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RecordingFile_Meeting");
        });
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admins__3213E83FE02627E5");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TimeSlot__3213E83FA10337C6");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsBooked)
                .HasDefaultValue(false)
                .HasColumnName("is_booked");
            entity.Property(e => e.SlotDate).HasColumnName("slot_date");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F337F0FBA");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<UserTimeSlot>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TimeslotId }).HasName("PK__UserTime__D59759BD5F6B7831");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TimeslotId).HasColumnName("timeslot_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");

            entity.HasOne(d => d.Timeslot).WithMany(p => p.UserTimeSlots)
                .HasForeignKey(d => d.TimeslotId)
                .HasConstraintName("FK__UserTimeS__times__540C7B00");

            entity.HasOne(d => d.User).WithMany(p => p.UserTimeSlots)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserTimeS__user___531856C7");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
