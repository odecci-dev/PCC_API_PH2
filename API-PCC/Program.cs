using API_PCC;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using static API_PCC.Controllers.UserController;
IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Path.GetPathRoot(Environment.SystemDirectory))
        .AddJsonFile("app/pcc/appconfig.json", optional: true, reloadOnChange: true)
        .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddDbContext<PCC_DEVContext>(options =>
options.UseSqlServer((config["ConnectionStrings:DevConnection"])));
var appSettingsSection = builder.Configuration.GetSection("Mail");
var emailSettingsSection = config.GetSection("ConnectionStrings:Mail");
var emailSettings = emailSettingsSection.Get<List<EmailSettings>>();
builder.Services.Configure<List<EmailSettings>>(emailSettingsSection);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Configuration.AddJsonFile("appconfig.json", optional: true, reloadOnChange: true);

builder.Services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

});
builder.Services.AddAuthentication("Basic").AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKey",
        authBuilder =>
        {
            authBuilder.RequireRole("Administrators");
        });

});
builder.Services.AddSingleton<JwtAuthenticationManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Web API");

});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
