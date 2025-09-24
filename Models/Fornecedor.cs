using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPro.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string RazaoSocial { get; set; } = null!;

        
        public string CNPJ { get; set; } = null!;

        [StringLength(30)]
        public string? Telefone { get; set; } = null!;

        [StringLength(200)]
        public string? Email { get; set; } = null!;

        public bool Ativo { get; set; } = true;
        [NotMapped]
        public List<Servico> Servicos { get; set; } = new();
    }
}
