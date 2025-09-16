public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}
