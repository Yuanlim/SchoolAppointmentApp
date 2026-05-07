using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;

namespace SchoolAppointmentApp.FunctionalClasses;

// Scoped Operation
public interface IDuplicateChecker
{
    Task<bool> IsDuplicateAsync(
        Roles Role, string email,
        string Id, string? phoneNumber
    );
}

internal sealed class DuplicateChecker(MyAppDbContext dbContext)
    : DataBaseService(dbContext), IDuplicateChecker
{
    public async Task<bool> IsDuplicateAsync(
        Roles Role, string email,
        string id, string? phoneNumber
    )
    {
        bool firstStage = default;

        switch (Role)
        {
            case Roles.student:
                // If student table contain this id 
                firstStage = await dbContext.Students.AsNoTracking()
                                                     .AnyAsync(s => s.StudentId == id);
                break;

            case Roles.teacher:
                firstStage = await dbContext.Teachers.AsNoTracking()
                                                     .AnyAsync(t => t.TeacherId == id);
                break;

            default:
                throw new("Invalid Role");
        }

        bool secondStage = await dbContext.Users.AsNoTracking()
                                                .AnyAsync(u =>
                                                    // Dup email
                                                    u.Email == email ||
                                                    // Dup phone number
                                                    (!string.IsNullOrWhiteSpace(phoneNumber)
                                                    && u.PhoneNumber == phoneNumber)
                                                );

        return firstStage || secondStage;
    }
}
