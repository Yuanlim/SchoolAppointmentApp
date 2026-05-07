using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Mapping;

public static class StudentMapping
{
    public static StudentDto StudentToDto(this Student student)
    {
        return new
        (
            StudentId: student.StudentId,
            Name: student.User.Name,
            PhoneNumber: student.User.PhoneNumber ?? "",
            ClassName: student.SchoolClass.ClassName,
            Email: student.User.Email
        );
    }
}