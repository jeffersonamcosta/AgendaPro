using AgendaPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    Servico = _db.Set<ServicoEvento>()
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

            
            if (filtro.ParticipantesIds != null && filtro.ParticipantesIds.Any())
            {
                EventosQuery = from e in EventosQuery
                               where filtro.ParticipantesIds.All(pid => pe.Any(pe => pe.EventoId == e.Id && pe.ParticipanteId == pid))
                               select e;
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
         ServicosIds = _db.Set<ServicoEvento>()
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

    // PUT: api/evento/atualiza/{id}
    [HttpPut("atualiza/{id}")]
    public IActionResult Atualizar(int id, [FromBody] Evento evento)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Busca o evento existente
            var eventoExistente = _db.Eventos.FirstOrDefault(e => e.Id == id);

            if (eventoExistente == null)
                return NotFound($"Evento com Id {id} não encontrado.");

            // Atualiza campos básicos
            eventoExistente.Nome = evento.Nome;
            eventoExistente.Observacoes = evento.Observacoes;
            eventoExistente.CEP = evento.CEP;
            eventoExistente.Endereco = evento.Endereco;
            eventoExistente.DataInicio = evento.DataInicio;
            eventoExistente.DataFim = evento.DataFim;
            eventoExistente.CapacidadeMaxima = evento.CapacidadeMaxima;
            eventoExistente.OrcamentoMaximo = evento.OrcamentoMaximo;
            eventoExistente.TipoEventoId = evento.TipoEventoId;
            eventoExistente.Ativo = evento.Ativo;

            // Materializa os relacionamentos antes de remover
            var participantesAtuais = _db.Set<ParticipanteEvento>()
                                         .Where(pe => pe.EventoId == id)
                                         .ToList();
            _db.Set<ParticipanteEvento>().RemoveRange(participantesAtuais);

            var servicosAtuais = _db.Set<ServicoEvento>()
                                     .Where(se => se.EventoId == id)
                                     .ToList();
            _db.Set<ServicoEvento>().RemoveRange(servicosAtuais);

            // Adiciona novos participantes se houver
            var participantesIds = evento.ParticipantesIds ?? new List<int>();
            if (participantesIds.Any())
            {
                // Valida participantes ativos
                var validos = _db.Participante
                                 .Where(p => participantesIds.Contains(p.Id) && p.Ativo)
                                 .Select(p => p.Id)
                                 .ToList();
                foreach (var pid in validos)
                {
                    _db.Set<ParticipanteEvento>().Add(new ParticipanteEvento
                    {
                        EventoId = id,
                        ParticipanteId = pid
                    });
                }
            }

            // Adiciona novos serviços se houver
            var servicosIds = evento.ServicosIds ?? new List<int>();
            if (servicosIds.Any())
            {
                // Valida serviços ativos
                var validos = _db.Servicos
                                 .Where(s => servicosIds.Contains(s.Id) && s.Ativo)
                                 .Select(s => s.Id)
                                 .ToList();
                foreach (var sid in validos)
                {
                    _db.Set<ServicoEvento>().Add(new ServicoEvento
                    {
                        EventoId = id,
                        ServicoId = sid
                    });
                }
            }

            _db.SaveChanges();

            return NoContent();

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar Evento: {ex.Message}");
        }
    }



}
