using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.EndPoints;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Jwt;
using System.Text.Json.Serialization;
using static SchoolAppointmentApp.FunctionalClasses.BlockChecker;
using Microsoft.Extensions.FileProviders;


Console.WriteLine(new PasswordHasher<object>()
       .HashPassword(default!, "Hello world"));

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyAppDbContext>(opt =>
  opt.UseNpgsql(connString));

// Register the Jwt options in IOptions so later can DI it in services, it anyone needs it
builder.Services.AddOptions<JwtConfiguration>()
                .Bind(builder.Configuration.GetSection("Jwt"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

// Transient, Scope, Singleton DI registration
builder.Services.AddSingleton<JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<IDuplicateChecker, DuplicateChecker>();
builder.Services.AddScoped<IProductListClasses, ProductListClasses>();
builder.Services.AddScoped<IOrderItemList, OrderItemListClasses>();
builder.Services.AddScoped<IOrderStatus, GetStatus>();
builder.Services.AddScoped<IGetCart, GetCartHandler>();
builder.Services.AddScoped<IGetCartItem, GetCartItemHandler>();
builder.Services.AddScoped<IGetUserId, GetUserId>();
builder.Services.AddScoped<IGetUser, GetUserService>();
builder.Services.AddScoped<IGetPost, GetPost>();
builder.Services.AddScoped<IGetFriend, GetFriend>();
builder.Services.AddScoped<IBlock, BlockChecker>();
builder.Services.AddScoped<IRelationship, RelationHandler>();
builder.Services.AddScoped<IProcessValidator, NullValidator>();
builder.Services.AddScoped<UnAuthorizedValidator>();
builder.Services.AddScoped<RegisterStartPolicies>();
builder.Services.AddScoped<NullValidator>();
builder.Services.AddTransient<EmailValidator>();
builder.Services.AddTransient<NameValidator>();
builder.Services.AddTransient<RoleValidator>();
builder.Services.AddTransient<IErrorResults, ErrorResultHandler>();


// Jwt Bearer
// IF appsettings.json Jwt doesn't exist, get default obj.
var Jwt = builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>()
          ?? throw new InvalidOperationException("JWT configuration missing");

var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.SecretKey ?? ""));


