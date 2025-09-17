using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AgendaPro.Models;

namespace AgendaPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipanteController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ParticipanteController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/participante/lista
        [HttpGet("lista")]
        public IActionResult GetTodos()
        {
            try
            {
                var Participante = _db.Participante.Where(p => p.Ativo).ToList();
                return Ok(Participante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar Participante: {ex.Message}");
            }
        }

        // GET: api/participante/lista/{id}
        [HttpGet("lista/{id}")]
        public IActionResult GetPorId(int id)
        {
            try
            {
                var participante = _db.Participante.Find(id);
                if (participante == null || !participante.Ativo)
                    return NotFound("Participante não encontrado ou inativo.");

                return Ok(participante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar participante: {ex.Message}");
            }
        }

        // POST: api/participante/novo
        [HttpPost("novo")]
        public IActionResult Criar([FromBody] Participante participante)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                _db.Participante.Add(participante);
                _db.SaveChanges();

                return CreatedAtAction(nameof(GetPorId), new { id = participante.Id }, participante);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict("Participante já cadastrado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar participante: {ex.Message}");
            }
        }


        // PUT: api/participante/atualiza/{id}
        [HttpPut("atualiza/{id}")]
        public IActionResult Atualizar(int id, [FromBody] Participante participante)
        {
            try
            {
                if (id != participante.Id) return BadRequest("ID não confere.");
                if (!_db.Participante.Any(p => p.Id == id && p.Ativo)) return NotFound();

                _db.Entry(participante).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar participante: {ex.Message}");
            }
        }

        // DELETE: api/participante/del/{id}
        [HttpDelete("del/{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                var participante = _db.Participante.Find(id);
                if (participante == null || !participante.Ativo) return NotFound();

                participante.Ativo = false; // exclusão lógica
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir participante: {ex.Message}");
            }
        }
    }
}
