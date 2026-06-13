using BackEndKrooq.Data;
using BackEndKrooq.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "https://krooq.up.railway.app",
                "https://front-end-krooq-production.up.railway.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("Jwt:Key não foi configurado.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new Exception("Jwt:Issuer não foi configurado.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception("Jwt:Audience não foi configurado.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProjetoService>();
builder.Services.AddScoped<AmbienteService>();

builder.Services.AddHttpClient<OpenAiService>();
builder.Services.AddScoped<IaService>();
builder.Services.AddScoped<EstimativaCustoService>();
builder.Services.AddHttpClient<IaImagemService>();
builder.Services.AddScoped<TarefaService>();

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    Console.WriteLine("Erro ao aplicar migrations:");
    Console.WriteLine(ex.Message);
    throw;
}

app.UseCors("PermitirTudo");

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/Scalar/"));

var webRoot = app.Environment.WebRootPath;

if (string.IsNullOrWhiteSpace(webRoot))
{
    webRoot = Path.Combine(
        app.Environment.ContentRootPath,
        "wwwroot"
    );
}

Console.WriteLine($"WebRoot encontrado: {webRoot}");

if (!Directory.Exists(webRoot))
{
    Directory.CreateDirectory(webRoot);
}

var pastaUploadsIa = Path.Combine(
    webRoot,
    "uploads",
    "ia"
);

if (!Directory.Exists(pastaUploadsIa))
{
    Directory.CreateDirectory(pastaUploadsIa);
}

app.UseStaticFiles();

File.WriteAllText(
    Path.Combine(webRoot, "teste.txt"),
    "KROOQ"
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine($"WebRoot: {app.Environment.WebRootPath}");
Console.WriteLine($"ContentRoot: {app.Environment.ContentRootPath}");

app.Run();