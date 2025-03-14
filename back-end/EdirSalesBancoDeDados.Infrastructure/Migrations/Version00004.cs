using FluentMigrator;

namespace EdirSalesBancoDeDados.Infrastructure.Migrations
{
    [Migration(4)]
    public class Version00004 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("MunicipeGrupo")
                .WithColumn("MunicipeId").AsInt32().NotNullable()  // Coluna de FK para Municipe
                .WithColumn("GrupoId").AsInt32().NotNullable();     // Coluna de FK para Grupo

            // Configurando as FKs
            Create.ForeignKey("FK_MunicipeGrupo_Municipe")
                .FromTable("MunicipeGrupo").ForeignColumn("MunicipeId")
                .ToTable("Municipes").PrimaryColumn("Id");

            Create.ForeignKey("FK_MunicipeGrupo_Grupo")
                .FromTable("MunicipeGrupo").ForeignColumn("GrupoId")
                .ToTable("Grupos").PrimaryColumn("Id");

            // Definindo a PK composta (MunicipeId, GrupoId)
            Create.PrimaryKey("PK_MunicipeGrupo")
                .OnTable("MunicipeGrupo")
                .Columns("MunicipeId", "GrupoId");

        }
    }
}
