using System.ComponentModel.DataAnnotations;

namespace AgendaPro.Models
{
    public class TipoParticipante
    {
        [Key]
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
    }
}
