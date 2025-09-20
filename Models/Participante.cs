using System.ComponentModel.DataAnnotations;

namespace AgendaPro.Models
{
    public class Participante
    {
        public int Id { get; set; }

        [ StringLength(100)]
        public string Nome { get; set; } = null!;

        
        public string Documento { get; set; } = null!; // CPF

        [StringLength(30)]
        public string? Telefone { get; set; } = null!;

        [StringLength(200)]
        public string? Email { get; set; } = null;

        public int TipoParticipanteId { get; set; } = 1; // 1=Normal, 0=VIP, etc

        public bool Ativo { get; set; } = true;
    }
}
