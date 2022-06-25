using curso.api.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace curso.api.Infraestruture.Data.Mappings
{
    public class CursoMapping : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.ToTable("TB_CURSO"); // criar a tabela com onome
            builder.HasKey(p => p.Codigo); // PK

            //campos da tabela
            builder.Property(p => p.Codigo).ValueGeneratedOnAdd(); // auto incremento
            builder.Property(p => p.Nome);
            builder.Property(p => p.Descricao);
            // chave FK
            builder.HasOne(p => p.Usuario) // FK
                .WithMany().HasForeignKey(fk => fk.CodigoUsuario);
        }
    }
}
