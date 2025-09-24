using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AgendaPro.Models
{
    public class Servico
    {
        public int Id { get; set; }

 
        public int FornecedorId { get; set; }

        [ StringLength(200)]
        public string Nome { get; set; } = null!;

        public decimal Preco { get; set; } = 0;

        public bool Ativo { get; set; } = true;
    }
}