// CORS for frontend
builder.Services.AddCors(options =>
{
  options.AddPolicy("FrontendCorsPolicy", policy =>
  {
    policy.WithOrigins(["http://localhost:3000", "http://localhost:3001"])
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});


// Treat passed in string able to convert to enum
builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


// Jwt validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // All Register as "Bearer" validation endpoints
                .AddJwtBearer(o =>
                {
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = Jwt.Issuer,
                    ValidAudience = Jwt.Audience,
                    IssuerSigningKey = SecurityKey,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                  };
                })
                .AddCookie("Cookie", c =>
                {
                  c.Events.OnRedirectToLogin = ctx =>
                  {
                    ctx.Response.StatusCode = 401;
                    return Task.CompletedTask;
                  };
                  c.Events.OnRedirectToAccessDenied = ctx =>
                  {
                    ctx.Response.StatusCode = 403;
                    return Task.CompletedTask;
                  };
                  c.LoginPath = "/login";
                  c.LogoutPath = "/logout";
                  c.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

// Endpoint role restriction setup
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy(
      "AdminAllowed", policy => policy.RequireRole("admin")
                                      .RequireClaim(ClaimTypes.NameIdentifier)
  );
  options.AddPolicy(
      "TeacherAllowed", policy => policy.RequireRole("teacher")
                                        .RequireClaim("TeacherId")
  );
  options.AddPolicy(
      "StudentAllowed", policy => policy.RequireRole("student")
                                        .RequireClaim("StudentId")
  );
  options.AddPolicy(
      "PrincipalAllowed", policy => policy.RequireRole("schoolPrincipal")
                                          .RequireClaim(ClaimTypes.NameIdentifier)
  );
  options.AddPolicy(
      "TeacherOrStudentAllowed", policy => policy.RequireRole("student", "teacher")
  );
  options.AddPolicy(
      "TeacherOrPrincipalAllowed", policy => policy.RequireRole("schoolPrincipal", "teacher")
  );
  options.AddPolicy(
      "AllRoleAllowed", policy => policy.RequireRole("student", "teacher", "admin", "schoolPrincipal")
  );
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
  exceptionHandlerApp.Run(async httpContext =>
  {
    var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
    if (pds == null
          || !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
    {
      // Fallback behavior
      await httpContext.Response.WriteAsync("Fallback: An error occurred.");
    }
  });
});

// login endpoints
app.MapPost("/login", async (
    HttpContext hc,
    IErrorResults errorHandler,
    IPasswordHasher<object> passwordHasher,
    JwtProvider jwtProvider,
    LoginDto dto,
    MyAppDbContext dbContext,
    RoleValidator roleValidator
) =>
{
  /*
    Accept request type:
    LoginDto

    Purpose:
    Authenticate user;

    For:
    All

    Process: 
    1. If the attempted login role, is an valid one;
    2. Check the related role database that, user id ever existed in that database;
    3. Verify the submitted password against the stored password hash;

    If Success:
    1. Note the user identity in a claim and encrypted to a Jwt;
    2. Using that claim to also register a new cookie;
    3. Return the token to user as an Credential

    If failed:
    1. Invalid input -> BadReq
    2. Password or Id wrong -> Unauthorized
  */

  ClaimsPrincipal? claimsPrincipal;

  // Exception handler
  if (dto.Role is null)
    return errorHandler.BadReqResult(
        title: "Login validation issue",
        message: "Role is required.",
        hc: hc // Provided info about the request
    );

  Roles? Role = roleValidator.IsValid(dto.Role);
  if (Role is null)
    return errorHandler.BadReqResult(
        title: "Login validation issue",
        message: "Unexpected Role",
        hc: hc
    );

  if (Role == Roles.admin)
  {
    // Query SQL admin by login id

    var admin = await dbContext.Admins
                               .AsNoTracking() // do not track changes
                               .FirstOrDefaultAsync(a =>
                                    a.AdminLoginId == dto.Id
                                );  // Find match id admin

    // No match
    if (admin is null)
      return errorHandler.UnauthorizedResult(
          title: "Login Failed",
          message: "Id or password is invalid.",
          hc: hc
      ); // No match

    // Verify password
    var verified = passwordHasher.VerifyHashedPassword(
        admin, admin.PasswordHash!, dto.Password
    );

    bool success = verified == PasswordVerificationResult.Success;

    // IF Success setup JWT
    if (!success)
      return errorHandler.UnauthorizedResult(
          title: "Login Failed",
          message: "Id or password is invalid.",
          hc: hc
      ); // No match

    // New identity claims
    var adminClaims = new[] {
          new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
          new Claim(ClaimTypes.Email, admin.Email ?? ""),
          new Claim(ClaimTypes.Role, "admin"),
        };

    // Generate a signed JWT token containing the admin claims
    var token = jwtProvider.Create(adminClaims);

    // User ClaimsPrincipal so that later when user post request to an api
    // Authorize -> We can access "who"
    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(adminClaims, "Cookie"));

    // Store user claims to a Cookie for later request
    await hc.SignInAsync("Cookie", claimsPrincipal);

    return Results.Ok(token);
  }

  // Login as School principal
  if (Role == Roles.schoolPrincipal)
  {
    // Check this id School principal existed
    SchoolPrincipal? sp = await dbContext.SchoolPrincipal.AsNoTracking() // Don't track changes
                                                         .FirstOrDefaultAsync( // Should be only 1
                                                            sp => sp.PrincipalId == dto.Id
                                                         );
    if (sp is null)
      return errorHandler.UnauthorizedResult(
          title: "Login Failed",
          message: "Id or password is invalid.",
          hc: hc
      ); // No match

    var success3 = passwordHasher.VerifyHashedPassword(sp, sp.PasswordHash, dto.Password);

    if (success3 != PasswordVerificationResult.Success)
      return errorHandler.UnauthorizedResult(
        title: "Login Failed",
        message: "Id or password is invalid.",
        hc: hc
      ); // No match

    var principalClaims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, sp.PrincipalId.ToString()),
      new Claim(ClaimTypes.Email, sp.Email ?? ""),
      new Claim(ClaimTypes.Role, "schoolPrincipal")
    };

    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(principalClaims, "Cookie"));

    var token = jwtProvider.Create(principalClaims);

    await hc.SignInAsync("Cookie", claimsPrincipal);

    return Results.Ok(token);
  }

  // Teacher and student login
  if (Role == Roles.teacher || Role == Roles.student)
  {
    // Determine which role and query the right one
    User? user = Role == Roles.student
                ? await dbContext.Users.AsNoTracking()
                                        .Include(u => u.Student) // When query auto include relation table record
                                        .FirstOrDefaultAsync(
                                            u => u.Student != null &&
                                            u.Student.StudentId == dto.Id
                                        )
                : await dbContext.Users.AsNoTracking()
                                        .Include(u => u.Teacher)
                                        .FirstOrDefaultAsync(
                                            u => u.Teacher != null &&
                                            u.Teacher.TeacherId == dto.Id
                                        );

    if (user is null)
      return errorHandler.UnauthorizedResult(
              title: "Login Failed",
              message: "Id or password is invalid.",
              hc: hc
            ); // No match

    var verified2 = passwordHasher.VerifyHashedPassword(
        user, user.PasswordHash!, dto.Password
    );

    bool success2 = verified2 == PasswordVerificationResult.Success;

    // IF Success setup JWT
    if (!success2)
      return errorHandler.UnauthorizedResult(
              title: "Login Failed",
              message: "Id or password is invalid.",
              hc: hc
            ); // No match

    var claims = new List<Claim> {
            new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new (ClaimTypes.Role, Role.ToString()!),
            new (ClaimTypes.Email, user.Email!)
        };

    if (Role == Roles.student)
      claims.Add(new Claim("StudentId", user.Student!.StudentId!));
    if (Role == Roles.teacher)
      claims.Add(new Claim("TeacherId", user.Teacher!.TeacherId!));

    var token2 = jwtProvider.Create(claims);

    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookie"));

    await hc.SignInAsync("Cookie", claimsPrincipal);

    return Results.Ok(token2);
  }

  return errorHandler.BadReqResult(
      title: "Login validation issue",
      message: "Unexpected Role",
      hc: hc
  );
}).AllowAnonymous();  // Doesn't required login

