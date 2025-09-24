using AgendaPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
                var Participante = _db.Participante.ToList();
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
         

                participante.Ativo = false; // exclusão lógica
                _db.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir participante: {ex.Message}");
            }
        }

        // Post: api/participante/pesquisar
        [HttpPost("pesquisar")]
        public IActionResult Pesquisar([FromBody] Participante filtro)
        {
            try
            {
                var query = _db.Participante.AsQueryable();

                // Nome
                if (!string.IsNullOrEmpty(filtro.Nome))
                    query = query.Where(p => p.Nome.Contains(filtro.Nome));

                // Documento
                if (!string.IsNullOrEmpty(filtro.Documento))
                    query = query.Where(p => p.Documento.Contains( filtro.Documento));

                if (!string.IsNullOrEmpty(filtro.Telefone))
                    query = query.Where(p => p.Telefone.Contains(filtro.Telefone));

                if (!string.IsNullOrEmpty(filtro.Email))
                    query = query.Where(p => p.Email.Contains(filtro.Email));

                if (filtro.TipoParticipanteId >0)
                    query = query.Where(p => p.TipoParticipanteId==filtro.TipoParticipanteId);

                if (filtro.Ativo == true || filtro.Ativo == false)
                    query = query.Where(p => p.Ativo == filtro.Ativo);




                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao pesquisar participante: {ex.Message}");
            }
        }
        // Post: api/participante/tipoparticipante
        [HttpPost("tipoparticipante")]
        public IActionResult BuscarTipos([FromBody] List<int> ids)
        {
            try
            {


                var tipos = _db.TipoParticipante
                               .Where(t => ids.Contains(t.Id) && t.Ativo)
                               .Select(t => t.Descricao)
                               .ToList();

                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar tipos de participante: {ex.Message}");
            }
        }


    }
}
