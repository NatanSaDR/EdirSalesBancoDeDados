using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(6)]
    public class Version00006 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("SolicitacaoGrupo")
                .WithColumn("SolicitacaoId").AsInt32().NotNullable()
                .WithColumn("GrupoId").AsInt32().NotNullable();

            Create.ForeignKey("FK_SolicitacaoGrupo_Grupo")
                .FromTable("SolicitacaoGrupo").ForeignColumn("GrupoId")
                .ToTable("Grupos").PrimaryColumn("Id");

            Create.ForeignKey("FK_SolicitacaoGrupo_Solicitacao")
                .FromTable("SolicitacaoGrupo").ForeignColumn("SolicitacaoId")
                .ToTable("Solicitacoes").PrimaryColumn("Id");


            Create.PrimaryKey("PK_SolicitacaoGrupo")
                .OnTable("SolicitacaoGrupo")
                .Columns("SolicitacaoId", "GrupoId");
        }
    }
}
