using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace examen_parcial_programacion1.Models
{
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;

        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        public IdentityUser Usuario { get; set; } = null!;

        public DateTime FechaMatricula { get; set; } = DateTime.Now;
        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
        public bool Activa { get; set; } = true;
    }
}
