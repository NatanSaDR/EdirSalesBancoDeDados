using EdirSalesBancoDeDados.Infra.Ioc;
using EdirSalesBancoDeDados.Infrastructure.Migrations;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// 0 = Escritorio  |  1 = Casa 
builder.Services.AddDependencyInjection(builder.Configuration, 0);
builder.Services.AddScoped<TokenService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                In = ParameterLocation.Header,
            },
            new List<string> ()
        }
    });

});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
 {
     policy.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
 });
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // M�ximo de 10 requisi��es
                Window = TimeSpan.FromSeconds(30) // A cada 30 segundos
            }
        ));
});

var app = builder.Build();

app.UseCors("AllowAll");

//teste git
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
MigratieDataBase();
app.MapControllers();

app.Run();

void MigratieDataBase()
{
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("ConnectionSQL_Escritorio")!;
        using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        DatabaseMigration.Migrate(connectionString, serviceScope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro na migra��o do banco: {ex.Message}");
    }
}