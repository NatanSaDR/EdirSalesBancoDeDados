using FluentMigrator;
using FluentMigrator.Builder.Create.Index;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(9)]
    public class Version00009 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Agentes")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("AgenteSolucao").AsString().NotNullable()
                .WithColumn("Contato").AsString().Nullable()
                // Campos herdados de EntityBase
                .WithColumn("DataCadastro").AsDateTime().NotNullable()  // Data de cadastro
                .WithColumn("DataAlteracao").AsDateTime().Nullable()  // Data de alteração
                .WithColumn("UsuarioCadastro").AsString().NotNullable()  // Usuário que cadastrou
                .WithColumn("UsuarioAlteracao").AsString().Nullable();  // Usuário que alterou
        }
    }
}
