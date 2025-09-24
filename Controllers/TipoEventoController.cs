using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaPro.Models;

namespace AgendaPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoEventoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public TipoEventoController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/tipoevento/lista
        [HttpGet("lista")]
        public IActionResult GetTodos()
        {
            try
            {
                var tipos = _db.TiposEvento.Where(t => t.Ativo).ToList();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar tipos de evento: {ex.Message}");
            }
        }

        // GET: api/tipoevento/lista/{id}
        [HttpGet("lista/{id}")]
        public IActionResult GetPorId(int id)
        {
            try
            {
                var tipo = _db.TiposEvento.Find(id);
                if (tipo == null || !tipo.Ativo) return NotFound("Tipo de evento não encontrado.");
                return Ok(tipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar tipo de evento: {ex.Message}");
            }
        }

        // POST: api/tipoevento/novo
        [HttpPost("novo")]
        public IActionResult Criar([FromBody] TiposEvento tipoEvento)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                _db.TiposEvento.Add(tipoEvento);
                _db.SaveChanges();

                return CreatedAtAction(nameof(GetPorId), new { id = tipoEvento.Id }, tipoEvento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar tipo de evento: {ex.Message}");
            }
        }

        // PUT: api/tipoevento/atualiza/{id}
        [HttpPut("atualiza/{id}")]
        public IActionResult Atualizar(int id, [FromBody] TiposEvento TipoEvento)
        {
            try
            {
                if (id != TipoEvento.Id) return BadRequest("ID não confere.");              

                _db.Entry(TipoEvento).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar tipo de evento: {ex.Message}");
            }
        }

        // DELETE lógico: api/tipoevento/del/{id}
        [HttpDelete("del/{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                var tipo = _db.TiposEvento.Find(id);
                tipo.Ativo = false;
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir tipo de evento: {ex.Message}");
            }
        }
    }
}
