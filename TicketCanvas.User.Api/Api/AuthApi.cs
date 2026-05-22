using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketCanvas.Common.Application;
using TicketCanvas.User.Api.Dtos;
using TicketCanvas.User.Data;
using TicketCanvas.User.Data.Models;
using UserModel = TicketCanvas.User.Data.Models.User;

namespace TicketCanvas.User.Api.Api;

public static class AuthApi
{
    public static IEndpointRouteBuilder MapAuthApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("auth");
        api.MapPost("/register", RegisterUser).WithName("RegisterUser");
        api.MapPost("/login", Login).WithName("Login");
        api.MapPost("/refresh", Refresh).WithName("Refresh");
        api.MapPost("/logout", Logout).WithName("Logout");
        return app;
    }

    public static async Task<Ok<UserResponse>> RegisterUser(
        RegisterUserRequest registerUserRequest,
        UserDbContext userDbContext,
        IMapper mapper,
        IPasswordHasher<UserModel> passwordHasher)
    {
        var user = mapper.Map<UserModel>(registerUserRequest);
        user.PasswordHash = passwordHasher.HashPassword(user, registerUserRequest.Password);
        user.Role = UserRole.Customer;

        userDbContext.Users.Add(user);
        await userDbContext.SaveChangesAsync();

        var userDto = mapper.Map<UserResponse>(user);

        return TypedResults.Ok(userDto);
    }

    public static async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Login(
        LoginRequest loginRequest,
        UserDbContext userDbContext,
        IPasswordHasher<UserModel> passwordHasher)
    {
        UserModel? user = await userDbContext.Users.SingleOrDefaultAsync(user => user.Email == loginRequest.Email);
        if (user == null)
            return TypedResults.Unauthorized();

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
        if (result == PasswordVerificationResult.Failed)
            return TypedResults.Unauthorized();

        TokenResponse tokens = GetTokens(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = GetRefreshTokenHash(tokens.RefreshToken),
            ExpiresAt = tokens.RefreshTokenExpiresAt
        };

        userDbContext.RefreshTokens.Add(refreshToken);
        await userDbContext.SaveChangesAsync();


        return TypedResults.Ok(tokens);
    }

    public static async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Refresh(RefreshRequest refreshRequest, UserDbContext userDbContext)
    {
        string refreshTokenHash = GetRefreshTokenHash(refreshRequest.RefreshToken);   
        RefreshToken? refreshToken = await userDbContext.RefreshTokens.SingleOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == refreshTokenHash);
        if (refreshToken == null)
            return TypedResults.Unauthorized();

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return TypedResults.Unauthorized();

        if (refreshToken.RevokedAt is not null)
        {
            List<RefreshToken> refreshTokens = await userDbContext.RefreshTokens
                .Where(rt => rt.UserId == refreshToken.UserId && rt.RevokedAt == null)
                .ToListAsync();
            refreshTokens.ForEach(rt => rt.RevokedAt = DateTime.UtcNow);
            await userDbContext.SaveChangesAsync();
            return TypedResults.Unauthorized();
        }

        UserModel? user = await userDbContext.Users.SingleOrDefaultAsync(user => user.Id == refreshToken.UserId);
        if (user == null)
            return TypedResults.Unauthorized();

        TokenResponse tokens = GetTokens(user);
                 
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = GetRefreshTokenHash(tokens.RefreshToken),
            ExpiresAt = tokens.RefreshTokenExpiresAt
        };

        refreshToken.RevokedAt = DateTime.UtcNow;
        userDbContext.RefreshTokens.Add(newRefreshToken);
        await userDbContext.SaveChangesAsync();

        return TypedResults.Ok(tokens);
    }

    public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(LogoutRequest logoutRequest, UserDbContext userDbContext)
    {
        string refreshTokenHash = GetRefreshTokenHash(logoutRequest.RefreshToken);
        RefreshToken? refreshToken = await userDbContext.RefreshTokens.SingleOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == refreshTokenHash);
        if (refreshToken == null)
            return TypedResults.Unauthorized();

        List<RefreshToken> refreshTokens = await userDbContext.RefreshTokens
            .Where(rt => rt.UserId == refreshToken.UserId && rt.RevokedAt == null)
            .ToListAsync();
        refreshTokens.ForEach(rt => rt.RevokedAt = DateTime.UtcNow);
        await userDbContext.SaveChangesAsync();
        return TypedResults.Ok();
    }
    
    private static string GetRefreshTokenHash(string refreshTokenString)
    {
        string refreshTokenHash;
        var refreshTokenBytes = Encoding.UTF8.GetBytes(refreshTokenString);
        var refreshTokenHashBytes = SHA256.HashData(refreshTokenBytes);
        refreshTokenHash = Convert.ToBase64String(refreshTokenHashBytes);
        return refreshTokenHash;
    }

    private static TokenResponse GetTokens(UserModel user)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText("private.pem"));

        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var tokenExpiresAt = DateTime.UtcNow.AddMinutes(15);

        var token = new JwtSecurityToken(
            issuer: "issuer",
            audience: "audience",
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: tokenExpiresAt,
            signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshTokenSourceBytes = RandomNumberGenerator.GetBytes(32);
        var refreshTokenString = Convert.ToBase64String(refreshTokenSourceBytes);
        
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(1);

        return new TokenResponse(tokenString, tokenExpiresAt, refreshTokenString, refreshTokenExpiresAt);
    }
}