// Logout user
app.MapPost("/logout", async (HttpContext hc) =>
{
  /*
    Purpose:
    Logout user who identity is in hc
  */

  await hc.SignOutAsync("Cookie");
  return Results.Ok();
}).RequireAuthorization("AllRoleAllowed") // You must login, but accepts all people
  .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" }); // Also used cookie


// Register user
app.MapPost("/register", async (
    MyAppDbContext dbContext,
    CreateAccount dto,
    IPasswordHasher<object> passwordHasher,
    IErrorResults errorHandler,
    RegisterStartPolicies policy,
    HttpContext hc
) =>
{
  /*
    Purpose:
    To let user register new account

    Constraint:
    (Check RegisterStartPolicies)

    Process:
    1. validate -> return a valid "data"
    2. Condition which Role is being register
    3. IF teacher add the information to a new Teacher obj and add it to Teachers table
    3. IF student provide ClassName check -> valid, then put the info and add to Students table

    Responds:
    1. badReq -> when fail validation
    2. conflict -> when validate through but acc is duplicated
  */

  (ValidRegister? data, IResult? result) = await policy.Validate(dto, errorHandler, hc);
  if (data is null)
    return result;

  if (data.Role == Roles.student)
  {

    var cls = await dbContext.SchoolClasses.AsNoTracking()
                                            .SingleOrDefaultAsync(
                                              s => s.ClassName == dto.Class
                                            );

    if (cls is null)
      return errorHandler.BadReqResult(
          title: "Register class issues",
          message: "Class was not found",
          hc: hc
      );

    // CREATE NEW STUDENT
    Student student = new()
    {
      StudentId = data.Id,
      ClassId = cls.ClassId,
      User = new()
      {
        Name = data.Name,
        PhoneNumber = dto.PhoneNumber ?? null,
        Email = data.Email,
        PasswordHash = passwordHasher.HashPassword(default!, data.Password)
      }
    };

    await dbContext.Students.AddAsync(student);
    await dbContext.SaveChangesAsync();

    StudentDto newStudent = new
    (
        StudentId: data.Id,
        Name: data.Name,
        ClassName: cls.ClassName,
        PhoneNumber: dto.PhoneNumber ?? "",
        Email: data.Email
    );

    return Results.Created($"/GetPerson/{student.StudentId}", newStudent);
  }
  else if (data.Role == Roles.teacher) // role is teacher 
  {
    Teacher teacher = new()
    {
      TeacherId = data.Id,
      Points = default,
      User = new()
      {
        Name = data.Name,
        PhoneNumber = dto.PhoneNumber ?? null,
        Email = data.Email,
        PasswordHash = passwordHasher.HashPassword(default!, data.Password)
      }
    };

    await dbContext.Teachers.AddAsync(teacher);
    await dbContext.SaveChangesAsync();

    TeacherDto newTeacher = new
    (
      TeacherId: data.Id,
      Name: data.Name,
      PhoneNumber: dto.PhoneNumber ?? "",
      Email: data.Email,
      Points: default
    );

    return Results.Created($"/GetPerson/{teacher.TeacherId}", newTeacher);
  }
  else
    return errorHandler.BadReqResult(
        title: "Register role issue",
        message: "This Role doesnt support register",
        hc: hc
    );
}).AllowAnonymous();

// Refresh page requesting current user data by claim 
app.MapGet("/auth/me", (
  ClaimsPrincipal user,
  IErrorResults errorHandler,
  HttpContext hc
) =>
{
  if (user.Identity?.IsAuthenticated != true)
    return errorHandler.UnauthorizedResult(
              title: "Auth Failed",
              message: "User is not Authenticated",
              hc: hc
            ); // No match

  var role = user.FindFirstValue(ClaimTypes.Role)!.ToLowerInvariant();
  var email = user.FindFirstValue(ClaimTypes.Email);
  var id = role == "student" ? user.FindFirstValue("StudentId")
           : role == "teacher" ? user.FindFirstValue("TeacherId")
           : user.FindFirstValue(ClaimTypes.NameIdentifier);

  return Results.Ok(new { id = id, role = role, email = email });
}).RequireAuthorization("AllRoleAllowed")
  .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie" });

// TODO: Return all possible school classes (no harm data)

app.MapGet("/", () => "Hello World");

app.UseCors("FrontendCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.ShoppingEndpoints().RequireAuthorization();
app.CommunityEndpoints().RequireAuthorization();
app.CommonEndpoints().RequireAuthorization();
app.ChatEndpoints().RequireAuthorization();
app.FriendShipEndpoints().RequireAuthorization();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-10.0
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "productImg")),
  RequestPath = "/productImg"
});


using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
  await db.Database.MigrateAsync();
}

app.Run();