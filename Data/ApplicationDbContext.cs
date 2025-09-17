using AgendaPro.Models;
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Participante> Participante { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Evento> ParticipanteEvento { get; set; }



}

