using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaPro.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventosController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EventosController(ApplicationDbContext db)
    {
        _db = db;
    }

    // POST: api/evento/novo
    [HttpPost("novo")]
    public IActionResult Criar([FromBody] Evento evento)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // IDs de participantes vindos no JSON
            var participantesIds = evento.ParticipantesIds ?? new List<int>();

            // IDs de serviços vindos no JSON
            var servicosIds = evento.ServicosIds ?? new List<int>();

            // Verifica participantes
            if (participantesIds.Any())
            {
                var participantesExistentes = _db.Participante
                                                 .Where(p => participantesIds.Contains(p.Id) && p.Ativo)
                                                 .Select(p => p.Id)
                                                 .ToList();

                if (participantesExistentes.Count != participantesIds.Count)
                    return BadRequest("Um ou mais participantes não são validos.");
            }

            // Verifica serviços
            if (servicosIds.Any())
            {
                var servicosExistentes = _db.Servicos
                                            .Where(s => servicosIds.Contains(s.Id) && s.Ativo)
                                            .Select(s => s.Id)
                                            .ToList();

                if (servicosExistentes.Count != servicosIds.Count)
                    return BadRequest("Um ou mais serviços não são validos.");
            }

            // Salva evento
            _db.Eventos.Add(evento);
            _db.SaveChanges();

            // Relaciona participantes
            foreach (var pid in participantesIds)
            {
                var pe = new ParticipanteEvento
                {
                    EventoId = evento.Id,
                    ParticipanteId = pid
                };
                _db.Set<ParticipanteEvento>().Add(pe);
            }

            // Relaciona serviços
            foreach (var sid in servicosIds)
            {
                var se = new ServicoEvento
                {
                    EventoId = evento.Id,
                    ServicoId = sid
                };
                _db.Set<ServicoEvento>().Add(se);
            }

            _db.SaveChanges();

            return CreatedAtAction(nameof(GetEventos), new { id = evento.Id }, evento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao criar Evento: {ex.Message}");
        }
    }

    // GET: api/evento/lista/{id?}
    [HttpGet("lista/{id?}")]
    public IActionResult GetEventos(int? id)
    {
        try
        {
            var query = _db.Eventos.AsQueryable();

            if (id.HasValue)
                query = query.Where(e => e.Id == id.Value);

            var eventos = query
                .Select(e => new
                {
                    e.Id,
                    e.Nome,
                    e.DataInicio,
                    e.DataFim,
                    e.CEP,
                    e.Endereco,
                    e.Observacoes,
                    e.CapacidadeMaxima,
                    e.OrcamentoMaximo,
                    e.TipoEventoId,
                    e.Ativo,
                    Participantes = _db.Set<ParticipanteEvento>()
                                       .Where(pe => pe.EventoId == e.Id)
                                       .Select(pe => pe.ParticipanteId)
                                       .ToList(),
                    Evento = _db.Set<ServicoEvento>()
                                      .Where(fe => fe.EventoId == e.Id)
                                      .Select(fe => fe.ServicoId)
                                      .ToList()
                })
                .ToList();

            if (id.HasValue && eventos.Count == 0)
                return NotFound("Evento não encontrado.");

            return Ok(eventos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar evento(s): {ex.Message}");
        }
    }
}
