using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Application.Dtos;
using TicketCanvas.Common.Http.Extensions;
using TicketCanvas.User.Api.Dtos;
using TicketCanvas.User.Data;
using UserModel = TicketCanvas.User.Data.Models.User;

namespace TicketCanvas.User.Api.Api;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("users");
        api.MapGet("/me", GetCurrentUser).RequireAuthorization().WithName("GetCurrentUser");
        api.MapPut("/me", UpdateUser).RequireAuthorization().WithName("UpdateUser");
        api.MapPut("/me/password", ChangePassword).RequireAuthorization().WithName("ChangePassword");
        api.MapGet("/", GetUsers).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("GetUsers");
        api.MapGet("/{id:guid}", GetUser).RequireAuthorization(cfg => cfg.RequireRole("Admin")).WithName("GetUser");
        return app;
    }

    public static async Task<Results<Ok<UserResponse>, NotFound>> GetCurrentUser(
        ClaimsPrincipal claimsPrincipal,
        UserDbContext userDbContext,
        IMapper mapper)
    {
        Guid userId = claimsPrincipal.GetUserId();
        UserModel? user = await userDbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
        if (user == null)
            return TypedResults.NotFound();

        UserResponse userDto = mapper.Map<UserResponse>(user);

        return TypedResults.Ok(userDto);
    }

    public static async Task<Results<Ok<UserResponse>, NotFound>> UpdateUser(
        UpdateUserRequest updateUserRequest,
        ClaimsPrincipal claimsPrincipal,
        UserDbContext userDbContext,
        IMapper mapper)
    {
        Guid userId = claimsPrincipal.GetUserId();
        UserModel? user = await userDbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
        if (user == null)
            return TypedResults.NotFound();

        mapper.Map(updateUserRequest, user);
        await userDbContext.SaveChangesAsync();

        UserResponse userDto = mapper.Map<UserResponse>(user);

        return TypedResults.Ok(userDto);
    }

    public static async Task<Results<Ok, NotFound, UnauthorizedHttpResult>> ChangePassword(
        ChangePasswordRequest changePasswordRequest,
        ClaimsPrincipal claimsPrincipal,
        UserDbContext userDbContext)
    {
        Guid userId = claimsPrincipal.GetUserId();
        UserModel? user = await userDbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
        if (user == null)
            return TypedResults.NotFound();

        var hasher = new PasswordHasher<object>();

        var result = hasher.VerifyHashedPassword(new(), user.PasswordHash, changePasswordRequest.CurrentPassword);
        if (result == PasswordVerificationResult.Failed)
            return TypedResults.Unauthorized();

        user.PasswordHash = hasher.HashPassword(new(), changePasswordRequest.NewPassword);
        await userDbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }

    public static async Task<Ok<PagedResponse<UserResponse>>> GetUsers(
        [AsParameters] GetUsersRequest request,
        UserDbContext userDbContext,
        [FromServices] IPagingHelper pagingHelper,
        CancellationToken cancellationToken)
    {
        var queryableUsers = userDbContext.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Email))
            queryableUsers = queryableUsers.Where(user => user.Email == request.Email);
            
        if (!string.IsNullOrEmpty(request.FirstName))
            queryableUsers = queryableUsers.Where(user => user.FirstName == request.FirstName);

        if (!string.IsNullOrEmpty(request.LastName))
            queryableUsers = queryableUsers.Where(user => user.LastName == request.LastName);

        var sortDictionary = new Dictionary<string, Expression<Func<UserModel, object?>>>
        {
            ["email"] = user => user.Email,
            ["firstName"] = user => user.FirstName,
            ["lastName"] = user => user.LastName,
            ["createdAt"] = user => user.CreatedAt,
        };

        var result = await pagingHelper.GetPagedItems<UserModel, UserResponse>(
            request,
            queryableUsers,
            sortDictionary,
            "createdAt",
            "desc",
            cancellationToken);

        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<UserResponse>, NotFound>> GetUser(
        Guid id,
        UserDbContext userDbContext,
        IMapper mapper)
    {
        UserModel? user = await userDbContext.Users.SingleOrDefaultAsync(user => user.Id == id);
        if (user == null)
            return TypedResults.NotFound();

        UserResponse userDto = mapper.Map<UserResponse>(user);

        return TypedResults.Ok(userDto);
    }
}
