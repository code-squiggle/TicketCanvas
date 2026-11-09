using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using TicketCanvas.Common.Application;
using TicketCanvas.Common.Infrastructure;
using TicketCanvas.Ticket.Api.MappingProfiles;
using TicketCanvas.User.Api.Api;
using TicketCanvas.User.Data;
using TicketCanvas.User.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAuthentication();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPagingHelper, PagingHelper>();

builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.AddSqlServerDbContext<UserDbContext>("user-db");

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalar();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapAuthApi();
app.MapUserApi();

app.Run();