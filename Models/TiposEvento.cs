using System.ComponentModel.DataAnnotations;

namespace AgendaPro.Models
{
    public class TiposEvento
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Descricao { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
    }
}
