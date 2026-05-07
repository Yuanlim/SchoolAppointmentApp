using Microsoft.AspNetCore.Identity;
using SchoolAppointmentApp.FunctionalClasses;

namespace SchoolAppointmentApp.Registrations;

internal static class ServicesDIContainer
{
    public static void AddServiceToContainer(this WebApplicationBuilder builder)
    {
        // Transient, Scope, Singleton DI registration
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
    }
}