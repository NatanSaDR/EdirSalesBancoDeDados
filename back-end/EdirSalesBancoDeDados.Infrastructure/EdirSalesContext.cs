using EdirSalesBancoDeDados.Domain;
using Microsoft.EntityFrameworkCore;

namespace EdirSalesBancoDeDados.Infrastructure
{
    public class EdirSalesContext : DbContext
    {
        public EdirSalesContext(DbContextOptions options) : base(options) { }

        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Municipe> Municipes { get; set; }
        public DbSet<Solicitacao> Solicitacoes { get; set; }
        public DbSet<Oficio> Oficios { get; set; }
        public DbSet<Telefone> Telefones { get; set; }
        public DbSet<Agente> Agentes { get; set; }
        public DbSet<ContatoAgente> ContatosAgente { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Relação 1 para muitos
            modelBuilder.Entity<Solicitacao>()
                .HasMany(s => s.Oficios)
                .WithOne(o => o.Solicitacao)
                .HasForeignKey(o => o.SolicitacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Municipe>()
                .HasMany(t => t.Telefones)
                .WithOne(m => m.Municipe)
                .HasForeignKey(m => m.MunicipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Agente>()
                .HasMany(c=>c.Contatos)
                .WithOne(a=>a.Agente)
                .HasForeignKey(a=>a.AgenteId)
                .OnDelete(DeleteBehavior.Cascade);


            // muitos para muitos, relacionamento entre grupo e municipe informo para o EF nao criar as tabelas pois ja existe
            modelBuilder.Entity<Grupo>()
                .HasMany(g => g.Municipes)
                .WithMany(m => m.Grupos)
                .UsingEntity<Dictionary<string, object>>(
                "MunicipeGrupo", // Nome da tabela deve corresponder ao que foi configurado no FluentMigrator
                j => j.HasOne<Municipe>().WithMany().HasForeignKey("MunicipeId"),
                j => j.HasOne<Grupo>().WithMany().HasForeignKey("GrupoId")
                );

            modelBuilder.Entity<Municipe>()
                .HasMany(m => m.Solicitacoes)
                .WithMany(s => s.Municipes)
                .UsingEntity<Dictionary<string, object>>(
                "MunicipeSolicitacao",
                j => j.HasOne<Solicitacao>().WithMany().HasForeignKey("SolicitacaoId"),
                j => j.HasOne<Municipe>().WithMany().HasForeignKey("MunicipeId")
                );

            modelBuilder.Entity<Solicitacao>()
                .HasMany(s => s.Grupos)
                .WithMany(g => g.Solicitacoes)
                .UsingEntity<Dictionary<string, object>>(
                "SolicitacaoGrupo",
                j => j.HasOne<Grupo>().WithMany().HasForeignKey("GrupoId"),
                j => j.HasOne<Solicitacao>().WithMany().HasForeignKey("SolicitacaoId")
                );

            modelBuilder.Entity<Agente>()
                .HasMany(s => s.Solicitacoes)
                .WithMany(a => a.Agentes)
                .UsingEntity<Dictionary<string, object>>(
                "AgenteSolicitacao",
                j => j.HasOne<Solicitacao>().WithMany().HasForeignKey("SolicitacaoId"),
                j => j.HasOne<Agente>().WithMany().HasForeignKey("AgenteId")
                );
        }
    }
}
