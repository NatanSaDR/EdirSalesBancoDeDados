using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(8)]
    public class Version00008 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Telefones")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Tipo").AsString().Nullable()
                .WithColumn("Numero").AsString().Nullable()
                .WithColumn("Observacao").AsString().Nullable()
                .WithColumn("MunicipeId").AsInt32().NotNullable();

            Create.ForeignKey("FK_TelefoneMunicipe")
                .FromTable("Telefones").ForeignColumn("MunicipeId")
                .ToTable("Municipes").PrimaryColumn("Id")
                .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
