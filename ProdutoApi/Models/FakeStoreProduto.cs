namespace ProdutoApi.Models
{
    public class FakeStoreProduto
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public decimal price { get; set; }
        public string description { get; set; } = "Produto integrado";
        public string image { get; set; } = string.Empty;
        public string category { get; set; } = "geral";
    }
}