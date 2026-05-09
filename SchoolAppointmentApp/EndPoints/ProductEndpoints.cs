using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class Products
{
  public static RouteGroupBuilder ProductEndpoints(this RouteGroupBuilder shoppingRoute)
  {
    var group = shoppingRoute.MapGroup("/Product");

    group.MapPost("/New", async (
        CreateProductDto dto,
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        CancellationToken ct,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

      // Validation of user
      (bool auth, _) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unautherized, user doesnt existed",
          hc: hc
        );

      Product product = new()
      {
        ProductName = dto.ProductName,
        ProductImageRoot = dto.ProductImageRoot,
        Description = dto.Description,
        PointCost = dto.PointCost,
        AvailableQuantity = dto.Quantity
      };

      await dbContext.AddAsync(product, ct);
      await dbContext.SaveChangesAsync(ct);

      return Results.Created($"/Product/{product.ProductId}", product);
    }).RequireAuthorization("PrincipalAllowed");

    group.MapPatch("/Change", async (
        PatchProductDto dto,
        ClaimsPrincipal user,
        MyAppDbContext dbContext,
        CancellationToken ct,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

      // Validation of user
      (bool auth, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unauthorized, user doesn't existed",
          hc: hc
        );

      var theProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId, ct);
      if (theProduct is null) return Results.NotFound("No such product");

      theProduct.ProductName = dto.ProductName ?? theProduct.ProductName;
      theProduct.ProductImageRoot = dto.ProductImageRoot ?? theProduct.ProductImageRoot;
      theProduct.Description = dto.Description ?? theProduct.Description;
      theProduct.PointCost = dto.PointCost ?? theProduct.PointCost;
      theProduct.AvailableQuantity = dto.Quantity ?? theProduct.AvailableQuantity;

      await dbContext.SaveChangesAsync(ct);

      return Results.Ok($"Product id {theProduct.ProductId} properties Changed");
    }).RequireAuthorization("PrincipalAllowed");

    group.MapDelete("/Remove/{productId}", async (
        int productId,
        MyAppDbContext dbContext,
        CancellationToken ct,
        ClaimsPrincipal user,
        IErrorResults errorHandler,
        HttpContext hc,
        UnAuthorizedValidator validator
    ) =>
    {
      var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

      // Validation of user
      (bool auth, _) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!auth)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unauthorized, user doesn't existed",
          hc: hc
        );

      int row = await dbContext.Products.Where(p => p.ProductId == productId)
                                        .ExecuteDeleteAsync(ct);

      if (row == 0)
        return errorHandler.NotFoundResult(
          title: "Product Error",
          message: $"No such product, in id {productId}",
          hc: hc
        );

      await dbContext.SaveChangesAsync(ct);

      return Results.NoContent();
    }).RequireAuthorization("PrincipalAllowed");

    group.MapGet("/Get/{id}", async (
      int id,
      UnAuthorizedValidator validator,
      ClaimsPrincipal user,
      CancellationToken ct,
      IErrorResults errorHandler,
      HttpContext hc,
      MyAppDbContext dbContext,
      IProductListClasses productHandler
    ) =>
    {
      (bool isTeacher, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );
      (bool isPrincipal, _) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!isTeacher && !isPrincipal)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unauthorized, user doesn't existed",
          hc: hc
        );

      Product? product = await productHandler.GetProductAsync(productId: id, ct: ct);
      if (product is null)
        return errorHandler.NotFoundResult(
          title: "Get Product Failed",
          message: "Product didn't exist",
          hc
        );

      return Results.Ok(product.ToProductDto());
    }).RequireAuthorization("TeacherOrPrincipalAllowed");

    group.MapGet("/GetList/", async (
      string? SearchString,
      int RequestExpandTimes,
      CancellationToken ct,
      IProductListClasses GetProductHandler,
      MyAppDbContext dbContext,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      UnAuthorizedValidator validator
    ) =>
    {
      // User input nothing or white space only, nothing happen
      // if (string.IsNullOrWhiteSpace(searchString)) return Results.NoContent();

      // Validation of user
      (bool isTea, _) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      (bool isSp, _) = await validator.IsResults<SchoolPrincipal>(
        expectedRole: Roles.schoolPrincipal,
        user: user,
        ct: ct
      );

      if (!isSp && !isTea)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: "Unauthorized, user doesn't existed",
          hc: hc
        );

      var productList = await GetProductHandler.GetProductList(
        searchString: SearchString,
        limitStep: RequestExpandTimes,
        maximumProductListedEachExpand: 5,
        ct: ct
      );

      return Results.Ok(productList);
    }).RequireAuthorization("TeacherOrPrincipalAllowed");

    group.MapPost("/New/in/List", async (
      ICollection<CreateProductDto> dto,
      UnAuthorizedValidator validator,
      CancellationToken ct,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      MyAppDbContext dbContext
    ) =>
    {
      (bool isSp, _) = await validator.IsResults<SchoolPrincipal>(Roles.schoolPrincipal, ct, user);

      if (!isSp)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unauthorized, user doesn't existed",
          hc: hc
        );

      ICollection<Product> products = [];
      foreach (var p in dto)
      {
        products.Add(new()
        {
          ProductName = p.ProductName,
          ProductImageRoot = p.ProductImageRoot,
          Description = p.Description,
          AvailableQuantity = p.Quantity,
          PointCost = p.PointCost
        });
      }

      await dbContext.AddRangeAsync(products);
      await dbContext.SaveChangesAsync();

      return Results.Created("", products);
    }).RequireAuthorization("PrincipalAllowed");

    return group;
  }
}