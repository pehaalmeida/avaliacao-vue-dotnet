using ProdutoApi.Data; // Importa o contexto e a biblioteca do EF Core
using Microsoft.EntityFrameworkCore;

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

// Adiciona os servi�os b�sicos da API (Controllers, Swagger etc)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Adiciona suporte � inje��o de HttpClient


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

app.Run(); // Inicia a aplica��o
