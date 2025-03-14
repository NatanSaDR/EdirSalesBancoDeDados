using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(5)]
    public class Version00005 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("MunicipeSolicitacao")
                .WithColumn("MunicipeId").AsInt32().NotNullable()
                .WithColumn("SolicitacaoId").AsInt32().NotNullable();

            Create.ForeignKey("FK_MunicipeSolicitacao_Municipe")
                .FromTable("MunicipeSolicitacao").ForeignColumn("MunicipeId")
                .ToTable("Municipes").PrimaryColumn("Id");

            Create.ForeignKey("FK_MunicipeSolicitacao_Solicitacao")
                .FromTable("MunicipeSolicitacao").ForeignColumn("SolicitacaoId")
                .ToTable("Solicitacoes").PrimaryColumn("Id");

            Create.PrimaryKey("PK_MunicipeSolicitacao")
                .OnTable("MunicipeSolicitacao")
                .Columns("MunicipeId", "SolicitacaoId");
        }
    }
}
