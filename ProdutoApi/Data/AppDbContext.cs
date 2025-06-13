using Microsoft.EntityFrameworkCore;
using ProdutoApi.Models;

namespace ProdutoApi.Data
{
    // Classe que representa a conex�o com o banco e o mapeamento das tabelas
    public class AppDbContext : DbContext
    {
        // Construtor que recebe as configura��es via inje��o de depend�ncia
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet representa uma tabela chamada Produtos no banco de dados
        public DbSet<Produto> Produtos { get; set; }
    }
}