using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AgendaPro.Models;

namespace AgendaPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FornecedorController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public FornecedorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: api/fornecedor/novo
        [HttpPost("novo")]
        public IActionResult Criar([FromBody] Fornecedor fornecedor)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var servicos = fornecedor.Servicos?.ToList() ?? new List<Servico>(); 
                fornecedor.Servicos = null; 
                _db.Fornecedores.Add(fornecedor);
                _db.SaveChanges();


                foreach (var s in servicos)
                {
                    var servico = new Servico
                    {
                        FornecedorId = fornecedor.Id,
                        Nome = s.Nome,
                        Preco = s.Preco,
                    };
                    _db.Servicos.Add(servico);
                }

                _db.SaveChanges();

                return CreatedAtAction(nameof(GetPorId), new { id = fornecedor.Id }, fornecedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar fornecedor: {ex.Message}");
            }
        }


        // GET: api/fornecedor/lista
        [HttpGet("lista")]
        public IActionResult GetTodos()
        {
            try
            {
                var fornecedores = _db.Fornecedores
                    .Where(f => f.Ativo)
                    .Select(f => new
                    {
                        f.Id,
                        f.RazaoSocial,
                        f.CNPJ,
                        f.Telefone,
                        f.Email,
                        Servicos = _db.Servicos.Where(s => s.FornecedorId == f.Id && s.Ativo)
                        .Select(s => new
                        {
                            s.Id,
                            s.Nome,
                            s.Preco
                        })
                        .ToList()

                    })
                    .ToList();

                return Ok(fornecedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar fornecedores: {ex.Message}");
            }
        }

        // GET: api/fornecedor/lista/{id}
        [HttpGet("lista/{id}")]
        public IActionResult GetPorId(int id)
        {
            try
            {
                var fornecedor = _db.Fornecedores.Find(id);
                if (fornecedor == null || !fornecedor.Ativo)
                    return NotFound("Fornecedor não encontrado ou inativo.");

                var servicos = _db.Servicos.Where(s => s.FornecedorId == id && s.Ativo).ToList();
                return Ok(new { fornecedor, servicos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar fornecedor: {ex.Message}");
            }
        }


        // PUT: api/fornecedor/atualiza/{id}
        [HttpPut("atualiza/{id}")]
        public IActionResult Atualizar(int id, [FromBody] Fornecedor fornecedorAtualizado)
        {
            try
            {
                var fornecedor = _db.Fornecedores.Find(id);
                if (fornecedor == null || !fornecedor.Ativo) return NotFound();

                fornecedor.RazaoSocial = fornecedorAtualizado.RazaoSocial;
                fornecedor.CNPJ = fornecedorAtualizado.CNPJ;
                fornecedor.Telefone = fornecedorAtualizado.Telefone;
                fornecedor.Email = fornecedorAtualizado.Email;

                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar fornecedor: {ex.Message}");
            }
        }

        // DELETE: api/fornecedor/del/{id}
        [HttpDelete("del/{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                var fornecedor = _db.Fornecedores.Find(id);
                if (fornecedor == null || !fornecedor.Ativo) return NotFound();

                fornecedor.Ativo = false;

                var servicos = _db.Servicos.Where(s => s.FornecedorId == id).ToList();
                foreach (var s in servicos)
                    s.Ativo = false;

                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir fornecedor: {ex.Message}");
            }
        }
    }
}
