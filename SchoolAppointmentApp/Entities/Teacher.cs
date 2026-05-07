using System;

namespace SchoolAppointmentApp.Entities;

public class Teacher
{
    public int UserId { set; get; }
    public required string TeacherId { get; set; }
    public int Points { set; get; }
    public int TodaysEarning { set; get; }
    public required User User { get; set; }
}

