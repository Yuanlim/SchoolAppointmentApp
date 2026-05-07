using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Mapping;

public static class TeacherMapping
{
    public static TeacherDto TeacherToDto(this Teacher teacher)
    {
        return new
        (
            TeacherId: teacher.TeacherId,
            Name: teacher.User.Name,
            PhoneNumber: teacher.User.PhoneNumber ?? "",
            Email: teacher.User.Email,
            Points: default
        );
    }
}