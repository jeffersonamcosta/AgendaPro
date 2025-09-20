using System;
using System.Collections.Generic;

namespace AgendaPro.Models
{
    public class EventoFiltro
    {
        public int? Id { get; set; } = null;
        public string? Nome { get; set; } = null;
        public DateTime? DataInicio { get; set; } = null;
        public DateTime? DataFim { get; set; } = null;
        public string? CEP { get; set; } = null;
        public string? Endereco { get; set; } = null;
        public string? Observacoes { get; set; } = null;
        public int? CapacidadeMaxima { get; set; } = null;
        public decimal? OrcamentoMaximo { get; set; } = null;
        public int? TipoEventoId { get; set; } = null;
        public bool? Ativo { get; set; } = null;

        public List<int>? ParticipantesIds { get; set; } = null;
        public List<int>? ServicosIds { get; set; } = null;
    }
}
