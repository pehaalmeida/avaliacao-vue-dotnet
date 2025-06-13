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
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        // PUT: api/produtos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Produtos.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
