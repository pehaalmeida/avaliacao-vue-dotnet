namespace ProdutoApi.Models
{
    // Classe que representa a tabela Produto no banco de dados
    public class Produto
    {
        public int Id { get; set; } // Chave prim�ria
        public string? Nome { get; set; } // Nome do produto
        public decimal Preco { get; set; } // Pre�o do produto
        public string? CodigoBarras { get; set; } // C�digo de barras
        public byte[]? Imagem { get; set; } // Imagem em formato de bytes
    }
}