using System.ComponentModel.DataAnnotations;

namespace AgendaPro.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string RazaoSocial { get; set; } = null!;

        [Required, StringLength(14)]
        public string CNPJ { get; set; } = null!;

        [StringLength(30)]
        public string? Telefone { get; set; } = null!;

        [StringLength(200)]
        public string? Email { get; set; } = null!;

        public bool Ativo { get; set; } = true;

        public List<Servico> Servicos { get; set; } = new();
    }
}
