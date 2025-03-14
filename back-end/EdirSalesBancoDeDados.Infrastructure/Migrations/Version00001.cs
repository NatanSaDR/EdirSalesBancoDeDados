using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(1)]
    public class Version00001 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Municipes")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()  // Chave primária com auto-incremento
                .WithColumn("Nome").AsString(150).NotNullable()  // Nome do munícipe
                .WithColumn("Sexo").AsString(10).Nullable()
                .WithColumn("Aniversario").AsDateTime().Nullable()
                .WithColumn("Logradouro").AsString(255).Nullable()// Logradouro
                .WithColumn("Numero").AsString(50).Nullable()  // Número
                .WithColumn("Complemento").AsString(100).Nullable()  // Complemento
                .WithColumn("Bairro").AsString(100).Nullable()  // Bairro
                .WithColumn("Cidade").AsString(100).Nullable()  // Cidade
                .WithColumn("Estado").AsString(50).Nullable()  // Estado
                .WithColumn("CEP").AsString(10).Nullable()
                .WithColumn("Observacao").AsString(500).Nullable()  // Observação
                .WithColumn("Email").AsString(150).Nullable()// E-mail

                // Campos herdados de EntityBase
                .WithColumn("DataCadastro").AsDateTime().NotNullable()  // Data de cadastro
                .WithColumn("DataAlteracao").AsDateTime().Nullable()  // Data de alteração
                .WithColumn("UsuarioCadastro").AsString().NotNullable()  // Usuário que cadastrou
                .WithColumn("UsuarioAlteracao").AsString().Nullable();  // Usuário que alterou
        }
    }
}
