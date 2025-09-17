namespace AgendaPro.Models
{
    public class ParticipanteEvento
    {
        public int EventoId { get; set; }
        public int ParticipanteId { get; set; }

        public Evento Evento { get; set; } = null!;
        public Participante Participante { get; set; } = null!;
    }
}
