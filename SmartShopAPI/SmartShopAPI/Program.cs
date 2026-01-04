using System.Text;

using FluentValidation;
using FluentValidation.AspNetCore;

using MassTransit;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using SmartShopAPI;
using SmartShopAPI.Authorization;
using SmartShopAPI.Data;
using SmartShopAPI.Entities;
using SmartShopAPI.Interfaces;
using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Interfaces.Services.Core;
using SmartShopAPI.Interfaces.Services.Infrastructure;
using SmartShopAPI.Interfaces.Services.Integrations;
using SmartShopAPI.Middleware;
using SmartShopAPI.Models.Dtos;
using SmartShopAPI.Models.Dtos.Product;
using SmartShopAPI.Models.Dtos.User;
using SmartShopAPI.Models.Settings;
using SmartShopAPI.Models.Validators;
using SmartShopAPI.Repositories;
using SmartShopAPI.Services.Core;
using SmartShopAPI.Services.Infrastructure;
using SmartShopAPI.Services.Integrations;

DotNetEnv.Env.Load("../../.env");
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var allowedUi = builder.Configuration["AllowedUi"]
                ?? "http://localhost:4200";
builder.Services.AddHealthChecks();

var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);

builder.Services.Configure<PayPalSettings>(
    builder.Configuration.GetSection("PayPal")
);

var paymentSettings = new PaymentsSettings();
builder.Configuration.GetSection("Payments").Bind(paymentSettings);
builder.Services.AddSingleton(paymentSettings);

var rabbitSettings = new RabbitMqSettings();
builder.Configuration.GetSection("RabbitMQ").Bind(rabbitSettings);
builder.Services.AddSingleton(rabbitSettings);
if (rabbitSettings.Host != "disabled") {
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumers(typeof(Program).Assembly);
    
        if (builder.Environment.IsEnvironment("IntegrationTest"))
        {
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        }
        else
        {
            x.AddEntityFrameworkOutbox<SmartShopDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UseSqlServer();
                o.UseBusOutbox();
                o.DisableInboxCleanupService();
            });
    
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitSettings.Host, (ushort)rabbitSettings.Port, "/", h =>
                {
                    h.Username(rabbitSettings.UserName);
                    h.Password(rabbitSettings.Password);
                });
    
                cfg.ConfigureEndpoints(context);
            });
        }
    });
}

builder.Services
    .AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
        };
    });
if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.Services.AddDbContext<SmartShopDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("SmartShopDbConnection")));
}

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<SmartShopSeeder>();
builder.Services.AddHttpClient<IEmailSender, ResendEmailSender>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<UpsertProductDto>, CreateProductDtoValidator>();
builder.Services.AddScoped<IValidator<QueryParams>, QueryParamsValidator>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IPaymentProvider, PayPalProvider>();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. " +
            "Enter your token below",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSmartShopUI",
        builder => builder.WithOrigins(allowedUi, "http://localhost:4288", "http://127.0.0.1:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SmartShopSeeder>();
    seeder.Seed();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSmartShopUI");
app.UseRouting();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.UseStaticFiles();
app.Run();

public partial class Program { }
