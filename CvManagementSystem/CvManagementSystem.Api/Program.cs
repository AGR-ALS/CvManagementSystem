using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserService.Api.Extensions.Authentication;
using UserService.Api.Extensions.Authorizations;
using UserService.Api.Extensions.Cors;
using UserService.Api.Extensions.Environment;
using UserService.Api.Extensions.MessageBrokers;
using UserService.Api.Extensions.Requests;
using UserService.Api.Extensions.Services;
using UserService.Api.Extensions.Storage;
using UserService.Api.Mapping;
using UserService.Api.Middleware;
using UserService.DataAccess.Context;
using UserService.DataAccess.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CvManagementDbContext>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthenticationWithJwtScheme(builder.Configuration);
builder.Services.AddAuthorizationWithDefaultRoles(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddDiServices();
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddRabbitMqViaMassTransit(builder.Configuration);
builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile));
var corsPolicyName = builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.ConfigureRequestHeaders();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsDockerEnvironment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.EnsureS3BucketExistsAsync();
}

if (app.Environment.IsDockerEnvironment() || app.Environment.IsProduction() || app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CvManagementDbContext>();
    dbContext.Database.Migrate();
    await DbSeeder.SeedAsync(dbContext);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors(corsPolicyName);
app.UseMiddleware<RefreshTokenAuthenticationMiddleware>();
app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();
