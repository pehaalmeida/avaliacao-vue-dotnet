namespace ProdutoApi.Models
{
    // Classe que representa a tabela Produto no banco de dados
    public class Produto
    {
        public int Id { get; set; } // Chave primária
        public string? Nome { get; set; } // Nome do produto
        public decimal Preco { get; set; } // Preço do produto
        public string? CodigoBarras { get; set; } // Código de barras
        public byte[]? Imagem { get; set; } // Imagem em formato de bytes
    }
}