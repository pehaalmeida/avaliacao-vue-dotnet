using ProdutoApi.Data; // Importa o contexto e a biblioteca do EF Core
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços básicos da API (Controllers, Swagger etc)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Adiciona suporte à injeção de HttpClient


// Registra o AppDbContext e conecta com o SQL Server usando a connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ativar o Swagger para testes as API
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run(); // Inicia a aplicação
