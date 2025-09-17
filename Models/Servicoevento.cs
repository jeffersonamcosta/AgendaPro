namespace AgendaPro.Models
{
    public class ServicoEvento
    {
        public int EventoId { get; set; }
        public int ServicoId { get; set; }

        public Evento Evento { get; set; } = null!;
        public Servico Servico { get; set; } = null!;
    }
}
