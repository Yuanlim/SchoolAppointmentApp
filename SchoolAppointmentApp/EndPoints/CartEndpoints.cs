using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolAppointmentApp.Data;
using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;
using SchoolAppointmentApp.FunctionalClasses;
using SchoolAppointmentApp.Mapping;

namespace SchoolAppointmentApp.EndPoints;

public static class MyCart
{
  public static RouteGroupBuilder CartEndpoints(this RouteGroupBuilder shoppingRoute)
  {
    var group = shoppingRoute.MapGroup("/Cart");

    // Get my own cart 
    group.MapGet("/Get", async (
      ClaimsPrincipal user,
      HttpContext hc,
      UnAuthorizedValidator validator,
      IErrorResults errorHandler,
      CancellationToken ct,
      MyAppDbContext dbContext
    ) =>
    {
      // Validation guard
      (_, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: $"Unauthorized, user doesn't existed",
          hc: hc
        );

      // Get chart
      Cart? cart = await dbContext.Carts.AsNoTracking()
                                        .Include(c => c.CartProductList)
                                          .ThenInclude(cpl => cpl.Product)
                                        .Where(
                                          c =>
                                          c.CustomerId == teacher.TeacherId &&
                                          !c.Ordered
                                        )
                                        .FirstOrDefaultAsync(ct);

      return Results.Ok(cart.ToCartDto());
    });


    // I delete something from cart
    group.MapDelete("/Product/Delete/{productId}", async (
      int productId,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      CancellationToken ct,
      UnAuthorizedValidator validator,
      IGetCart getCartHandler,
      IGetCartItem getCartItemHandler,
      MyAppDbContext dbContext,
      CartHandler cartHandler
    ) =>
    {
      // Validation of user
      (_, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: "Unauthorized, user doesn't existed",
          hc: hc
        );

      Cart? cart = await getCartHandler.GetCartQueryAsync(user, ct)
                                        .FirstOrDefaultAsync(ct);

      if (cart is null)
        return errorHandler.NotFoundResult(
          title: "Get request reported empty.",
          message: "Teacher has no unordered cart.",
          hc: hc,
          user: teacher.User
        );

      // Find CartItem and delete as tracking
      CartItem? cartItem = await getCartItemHandler.ThatItemQuery(cart, productId)
                                                    .FirstOrDefaultAsync(ct);

      if (cartItem is null)
        return errorHandler.NotFoundResult(
          title: "Cart Error",
          message: "Teacher has cart, but no this item.",
          hc: hc
        );

      cart.CartProductList.Remove(cartItem);

      // Recompute cart cost
      cart.TotalCost = cartHandler.RecomputeCartTotalPrice(cart);

      await dbContext.SaveChangesAsync(ct);

      return Results.NoContent();
    });


    // I change quantity
    group.MapPatch("/Product/Patch", async (
      WishItemDto dto,
      ClaimsPrincipal user,
      IErrorResults errorHandler,
      HttpContext hc,
      CancellationToken ct,
      UnAuthorizedValidator validator,
      IGetCart getCartHandler,
      IGetCartItem getCartItemHandler,
      IProductListClasses productHandler,
      MyAppDbContext dbContext,
      CartHandler cartHandler
    ) =>
    {
      (_, Teacher? teacher) = await validator.IsResults<Teacher>(
        expectedRole: Roles.teacher,
        user: user,
        ct: ct
      );

      if (teacher is null)
        return errorHandler.UnauthorizedResult(
          title: "Reported fake user",
          message: "Unauthorized, user doesn't existed",
          hc: hc
        );

      // Get Product quantity， check ask for quantity larger than available quantity
      Product? product = await productHandler.GetProductAsync(productId: dto.ProductId, ct);
      if (product is null)
        return errorHandler.NotFoundResult(
          title: "Get Product Failed",
          message: "Product didn't exist",
          hc
        );
      if (product.AvailableQuantity < dto.Quantity)
        return errorHandler.BadReqResult(
          title: "Insufficient",
          message: "No enough stock",
          hc: hc,
          user: teacher.User
        );

      // Find cart
      Cart? cart = await getCartHandler.GetCartQueryAsync(user, ct).FirstOrDefaultAsync();
      if (cart is null)
        return errorHandler.NotFoundResult(
          title: "Get request reported empty.",
          message: "Teacher has no unordered cart.",
          hc: hc,
          user: teacher.User
        );

      // Find CartItem as tracking
      CartItem? cartItem = await getCartItemHandler.ThatItemQuery(
        cart, dto.ProductId
      ).FirstOrDefaultAsync(ct);
      if (cartItem is null)
        return errorHandler.NotFoundResult(
          title: "Cart delete error",
          message: "The cart doesn't have that item",
          hc: hc,
          user: teacher.User
        );

      // Treat 0 as delete
      if (dto.Quantity == 0)
        cart.CartProductList.Remove(cartItem);
      else
        cartItem.Quantity = dto.Quantity;

      cart.TotalCost = cartHandler.RecomputeCartTotalPrice(cart);
      await dbContext.SaveChangesAsync(ct);
      return Results.Ok(cartItem.ToCartItemDto());
    });

    return group;
  }
}