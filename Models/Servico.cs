using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPro.Models
{
    public class Servico
    {
        public int Id { get; set; }

        [Required]
        public int FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        public Fornecedor Fornecedor { get; set; } = null!;

        [Required, StringLength(200)]
        public string Nome { get; set; } = null!;

        [Required]
        public decimal Preco { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
