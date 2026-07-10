using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Resend;
using Tp_Programacion.Config;
using Tp_Programacion.Models.Role;
using Tp_Programacion.Repository;
using Tp_Programacion.Services;
using Tp_Programacion.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Cursos API",
        Description = "Plataforma de cursos",
        TermsOfService = new Uri("https://www.cursosAPI.com"),
    });
});


//Services
builder.Services.AddScoped<CursoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEncoderService, EncoderService>();
builder.Services.AddScoped<EmailService>();

// Repositories

builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();

//mapper
builder.Services.AddAutoMapper(cfg => { }, typeof(Mapping));
// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("devConnection"));
});

// Autenticacion basada unicamente en cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Cookie.IsEssential = true;
        opt.ExpireTimeSpan = TimeSpan.FromDays(1);

        // Para que la API devuelva códigos de estado en vez de redirigir a páginas web
        opt.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        opt.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// Filter
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
        .Where(x => x.Value?.Errors.Count > 0)
        .ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
        );
        ResponseValidation validation = new ResponseValidation(errors);
        return new BadRequestObjectResult(validation);
    };
});

// Email
builder.Services.AddResend(o =>
{
    o.ApiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN")!;
});

var app = builder.Build();


app.UseCors(options =>
{
    options.WithOrigins(
        "http://localhost:5173",
        "https://cursos-api.vercel.app"
    );
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowCredentials();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
