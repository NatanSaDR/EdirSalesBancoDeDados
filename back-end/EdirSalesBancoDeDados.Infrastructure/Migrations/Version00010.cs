using System.Data;
using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(10)]
    public class Version00010 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("AgenteSolicitacao")
                .WithColumn("AgenteId").AsInt32().NotNullable()
                .WithColumn("SolicitacaoId").AsInt32().NotNullable();

            Create.ForeignKey("FK_AgenteSolicitacao_Agente")
                .FromTable("AgenteSolicitacao").ForeignColumn("AgenteId")
                .ToTable("Agentes").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);

            Create.ForeignKey("FK_AgenteSolicitacao_Solicitacao")
                .FromTable("AgenteSolicitacao").ForeignColumn("SolicitacaoId")
                .ToTable("Solicitacoes").PrimaryColumn("Id");

            Create.PrimaryKey("PK_AgenteSolicitacao")
                .OnTable("AgenteSolicitacao")
                .Columns("AgenteId", "SolicitacaoId");
        }
    }
}
