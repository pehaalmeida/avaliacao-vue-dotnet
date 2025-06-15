using Microsoft.EntityFrameworkCore;
using ProdutoApi.Models;

namespace ProdutoApi.Data
{
    // AppDbContext representa o contexto do banco de dados
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } // Construtor que recebe as opções de configuração do DbContext
        public DbSet<Produto> Produtos { get; set; } // DbSet representa uma tabela chamada Produtos no banco de dados
    
    }
}