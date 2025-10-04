using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public DbSet<Curso> Cursos { get; set; }
public DbSet<Matricula> Matriculas { get; set; }

protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Restricción: Un usuario no puede matricularse dos veces en el mismo curso.
    builder.Entity<Matricula>()
        .HasIndex(m => new { m.CursoId, m.UsuarioId })
        .IsUnique();

    // Restricción: HorarioFin > HorarioInicio
    builder.Entity<Curso>()
        .ToTable(b => b.HasCheckConstraint("CK_Curso_Horario", "[HorarioFin] > [HorarioInicio]"));
}