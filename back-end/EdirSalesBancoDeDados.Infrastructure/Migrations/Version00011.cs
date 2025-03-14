using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(11)]
    public class Version00011 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("ContatosAgente")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Contato").AsString().Nullable()
                .WithColumn("AgenteId").AsInt32().NotNullable()
                // Campos herdados de EntityBase
                .WithColumn("DataCadastro").AsDateTime().NotNullable()  // Data de cadastro
                .WithColumn("DataAlteracao").AsDateTime().Nullable()  // Data de alteração
                .WithColumn("UsuarioCadastro").AsString().NotNullable()  // Usuário que cadastrou
                .WithColumn("UsuarioAlteracao").AsString().Nullable();  // Usuário que alterou

            Create.ForeignKey("FK_ContatosAgente_AgenteSolucao")
                .FromTable("ContatosAgente").ForeignColumn("AgenteId")
                .ToTable("Agentes").PrimaryColumn("Id")
                .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
