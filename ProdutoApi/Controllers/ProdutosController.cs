using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProdutoApi.Data;
using ProdutoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ProdutoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos(
        [FromQuery] string? nome,
        [FromQuery] string? codigoBarras,
        [FromQuery] string? ordenarPor,
        [FromQuery] string? ordem,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
                var query = _context.Produtos.AsQueryable();

                // Filtro por nome (ignora maiúsculas/minúsculas)
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    query = query.Where(p =>
                        !string.IsNullOrWhiteSpace(p.Nome) &&
                        p.Nome.ToLower().Contains(nome!.ToLower()));
                }

                // Filtro por código de barras
                if (!string.IsNullOrWhiteSpace(codigoBarras))
                    query = query.Where(p => p.CodigoBarras == codigoBarras);

                // Ordenação por preço
                if (!string.IsNullOrWhiteSpace(ordenarPor) && ordenarPor.ToLower() == "preco")
                {
                    query = (!string.IsNullOrWhiteSpace(ordem) && ordem.ToLower() == "desc")
                      ? query.OrderByDescending(p => p.Preco)
                      : query.OrderBy(p => p.Preco);
                }

            // Paginação
            query = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                return await query.ToListAsync();
        }

        // GET: api/produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        // POST: api/produtos
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(
        Produto produto,
        [FromServices] IWebHostEnvironment env,
        [FromServices] IHttpClientFactory httpClientFactory)
        {
            // Validação dos dados recebidos
            if (string.IsNullOrWhiteSpace(produto.Nome) ||
                string.IsNullOrWhiteSpace(produto.CodigoBarras) ||
                produto.Preco <= 0 ||
                string.IsNullOrWhiteSpace(produto.ImagemBase64))
            {
                return BadRequest("Todos os campos são obrigatórios, incluindo a imagem em base64.");
            }

            // Garante que a pasta de imagens existe
            var pastaImagem = Path.Combine(env.WebRootPath, "imagens");
            Directory.CreateDirectory(pastaImagem);

            // Gera nome único para salvar o arquivo
            var nomeArquivo = $"{Guid.NewGuid()}.png";
            var caminhoImagem = Path.Combine(pastaImagem, nomeArquivo);

            try
            {
                var imagemBase64Limpa = produto.ImagemBase64
                    .Trim()
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace(" ", "");

                Console.WriteLine("Base64 recebida:");
                Console.WriteLine(imagemBase64Limpa);

                var imagemBytes = Convert.FromBase64String(imagemBase64Limpa);

                Console.WriteLine("Tamanho em bytes: " + imagemBytes.Length);

                await System.IO.File.WriteAllBytesAsync(caminhoImagem, imagemBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO AO CONVERTER BASE64:");
                Console.WriteLine(ex.Message);
                return BadRequest("Imagem enviada não está em formato base64 válido.");
            }


            // Salva o produto no banco (com base64 incluído)
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Gera URL pública para a imagem
            var scheme = Request?.Scheme ?? "http";
            var host = Request?.Host.ToString() ?? "localhost:5090";
            var urlImagem = $"{scheme}://{host}/imagens/{nomeArquivo}";

            // Cria objeto para enviar à Fake Store
            var fakeProduct = new FakeStoreProduto
            {
                title = produto.Nome!,
                price = produto.Preco,
                image = urlImagem,
                category = "geral"
            };

            // Envia o produto para a Fake Store
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://fakestoreapi.com/products", fakeProduct);

            if (response.IsSuccessStatusCode)
            {

                // Lê como string primeiro
                var respostaFake = await response.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Produto replicado com sucesso na Fake Store:");
                Console.WriteLine(respostaFake);

                // Desserializa manualmente o ID a partir da string (sem perder o conteúdo)
                var fakeResponse = JsonSerializer.Deserialize<FakeStoreProduto>(respostaFake);

                // Salva o ID retornado
                produto.IdFakeStore = fakeResponse?.id;
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("❌ Erro ao replicar na Fake Store:");
                Console.WriteLine("Status: " + response.StatusCode);
                var erro = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Detalhes: " + erro);
            }


            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }


        // PUT: api/produtos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(
        int id,
        Produto produtoAtualizado,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IWebHostEnvironment env)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
                return NotFound();

            // Atualiza os dados locais
            produto.Nome = produtoAtualizado.Nome;
            produto.Preco = produtoAtualizado.Preco;
            produto.CodigoBarras = produtoAtualizado.CodigoBarras;
            produto.ImagemBase64 = produtoAtualizado.ImagemBase64;

            // Atualiza imagem (opcional)
            if (!string.IsNullOrWhiteSpace(produto.ImagemBase64))
            {
                var pastaImagem = Path.Combine(env.WebRootPath, "imagens");
                Directory.CreateDirectory(pastaImagem);

                var nomeArquivo = $"{Guid.NewGuid()}.png";
                var caminhoImagem = Path.Combine(pastaImagem, nomeArquivo);

                var base64Limpo = produto.ImagemBase64.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
                var imagemBytes = Convert.FromBase64String(base64Limpo);
                await System.IO.File.WriteAllBytesAsync(caminhoImagem, imagemBytes);

                // Gera nova URL da imagem
                var scheme = Request?.Scheme ?? "http";
                var host = Request?.Host.ToString() ?? "localhost:5090";
                var urlImagem = $"{scheme}://{host}/imagens/{nomeArquivo}";

                // Atualiza na Fake Store
                if (produto.IdFakeStore.HasValue)
                {
                    var client = httpClientFactory.CreateClient();
                    var fakeProduct = new FakeStoreProduto
                    {
                        title = produto.Nome!,
                        price = produto.Preco,
                        image = urlImagem,
                        category = "geral"
                    };

                    var url = $"https://fakestoreapi.com/products/{produto.IdFakeStore}";

                    // 🧾 Loga o que será enviado
                    Console.WriteLine("🔄 Atualizando produto na Fake Store:");
                    Console.WriteLine("URL: " + url);
                    Console.WriteLine("Payload:");
                    Console.WriteLine(JsonSerializer.Serialize(fakeProduct));

                    var resposta = await client.PutAsJsonAsync(url, fakeProduct);

                    // 📥 Loga a resposta da Fake Store
                    Console.WriteLine("Resposta Fake Store → " + resposta.StatusCode);
                    var respostaBody = await resposta.Content.ReadAsStringAsync();
                    Console.WriteLine(respostaBody);
                }

            }

            await _context.SaveChangesAsync();
            return Ok(produto);
        }


        // DELETE: api/produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(
        int id,
        [FromServices] IHttpClientFactory httpClientFactory)
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                    return NotFound();

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();

                // Apaga também na Fake Store
                if (produto.IdFakeStore.HasValue)
                {
                    var client = httpClientFactory.CreateClient();
                    var url = $"https://fakestoreapi.com/products/{produto.IdFakeStore}";
                    var resposta = await client.DeleteAsync(url);
                    Console.WriteLine("DELETE Fake Store → " + resposta.StatusCode);
                }

                return NoContent();
            }

    }
}
