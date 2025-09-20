using AgendaPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

    // Post: api/eventos/pesquisar
    [HttpPost("pesquisar")]
    public IActionResult Pesquisar([FromBody] EventoFiltro filtro)
    {
        try
        {
            var EventosQuery = _db.Eventos.AsQueryable();
            var se = _db.Servicos.AsQueryable();
            var pe = _db.ParticipanteEvento.AsQueryable();


            if (!string.IsNullOrEmpty(filtro.Nome))
                EventosQuery = EventosQuery.Where(e => e.Nome.Contains(filtro.Nome));

        
            if (!string.IsNullOrEmpty(filtro.CEP))
                EventosQuery = EventosQuery.Where(e => e.CEP.Contains(filtro.CEP));

            if (!string.IsNullOrEmpty(filtro.Endereco))
                EventosQuery = EventosQuery.Where(e => e.Endereco.Contains(filtro.Endereco));

            if (filtro.OrcamentoMaximo>0)
                EventosQuery = EventosQuery.Where(e => e.OrcamentoMaximo < (filtro.OrcamentoMaximo));

            if (filtro.CapacidadeMaxima > 0)
                EventosQuery = EventosQuery.Where(e => e.CapacidadeMaxima < (filtro.CapacidadeMaxima));

            if (filtro.TipoEventoId > 0)
                EventosQuery = EventosQuery.Where(e => e.TipoEventoId == (filtro.TipoEventoId));

            if (filtro.Ativo == true || filtro.Ativo == false)
                EventosQuery = EventosQuery.Where(e => e.Ativo == filtro.Ativo);


            if (!string.IsNullOrEmpty(filtro.DataInicio.ToString()))
            {
                EventosQuery = EventosQuery.Where(e => e.DataInicio > (filtro.DataInicio));
            }

            if (!string.IsNullOrEmpty(filtro.DataFim.ToString()))
            {
                EventosQuery = EventosQuery.Where(e => e.DataFim < (filtro.DataFim));
            }

            var eventos = EventosQuery
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
        ParticipantesIds = _db.Set<ParticipanteEvento>()
                           .Where(pe => pe.EventoId == e.Id)
                           .Select(pe => pe.ParticipanteId)
                           .ToList(),
        EventoIds = _db.Set<ServicoEvento>()
                          .Where(fe => fe.EventoId == e.Id)
                          .Select(fe => fe.ServicoId)
                          .ToList()
    })
    .ToList();


            return Ok(eventos.ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao pesquisar: {ex.Message}");
        }
    }

}
