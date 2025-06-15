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
        private readonly ILogger<ProdutosController> _logger;

        public ProdutosController(AppDbContext context, ILogger<ProdutosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Retorna uma lista paginada de produtos, com filtros opcionais por nome, código de barras e ordenacao por preco 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos(
            [FromQuery] string? nome,
            [FromQuery] string? codigoBarras,
            [FromQuery] string? ordenarPor,
            [FromQuery] string? ordem,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("[AVISO] Iniciando listagem de produtos");

            var query = _context.Produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(p =>
                    !string.IsNullOrWhiteSpace(p.Nome) &&
                    p.Nome.ToLower().Contains(nome.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(codigoBarras))
            {
                query = query.Where(p => p.CodigoBarras == codigoBarras);
            }

            if (!string.IsNullOrWhiteSpace(ordenarPor) && ordenarPor.ToLower() == "preco")
            {
                query = ordem?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Preco)
                    : query.OrderBy(p => p.Preco);
            }

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        // GET: Retorna um produto especifico pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            _logger.LogInformation("[AVISO] Buscando produto com ID: {Id}", id);

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                _logger.LogWarning("[AVISO] Produto ID {Id} não encontrado para exibição", id);
                return NotFound();
            }

            return produto;
        }

        // POST: Cria um novo produto, valida os campos obrigatórios e salva a imagem no disco
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(
            Produto produto,
            [FromServices] IWebHostEnvironment env,
            [FromServices] IHttpClientFactory httpClientFactory)
        {
            _logger.LogInformation("[AVISO] Iniciando criação de produto: {@Produto}", produto);

            if (string.IsNullOrWhiteSpace(produto.Nome) ||
                string.IsNullOrWhiteSpace(produto.CodigoBarras) ||
                produto.Preco <= 0 ||
                string.IsNullOrWhiteSpace(produto.ImagemBase64))
            {
                _logger.LogWarning("[AVISO] Dados inválidos recebidos ao tentar criar produto");
                return BadRequest("Todos os campos são obrigatórios, incluindo a imagem em base64.");
            }

            var pastaImagem = Path.Combine(env.WebRootPath, "imagens");
            Directory.CreateDirectory(pastaImagem);

            var nomeArquivo = $"{Guid.NewGuid()}.png";
            var caminhoImagem = Path.Combine(pastaImagem, nomeArquivo);

            try
            {
                var imagemBase64Limpa = produto.ImagemBase64
                    .Trim()
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace(" ", "");

                var imagemBytes = Convert.FromBase64String(imagemBase64Limpa);
                await System.IO.File.WriteAllBytesAsync(caminhoImagem, imagemBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao converter a imagem base64");
                return BadRequest("Imagem enviada não está em formato base64 válido.");
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            var scheme = Request?.Scheme ?? "http";
            var host = Request?.Host.ToString() ?? "localhost:5091";
            var urlImagem = $"{scheme}://{host}/imagens/{nomeArquivo}";

            var fakeProduct = new FakeStoreProduto
            {
                title = produto.Nome!,
                price = produto.Preco,
                image = urlImagem,
                category = "geral"
            };

            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://fakestoreapi.com/products", fakeProduct);

            if (response.IsSuccessStatusCode)
            {
                var respostaFake = await response.Content.ReadAsStringAsync();
                var fakeResponse = JsonSerializer.Deserialize<FakeStoreProduto>(respostaFake);
                produto.IdFakeStore = fakeResponse?.id;
                await _context.SaveChangesAsync();
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("[AVISO] Erro ao replicar na Fake Store: {Status} - {Detalhes}", response.StatusCode, erro);
            }

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        // PUT: Atualiza os dados de um produto existente e reflete a alteração também na Fake Store
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(
            int id,
            Produto produtoAtualizado,
            [FromServices] IHttpClientFactory httpClientFactory,
            [FromServices] IWebHostEnvironment env)
        {
            _logger.LogInformation("[AVISO] Iniciando atualização de produto ID: {Id}", id);

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                _logger.LogWarning("[AVISO] Produto ID {Id} não encontrado para atualização", id);
                return NotFound();
            }

            produto.Nome = produtoAtualizado.Nome;
            produto.Preco = produtoAtualizado.Preco;
            produto.CodigoBarras = produtoAtualizado.CodigoBarras;
            produto.ImagemBase64 = produtoAtualizado.ImagemBase64;

            if (!string.IsNullOrWhiteSpace(produto.ImagemBase64))
            {
                var pastaImagem = Path.Combine(env.WebRootPath, "imagens");
                Directory.CreateDirectory(pastaImagem);

                var nomeArquivo = $"{Guid.NewGuid()}.png";
                var caminhoImagem = Path.Combine(pastaImagem, nomeArquivo);

                var base64Limpo = produto.ImagemBase64.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");
                var imagemBytes = Convert.FromBase64String(base64Limpo);
                await System.IO.File.WriteAllBytesAsync(caminhoImagem, imagemBytes);

                var scheme = Request?.Scheme ?? "http";
                var host = Request?.Host.ToString() ?? "localhost:5091";
                var urlImagem = $"{scheme}://{host}/imagens/{nomeArquivo}";

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
                    var resposta = await client.PutAsJsonAsync(url, fakeProduct);
                    var respostaBody = await resposta.Content.ReadAsStringAsync();
                    _logger.LogInformation("[AVISO] Resposta Fake Store (PUT): {Status} - {Body}", resposta.StatusCode, respostaBody);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(produto);
        }

        // DELETE: Remove um produto do banco e da Fake Store
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(
            int id,
            [FromServices] IHttpClientFactory httpClientFactory)
        {
            _logger.LogInformation("[AVISO] Iniciando exclusão de produto ID: {Id}", id);

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                _logger.LogWarning("[AVISO] Produto ID {Id} não encontrado para exclusão", id);
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            if (produto.IdFakeStore.HasValue)
            {
                var client = httpClientFactory.CreateClient();
                var url = $"https://fakestoreapi.com/products/{produto.IdFakeStore}";
                var resposta = await client.DeleteAsync(url);
                _logger.LogInformation("[AVISO] Produto deletado da Fake Store - Status: {Status}", resposta.StatusCode);
            }

            _logger.LogInformation("[AVISO] Produto ID {Id} excluído com sucesso", id);
            return NoContent();
        }
    }
}
