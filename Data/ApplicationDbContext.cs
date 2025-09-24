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
    public DbSet<ParticipanteEvento> ParticipanteEvento { get; set; }

    public DbSet<ServicoEvento> ServicoEvento { get; set; }
    public DbSet<TiposEvento> TiposEvento { get; set; }
    public DbSet<TipoParticipante> TipoParticipante { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // chave composta ParticipanteEvento
        modelBuilder.Entity<ParticipanteEvento>()
            .HasKey(pe => new { pe.EventoId, pe.ParticipanteId });

        // chave composta ServicoEvento
        modelBuilder.Entity<ServicoEvento>()
            .HasKey(se => new { se.EventoId, se.ServicoId });
    }




}

