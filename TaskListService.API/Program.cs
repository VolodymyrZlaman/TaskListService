using Scalar.AspNetCore;
using TaskListService.API.Extensions;
using TaskListService.API.Services;
using TaskListService.Application;
using TaskListService.Application.Contracts.Infrastructure;
using TaskListService.Infrastructure;
using TaskListService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceService(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("TaskList API")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Add user validation middleware
app.UseUserValidation();

app.MapControllers();
app.Run();