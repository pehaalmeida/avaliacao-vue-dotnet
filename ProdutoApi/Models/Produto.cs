namespace ProdutoApi.Models
{
    // Classe que representa a tabela Produto no banco de dados
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public string? CodigoBarras { get; set; }
        public string? ImagemBase64 { get; set; }
        public int? IdFakeStore { get; set; }
    }
}