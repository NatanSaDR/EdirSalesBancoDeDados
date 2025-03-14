using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(3)]
    public class Version00003 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Solicitacoes")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()  // Chave primária com auto-incremento
                .WithColumn("Tipo").AsString(150).NotNullable()      // Tipo de Solicitação
                .WithColumn("Descricao").AsString(500).NotNullable()  // Descrição da Solicitação
                .WithColumn("Observacao").AsString(500).Nullable()   // Observações adicionais
                .WithColumn("Status").AsString(50).NotNullable()     // Status da Solicitação
                .WithColumn("DataFinalizado").AsDateTime().Nullable()
                .WithColumn("SEI").AsString(255).Nullable()

                // Campos herdados de EntityBase
                .WithColumn("DataCadastro").AsDateTime().NotNullable()  // Data de cadastro
                .WithColumn("DataAlteracao").AsDateTime().Nullable()  // Data de alteração
                .WithColumn("UsuarioCadastro").AsString().NotNullable()  // Usuário que cadastrou
                .WithColumn("UsuarioAlteracao").AsString().Nullable();  // Usuário que alterou
        }
    }
}
