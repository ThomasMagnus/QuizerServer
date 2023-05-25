using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Quizer.IServices;
using Quizer.Models;
using Quizer.Services;
using QuizerServer.HelperClasses;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddTransient<IServiceCollection, ServiceCollection>();
builder.Services.AddScoped<ISubjectsProps, SubjectsProps>();

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logget.txt"));
ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddMvc();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        try
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = builder?.Configuration?.GetSection("JwtSettings:Issuer")?.Value,

                ValidateAudience = true,
                ValidAudience = builder?.Configuration?.GetSection("JwtSettings:Audience")?.Value,

                ValidateLifetime = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(s: builder?.Configuration?.GetSection("JwtSettings:SecretKey")?.Value!)
                        ),

                ValidateIssuerSigningKey = true
            };
        } 
        catch (ArgumentNullException ex)
        {
            logger.LogError(ex.Message);
        }
    });


builder.Services.AddAuthorization();
builder.Services.AddSession();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseMiddleware<AuthorizationMiddleware>();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpLogging();

app.MapControllerRoute(name: "GetSubjects", pattern: "{controller=UserPage}/{action=GetSubjects}");
app.MapControllerRoute(name: "DetectToken", pattern: "{controller=Authorization}/{action=DetectToken}");
app.MapControllerRoute(name: "DetectAuthTeacher", pattern: "{controller=Teacher}/{action=DetectAuth}");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
