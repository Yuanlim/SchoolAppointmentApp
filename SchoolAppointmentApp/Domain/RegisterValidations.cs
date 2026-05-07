using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.FunctionalClasses;

internal sealed class RegisterStartPolicies(
    EmailValidator emailValidator,
    RoleValidator roleValidator,
    NameValidator nameValidator,
    IDuplicateChecker duplicateChecker
)
{
    /// <summary>
    /// The Policy is:
    /// 1. should be valid role.
    /// 2. email should be [word||number]@(gmail.com||nkust.edu.tw)
    /// 3. name must be more than 3 char, cant contain symbols or numbers.
    /// 4. cant be existed email or id or phoneNumber.
    /// </summary>

    private readonly EmailValidator _emailValidator = emailValidator;
    private readonly RoleValidator _roleValidator = roleValidator;
    private readonly NameValidator _nameValidator = nameValidator;
    private readonly IDuplicateChecker _duplicateChecker = duplicateChecker;

    public async Task<(ValidRegister?, IResult?)> Validate(
        CreateAccount dto,
        IErrorResults errorHandler,
        HttpContext hc
    )
    {
        // Input checker
        if (dto.Email is null || !_emailValidator.IsValid(dto.Email)) // Check format
            return (null, errorHandler.BadReqResult(
                title: "Register email issues",
                message: "We only supported @gmail.com and @nkust.edu.tw registration, and before @ must contain at least an alphabet or a number",
                hc: hc,
                user: default
            ));

        if (dto.Name is null || !_nameValidator.IsValid(dto.Name))
            return (null, errorHandler.BadReqResult(
                title: "Register name issues",
                message: "Name must be longer than 3 characters",
                hc: hc
            ));

        Roles? Role = _roleValidator.IsValid(dto.Role ?? "");
        if (Role is null || dto.Role is null)
            return (null, errorHandler.BadReqResult(
                title: "Register role invalid",
                message: "Role is required",
                hc: hc
            ));

        if (Role == Roles.admin || Role == Roles.schoolPrincipal)
            return (null, errorHandler.BadReqResult(
                title: "Register in invalid role",
                message: "Unexpected role",
                hc: hc
            ));

        if (string.IsNullOrWhiteSpace(dto.Id))
            return (null, errorHandler.BadReqResult(
                title: "Invalid register Id",
                message: "Id cant be null.",
                hc: hc
            ));

        if (string.IsNullOrWhiteSpace(dto.Password))
            return (null, errorHandler.BadReqResult(
                title: "Invalid register password",
                message: "Password cant be null.",
                hc: hc
            ));

        // Database validation
        if (await _duplicateChecker.IsDuplicateAsync(Role.Value, dto.Email, dto.Id, dto.PhoneNumber)) // Check database if email or id or phoneNumber existed
            return (null, errorHandler.ConflictResult(
                title: "Register duplicate issues",
                message: "Phone number or Email or StudentId / TeacherId has been register",
                hc: hc
            ));

        return (
            new ValidRegister(
                Id: dto.Id,
                Name: dto.Name,
                Password: dto.Password,
                Email: dto.Email,
                Role: Role.Value
            ), null
        );
    }
}