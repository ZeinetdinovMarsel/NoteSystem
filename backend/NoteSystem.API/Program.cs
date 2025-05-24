using Microsoft.EntityFrameworkCore;
using NoteSystem.API.Middleware;
using NoteSystem.Core.Interfaces;
using NoteSystem.BusinessLogic.Services;
using NoteSystem.BusinessLogic.Validator;
using NoteSystem.DataAccess;
using NoteSystem.DataAccess.Repositories;
using NoteSystem.Infrastructure;
using NoteSystem.BusinessLogic.Extentions;
using Hangfire;
using Hangfire.PostgreSql;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NoteSystemDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(NoteSystemDbContext)));
});


builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection(nameof(EncryptionSettings)));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<UserValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteService, NoteService>();

builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<ICryptoService, XorCryptoService>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReminderService, ReminderService>();


builder.Services.AddAuth(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите токен в формате: Bearer {your token}"
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
            new string[] { }
        }
    });
});

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("NoteSystemDbContext"));
    }));

builder.Services.AddHangfireServer();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
    options.WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

app.UseExceptionHandlingMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapControllers();

app.Run();
