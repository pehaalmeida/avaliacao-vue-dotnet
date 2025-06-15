using ProdutoApi.Data; // Importa o contexto e a biblioteca do EF Core
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Habilita CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   // libera para o front Vue
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configura o Serilog para registrar logs em arquivos pasta "Logs"
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog(); 

// Configura o JSON para usar PascalCase
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Adiciona o HttpClient para chamadas HTTP


// Registra o AppDbContext e conecta com o SQL Server usando a connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ativar o Swagger para testes as API
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run(); // Inicia a aplicação
