using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiTenantAPI.API.Middleware;
using MultiTenantAPI.Application.Auth;
using MultiTenantAPI.Application.Products;
using MultiTenantAPI.Application.Tenants;
using MultiTenantAPI.Domain.Entities;
using MultiTenantAPI.Infrastructure.Auth;
using MultiTenantAPI.Infrastructure.Persistence;
using MultiTenantAPI.Infrastructure.Products;
using MultiTenantAPI.Infrastructure.Tenants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<TenantSessionOptions>(builder.Configuration.GetSection(TenantSessionOptions.SectionName));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problem);
        };
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

builder.Services.AddDbContext<MasterDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MasterDb");
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
});

builder.Services.AddDbContext<TenantDbContext>((sp, options) =>
{
    var tenantProvider = sp.GetRequiredService<ITenantProvider>();
    if (tenantProvider.TryGetCurrentTenant(out var tenant) && tenant is not null)
    {
        options.UseSqlServer(tenant.ConnectionString, sql => sql.EnableRetryOnFailure());
    }
});

builder.Services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ITenantStore, TenantStore>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IProductRepository, DapperProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();

builder.Services.AddScoped<MultiTenantAPI.Application.Abstractions.IDbConnectionFactory, TenantDbConnectionFactory>();

builder.Services.AddScoped<ITenantSessionService>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TenantSessionOptions>>().Value;
    return options.Mode switch
    {
        TenantSessionMode.Redis => ActivatorUtilities.CreateInstance<RedisTenantSessionService>(sp),
        _ => ActivatorUtilities.CreateInstance<CookieTenantSessionService>(sp)
    };
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = exceptionFeature?.Error.Message
        };
        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
