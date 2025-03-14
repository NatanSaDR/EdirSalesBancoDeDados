using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(7)]
    public class Version00007 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Oficios")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Chave primária com auto-incremento
                .WithColumn("NumeroOficio").AsString(255).NotNullable()
                .WithColumn("SolicitacaoId").AsInt32().NotNullable(); // FK para Solicitação

            // Definindo a FK entre Ofício e Solicitação (muitos para um)
            Create.ForeignKey("FK_Oficio_Solicitacao")
                .FromTable("Oficios").ForeignColumn("SolicitacaoId")
                .ToTable("Solicitacoes").PrimaryColumn("Id")
                .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
