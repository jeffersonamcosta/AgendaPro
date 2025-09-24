using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AgendaPro.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ServicoController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/servico/lista
        [HttpGet("lista")]
        public IActionResult GetTodos()
        {
            try
            {
                var servicos = _db.Servicos.ToList();
                return Ok(servicos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar serviços: {ex.Message}");
            }
        }

        // GET: api/servico/lista
        [HttpGet("listaValidos")]
        public IActionResult GetTodosValidos()
        {
            try
            {
                var servicos = (from s in _db.Servicos
                                join f in _db.Fornecedores on s.FornecedorId equals f.Id
                                where s.Ativo && f.Ativo
                                select new
                                {
                                    id = s.Id,
                                    fornecedornome = f.RazaoSocial,
                                    nome = s.Nome,
                                    preco = s.Preco
                                }).ToList();

                return Ok(servicos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar serviços: {ex.Message}");
            }
        }

        // GET: api/servico/lista/{id}
        [HttpGet("lista/{id}")]
        public IActionResult GetPorId(int id)
        {
            try
            {
                var servico = _db.Servicos.Find(id);              

                return Ok(servico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar serviço: {ex.Message}");
            }
        }

        // POST: api/servico/novo
        [HttpPost("novo")]
        public IActionResult Criar([FromBody] Servico servico)
        {
            try
            {
                

                _db.Servicos.Add(servico);
                _db.SaveChanges();

                return CreatedAtAction(nameof(GetPorId), new { id = servico.Id }, servico);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar serviço: {ex.Message}");
            }
        }

        // PUT: api/servico/atualiza/{id}
        [HttpPut("atualiza/{id}")]
        public IActionResult Atualizar(int id, [FromBody] Servico servico)
        {
            try
            {
                if (id != servico.Id) return BadRequest("ID não confere.");               

                _db.Entry(servico).State = EntityState.Modified;
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar serviço: {ex.Message}");
            }
        }

        // DELETE: api/servico/del/{id}
        [HttpDelete("del/{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                var servico = _db.Servicos.Find(id);
                if (servico == null || !servico.Ativo) return NotFound();

                servico.Ativo = false; 
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir serviço: {ex.Message}");
            }
        }

        // POST: api/servico/pesquisar
        [HttpPost("pesquisar")]
        public IActionResult Pesquisar([FromBody] Servico filtro)
        {
            try
            {
                var query = _db.Servicos.AsQueryable();

                if (!string.IsNullOrEmpty(filtro.Nome))
                    query = query.Where(s => s.Nome.Contains(filtro.Nome));

                if (filtro.FornecedorId > 0)
                    query = query.Where(s => s.FornecedorId == filtro.FornecedorId);

                if (filtro.Preco > 0)
                    query = query.Where(s => s.Preco <= filtro.Preco);

                if (filtro.Ativo || !filtro.Ativo)
                    query = query.Where(s => s.Ativo == filtro.Ativo);

                var resultado = query.ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao pesquisar serviços: {ex.Message}");
            }
        }
    }
}

