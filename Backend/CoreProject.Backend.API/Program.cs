using CoreProject.Backend.API.Common.Services;
using CoreProject.Backend.API.Middleware;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Application.SystemInfo;
using CoreProject.Backend.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.ResponseStatusCode
        | HttpLoggingFields.Duration;
});
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<GetSystemInfoQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program
{
}
