using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPro.Models
{
    public class Evento
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = "Novo evento";

        public DateTime DataInicio { get; set; } = DateTime.Today;
        public DateTime DataFim { get; set; } = DateTime.Today.AddDays(1);

        public string CEP { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;

        public int CapacidadeMaxima { get; set; } = 0;
        public decimal OrcamentoMaximo { get; set; } = 0;

        [Required]
        public int TipoEventoId { get; set; } = 0;

        public bool Ativo { get; set; } = true;

        [NotMapped]
        public List<int> ParticipantesIds { get; set; } = new();

        [NotMapped]
        public List<int> ServicosIds { get; set; } = new();
    }
}
