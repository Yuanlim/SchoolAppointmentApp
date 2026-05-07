namespace SchoolAppointmentApp.Entities;

public class Student
{
    public required string StudentId { set; get; }
    public int ClassId { set; get; }
    public required SchoolClass SchoolClass { set; get; }
    public int UserId { set; get; } // FK.keys of User.Id
    public required User User { get; set; }
}


