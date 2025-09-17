using System.ComponentModel.DataAnnotations;

namespace AgendaPro.Models
{
    public class FornecedorEvento
    {
        [Required]
        public int EventoId { get; set; }
        [Required]
        public int FornecedorId { get; set; }
    }
}
