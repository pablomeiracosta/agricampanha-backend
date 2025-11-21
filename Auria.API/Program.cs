using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Auria.API.Mappings;
using Auria.API.Validators;
using Auria.Bll.Services;
using Auria.Bll.Services.Interfaces;
using Auria.Data.Context;
using Auria.Data.Repositories;
using Auria.Data.Repositories.Interfaces;
using Auria.Structure;

var builder = WebApplication.CreateBuilder(args);

// Inicializa o AuriaContext
var auriaContext = AuriaContext.Initialize(builder.Configuration);

// Configura Serilog
Log.Logger = auriaContext.Log;
builder.Host.UseSerilog();

// Adiciona o AuriaContext como singleton
builder.Services.AddSingleton(auriaContext);

// Registra o Serilog.ILogger no container de DI
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

// Configura o DbContext
builder.Services.AddDbContext<AuriaDbContext>(options =>
    options.UseSqlServer(auriaContext.Settings.ConnectionString));

// Configura AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configura o Mapper no AuriaContext após criar o ServiceProvider temporário
var tempServiceProvider = builder.Services.BuildServiceProvider();
var mapper = tempServiceProvider.GetRequiredService<AutoMapper.IMapper>();
auriaContext.ConfigureMapper(mapper);

// Registra Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<INoticiaRepository, NoticiaRepository>();
builder.Services.AddScoped<ICategoriaNoticiaRepository, CategoriaNoticiaRepository>();
builder.Services.AddScoped<ICotacaoRepository, CotacaoRepository>();

// Registra Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INoticiaService, NoticiaService>();
builder.Services.AddScoped<ICategoriaNoticiaService, CategoriaNoticiaService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IEmailService, GmailService>();
builder.Services.AddScoped<ICotacaoService, CotacaoService>();

// Configura Controllers com limites aumentados
builder.Services.AddControllers(options =>
{
    options.MaxModelValidationErrors = 50;
});

// Configura Kestrel para aceitar requisições maiores
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
});

// Configura FormOptions para upload de arquivos
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
    options.BufferBodyLengthLimit = 100 * 1024 * 1024;
});

// Configura FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// Configura JWT Authentication
var key = Encoding.ASCII.GetBytes(auriaContext.Settings.Jwt.SecretKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = auriaContext.Settings.Jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = auriaContext.Settings.Jwt.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configura Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auria API - Agricampanha",
        Version = "v1",
        Description = "API para gerenciamento de notícias e autenticação"
    });

    // Configuração para JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auria API v1");
    c.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("Iniciando aplicação Auria API - Agricampanha");

app.Run();
