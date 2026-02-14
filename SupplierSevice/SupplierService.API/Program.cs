using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using SupplierService.Infrastructure.Persistence;
using SupplierService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SupplierService.API.Middleware;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SupplierDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();

builder.Services.AddScoped<CreateSupplierUseCase>();
builder.Services.AddScoped<GetSuppliersUseCase>();
builder.Services.AddScoped<GetSupplierByIdUseCase>();
builder.Services.AddScoped<UpdateSupplierUseCase>();
builder.Services.AddScoped<DeleteSupplierUseCase>();

builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<GetProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<DeleteProductUseCase>();

builder.Services.AddScoped<CreateDeliveryUseCase>();
builder.Services.AddScoped<GetDeliveriesUseCase>();
builder.Services.AddScoped<GetDeliveryByIdUseCase>();
builder.Services.AddScoped<UpdateDeliveryUseCase>();
builder.Services.AddScoped<DeleteDeliveryUseCase>();

builder.Services.AddHttpClient(nameof(UserStatusMiddleware));

var key = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// Apply migrations at startup (container/dev friendliness).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupplierDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handler must run early in pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (!string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseMiddleware<UserStatusMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
